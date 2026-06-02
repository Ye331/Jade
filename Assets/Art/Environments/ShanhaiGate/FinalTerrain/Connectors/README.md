# ShanhaiGate Terrain Connectors

These sprites are small cover pieces for hiding seams between graybox terrain skins.
Keep the original graybox `BoxCollider2D` objects as the gameplay source of truth.
Connectors should use only `SpriteRenderer`; do not add colliders.

## Sprite Set

- `Platform_End_Left.png`: visible left broken end of a platform.
- `Platform_End_Right.png`: visible right broken end of a platform.
- `Platform_Wall_Corner_Left.png`: platform meeting a wall on the left side.
- `Platform_Wall_Corner_Right.png`: platform meeting a wall on the right side.
- `Pit_Platform_Lip_Left.png`: left transition from safe platform into a pit or trap.
- `Pit_Platform_Lip_Right.png`: right transition from pit or trap back into safe platform.

## Placement Rules

- Sorting order: use `6` to `9`, above the main terrain skin.
- Parent: create an `Art_Connector_*` child under the graybox terrain object, or put it under a scene-level `Art_TerrainConnectors` object.
- Alignment: match the connector's grass/stone top to the collider's top edge.
- Overlap: push the connector `0.1` to `0.3` Unity units into both neighboring pieces so it covers the seam.
- Scale: adjust only enough to match the graybox thickness. Avoid large non-uniform stretching.

## Common Joins

- Platform to trap:
  - Put `Pit_Platform_Lip_Left.png` on the left edge of the trap.
  - Put `Pit_Platform_Lip_Right.png` on the right edge of the trap.
  - The lip should visually sit on top of the trap rim, but the trap collider stays unchanged.

- Platform to wall:
  - If the wall is on the platform's left side, use `Platform_Wall_Corner_Left.png`.
  - If the wall is on the platform's right side, use `Platform_Wall_Corner_Right.png`.
  - Place it in front of both the platform skin and wall skin.

- Exposed platform end:
  - Use `Platform_End_Left.png` or `Platform_End_Right.png`.
  - Let the broken stones hang slightly outside the collider for a natural silhouette.

