#!/usr/bin/env python3
"""Cut the drawn rope skins (Assets/Art/Reference/rope_skins.png: 8 vertical strips side by side on white, ChatGPT 25/11)
into seamless tiles Resources/Art/rope_<name>.png.

Each strip is found as one blob (left to right), then cut to a whole number of its own pattern repeats: the vertical
period is the shift at which the strip best matches itself (autocorrelation), so the tile's bottom runs straight into
its top when SwingRope tiles it. The tile is scaled so its width matches the classic rope's (24 px), keeping the aspect.

Usage: python3 -I Tools/cut_ropes.py   (then ProjectBuilder.ConfigureArt)
"""
from __future__ import annotations

import sys
from pathlib import Path

import numpy as np
from PIL import Image

sys.path.insert(0, str(Path(__file__).resolve().parent))
from cut_props import blobs, reading_order  # noqa: E402
from slice_sheets import remove_background  # noqa: E402

ROOT = Path(__file__).resolve().parent.parent
SHEET = ROOT / "Assets/Art/Reference/rope_skins.png"
OUT = ROOT / "Assets/Resources/Art"
NAMES = ["candy", "vine", "chain", "ice", "rainbow", "ribbon", "hazard", "magic"]
CLASSIC_WIDTH = 24     # px, Resources/Art/rope.png
MIN_PERIOD = 40        # px in the sheet: a shorter "period" is the outline matching itself


def period(strip: np.ndarray) -> int:
    """Best vertical repeat of the strip's colours, in sheet pixels."""
    core = strip[:, strip.shape[1] // 4: 3 * strip.shape[1] // 4, :3].astype(float).mean(axis=1)   # one colour per row
    core -= core.mean(axis=0)
    best, best_score = 0, -1.0
    for p in range(MIN_PERIOD, len(core) // 2):
        a, b = core[:-p], core[p:]
        score = float((a * b).sum() / (np.sqrt((a * a).sum() * (b * b).sum()) + 1e-9))
        if score > best_score:
            best, best_score = p, score
    return best


def main() -> None:
    rgb = np.asarray(Image.open(SHEET).convert("RGB"))
    alpha = remove_background(rgb)
    rgba = np.dstack([rgb, alpha])
    boxes = sorted(blobs(alpha), key=lambda b: b[0])
    if len(boxes) != len(NAMES):
        sys.exit(f"found {len(boxes)} strips, expected {len(NAMES)}: {boxes}")
    for name, (x0, y0, x1, y1) in zip(NAMES, boxes):
        strip = rgba[y0:y1, x0:x1]
        margin = (y1 - y0) // 8                    # the ends of a drawn strip are often rounded off
        body = strip[margin: len(strip) - margin]
        p = period(body)
        reps = max(1, (len(body) // 2) // p)       # a few repeats: one long tile reads better than a tiny one
        start = (len(body) - reps * p) // 2
        tile = Image.fromarray(body[start: start + reps * p], "RGBA")
        k = CLASSIC_WIDTH / tile.width * 1.6       # drawn strips carry their outline inside: a little wider than the classic
        tile = tile.resize((max(1, round(tile.width * k)), max(1, round(tile.height * k))), Image.LANCZOS)
        tile.save(OUT / f"rope_{name}.png")
        print(f"rope_{name}: period {p}px x{reps} -> {tile.size}")


if __name__ == "__main__":
    main()
