#!/usr/bin/env python3
"""Cut single props (machine parts, arena objects, UI pieces) out of ChatGPT sheets on a white background.

Unlike the hero sheets (Tools/slice_sheets.py), a prop sheet has no fixed grid: every separate drawing is found as
one connected blob of non-background pixels, and the blobs are named in reading order (top row first, left to right;
two blobs are in the same row when their vertical centres are within ROW_TOLERANCE of the sheet height).
The white that touches the border becomes transparent, as in slice_sheets.py, so white snow inside an outline stays.

Usage: python3 -I Tools/cut_props.py   (then Seal Catapult -> 2. Build Project)
"""
from __future__ import annotations

import sys
from pathlib import Path

import numpy as np
from PIL import Image

sys.path.insert(0, str(Path(__file__).resolve().parent))
from slice_sheets import keep_body, remove_background  # noqa: E402  (same background rule as the hero sheets)

ROOT = Path(__file__).resolve().parent.parent
REF = ROOT / "Assets/Art/Reference"
OUT = ROOT / "Assets/Resources/Art"

MIN_AREA = 900          # px; smaller blobs are specks or stray marks
ROW_TOLERANCE = 0.12    # fraction of the sheet height
PAD = 4

# sheet -> names in reading order (None = drop that blob)
SHEETS: dict[str, list[str | None]] = {
    "catapult_parts.png": ["catapult_arm", None, "snowball"],
    # 16/11: the cart redrawn with a flat standing deck for the operator in place of the boiler (user: "thiết kế lại xe
    # bắn để hải cẩu đứng đúng chỗ"); arm and snowball stay from the first sheet
    "catapult_parts_2.png": [None, "catapult_base", None],
    "arena_props.png": [
        # names the game already loads; the heavy "stone" block is drawn as a wooden crate, the wedge is unused
        "block_ice", None, "block_snow", "block_stone",
        "crate_tnt", "bumper", "button_up", "button_down",
        "fish", None, None, None,   # cage/cage_open: cage_cpc.png now; the HUD badge is no longer used
    ],
    # UI kit: panel, ribbon, 3 pill buttons (green = primary), 6 round icon buttons, stars, bear face, tap hint, medal
    "ui_kit.png": [
        "ui_panel", "ribbon",
        "ui_button_gold", "ui_button_warm", "ui_button",
        "icon_sound", "icon_music", "icon_help", "icon_pause", "icon_play", "icon_retry",
        "star_full", "star_empty", None, "hud_bear", "glyph_tap", "badge_perfect",
    ],
    # map + HUD kit (report §15): hero portrait, "?" stone node, cleared star node, "you are here" glow pad,
    # 18/11: the brand logo, one for every language (home, boot, About)
    "logo.png": ["logo"],
    # 19/11: CPC's tall slim bird cage on a red base, closed and opened ("xem lại hoạt họa trong lồng của cpc")
    "cage_cpc.png": ["cage", "cage_open"],
    # 23/11: skin shop pieces: effect particles (trails, impacts), the "owned" tick, and the whacker's ball skins
    "skin_props.png": [
        "fx_heart", "fx_bubble", "fx_note", "fx_flame", "fx_bolt", "fx_blossom", "mark_tick",
        "ball_stone", "ball_melon", "ball_coconut", "ball_disco", "ball_beach", "ball_bowling", "ball_golden", "ball_puffer",
    ],
    # 23/11: drawn launcher skins, each a ChatGPT redraw of the classic arm (top) and cart (bottom) on one sheet
    **{f"catapult_skin_{t}.png": [f"cart_{t}_arm", f"cart_{t}_base"] for t in ("ice", "pirate", "candy", "royal", "dragon", "rocket", "bamboo", "lava")},
    # 23/11: the pirate outfit's HUD portrait and the tricorn that falls off in the arena (outfit_pirate/hud_hat.png)
    "outfit_pirate/hud_hat.png": ["pirate_hud_pup", "pirate_pup_hat"],
    "outfit_ninja/hud_hat.png": ["ninja_hud_pup", None],   # its headband came out with fur on it: hat_band.png
    "outfit_ninja/hat_band.png": ["ninja_pup_hat"],
    # 23/11: the ice podium the selected skin stands on in the shop
    "shop_podium.png": ["shop_podium"],
    # 18/11: the red X that closes a modal (top-right corner of every panel)
    "ui_close.png": ["icon_close"],
    # 18/11: pieces of the redesigned modals (pause, result, settings, stats, how to play), Ice crystal style
    "ui_modal_kit.png": [
        None, None, "mark_box", "ui_row", None,   # rays, tick and bulb are no longer used (19/11)
        "row_music", "row_sound", "row_globe", "row_chart", "row_trash",
        "row_bear", "row_paw1", "row_paw2", "row_paw3", "row_star",
        "step_1", "step_2", "step_3", "step_4",
    ],
    # 18/11: the icons drawn on the buttons, painted in the ice-crystal UI style (replace make_art's flat SVG glyphs)
    "ui_icons.png": [
        "glyph_back", "glyph_gear", "glyph_help", "glyph_lock",
        "glyph_map", "glyph_music", "glyph_next", "glyph_pause",
        "glyph_play", "glyph_retry", "glyph_shop", "glyph_sound",
    ],
    # 20/11: the Story button's own icon, an open book in the ui_icons.png style (it shared the Play triangle)
    "ui_icon_book.png": ["glyph_book"],
    # bear portrait (replaces the ui_kit one, same ring as the hero), plain card for the mission panel
    "map_ui_kit.png": ["hud_pup", "node_stone", "node_star", "node_glow", "hud_bear", "ui_card"],
    # hat knocked off a pup, ice wedge (rises to the right), dent left in rock (report §16.2, §17.1)
    "arena_extras.png": ["pup_hat", "block_wedge", "dent"],
    # CPC ice break (video 17/11): chunks that pile up where a block broke and fly up, gold impact rings, grey smoke, twinkle
    "ice_fx.png": [
        "ice_chunk_0", "ice_chunk_1", "ice_chunk_2", "ice_chunk_3",
        "ice_chunk_4", "ice_chunk_5", "ice_chunk_6", "ice_chunk_7", None, "ice_chunk_8",   # the last chunk was drawn with a speck and a tiny cube beside it
        "impact_ring", "impact_ring_small", "smoke_ball", "twinkle",
    ],
}


