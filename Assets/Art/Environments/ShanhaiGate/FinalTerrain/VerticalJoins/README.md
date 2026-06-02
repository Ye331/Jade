# ShanhaiGate Vertical Joins And Wall Traps

These sprites extend the forest ruin terrain set with vertical platform-to-wall joins and wall-mounted hazards.
They are visual assets only. Keep gameplay collision and damage on your existing graybox/hazard objects.

## Platform To Lower Wall

- `Platform_To_Wall_LeftDrop.png`: a platform that drops into a wall under the left side.
- `Platform_To_Wall_CenterDrop.png`: a platform that drops into a centered lower wall.
- `Platform_To_Wall_RightDrop.png`: a platform that drops into a wall under the right side.

Use these when a walkable platform visually continues into a wall below it.
Align the grass top to the platform collider top. Let the lower wall overlap the wall graybox by `0.1` to `0.3` Unity units to hide the seam.

## Wall Traps

- `WallTrap_Spikes_LeftFacing.png`: spikes protruding left from a wall.
- `WallTrap_Spikes_RightFacing.png`: mirrored spikes protruding right from a wall.
- `WallTrap_CrystalSaw.png`: circular jade crystal blade in a stone wall mount.
- `WallTrap_ThornVine.png`: thorn vine hazard over a wall plaque.
- `WallTrap_ThornVine_Mirrored.png`: mirrored thorn vine hazard.
- `Hazard_Warning_Plaque.png`: warning trim for nearby trap language.

Put trap art on top of the wall skin, then keep or add a separate hazard collider that matches only the dangerous part.
Do not use the full SpriteRenderer bounds as the damage collider, because decorative stone and leaves are larger than the actual danger area.

## Recommended Renderer Settings

- Platform-to-wall joins: sorting order `6` to `9`.
- Wall trap art: sorting order `10` to `12`.
- Hazard warning plaque: sorting order `9` to `11`.

