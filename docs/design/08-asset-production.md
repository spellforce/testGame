# Asset Production

## Tools and Rules

- Draw in Aseprite (or LibreSprite / Pixelorama) with the palette `art/palette/iron_horizon_32.gpl` loaded and locked.
- Draw everything at 1x (1 art px = 1 pixel). Never upscale in the art tool; Godot does all scaling.
- Export PNG, indexed or RGBA, with no color profile. Keep the `.aseprite` source next to the export in a `src/` folder.
- Animations: one horizontal strip per animation, frames of equal size, 12 fps unless noted.

## Asset List

| Asset | Size (px) | Frames | Notes |
| --- | --- | --- | --- |
| Card frame, per family | 48 x 56 | 1 | Body, outline, separator, shape cue. No title or icon. |
| Card back, per family | 48 x 56 | 1 | Includes the 24 x 24 emblem |
| Card icons | up to 32 x 26 | 1 (2 to 4 for animated idle, optional) | One per card, transparent background |
| Badge plates | 13 x 9 | 1 | Bone, red, steel |
| Badge icons | 5 x 5 | 1 | Heart, coin, bolt, fuel, lock |
| Digit fonts | 3 x 5 and 5 x 7 per glyph | --- | 0 to 9, +, -, /; as a sprite font |
| State outlines | 50 x 58 | 1 | Cyan and red, drawn 1 px outside the card |
| Corner brackets | 3 x 3 | 1 | Cyan, 4 rotations drawn by hand |
| Timer bar | 40 x 5 | --- | 9-slice: frame, fill |
| Card flip | 48 x 56 | 4 | Per family: back, narrow, edge, face-mask |
| Dissolve mask | 48 x 56 | 6 | Shared, white on transparent, 4 x 4 blocks |
| Dust puff | 8 x 4 | 2 | |
| Sparks | 8 x 8 | 3 | |
| Scorch decal | 32 x 16 | 1 | |
| Board tile | 16 x 16 | 1 | Tiling deck plate |
| Board seam | 64 x 2 | 1 | |
| Board rail | 3 px 9-slice | --- | Corners 3 x 3 |
| Board wear decals | 4 to 24 | 1 | 6 to 10 variants (rust spots, stencils) |
| Slot plates | 60 x 81 | 1 | Active slot, locked slot "???" |
| New-content starburst | 7 x 7 | 2 | |
| World terrain tiles | 16 x 16 | 1 | 16 to 24 tiles incl. transitions (as a Godot TileSet) |
| World props | 16 to 64 | 1 | 20 to 30 props |
| Ambient animals | 16 x 16 to 24 x 24 | 4 to 6 | Vulture, dog, tumbleweed, dust devil |
| UI panel 9-slice | 16 x 16 source, 5 px corners | --- | Steel panel, light plate, inset |
| UI buttons | 9-slice 16 x 16 | 3 states | Normal, hover, pressed; primary, normal, danger |
| UI icons | 9 x 9 | 1 | Resources, pause, play, arrows, collapse |
| Checkbox, slider, scrollbar | see [07 Menus and HUD](07-menus-and-hud.md) | --- | |
| Cursors | see [07 Menus and HUD](07-menus-and-hud.md) | --- | Export at 1x, 2x, 4x (hardware cursors are not scaled by Godot) |
| Logo | about 320 x 64 | 1 | |

## Layered Cards

A finished card is built in Godot from layers. Artists never draw complete cards:

1. Shadow (a flat `ink` rectangle at 45%, drawn by code)
2. Card frame (per family)
3. Icon (per card)
4. Title text (`font.card`, drawn by code)
5. Badges (plate + digits, drawn by code)
6. State layers (outline, brackets, new dot)

So a new card needs just one 32 x 26 icon and one line of data.

## Icon Checklist

- [ ] Uses only palette colors; `ink` outline around the whole silhouette.
- [ ] Readable as a pure black silhouette at 1x.
- [ ] Light from the top left, 3 to 4 shades.
- [ ] No dithering, no isolated noise pixels.
- [ ] Contrasts with its family's body color (check vehicles on `cyan.1`, not on white).
- [ ] Fits in 32 x 26, visually centered (a wide vehicle can be 32 x 18).

## Naming

```text
<type>_<family-or-group>_<name>.png         (single image)
<type>_<family-or-group>_<name>_strip<N>.png  (animation strip with N frames)

card_frame_vehicle.png
card_back_enemy.png
card_icon_vehicle_scout_buggy.png
card_icon_enemy_raider_truck.png
fx_card_flip_event_strip4.png
fx_dissolve_strip6.png
world_prop_wrecked_car_01.png
world_animal_vulture_strip6.png
ui_panel_steel.png
ui_button_primary_hover.png
```

Lowercase snake_case, no spaces. The icon name after the family must match the card's ID in the game data.

## First Batch (In Order)

1. Palette file and the board tile, seam and rail. They set the value range.
2. The 6 card frames and 6 card backs.
3. 1 icon per family (6 icons): the style reference for all later icons.
4. Badge plates, digit fonts, state outlines, timer bar.
5. World terrain tiles and 10 props, enough to fill the area around the board.
6. UI panel kit, buttons, 10 UI icons, cursors.
7. Effects: dust, sparks, flip, dissolve.
8. Ambient animals, logo.

After step 3, put the board and all 6 cards in Godot and review at zoom 0.5 and 1.0 on a 1080p screen before drawing more.