# Enclosed background (holes, gaps between beams) is not connected to the border, so it is cleared from seed points
# given in the cut part's own pixels (measure on the output PNG).
HOLES: dict[str, list[tuple[int, int]]] = {
    "catapult_base": [(380, 62), (370, 150), (325, 225), (440, 175)],   # hub hole and the A-frame gaps (cart of 16/11)
    "catapult_arm": [(48, 100)],
}


CLASSIC_SIZE = {"catapult_base": (729, 426), "catapult_arm": (728, 156)}   # the parts HOLES was measured on


def clear_holes(part: Image.Image, seeds: list[tuple[int, int]]) -> Image.Image:
    """Make the light region around each seed transparent (flood fill through near-white pixels)."""
    x = np.asarray(part).copy()
    light = (x[..., :3] > WHITE_HOLE).all(axis=2)
    for sx, sy in seeds:
        if not (0 <= sy < light.shape[0] and 0 <= sx < light.shape[1]):
            raise ValueError(f"hole seed {sx},{sy} is outside the {light.shape[1]}x{light.shape[0]} part (re-measure HOLES)")
        if not light[sy, sx]:
            print(f"  hole seed {sx},{sy} is not on a light pixel, skipped")
            continue
        reach = np.zeros_like(light)
        reach[sy, sx] = True
        while True:
            grown = reach.copy()
            grown[1:, :] |= reach[:-1, :]
            grown[:-1, :] |= reach[1:, :]
            grown[:, 1:] |= reach[:, :-1]
            grown[:, :-1] |= reach[:, 1:]
            grown &= light
            if (grown == reach).all():
                break
            reach = grown
        x[reach, 3] = 0
    return Image.fromarray(x, "RGBA")


WHITE_HOLE = 225
# Parts with no white of their own (bars, rings): every near-white pixel is background seen through them.
CLEAR_LIGHT = {"cage", "cage_open", "impact_ring", "impact_ring_small"}


def blobs(alpha: np.ndarray) -> list[tuple[int, int, int, int]]:
    """Bounding boxes (x0, y0, x1, y1) of connected opaque regions, found on a 4x downsampled mask."""
    step = 4
    small = alpha[::step, ::step] > 0
    h, w = small.shape
    seen = np.zeros_like(small)
    boxes = []
    for y in range(h):
        for x in range(w):
            if not small[y, x] or seen[y, x]:
                continue
            stack, x0, y0, x1, y1, n = [(y, x)], x, y, x, y, 0
            seen[y, x] = True
            while stack:
                cy, cx = stack.pop()
                n += 1
                x0, y0, x1, y1 = min(x0, cx), min(y0, cy), max(x1, cx), max(y1, cy)
                for ny, nx in ((cy + 1, cx), (cy - 1, cx), (cy, cx + 1), (cy, cx - 1)):
                    if 0 <= ny < h and 0 <= nx < w and small[ny, nx] and not seen[ny, nx]:
                        seen[ny, nx] = True
                        stack.append((ny, nx))
            if n * step * step >= MIN_AREA:
                # Samples are every `step` px: the true left/top edge can lie up to step-1 px before the first sample
                # (cut() trims the box back to the real alpha edge afterwards).
                boxes.append((max(0, x0 * step - step + 1), max(0, y0 * step - step + 1), min((x1 + 1) * step, alpha.shape[1]), min((y1 + 1) * step, alpha.shape[0])))
    return boxes


