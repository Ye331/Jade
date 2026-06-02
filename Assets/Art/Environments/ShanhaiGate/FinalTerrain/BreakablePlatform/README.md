# ShanhaiGate Breakable Platform Art

These sprites are visual states for breakable platforms.
Keep `BreakablePlatform2D`, its collider, and gameplay timing on the graybox object.

## Sprite Set

- `BreakablePlatform_Intact.png`: normal walkable state.
- `BreakablePlatform_ObviousIntact.png`: recommended initial state. It has stronger cracks, a warning glyph, and jade fracture lines so players can read it as breakable.
- `BreakablePlatform_Cracked.png`: warning state.
- `BreakablePlatform_Fracturing.png`: near-break state with jade cracks.
- `BreakablePlatform_Rubble.png`: destroyed visual after the platform breaks.
- `BreakablePlatform_BrokenCap_Left.png`: left broken cap/edge.
- `BreakablePlatform_BrokenCap_Right.png`: right broken cap/edge.
- `BreakablePlatform_Debris_Chips.png`: small decorative debris particles.

## Placement

- Align the intact/cracked/fracturing sprites' grass top to the platform collider top.
- Use a separate child `SpriteRenderer` for each visual state, then toggle them from script or animation.
- Do not use rubble or debris as walkable collision.
- Suggested sorting order: `6` to `9`.

## Fast Setup

Select a GameObject with `BreakablePlatform2D`, then run:

`Jade/Apply Breakable Platform Art To Selected`

The tool creates `Art_BreakablePlatform`, hides the graybox renderer, binds the visual renderer to `BreakablePlatform2D`, assigns the break animation frames, and hides the renderer after the final fracture frame.
