# ShanhaiGate Cave Terrain Draft

Bright cave terrain draft for the drop-into-cave section. These are visual sprites only; keep the graybox colliders as gameplay truth.

## Sprites

- `Cave_Floor_Top_01.png`: walkable cave floor top, best for the first landing platforms.
- `Cave_Floor_Fill_01.png`: continuous underground fill below playable floors.
- `Cave_Ceiling_01.png`: overhead cave ceiling, use above the player after the drop.
- `Cave_Wall_Left_01.png`: left side wall for vertical shafts and cave boundaries.
- `Cave_Wall_Right_01.png`: right side wall for vertical shafts and cave boundaries.
- `Cave_Entrance_Transition_01.png`: forest-to-cave entrance module for the first drop.
- `CaveTerrain_Source_Chroma.png`: original generated sheet, kept for reference only.

## Placement Notes

- Sorting order: floor and walls around `2` to `6`, ceiling around `8` if it should overlap background, decorations above terrain only when they do not block jumps.
- Use `Cave_Entrance_Transition_01` as the visual mask where the player falls from forest platforms into the cave.
- Use `Cave_Floor_Fill_01` below floor colliders to keep the camera from seeing floating platforms.
- Use only `SpriteRenderer`; do not add colliders to these art objects.