def reading_order(boxes: list[tuple[int, int, int, int]], height: int) -> list[tuple[int, int, int, int]]:
    rows: list[list[tuple[int, int, int, int]]] = []
    for b in sorted(boxes, key=lambda b: (b[1] + b[3]) / 2):
        cy = (b[1] + b[3]) / 2
        if rows and abs(cy - np.mean([(r[1] + r[3]) / 2 for r in rows[-1]])) < ROW_TOLERANCE * height:
            rows[-1].append(b)
        else:
            rows.append([b])
    return [b for row in rows for b in sorted(row, key=lambda b: b[0])]


def cut(sheet: str, names: list[str | None]) -> None:
    """Cuts one sheet; ValueError when its parts do not match SHEETS (nothing of that sheet is written)."""
    path = REF / sheet
    if not path.exists():
        print(f"{sheet}: not found, skipped")
        return
    rgb = np.asarray(Image.open(path).convert("RGB"))
    alpha = remove_background(rgb)
    rgba = Image.fromarray(np.dstack([rgb, alpha]), "RGBA")
    boxes = reading_order(blobs(alpha), rgb.shape[0])
    if len(boxes) != len(names):
        raise ValueError(f"found {len(boxes)} parts, expected {len(names)}: {boxes}")
    parts = []
    for name, box in zip(names, boxes):   # build every part first, so a bad seed never leaves a half-written sheet
        if name is None:
            continue
        part = rgba.crop(box)
        part = part.crop(part.getchannel("A").getbbox())
        canvas = Image.new("RGBA", (part.width + PAD * 2, part.height + PAD * 2), (0, 0, 0, 0))
        canvas.alpha_composite(part, (PAD, PAD))
        if name in HOLES:
            canvas = clear_holes(canvas, HOLES[name])
        if name.startswith("cart_"):
            # A drawn launcher skin keeps the classic silhouette: its holes sit at the same fractions of the picture.
            classic = "catapult_" + name.rsplit("_", 1)[1]
            cw, ch = CLASSIC_SIZE[classic]
            # ...roughly: a redraw shifts them a little, so the points round each one are tried too. A fill that takes
            # more than 6 % of the picture ran into snow or icing, not a hole, and is dropped.
            for x, y in HOLES[classic]:
                for dx, dy in ((0, 0), (-0.04, 0), (0.04, 0), (0, -0.06), (0, 0.06), (-0.02, -0.12), (0, -0.18), (-0.02, -0.24)):
                    seed = (round((x / cw + dx) * canvas.width), round((y / ch + dy) * canvas.height))
                    if not (0 <= seed[0] < canvas.width and 0 <= seed[1] < canvas.height):
                        continue
                    if not all(v > WHITE_HOLE for v in canvas.getpixel(seed)[:3]) or canvas.getpixel(seed)[3] == 0:
                        continue
                    tried = clear_holes(canvas, [seed])
                    gone = int((np.asarray(canvas)[..., 3] > 0).sum() - (np.asarray(tried)[..., 3] > 0).sum())
                    if gone < 0.06 * canvas.width * canvas.height:
                        canvas = tried
        if name.startswith("ice_chunk_"):
            canvas = keep_body(canvas, share=0.99)   # only the chunk itself; chunks drawn close together: drop a neighbour's corner caught in the box
        if name in CLEAR_LIGHT:
            x = np.asarray(canvas).copy()
            x[(x[..., :3] > 238).all(axis=2), 3] = 0
            canvas = Image.fromarray(x, "RGBA")
        parts.append((name, canvas))
    OUT.mkdir(parents=True, exist_ok=True)
    for name, canvas in parts:
        canvas.save(OUT / f"{name}.png")
        print(f"{sheet}: {name} {canvas.size}")


def main() -> None:
    failed = []
    for sheet, names in SHEETS.items():
        try:
            cut(sheet, names)
        except ValueError as e:
            failed.append(f"{sheet}: {e}")
            print(f"{sheet}: FAILED, {e}")
    if failed:
        sys.exit("some sheets were not cut:\n  " + "\n  ".join(failed))


if __name__ == "__main__":
    main()
