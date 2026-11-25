#!/usr/bin/env python3
"""Cut the hero sprite sheets (Assets/Art/Reference/*.png, made in ChatGPT) into game frames.

Each sheet is an even grid on a white background. Per frame: the white that touches the image border
becomes transparent (white fur inside the navy outline stays), the frame is cropped, scaled so every
sheet draws the pup at the same size (the red beanie measured on a standing row, BEANIE_PX across;
the standing pup is then about 300 px tall), and placed on a per-animation canvas around a fixed
anchor so frames never jump. Rows with the same name join into one animation.
  bottom - feet on the canvas bottom, body centroid at the canvas x centre (standing poses)
  center - body centroid at the canvas centre (in the air)
  rope   - top of the drawn rope at the canvas top centre (hang; the game's rope continues it)
  cell   - every frame keeps its place in the grid cell (lever: the lever base must not move)
PupRig reads the anchor per pose; keep ANCHOR in sync with PupRig.cs.

Usage: python3 -I Tools/slice_sheets.py   (then run ProjectBuilder.BuildAll to reimport)

Outfit skins (23/11): python3 -I Tools/slice_sheets.py --outfit pirate
cuts Assets/Art/Reference/outfit_pirate/<same sheet names> (ChatGPT redraws of the pup sheets in other clothes, same
grid and poses) into pirate_pup_<anim>_<i>.png. An outfit has no red beanie to measure, so each sheet takes the scale
measured on its ORIGINAL sheet (times the width ratio, should the redraw come back at another resolution).
"""
from __future__ import annotations

import re
import sys
from pathlib import Path

import numpy as np
from PIL import Image, ImageFilter

ROOT = Path(__file__).resolve().parent.parent
REF = ROOT / "Assets/Art/Reference"
OUT = ROOT / "Assets/Resources/Art"

OUTFIT: str | None = None   # set by --outfit: the outfit being cut (see the module docstring)

BEANIE_PX = 50.0        # sqrt of the red beanie area on a standing pup, output pixels (idle measured 50)
WHITE = 232             # all channels above this count as background when connected to the border
# Sheets whose outline has gaps (sheet S: the tumbling head's top line is missing, so 232 leaked into the fur):
# only near-pure white counts as background there. Costs a thin light fringe.
WHITE_FOR = {"pup_sheet_t4.png": 248, "pup_sheet_f2.png": 248, "pup_sheet_r2.png": 248, "pup_sheet_f3.png": 248, **{f"pup_sheet_16_{a}.png": 248 for a in ("fling_cartwheel", "fling_backflip", "fling_corkscrew", "tumble", "slide", "flat", "stuck_wall", "stuck_buried", "stuck_splat")}, "pup_sheet_x.png": 248}
PAD = 4

# Front-view sheets H, I, J, K (ChatGPT, 16/11) share one scale, so frames never jump in size between poses. 1.0 makes the
# standing front pup about 270 px tall, the size of the side-view cheer/lever frames and the hanging pup (user 16/11: the
# queue at 1.226 looked bigger than the pup on the rope).
FRONT_SCALE = 1.0
# ChatGPT draws the pup a little bigger or smaller on each new sheet, so the front sheets are sized by their helmet instead:
# 40.8 px is the helmet of sheet H at FRONT_SCALE (50 px at its measured 1.226), the size the user approved.
FRONT_BEANIE = 40.8

