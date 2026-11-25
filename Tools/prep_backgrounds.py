#!/usr/bin/env python3
"""Turn ChatGPT background paintings into game backgrounds.

Each painting in Assets/Art/Reference/ is 1536x1024. The pit is a fixed screen and phones are wider than 3:2 (up to
20:9), so the pit painting gets side pads from a second ChatGPT painting (pit_side_walls.png: left half plain wall,
right half sky + ledge + wall), faded into its edges. Repeating the painting's own edge strip was tried and looked
like woven cloth; mirroring turned cobbles into hearts.

Run: python3 -I Tools/prep_backgrounds.py   (then Seal Catapult -> 2. Build Project)
"""
from pathlib import Path

import numpy as np
from PIL import Image, ImageDraw

ROOT = Path(__file__).resolve().parent.parent
REF = ROOT / "Assets" / "Art" / "Reference"
OUT = ROOT / "Assets" / "Resources" / "Art"

# output sprite -> painting it comes from (Tools/make_art.py leaves these alone once the painting exists)
PAINTINGS = {"bg_pit": "pit_bg_wide5.png", "bg_arena_sky": "arena_sky.png",
             "map_bg": "map_island.png", "bg_menu": "menu_bay.png",
             "bg_shop": "shop_bay.png",   # 23/11: the shop page's bright bay (design 1 of shop_mockups.png)
             **{f"story_{i}": f"story_{i}.png" for i in range(1, 6)},
             "rock_cobble": "pit_side_walls_box.png", "rock_top": "pit_side_walls_box.png"}


def build_pit() -> None:
    """bg_pit.png is ONE painting (pit_bg_wide5.png, 16:9): rock on every side is part of the picture, wide enough
    for 21:9 at the scale LaunchPhase uses. Nothing is pasted or padded any more (user 13/11: the composed pit
    "nhìn vẫn lộ là bị ghép")."""
    copy_plain("bg_pit", "pit_bg_wide5.png")   # 16/11: the bridge painted one bay longer


def copy_plain(name: str, source: str) -> None:
    """Paintings that are scaled to cover the screen (the arena sky) are used as they are."""
    path = REF / source
    if not path.exists():
        print(f"skip {name}: {source} not in Assets/Art/Reference yet")
        return
    Image.open(path).convert("RGB").save(OUT / f"{name}.png", optimize=True)
    print(f"{name}.png")


RIM_SIZE = (640, 96)  # px of the seamless snow rim strip


def draw_snow(out: Path) -> None:
    """rock_top.png: the painted snow cap of the launch pit (right half of pit_side_walls_box.png), cut out column by
    column between its navy top outline and its navy scalloped underside, made to tile sideways. The arena's rims are
    then the very same snow as the pit's (user 13/11: a drawn vector rim did not match the painted pit).
"""
    wall = np.asarray(Image.open(REF / "pit_side_walls_box.png").convert("RGB")).astype(int)
    x0, x1, y0, depth = 800, 1500, 330, 110
    band = wall[y0:y0 + depth, x0:x1]
    navy = (band[..., 0] < 75) & (band[..., 2] > 105)
    alpha = np.zeros(band.shape[:2], np.uint8)
    for x in range(band.shape[1]):
        rows = np.flatnonzero(navy[:, x])
        if rows.size == 0:
            continue
        top = rows[0]
        under = rows[rows > top + 30]                      # the scalloped underside's outline
        bottom = (under[0] + 7) if under.size else top + 60
        alpha[top:min(bottom, top + 78, depth), x] = 255   # never deeper than the deepest scallop (stray navy in the rock)
    rgba = np.dstack([band.astype(np.uint8), alpha])
    used = np.flatnonzero(alpha.any(axis=1))
    if used.size == 0:
        raise SystemExit(f"rock_top: no navy cap outline found in pit_side_walls_box.png at x {x0}-{x1}, y {y0}-{y0 + depth}")
    rgba = rgba[used[0]:used[-1] + 1].astype(float)
    overlap = 100                                           # wrap sideways: fade the far end into the start
    t = np.linspace(0, 1, overlap)[None, :, None]
    body = rgba[:, :-overlap].copy()
    body[:, :overlap] = rgba[:, -overlap:] * (1 - t) + body[:, :overlap] * t
    Image.fromarray(body.round().astype(np.uint8), "RGBA").resize(RIM_SIZE, Image.LANCZOS).save(out / "rock_top.png", optimize=True)
    print(f"rock_top.png from the pit cap: {body.shape[1]}x{body.shape[0]} px")


