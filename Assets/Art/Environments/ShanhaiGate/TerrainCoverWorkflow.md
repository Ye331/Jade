# ShanhaiGate Terrain Cover Workflow

This scene should keep graybox objects as collision and place art as child cover sprites.

## Rule

- Parent terrain object keeps `BoxCollider2D`.
- Parent graybox `SpriteRenderer` can be hidden.
- Child `Art_TerrainSkin` holds the visual terrain sprite.
- For platforms, the art sprite top edge aligns to the collider top edge.
- The art sprite may extend downward below the collider to create thick ground.
- Do not use background art in this pass.

## Unity Steps

Preferred window workflow:

1. Put final terrain sprites under `Assets/Art/Environments/ShanhaiGate/FinalTerrain`.
2. Open `Jade/ShanhaiGate Terrain Cover Window`.
3. Drag the five final terrain sprites into `Platform Long`, `Platform Short`, `Block Thick`, `Wall Vertical`, and `Pit Rim`.
4. Select one or more terrain GameObjects in the Hierarchy, such as `Land01`.
5. Click `Apply Typed Covers To Selected Terrain`.
6. Check that the playable top edge still matches the original collider.

Quick selection workflow:

1. Select one final terrain `Sprite` asset in the Project window.
2. Also select one or more terrain GameObjects in the Hierarchy.
3. Run `Jade/Apply Selected Sprite As Terrain Cover`.

Single-sprite window workflow:

1. Drag one final terrain `Sprite` into `Terrain Sprite`.
2. Select one or more terrain GameObjects in the Hierarchy.
3. Click `Apply To Selected Terrain`.

## Asset Requirements

- Transparent PNG.
- Side-view platform or wall sprite.
- Clear flat top edge for platforms.
- Extra thickness should extend downward, not upward.
- No baked background.
- No cast shadow outside the sprite silhouette.
- Keep decoration away from the exact top edge when it would confuse the landing surface.

## Window Controls

- `Width Multiplier`: use small changes only, usually `0.95` to `1.05`.
- `Height Multiplier`: use this when the terrain should feel thicker or thinner without changing collision.
- `Top Edge Y Offset`: use this only if the art's visible top edge is not exactly at the sprite's top bound.
- `Wall X Offset`: use this for vertical walls where the contact side needs slight visual adjustment.

## Typed Cover Mapping

- `Wall*` objects use `Wall Vertical`.
- `Gap*` and `Trap*` objects use `Pit Rim`.
- Objects with collider world height `>= 1.5` use `Block Thick`.
- Remaining objects with collider world width `>= 8` use `Platform Long`.
- Smaller remaining objects use `Platform Short`.