# sheet file, output prefix, columns, rows, row names, scale rule: a row name (its red beanie must measure BEANIE_PX)
# or ("height", row name, px) (that row's frames are scaled to this height). A missing sheet is skipped with a note.
SHEETS = [
    # idle and walk now come from sheet I (front view, user 16/11); sheet A keeps its other rows at its measured scale
    ("pup_sheet_a.png", "pup", 4, 4, [None, None, "flail", "cheer"], ("scale", 1.069)),   # hang: sheet K
    ("pup_sheet_c.png", "pup", 4, 4, ["leap", "glide", "dizzy", "ball"], "dizzy"),
    ("pup_sheet_d.png", "pup", 4, 4, ["lever", "lever", "launch", "fall"], "lever"),
    # hero sheet E (docs/pup-art-brief-2.md): level flight (2 rows = 8 frames), nose-down turn + ninja hang, getting up
    ("pup_sheet_e.png", "pup", 4, 4, ["fly", "fly", "divening", "getup"], "getup"),
    # hero sheet F (v10 redesign, ChatGPT edit of sheet A): grab the rope, stuck upside down, flinch, spin ball
    ("pup_sheet_f.png", "pup", 4, 4, ["grab", "stuck", "flinch", "spin"], "flinch"),
    # hero sheet G (report §11): saluting on the mission card; four near-identical rows, the first one is the loop
    ("pup_sheet_g.png", "pup", 4, 4, ["salute"] * 4, "salute"),
    # hero sheet H (CPC live study 16/11, ChatGPT edit of sheet F): queue fidgets and the helmetless stuck pup
    ("pup_sheet_h.png", "pup", 4, 4, [None, None, None, "stuck_bare"], ("scale", FRONT_SCALE)),   # wave/hattip/look: sheets L, M
    # sheet T (user 17/11: "hải cẩu dính tường chưa đẹp và hợp lí"): splatted flat on its back, limbs kicking, no hat;
    # rows 3-4 repeat rows 1-2. Cut back-down so the frame's bottom line is the rock surface.
    # hero sheet I (front view, user 16/11: "vẽ lại cả khung đứng chờ và đi bộ nhìn thẳng"): idle, waddle, bare idle, and
    # the mission card's pick-up-the-helmet loop
    ("pup_sheet_i.png", "pup", 4, 4, ["idle", None, "idle_bare", "pickhat"], ("beanie", "idle", FRONT_BEANIE)),   # walk: sheet O
    # hero sheet J (user 16/11: more funny queue fidgets): hop for joy, dance, yawn, sneeze; front view, sheet H's scale
    # sheet J (4-frame hop, dance, yawn, sneeze) is replaced by the 8-frame sheets M, N, O and no longer cut.
    # 8-frame front-view fidgets (user 16/11: "chưa đủ khung hình"): each action spans two rows, read left to right
    ("pup_sheet_l.png", "pup", 4, 4, ["wave", "wave", "hattip", "hattip"], ("beanie", "wave", FRONT_BEANIE)),
    ("pup_sheet_m.png", "pup", 4, 4, ["look", "look", "hop", "hop"], ("beanie", "look", FRONT_BEANIE)),
    # captive's cry for help on the map (user 18/11: "chưa có hoạt ảnh help cho hải cẩu ở trong lồng"): flippers up and
    # waving, mouth open, a little hop; 8 frames in rows 1-2 (rows 3-4 repeat them)
    # pup_sheet_help.png is kept only as the style base for new front-view pup sheets: its "help" frames were replaced
    # by the cage's pup_captive (19/11).
    # 18/11: the home screen's snowball fight, drawn for it: throw a snowball, then hit by one (8 frames each).
    ("pup_sheet_throw.png", "pup", 4, 4, ["throw", "throw", "hit", "hit"], ("beanie", "throw", FRONT_BEANIE)),
    # 18/11: the modal cast's pups are 16-frame sheets only (pup_sheet_16_modal_*.png, SMOOTH_SIZE); a first cute
    # 8-frame set (waving, crying) was dropped: "đừng trẻ con quá".
    ("pup_sheet_n.png", "pup", 4, 4, ["dance", "dance", "yawn", "yawn"], ("beanie", "dance", FRONT_BEANIE)),
    ("pup_sheet_o.png", "pup", 4, 4, ["sneeze", "sneeze", "walk", "walk"], ("beanie", "walk", FRONT_BEANIE)),
    # hero sheet K (user 16/11: "thêm hoạt ảnh đu nữa"): front-view hang (flipper tips = the rope point), a fast excited
    # hang, reaching up for the rope, crouching ready at the bridge edge
    ("pup_sheet_k.png", "pup", 4, 4, ["hang", "hang_fast", "reach", "ready"], ("beanie", "reach", FRONT_BEANIE)),
    # 16/11: the operator at its own lever on the cart's deck (wait, pull), and the no-rope launch (jump off, fall)
    # 16/11 (user: "hải cẩu điều khiển máy đứng ngang"): the operator side-on, facing left toward the pit; sheet P (front
    # view) is kept in Reference but no longer cut
    ("pup_sheet_r.png", "pup", 4, 4, ["op_wait", "op_wait", "op_pull", "op_pull"], ("height", "op_wait", 262)),   # side view: the helmet shows more red, so sized by height like the front idle
    ("pup_sheet_q.png", "pup", 4, 4, ["drop_jump", "drop_jump", "drop_fall", "drop_fall"], ("beanie", "drop_jump", FRONT_BEANIE)),
    # bare-headed pup (hat knocked off in the arena): ChatGPT edits of sheets E, walk, C and A with the hat removed; only
    # the rows used after the hat is gone are cut. No beanie to measure: each uses ("scale", s), the scale this script
    # prints for its hatted sheet (the edits keep the same canvas and cell layout), so both sets are the same size.
    ("pup_sheet_e_bare.png", "pup", 4, 4, [None, None, None, "getup_bare"], ("scale", 1.231)),
    # sheet S (CPC video 17/11, ChatGPT edit of sheet E bare): belly slide and rag-doll somersault after the bounces
    # sheets U-X (user 17/11: no curled ball when flung, "nhiều kiểu" for every action): rag-doll fling styles and a
    # knocked-flat rest (edits of S), two more launch flights (edit of D, same 35° flight), two more stuck styles (edit of T)
    # sheet T4 (user 17/11: "kiểu nằm dính hình như bị sai, vẽ lại"; CPC: flat as a pancake against the rock): one
    # 16-frame splat for every rock face, the pup's back on the frame's top line
    ("pup_sheet_t4.png", "pup", 4, 4, ["stuck_splat"] * 4, ("scale", FRONT_SCALE)),
    # sheet F2 (user 17/11: "phải vẽ lại văng … xem kĩ lại cpc"; CPC at 60 fps: a flung penguin is a stiff arched body,
    # flippers pressed back, feet trailing, and the whole sprite spins): one held pose with a small shiver, 16 frames
    ("pup_sheet_f2.png", "pup", 4, 4, ["flung"] * 4, ("scale", 1.231)),
    # sheet R2: CPC's ground roll, curled round seen from the side (the code rolls it like a wheel)
    ("pup_sheet_r2.png", "pup", 4, 4, ["roll"] * 4, ("fur", "roll", 138)),
    # sheets Y, Z, Q2 (user 17/11: swing, bungee and no-rope drop each their own ride, gently funny, "đừng lố quá"):
    # edits of K (swing, bungee: rope point at the top centre) and Q (jump off, fall); a Q2 replaces Q's rows
    ("pup_sheet_y.png", "pup", 4, 4, ["swing"] * 4, ("scale", 1.076)),
    ("pup_sheet_z.png", "pup", 4, 4, ["bungee"] * 4, ("scale", 1.076)),
    ("pup_sheet_q2.png", "pup", 4, 4, ["drop_jump", "drop_jump", "drop_fall", "drop_fall"], ("scale", 0.942)),
    ("pup_walk_bare.png", "pup", 6, 1, ["walk_bare"], ("scale", 0.592)),
    ("pup_sheet_c_bare.png", "pup", 4, 4, [None, None, "dizzy_bare", "ball_bare"], ("scale", 1.058)),
    ("pup_sheet_a_bare.png", "pup", 4, 4, [None, None, "flail_bare", "cheer_bare"], ("scale", 1.069)),
    # polar bear guard (docs/bear-art-brief.md): standing bear 260 px tall, feet on the canvas bottom
    ("bear_sheet.png", "bear", 4, 4, ["idle", "taunt", "dizzy", "ko"], ("height", "idle", 260)),
    ("bear_sheet_b.png", "bear", 4, 2, ["eat", "sleep"], ("height", "eat", 260)),
    ("bear_sheet_c.png", "bear", 4, 4, ["flex", "laugh", "scratch", "juggle"], ("height", "flex", 260)),
    # v10: more stand-in-place antics (GDD §R4): shadow-boxing, sitting hugging knees, smug nod, eating from a bowl
    ("bear_sheet_d.png", "bear", 4, 4, ["punch", "sit", "smug", "bowl"], ("height", "punch", 260)),
    # report §11: sitting on a ledge with legs dangling. ChatGPT drew four different sitting angles per row; only the
    # third column (legs hanging straight down) is the dangle, so that column read top to bottom is the loop.
    ("bear_sheet_e.png", "bear", 4, 4, ["dangle"] * 4, ("height", "dangle", 230), 2),
    # Sheet F (18/11): the home screen's snowball fight, drawn for it (throw, then hit by a snowball), 8 frames each.
    ("bear_sheet_f.png", "bear", 4, 4, ["throw", "throw", "hit", "hit"], ("height", "hit", 260)),   # sized by the upright hit row (the throw row crouches),
]
ANCHOR = {"roll": "center", "flung": "center", "stuck_splat": "rope", "swing": "rope", "bungee": "rope", "fling_cartwheel": "center", "fling_backflip": "center", "fling_corkscrew": "center", "flat": "bottom", "launch_panic": "center", "launch_hero": "center", "stuck_buried": "bottom", "stuck_wall": "bottom", "slide": "center", "tumble": "center", "op_wait": "bottom", "op_pull": "bottom", "drop_jump": "center", "drop_fall": "center", "hang_fast": "rope", "reach": "bottom", "ready": "bottom", "hop": "bottom", "dance": "bottom", "yawn": "bottom", "sneeze": "bottom", "pickhat": "bottom", "wave": "bottom", "hattip": "bottom", "look": "bottom", "stuck_bare": "center", "getup_bare": "bottom", "walk_bare": "bottom", "dizzy_bare": "bottom", "cheer_bare": "bottom", "idle_bare": "bottom", "flail_bare": "center", "ball_bare": "center", "salute": "bottom", "dangle": "bottom", "punch": "bottom", "exit": "center", "captive": "bottom", "flexfail": "bottom", "doze": "bottom", "backfire": "bottom", "loader": "bottom", "modal_wait": "bottom", "modal_win": "bottom", "modal_lose": "bottom", "modal_music": "bottom", "modal_trophy": "bottom", "modal_read": "bottom", "modal_mock": "bottom", "modal_ko": "bottom", "throw": "bottom", "hit": "bottom", "sit": "bottom", "smug": "bottom", "bowl": "bottom", "grab": "bottom", "stuck": "center", "flinch": "bottom", "spin": "center", "fly": "center", "divening": "center", "dive": "center", "fling_spin": "center", "fling_star": "center", "fling_top": "center", "fling_paddle": "center", "fling_drill": "center", "ninja": "center", "getup": "bottom", "eat": "bottom", "sleep": "bottom",
          "laugh": "bottom", "scratch": "bottom", "juggle": "bottom", "flex": "bottom",
          "leap": "bottom", "walk": "bottom", "idle": "bottom", "cheer": "bottom", "dizzy": "bottom", "taunt": "bottom", "ko": "bottom",
          "hang": "rope", "flail": "center", "glide": "center", "ball": "center",
          "lever": "cell", "launch": "center", "fall": "center"}
