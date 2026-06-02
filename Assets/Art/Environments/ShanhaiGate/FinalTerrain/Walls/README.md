# ShanhaiGate Wall Sprites

These wall sprites match the `Connectors` set: bright forest ruin stone, moss, vines, and small pink flowers.
Use them as visual skins only. Keep the graybox `BoxCollider2D` objects for gameplay collision.

## Sprite Set

- `Wall_Body_Tall.png`: narrow vertical wall skin for tall graybox wall objects.
- `Wall_Block_Thick.png`: thick square wall/block skin with a grassy top.
- `Wall_Side_Cap_Left.png`: exposed rounded wall side for a left-facing edge.
- `Wall_Side_Cap_Right.png`: exposed rounded wall side for a right-facing edge.
- `Wall_Top_Cap.png`: mossy wall top/parapet, good for platform-to-wall joins.
- `Wall_Base_Rooted.png`: decorative cracked base with roots and plants.

## How To Connect

- Platform to wall:
  - Use `Platform_Wall_Corner_Left.png` or `Platform_Wall_Corner_Right.png` from `../Connectors`.
  - Put the corner connector in front of both platform and wall skins.
  - Use `Wall_Top_Cap.png` when the wall needs a wider mossy top.

- Wall side edge:
  - Use `Wall_Side_Cap_Left.png` or `Wall_Side_Cap_Right.png` to hide a hard vertical cut.
  - Let leaves/vines hang slightly outside the collider.

- Tall wall:
  - Apply `Wall_Body_Tall.png` to the wall graybox.
  - Align the visual side to the wall collider side.
  - If the top is visible, add `Wall_Top_Cap.png` or a platform-wall corner connector above it.

## Recommended Renderer Settings

- Main wall skin sorting order: `3` to `5`.
- Connector/cap sorting order: `6` to `9`.
- Do not add new colliders to these art pieces.