def build_terrain() -> None:
    """Arena rock: cobble texture and snow rim from the pit's own painting (pit_side_walls_box.png)."""
    path = REF / "pit_side_walls_box.png"
    if not path.exists():
        print("skip rock_*: pit_side_walls_box.png not in Assets/Art/Reference yet")
        return
    # Cobble texture: the pit's own stone (left half of pit_side_walls_box.png is a plain cobble wall), so the arena's
    # rock matches the launch pit (user 13/11: "pha 2 đất đá chưa nhất quán"). Square crop, wrap both ways.
    wall = Image.open(path).convert("RGB")
    # No blending at all: one unbroken piece of the painted wall (768x1024 px = 8.25 x 11 world units at the pit's
    # scale). AttackPhase.Cobble maps it in world space and MIRRORS it sideways, so it never shows a seam or the
    # doubled, smeared stones that cross-fading a small tile produced (user 13/11: "nhìn vẫn lộ là bị ghép").
    tex = wall.crop((0, 0, 760, 1024))
    tex.save(OUT / "rock_cobble.png", optimize=True)
    # Snow rim, drawn here in the pit painting's snow colours (white, pale blue shade, navy
    # outline). The strip cut from arena_terrain.png carried a band of that sheet's brown rock along its top, which
    # showed as a brown edge outside the snow on every slope (user 13/11).
    draw_snow(OUT)
    # (Ramps are cobble meshes now, AttackPhase.Cobble: the rock_ramp mask and the snow_knob joint are no longer made.)
    print("rock_cobble.png rock_top.png")


def crop_16_9(name: str, source: str) -> None:
    """Centre crop to 16:9 (the map is laid out on a 1920x1080 canvas and drawn envelope-fitted)."""
    path = REF / source
    if not path.exists():
        print(f"skip {name}: {source} not in Assets/Art/Reference yet")
        return
    img = Image.open(path).convert("RGB")
    h = min(img.height, round(img.width * 9 / 16))
    w = min(img.width, round(h * 16 / 9))
    x, y = (img.width - w) // 2, (img.height - h) // 2
    img.crop((x, y, x + w, y + h)).save(OUT / f"{name}.png", optimize=True)
    print(f"{name}.png  {w}x{h}")


def main() -> None:
    OUT.mkdir(parents=True, exist_ok=True)
    build_pit()
    crop_16_9("map_bg", PAINTINGS["map_bg"])
    crop_16_9("bg_menu", PAINTINGS["bg_menu"])
    crop_16_9("bg_shop", PAINTINGS["bg_shop"])
    for i in range(1, 6):
        crop_16_9(f"story_{i}", PAINTINGS[f"story_{i}"])
    build_terrain()
    copy_plain("bg_arena_sky", PAINTINGS["bg_arena_sky"])
    # One map painting per castle world (user 17/11: "sang thành khác thì map cx phải khác", "màu nền … khác nhau");
    # trails come from the red-dot copies via Tools/map_trail.py. Kept at the painting frame MapView measures (1672×941).
    copy_plain("map_bg_world", "map_world.png")
    for world in ("canyon", "forest", "harbour", "caves", "volcano", "blossom", "palace", "tundra", "throne"):
        copy_plain(f"map_bg_{world}", f"map_{world}.png")
    build_scenes()


WORLDS = ("canyon", "forest", "harbour", "caves", "volcano", "blossom", "palace", "tundra", "throne")
SKY_SIZE = (1536, 1024)    # arena_sky.png: AttackPhase places the horizon by this frame
WALL_SIZE = (1536, 1024)   # pit_side_walls_box.png
PIT_SIZE = (1672, 941)     # pit_bg_wide5.png: LaunchPhase's colliders are measured on it


def cover(im: Image.Image, size: tuple[int, int]) -> Image.Image:
    """Scales to cover `size`, then centre-crops: ChatGPT returns a slightly different aspect."""
    k = max(size[0] / im.width, size[1] / im.height)
    im = im.resize((round(im.width * k), round(im.height * k)), Image.LANCZOS)
    x, y = (im.width - size[0]) // 2, (im.height - size[1]) // 2
    return im.crop((x, y, x + size[0], y + size[1]))


def build_scenes() -> None:
    """Each castle's own scenery for both phases (user 17/11: "mỗi thành thì cx thay đổi phong cảnh trong cả pha 1 và pha
    2 (đất đá, băng…)", "đổi cả thanh đứng nữa"), from three ChatGPT pictures per world in Assets/Art/Reference:
    pit_<w>.png (an EDIT of pit_bg_wide5.png that keeps every outline, so the colliders stay; its bridge is painted in
    the world's own material), arena_sky_<w>.png and wall_<w>.png (an edit of pit_side_walls_box.png; its left part is
    the cobble texture). Only painted pictures are used: a missing one leaves the Frost Bay art, tinted (WorldTheme)."""
    for w in WORLDS:
        for name, source, size, crop in ((f"bg_pit_{w}", f"pit_{w}.png", PIT_SIZE, None),
                                         (f"bg_arena_sky_{w}", f"arena_sky_{w}.png", SKY_SIZE, None),
                                         (f"rock_cobble_{w}", f"wall_{w}.png", WALL_SIZE, (0, 0, 760, 1024))):
            path = REF / source
            if not path.exists():
                print(f"skip {name}: {source} not in Assets/Art/Reference yet")
                continue
            raw = Image.open(path).convert("RGB")
            if name.startswith("bg_pit_") and raw.size != size:
                # The pit's colliders are measured on the 1672x941 original: a rescaled or cropped edit moves the rock.
                raise SystemExit(f"{source} is {raw.size[0]}x{raw.size[1]}, not {size[0]}x{size[1]}: repaint it at that size")
            im = cover(raw, size)
            (im.crop(crop) if crop else im).save(OUT / f"{name}.png", optimize=True)
            print(f"{name}.png")


if __name__ == "__main__":
    main()