# Cell order per (sheet, row) where the drawing's order differs from what PupRig plays: idle wants 0-1-2 breathing and the
# blink last (frame 3); sheet I drew the blink third.
REORDER = {("pup_sheet_i.png", "idle"): [0, 1, 3, 2], ("pup_sheet_i.png", "idle_bare"): [0, 1, 3, 2],
           # sheet O's two ACHOO cells: the lifted helmet opens the white face to the white background, so the background
           # cut eats it; the neighbouring cells stand in (keys are per row: sneeze row 1, then row 2)
           ("pup_sheet_o.png", "sneeze"): [0, 1, 2, 2]}
REORDER_ROW2 = {("pup_sheet_o.png", "sneeze"): [1, 1, 2, 3]}

# single sprites used by menus and the dive (ball spins in code, so one ball frame is enough)
ALIASES = {"pup": "pup_idle_0", "pup_ball": "pup_ball_0", "pup_ball_bare": "pup_ball_bare_0", "pup_dizzy": "pup_dizzy_2", "bear": "bear_idle_0", "bear_helmet": "bear_idle_0"}


# Sheets where white fur meets the white background through a small gap in the outline (19/11: the bare-headed pup's
# muzzle is open by the whiskers, and asking for a "closed outline" did not close it): the background may only pass
# through openings wider than 2 x this many px + 1, so the gap is sealed and the head stays.
SEAL_GAPS = {"pup_sheet_16_fling_drill.png": 6}


