# Final Terrain Sprite Delivery

Put approved ShanhaiGate terrain art here.

Unity automatically imports images in this folder as:

- Sprite, single mode.
- 100 pixels per unit.
- Alpha transparency enabled.
- No mipmaps.
- Uncompressed texture.
- Bilinear filtering.
- Clamp wrap mode.

## Required First Sprites

Use these filenames when possible:

- `Platform_Long.png`
- `Platform_Short.png`
- `Block_Thick.png`
- `Wall_Vertical.png`
- `Pit_Rim.png`

## Suggested Canvas Sizes

- `Platform_Long`: `2048 x 512`
- `Platform_Short`: `1024 x 384`
- `Block_Thick`: `1024 x 768`
- `Wall_Vertical`: `512 x 2048`
- `Pit_Rim`: `1024 x 256`

These sizes are not collision sizes. They are art canvases. The cover tool scales the sprite to match the graybox width and aligns platform art to the collider top edge.

## Top Edge Rule

For platforms, the playable surface must be near the top of the sprite:

- Trim empty transparent padding above the playable edge.
- Do not place tall flowers, leaves, or vines above the exact landing edge.
- Grass can break the silhouette slightly, but keep it low and readable.
- Let thickness, roots, vines, stones, and shadows extend downward.

If the visible landing edge is not exactly at the top of the sprite bounds, use `Top Edge Y Offset` in `Jade/ShanhaiGate Terrain Cover Window`.

## Style Notes

Target a soft forest ruin look:

- Misty teal and mint shadows.
- Blue-gray stone or faded ruin blocks.
- Soft pink flowers and warm dusk highlights.
- Darker underside shapes for depth.
- Clean readable landing edge.

Avoid:

- Dark horror cave palettes.
- Noisy small rubble texture.
- Baked background trees or sky.
- Pixelated low-resolution artifacts unless the whole game commits to pixel art.

