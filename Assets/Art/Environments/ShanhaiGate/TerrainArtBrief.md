# ShanhaiGate Terrain Art Brief

## Target

Make ShanhaiGate terrain feel like a soft, painterly forest ruin platformer:

- Thick readable platforms.
- Clear grassy playable top edge.
- Stone or ruin underside below the collision line.
- Hanging vines and small flowers as decoration.
- Gentle teal, mint, pink, blue-gray, and warm dusk accents.
- Airy and inviting, not horror, not cave-dark, not random pixel rubble.

## Reference Read

The supplied references share these traits:

- Terrain is thick, but the playable top edge stays simple and readable.
- The top edge is usually a grass or moss band.
- Stone blocks underneath are grouped into large calm shapes, not noisy tiny texture.
- Foliage overlaps the terrain silhouette, but does not hide where the player lands.
- Foreground details are darker; playable surfaces are brighter.
- Backgrounds are misty and low contrast, but this pass should not create backgrounds yet.

## What We Should Not Do

- Do not stretch a concept-art platform over every graybox object.
- Do not generate random tiles and call them final.
- Do not force the art height to match the graybox collider height.
- Do not let flowers, vines, or dark silhouettes cover the exact landing edge.
- Do not bake background trees or sky into terrain sprites.

## Terrain Sprite Requirements

Each terrain sprite should be a transparent PNG used as a SpriteRenderer cover.

Platform sprites:

- Top edge should be flat enough to align with `BoxCollider2D` top.
- Art may extend downward below the collider to show thickness.
- Top 10-15 percent should remain visually clean: grass, moss, stone lip, small flowers only.
- Underside can have stones, roots, vines, cracks, and depth shadows.

Wall sprites:

- Should fit vertical collision.
- Side edge should be readable for wall contact.
- Detail can be denser than platforms, but keep silhouette clean.

Hazard or pit rim sprites:

- Must not look like a safe floor unless it is actually safe.
- Use mist, blue-green depth, or broken rim treatment.
- Avoid strong horizontal top highlight if the player should not stand there.

## First Asset Set

Start with these only:

- `Platform_Long`: for `Start_ContinuousLand`, `Land01`, `Land02`.
- `Platform_Short`: for `Land06`, `Land07`, short jump platforms.
- `Block_Thick`: for `Land03`, `Land04`, `Land05`.
- `Wall_Vertical`: for `Wall01`, `Wall02`, `Wall03`.
- `Pit_Rim`: for `Gap*_FallCatch` or trap readability.

## Unity Placement Rule

- Keep graybox as collision.
- Add or update child `Art_TerrainSkin`.
- Align platform sprite top edge to collider top edge.
- Scale width to collider width.
- Preserve sprite aspect ratio so terrain can extend downward.
- Hide the parent graybox renderer only after art cover is in place.