def remove_background(rgb: np.ndarray, white: int = WHITE, seal: int = 0) -> np.ndarray:
    """Alpha mask: 0 where near-white pixels connect to the border, else 255."""
    light = (rgb > white).all(axis=2)
    if seal:
        size = 2 * seal + 1
        as_img = lambda m: Image.fromarray(m.astype(np.uint8) * 255)
        wide = np.asarray(as_img(light).filter(ImageFilter.MinFilter(size))) > 0   # light with room around it
        outside = remove_background(np.dstack([wide] * 3).astype(np.uint8) * 255, 128) == 0
        return np.where((np.asarray(as_img(outside).filter(ImageFilter.MaxFilter(size))) > 0) & light, 0, 255).astype(np.uint8)
    reach = np.zeros_like(light)
    reach[0, :], reach[-1, :], reach[:, 0], reach[:, -1] = light[0, :], light[-1, :], light[:, 0], light[:, -1]
    while True:   # grow the border region through light pixels until it stops
        grown = reach.copy()
        grown[1:, :] |= reach[:-1, :]
        grown[:-1, :] |= reach[1:, :]
        grown[:, 1:] |= reach[:, :-1]
        grown[:, :-1] |= reach[:, 1:]
        grown &= light
        if (grown == reach).all():
            break
        reach = grown
    return np.where(reach, 0, 255).astype(np.uint8)


def beanie(img: Image.Image) -> float:
    """sqrt of the red beanie area: a size measure that ignores poses, props and the scarf."""
    x = np.asarray(img.convert("RGBA")).astype(int)
    red = (x[..., 0] > 170) & (x[..., 1] < 80) & (x[..., 2] < 80) & (x[..., 3] > 200)
    return float(red.sum()) ** 0.5


