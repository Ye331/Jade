# ShanhaiGate Background Layers

Recommended default setup uses no foreground vine overlay. The gameplay area should stay clean and readable.

## Use These Layers

- `BG_Sky_Mist.png`
  - Sorting order: `-50`
  - Full rectangular sky/fog layer.

- `BG_Far_Ruins.png`
  - Sorting order: `-40`
  - Distant ruin silhouettes. Keep opacity around `0.7` to `0.9`.

- `BG_Mid_ForestRuins.png`
  - Sorting order: `-30`
  - Mid-distance forest and ruin shapes. Keep opacity around `0.3` to `0.5`.

- `BG_Near_FoliageFrame_Light.png`
  - Sorting order: `-15`
  - Soft edge framing. Keep it subtle so it does not compete with platforms.

## Do Not Use By Default

- `BG_Foreground_Vines.png`
- `BG_Foreground_Vines_Light.png`

These are too visually busy for the current level read. Keep them only as optional decorative pieces if you later crop individual vines manually.

## Preview

- `BG_Composite_NoForegroundVines_Preview.png`
  - Recommended look.

- `BG_Composite_GameplaySafe_Preview.png`
  - Older preview with a lighter vine overlay.

- `BG_Composite_Preview.png`
  - Older preview with too much foreground vine coverage.

## Unity Setup

Fast path:

Run `Jade/Create ShanhaiGate Dynamic Background`.

The tool creates `Art_Background` with the four recommended layers, assigns sorting orders, scales them to the main camera, and adds `ParallaxBackgroundLayer2D` for subtle motion.

Manual setup:

Create an empty `Art_Background` object, then add child objects:

- `BG_Sky`
- `BG_FarRuins`
- `BG_MidForestRuins`
- `BG_NearFoliageFrame`

Add a `SpriteRenderer` to each child and assign the matching sprite.
For long horizontal sections, duplicate the far/mid layers side by side instead of stretching one sprite too far.

## Motion Notes

`ParallaxBackgroundLayer2D` follows the main camera with a low parallax factor:

- Sky barely moves.
- Far ruins move slowly.
- Mid forest moves slightly faster.
- Near foliage moves the most, but still stays behind gameplay.

Each layer also has tiny drift and vertical floating. Keep the values subtle; the background should feel alive without distracting from jumps and hazards.
