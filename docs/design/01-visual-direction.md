# Visual Direction

## Tone

A convoy kept alive with scavenged parts: sun-bleached paint, rusted plates, welded repairs, dust in every seam. The board is the commander's steel deck plate. Around it lies the wasteland: dunes, dead trees, wrecked cars, tire piles, broken pylons.

Stacklands pairs a calm, light board with a busy illustrated world around it. Keep that structure. Swap the soft green meadow for a warm, dusty wasteland and a cool gray steel board.

## Pixel-Art Rules

1. **Palette**: use only the 32 colors in [02 Design Tokens](02-design-tokens.md). Transparency is on or off; the only exceptions are the shadow and overlay tokens.
2. **Outlines**: every card and interactive object has a 1 px `ink` outline. Props in the world may use selective outlines (a darker shade of their own ramp) so they sit behind the cards visually.
3. **No mixels**: never scale or rotate an individual sprite. Zoom is the only scaling, and it applies to the whole world at once. Animation uses translation, frame swaps and palette flashes.
4. **Shading**: 3 to 4 shades per material. Light comes from the top left. No pillow shading (light in the middle, dark all around).
5. **Anti-aliasing**: manual only, on curves in large sprites (48 px or bigger). None in icons 32 px or smaller.
6. **Dithering**: only on large, flat areas (terrain, sky in key art, board plate wear). Never inside card icons.
7. **Clusters**: avoid single isolated pixels ("pixel noise"), except deliberate highlights like a headlamp glint or rivet.
8. **Resting positions**: cards at rest snap to whole art pixels so they stay crisp (see [06 Interaction and Motion](06-interaction-motion.md)).

## Materials

| Material | Ramp | Notes |
| --- | --- | --- |
| Steel (board, frames, vehicles) | `steel.1` to `steel.7` | Cool gray; rivets are 1 px highlight + 1 px shadow |
| Rust | `rust.0` to `rust.4` | On edges and bolts of old metal; also the enemy color family |
| Sand and dust | `sand.0` to `sand.4` | World terrain, dust particles |
| Paint: cyan | `cyan.0` to `cyan.3` | Player-owned vehicles, focus, friendly UI |
| Paint: amber | `amber.0` to `amber.3` | Events, actions, heat, timers |
| Paint: olive | `olive.0` to `olive.2` | Searchable buildings and military surplus |
| Enamel / bone | `bone`, `white` | Labels, equipment, light text |
| Hazard yellow | `hazard` | Equipment stripes, warning marks |

## Board Look

- Base: `steel.6` deck plate, tiled from a 16 x 16 pattern with faint `steel.5` diamond tread.
- Plate seams every 64 art px, like large deck panels: a 1 px `steel.5` line with a 1 px `steel.7` highlight below it.
- Sparse wear: small rust spots (`rust.1`, 2 to 4 px), dust along the bottom edge, a few faded stencil numbers in `steel.4`.
- Rail: 3 px border around the board: `ink`, `steel.7`, `steel.4` (outside to inside). This replaces Stacklands' white rounded outline.
- Keep the board calm: wear marks cover less than 5% of its area. Cards must be the busiest thing on it.

## World Look (Outside the Board)

- Terrain: base `sand.2`, shadows `sand.1`, highlights `sand.3`; 16 x 16 tiles with dithered transitions, cracked earth, tire tracks.
- Props: wrecked cars, oil drums, dead trees, bones, tire piles, broken pylons, sandbag walls. Mostly 16 to 64 px.
- Ambient life, like Stacklands' cows and geese: vultures circling, a tumbleweed rolling, a scavenger dog, dust devils. At most 4 animated at once, 4 to 6 frame loops at 8 fps.
- The world is darker and warmer than the board, so the board always reads as the focus.

## Card Art (Icons)

Each card shows a central icon, like Stacklands. Ours are small pixel illustrations instead of line drawings.

- Size: up to 32 x 26 art px, centered in the card's art area.
- One subject, readable as a silhouette in pure `ink` on the card body color.
- Vehicles: side or 3/4 view, wheels and weapon clearly separated.
- Enemies: facing the viewer or charging; aggressive, uneven silhouette.
- Buildings: front view, entrance clearly visible.
- Events: a symbol (radio mast, storm cloud, flare, crate on a parachute).
- Equipment: a single object (gun, armor plate, engine, drill).

## Text

Text uses pixel fonts that support Chinese (see [02 Design Tokens](02-design-tokens.md)). Never smooth or scale text by non-integer factors in the UI. In the world, text scales with zoom like everything else.

## Accessibility

- Card title text meets 4.5:1 contrast against its card body (checked in [02 Design Tokens](02-design-tokens.md)).
- Each card family is identifiable without color, from its silhouette cue (see [05 Card Families](05-card-families.md)).
- No flashes faster than 3 per second. Hit flashes are 2 frames once.