def cells(path: Path, cols: int, rows: int) -> list[list[Image.Image | None]]:
    """Grid cells with the background removed, uncropped (frames keep their place in the cell). An empty cell stays in
    the grid as None so later frames keep their number (a spare or missing drawing never renumbers the rest)."""
    rgb = np.asarray(Image.open(path).convert("RGB"))
    rgba = Image.fromarray(np.dstack([rgb, remove_background(rgb, WHITE_FOR.get(path.name, WHITE), SEAL_GAPS.get(path.name, 0))]), "RGBA")
    w, h = rgba.size
    grid = []
    for r in range(rows):
        row = []
        for c in range(cols):
            cell = rgba.crop((w * c // cols, h * r // rows, w * (c + 1) // cols, h * (r + 1) // rows))
            if cell.getchannel("A").getbbox() is None:
                print(f"{path.name}: empty cell row {r} col {c} (no frame written for it)")
                cell = None
            row.append(cell)
        grid.append(row)
    return grid


def anchor_point(img: Image.Image, mode: str) -> tuple[float, float]:
    a = np.asarray(img.getchannel("A")) > 128
    ys, xs = np.nonzero(a)
    if mode == "rope":   # drawn rope: the opaque run in the top rows
        top = np.nonzero(a[:6].any(axis=0))[0]
        if top.size == 0:
            raise ValueError("rope anchor: no rope drawn in the top 6 rows of the frame")
        return float(top.mean()), 0.0
    cx, cy = float(xs.mean()), float(ys.mean())
    return cx, (float(img.height) if mode == "bottom" else cy)


def place(frames: list[Image.Image], mode: str) -> list[Image.Image]:
    """Same canvas for the whole animation, each frame's anchor on the same canvas point."""
    anchors = [anchor_point(f, mode) for f in frames]
    half_w = max(max(ax, f.width - ax) for f, (ax, _) in zip(frames, anchors))
    if mode == "center":
        half_h = max(max(ay, f.height - ay) for f, (_, ay) in zip(frames, anchors))
        size, origin = (half_w * 2, half_h * 2), (half_w, half_h)
    else:
        size, origin = (half_w * 2, max(f.height for f in frames)), (half_w, 0.0)
    w, h = int(round(size[0])) + PAD * 2, int(round(size[1])) + PAD * 2
    out = []
    for f, (ax, ay) in zip(frames, anchors):
        canvas = Image.new("RGBA", (w, h), (0, 0, 0, 0))
        x = int(round(PAD + origin[0] - ax))
        y = h - PAD - f.height if mode == "bottom" else int(round(PAD + origin[1] - ay))
        canvas.alpha_composite(f, (x, y))
        out.append(canvas)
    return out


def crop(frames: list[Image.Image], mode: str) -> list[Image.Image]:
    """Each frame to its own bounding box, or all to their shared box in "cell" mode."""
    boxes = [f.getchannel("A").getbbox() for f in frames]
    if mode == "cell":
        union = (min(b[0] for b in boxes), min(b[1] for b in boxes), max(b[2] for b in boxes), max(b[3] for b in boxes))
        return [f.crop(union) for f in frames]
    return [f.crop(b) for f, b in zip(frames, boxes)]


# Front-view sheets (16/11): ChatGPT sprinkles yellow "!" ticks and motion strokes around the pup, a different set in
# every frame, so they blink on and off as the frames play (user: "hải cẩu trên hàng bị nhấp nháy"). Only the pup's
# body and anything nearly as big (a helmet lying beside it) is kept.
FX_FREE = {"bear_sheet_f.png", "bear_sheet_16_ko.png", "bear_sheet_16_flex.png", "bear_sheet_16_bowl.png", "bear_sheet_16_throw.png", "pup_sheet_16_fall.png", "pup_sheet_16_throw.png", "pup_sheet_throw.png", "pup_sheet_t4.png", "pup_sheet_f2.png", "pup_sheet_r2.png", "pup_sheet_f3.png", "pup_sheet_y.png", "pup_sheet_z.png", "pup_sheet_q2.png", *(f"pup_sheet_16_{a}.png" for a in ("fling_cartwheel", "fling_backflip", "fling_corkscrew", "tumble", "slide", "flat", "launch_panic", "launch_hero", "stuck_wall", "stuck_buried", "stuck_splat")), "pup_sheet_x.png", "pup_sheet_r.png", "pup_sheet_p.png", "pup_sheet_q.png", "pup_sheet_h.png", "pup_sheet_i.png", "pup_sheet_k.png", "pup_sheet_l.png", "pup_sheet_m.png",
           "pup_sheet_n.png", "pup_sheet_o.png"}
KEEP_SHARE = 0.08
# Within one ChatGPT row the pup also grows and shrinks a few percent from cell to cell, which pops as the frames play.
# Where the helmet sits on the head the whole time, each frame is sized by its own helmet instead of the row's median.
PER_FRAME_SIZE = {"idle", "walk", "wave", "look", "hop", "dance", "yawn", "hang", "hang_fast", "reach", "ready"}


def even_size(img: Image.Image) -> Image.Image:
    b = beanie(img)
    if b < FRONT_BEANIE * 0.5:
        raise ValueError(f"even_size: helmet not found ({b:.1f} px)")
    k = min(1.25, max(0.8, FRONT_BEANIE / b))   # a tilted or hidden helmet must not blow a frame up
    if abs(k - 1.0) < 0.01:
        return img
    return img.resize((max(1, round(img.width * k)), max(1, round(img.height * k))), Image.LANCZOS)   # parts at least this share of the body's area stay (a dropped helmet is ~15 %)


def keep_body(img: Image.Image, share: float = KEEP_SHARE) -> Image.Image:
    """Clears every opaque island smaller than `share` of the largest one (4-connected flood fill)."""
    a = np.asarray(img.getchannel("A")) > 40
    h, w = a.shape
    label = np.zeros((h, w), dtype=np.int32)
    sizes = [0]
    for y0, x0 in zip(*np.nonzero(a)):
        if label[y0, x0]:
            continue
        n = len(sizes)
        label[y0, x0] = n
        stack, count = [(y0, x0)], 0
        while stack:
            y, x = stack.pop()
            count += 1
            for yy, xx in ((y - 1, x), (y + 1, x), (y, x - 1), (y, x + 1)):
                if 0 <= yy < h and 0 <= xx < w and a[yy, xx] and not label[yy, xx]:
                    label[yy, xx] = n
                    stack.append((yy, xx))
        sizes.append(count)
    if len(sizes) <= 2:
        return img
    big = max(sizes)
    keep = np.array([s >= big * share for s in sizes])
    keep[0] = False
    rgba = np.asarray(img).copy()
    rgba[..., 3] = np.where(keep[label], rgba[..., 3], 0)
    return Image.fromarray(rgba)


# Rows that ChatGPT drew standing on a slab of ice ledge: the slab is erased so the pup stands on the game's own bridge.
STRIP_LEDGE = {"grab"}
# Rows no pose plays any more (20/11): still measured, since their sheets are scaled by them, but not written.
UNUSED = {"leap", "lever", "grab", "reach", "fall", "glide", "getup", "getup_bare", "flinch", "hang_fast", "walk_bare", "cheer_bare"}


def strip_ledge(img: Image.Image) -> Image.Image:
    """Erase a wide slab under the feet: the band of rows, counted up from the bottom, opaque across 55% of the width."""
    a = np.asarray(img).copy()
    opaque = a[..., 3] > 128
    h, w = opaque.shape
    y = h - 1
    while y > 0 and opaque[y].sum() >= 0.55 * w:
        y -= 1
    if y < h - 1:                     # a slab was there (frames on the rope have none)
        a[max(0, y - 3):, :, 3] = 0   # a few rows more take the slab's snow lip
    out = Image.fromarray(a, "RGBA")
    box = out.getchannel("A").getbbox()
    return out.crop(box) if box else out


def pad(img: Image.Image) -> Image.Image:
    canvas = Image.new("RGBA", (img.width + PAD * 2, img.height + PAD * 2), (0, 0, 0, 0))
    canvas.alpha_composite(img, (PAD, PAD))
    return canvas


def median_of(values: list[float], what: str) -> float:
    if not values:
        raise ValueError(f"no frames to measure {what} on")
    return float(np.median(values))


def remove_stale(prefix: str, anim: str, keep: set[str]) -> None:
    """Frames of this animation left from an earlier cut that has more frames (exact name match: walk is not walk_bare)."""
    pattern = re.compile(rf"{re.escape(prefix)}_{re.escape(anim)}_\d+")
    for png in OUT.glob(f"{prefix}_{anim}_*.png"):
        if pattern.fullmatch(png.stem) and png.stem not in keep:
            png.unlink()
            png.with_suffix(".png.meta").unlink(missing_ok=True)
            print(f"  removed stale {png.name}")


# The bare-headed pup is the same animal in every pose (user 20/11: "kích cỡ hải cẩu phải bằng nhau ở mọi hoạt ảnh"):
# poses that curl, stretch or turn cannot be sized by height, so they are sized by how much white fur shows.
# 138 = idle_bare / dizzy_bare / exit as committed.
BARE_FUR = 138


def fur(img: Image.Image) -> float:
    """Square root of the white-fur area of a frame, in px (scarf, outline and face left out)."""
    a = np.asarray(img.convert("RGBA")).astype(int)
    return float(((a[..., 3] > 128) & (a[..., :3].min(axis=2) > 200)).sum()) ** 0.5


def out_name(prefix: str, anim: str, slot: int) -> str | None:
    """File name for a frame by its slot in the animation's cells; None for a cell that is not a frame."""
    if anim != "divening":
        return f"{prefix}_{anim}_{slot}"
    # the mixed row of sheet E: two nose-down frames, then the ninja hang; the 4th cell is a spare (PupRig loads 1)
    return f"{prefix}_dive_{slot}" if slot < 2 else (f"{prefix}_ninja_0" if slot == 2 else None)


# 16-frame redraws (user 17/11: "tăng khung hình lên để hoạt ảnh mượt mà"): Reference/pup_sheet_16_<anim>.png holds one
# animation in all 16 cells. When it exists it replaces that animation's 8-frame rows, at the scale of its first sheet.
SMOOTH_SCALE = {"fling_cartwheel": 1.231, "fling_backflip": 1.231, "fling_corkscrew": 1.231, "tumble": 1.231, "slide": 1.231,
                "flat": 1.231, "launch_panic": 0.890, "launch_hero": 0.890, "stuck_wall": FRONT_SCALE,
                "stuck_buried": FRONT_SCALE, "stuck_splat": FRONT_SCALE}
# 16-frame in-between redraws of older sets (18/11, "tăng khung hình cho tất cả hoạt ảnh"): sized to match the frames
# they replace (matched_height), so poses lying, sitting or crouching keep their size.
SMOOTH_MATCH = {("pup", a) for a in ("cheer", "walk", "wave", "hattip", "hop", "flail", "launch", "glide", "fall", "dizzy",
                                    "getup", "fly", "spin", "flinch", "dance", "yawn", "look", "sneeze", "stuck",
                                    "throw", "hit", "op_wait", "drop_jump", "drop_fall", "pickhat",
                                    "flail_bare", "dizzy_bare", "getup_bare", "stuck_bare", "cheer_bare", "hang", "hang_fast", "walk_bare",
                                    "op_pull", "dive")} | {("bear", a) for a in ("idle", "taunt", "dizzy", "ko", "eat", "sleep", "flex", "laugh", "scratch", "juggle",
                                     "punch", "sit", "smug", "bowl", "dangle", "throw", "hit")}
# Height in px of frame 0 of each pup set before its 16-frame redraw (measured on the committed frames, 19/11).
SMOOTH_FIRST = {"cheer": 265, "walk": 269, "wave": 271, "hattip": 271, "hop": 268, "flail": 262, "launch": 209, "glide": 220,
                "fall": 216, "dizzy": 263, "getup": 209, "fly": 257, "spin": 261, "flinch": 317, "dance": 267, "yawn": 271,
                "look": 264, "sneeze": 264, "stuck": 247,
                "op_wait": 262, "op_pull": 262, "drop_jump": 252, "drop_fall": 232, "pickhat": 246, "reach": 262,
                "flail_bare": 258, "dizzy_bare": 254, "getup_bare": 172, "stuck_bare": 207, "cheer_bare": 245,
                "hang": 301, "hang_fast": 277, "walk_bare": 245, "dive": 248}
# 16-frame redraws of any character (user 18/11: "tăng khung hình cho tất cả hoạt ảnh"): (kind, anim) -> sizing without
# the row name, measured on the 16 frames themselves.
SMOOTH_SIZE = {
    ("pup", "modal_wait"): ("beanie", FRONT_BEANIE), ("pup", "modal_win"): ("beanie", FRONT_BEANIE),
    ("pup", "modal_lose"): ("beanie", FRONT_BEANIE), ("pup", "modal_music"): ("beanie", FRONT_BEANIE),
    ("pup", "modal_trophy"): ("beanie", FRONT_BEANIE), ("pup", "modal_read"): ("beanie", FRONT_BEANIE),
    ("bear", "modal_mock"): ("height", 260), ("bear", "modal_ko"): ("height", 150),
    ("bear", "flexfail"): ("height", 260), ("bear", "doze"): ("height", 260), ("bear", "backfire"): ("first", 262),   # frame 1 is the bear standing
    ("pup", "loader"): ("height", 420),
    ("pup", "captive"): ("beanie", FRONT_BEANIE),
    ("pup", "ready"): ("first", 283),   # frame 1 stands like idle_2, then sinks into the crouch
    ("pup", "ninja"): ("first", 304),
    **{("pup", a): ("fur", 138) for a in ("flung", "fling_spin", "fling_star", "fling_paddle", "fling_drill", "fling_top", "slide")},   # BARE_FUR
    ("pup", "exit"): ("beanie", FRONT_BEANIE),   # CPC's sheepish hop out after the turn (19/11)
    # Bears: frame 1 of each redraw as tall as frame 0 of the committed 4-frame set (19/11). Matching the frames already
    # on disk drifted run after run (idle -32 %, flex +26 %: bears swelled when they changed antic).
    **{("bear", n): ("first", h) for n, h in {"idle": 260, "taunt": 259, "dizzy": 288, "ko": 142, "eat": 260, "sleep": 239,
                                              "flex": 260, "laugh": 262, "scratch": 270, "juggle": 260, "punch": 262, "sit": 243,
                                              "smug": 266, "bowl": 268, "dangle": 230}.items()},
    ("bear", "throw"): ("first", 232), ("bear", "hit"): ("first", 260),   # frame 1 as tall as the 8-frame set's
    ("pup", "throw"): ("scale", 1.136), ("pup", "hit"): ("scale", 1.136),   # the scale pup_sheet_throw.png was cut at   # the map's caged pup calling for rescue (19/11)   # the loading act: pup on its rolling snowball
}


def matched_height(kind: str, anim: str) -> float | None:
    """Median drawn height (alpha box) of the frames already cut for this animation: a 16-frame redraw is scaled to
    match them, so a lying or sitting bear keeps the size its sheet gave it."""
    heights = []
    for f in sorted(OUT.glob(f"{kind}_{anim}_*.png")):
        if not f.stem[len(kind) + len(anim) + 2:].isdigit():
            continue   # pup_hang_fast_0 is not a frame of pup_hang
        box = Image.open(f).getchannel("A").getbbox()
        if box:
            heights.append(box[3] - box[1])
    return sorted(heights)[len(heights) // 2] if heights else None


def with_smooth(sheets: list) -> list:
    if (REF / "pup_sheet_q2.png").exists():
        sheets = [e for e in sheets if e[0] != "pup_sheet_q.png"]
    # Every 16-frame redraw: <kind>_sheet_16_<anim>.png. Sizing: SMOOTH_SIZE, else the pup scale of SMOOTH_SCALE, else
    # ("match",): the height of the frames it replaces. The older rows of that animation are dropped, except a row its
    # sheet is measured by (dropping it broke the sheet's scale); the redraw is cut last, so it overwrites those frames.
    pairs = [("pup", a) for a in SMOOTH_SCALE] + list(SMOOTH_SIZE) + [tuple(k) for k in SMOOTH_MATCH]
    smooth = {(k, a) for k, a in pairs if (REF / f"{k}_sheet_16_{a}.png").exists()}
    out = []
    origin = {}   # (kind, anim) -> the sizing rule of the sheet it was first drawn on
    for e in sheets:
        for n in e[4]:
            if n:
                origin.setdefault((e[1], n), e[5])
    for e in sheets:
        rule = e[5]
        keep = rule if isinstance(rule, str) else rule[1] if isinstance(rule, tuple) and rule[0] in ("beanie", "height", "fur") else None
        out.append((e[0], e[1], e[2], e[3], [None if (e[1], n) in smooth and n != keep else n for n in e[4]], *e[5:]))
    for k, a in sorted(smooth):
        size = SMOOTH_SIZE.get((k, a))
        if size is None and k == "pup" and a in SMOOTH_FIRST:
            # Frame 1 of a redraw is the old frame 1 redrawn: give it the old frame's height. (Sizing by the beanie or
            # by the median height made redraws 5-20 % too big: snow over the beanie, a pup that gets up from lying.)
            size = ("first", SMOOTH_FIRST[a])
        if size is None and (k, a) in SMOOTH_MATCH:
            px = matched_height(k, a)
            if px is None:
                print(f"{k}_sheet_16_{a}.png: skipped, no older frames to match the size of")
                continue
            size = ("height", px)
        if size is None:
            size = ("scale", SMOOTH_SCALE[a])
        if size[0] in ("beanie", "height", "first", "fur"):
            size = (size[0], a, *size[1:])   # the row named for measuring is this animation itself
        out.append((f"{k}_sheet_16_{a}.png", k, 4, 4, [a] * 4, size))
    return out


def main() -> None:
    global OUTFIT
    if "--outfit" in sys.argv:
        at = sys.argv.index("--outfit") + 1
        if at >= len(sys.argv):
            sys.exit("usage: slice_sheets.py --outfit <name>")
        OUTFIT = sys.argv[at]
    failed = []
    for entry in with_smooth(SHEETS):
        if OUTFIT and entry[1] != "pup":
            continue
        try:
            cut_sheet(entry)
        except (ValueError, OSError, KeyError, IndexError, ZeroDivisionError) as e:   # a row that measures 0 (no beanie found) fails alone
            failed.append(f"{entry[0]}: {e}")
            print(f"{entry[0]}: FAILED, {e}")
    for alias, source in ALIASES.items():
        if OUTFIT:
            if alias.startswith("pup") and (OUT / f"{OUTFIT}_{source}.png").exists():
                Image.open(OUT / f"{OUTFIT}_{source}.png").save(OUT / f"{OUTFIT}_{alias}.png")
            continue
        if (OUT / f"{source}.png").exists():
            Image.open(OUT / f"{source}.png").save(OUT / f"{alias}.png")
        else:
            print(f"alias {alias}: source {source}.png missing, old copy left as is")
    if failed:
        sys.exit("some sheets were not cut:\n  " + "\n  ".join(failed))


def box_size(img: Image.Image) -> float:
    """Square root of the drawing's bounding-box area: a size that holds for any pose (standing, flying, curled)."""
    b = img.getchannel("A").getbbox()
    return ((b[2] - b[0]) * (b[3] - b[1])) ** 0.5 if b else 0.0


def match_classic(prefix: str, anim: str, slots: list[int], frames: list[Image.Image]) -> list[Image.Image]:
    """An outfit redraw comes back a little bigger or smaller than the sheet it copies (up to 22 % on 23/11): scale the
    whole animation so its median size matches the classic frames already cut, so the pup never changes size when it
    changes clothes. Clamped, so a bad redraw cannot blow up."""
    classic = [OUT / f"{prefix.split('_', 1)[1]}_{anim}_{i}.png" for i in slots]
    classic = [box_size(Image.open(c)) for c in classic if c.exists()]
    mine = [box_size(f) for f in frames]
    if not classic or not any(mine):
        return frames
    k = sorted(classic)[len(classic) // 2] / sorted(mine)[len(mine) // 2]
    if not 0.8 <= k <= 1.25:
        print(f"  {anim}: redraw is {1 / k:.2f}x the classic size, only partly corrected (redraw this sheet)")
    k = min(1.25, max(0.8, k))
    if abs(k - 1.0) < 0.02:
        return frames
    return [f.resize((max(1, round(f.width * k)), max(1, round(f.height * k))), Image.LANCZOS) for f in frames]


def cut_sheet(entry: tuple) -> None:
    """Cuts one sheet; ValueError when it cannot be measured or anchored (main collects these and exits 1)."""
    name, prefix, cols, rows, row_names, rule = entry[:6]
    only_col = entry[6] if len(entry) > 6 else None
    if not (REF / name).exists():
        print(f"{name}: not found, skipped")
        return
    OUT.mkdir(parents=True, exist_ok=True)
    grid = cells(REF / name, cols, rows)
    if only_col is not None:
        grid = [[row[only_col]] for row in grid]
    if isinstance(rule, tuple) and rule[0] == "scale":
        scale = rule[1]
    elif isinstance(rule, tuple) and rule[0] == "first":   # ("first", row, px): that row's FIRST frame is px tall
        _, row, px = rule
        first = next(f for f in grid[row_names.index(row)] if f)
        if name in FX_FREE:
            first = keep_body(first)
        box = first.getchannel("A").getbbox()
        scale = px / (box[3] - box[1])
    elif isinstance(rule, tuple) and rule[0] == "fur":   # ("fur", row, px): sqrt of that row's median white-fur area
        _, row, px = rule
        scale = px / median_of([fur(f) for f in grid[row_names.index(row)] if f], f"the fur of row '{row}'")
    elif isinstance(rule, tuple) and rule[0] == "beanie":   # ("beanie", row, px): that row's helmet measures px
        _, row, px = rule
        scale = px / median_of([beanie(f) for f in grid[row_names.index(row)] if f], f"the beanie of row '{row}'")
    elif isinstance(rule, tuple):
        _, row, px = rule
        heights = [f.getchannel("A").getbbox()[3] - f.getchannel("A").getbbox()[1] for f in grid[row_names.index(row)] if f]
        scale = px / median_of(heights, f"the height of row '{row}'")
    else:
        scale = BEANIE_PX / median_of([beanie(f) for f in grid[row_names.index(rule)] if f], f"the beanie of row '{rule}'")
    if OUTFIT:
        redraw = REF / f"outfit_{OUTFIT}" / name
        if not redraw.exists():
            print(f"{name}: no {OUTFIT} redraw yet, skipped")
            return
        scale *= Image.open(REF / name).width / Image.open(redraw).width
        grid = cells(redraw, cols, rows)
        if only_col is not None:
            grid = [[row[only_col]] for row in grid]
        prefix = f"{OUTFIT}_{prefix}"
    anims: dict[str, list[Image.Image | None]] = {}
    for row, anim in zip(grid, row_names):
        if anim is not None:   # None = a row of the sheet that is not used
            order = REORDER_ROW2.get((name, anim)) if anim in anims and len(anims[anim]) >= cols else None
            order = order or REORDER.get((name, anim))
            anims.setdefault(anim, []).extend([row[i] for i in order] if order else row)
    # Measure and place every anim first, then write: a sheet that fails leaves its old frames untouched.
    results = []
    for anim, cells_ in anims.items():
        if OUTFIT and anim in UNUSED:
            continue
        slots = [i for i, f in enumerate(cells_) if f is not None and out_name(prefix, anim, i)]
        gaps = [i for i, f in enumerate(cells_) if f is None and out_name(prefix, anim, i)]
        if gaps:   # the rigs load frames 0..n-1 in order; a hole would show an empty sprite
            raise ValueError(f"{anim}: empty cell(s) {gaps}")
        if not slots:
            raise ValueError(f"{anim}: every cell is empty")
        frames = [cells_[i] for i in slots]
        if name in FX_FREE:
            frames = [keep_body(f) for f in frames]
        mode = ANCHOR[anim]
        scaled = [f.resize((max(1, round(f.width * scale)), max(1, round(f.height * scale))), Image.LANCZOS)
                  for f in crop(frames, mode)]
        if anim in STRIP_LEDGE:
            scaled = [strip_ledge(f) for f in scaled]
        if OUTFIT:
            scaled = match_classic(prefix, anim, slots, scaled)
        if name in FX_FREE and anim in PER_FRAME_SIZE and not OUTFIT:   # even_size measures the beanie
            scaled = [even_size(f) for f in scaled]
        try:
            placed = place(scaled, mode) if mode != "cell" else [pad(f) for f in scaled]
        except ValueError as e:
            raise ValueError(f"{anim}: {e}") from e
        results.append((anim, slots, placed))
    for anim, slots, placed in results:
        written = set()
        for slot, frame in zip(slots, placed if anim not in UNUSED else []):
            written.add(out_name(prefix, anim, slot))
            frame.save(OUT / f"{out_name(prefix, anim, slot)}.png")
        if OUTFIT:
            pass   # an outfit's frames are only ever written whole; nothing older to clear
        elif anim == "divening":
            remove_stale(prefix, "dive", written)
            remove_stale(prefix, "ninja", written)
        else:
            remove_stale(prefix, anim, written)
        print(f"{name}: {anim} x{len(placed)} -> {placed[0].size} (scale {scale:.3f})")

if __name__ == "__main__":
    main()
