# Canvas and Layout

## Stacklands Reference Measurements

Taken from `1.jpeg` and `2.jpeg` (1920 x 1080, minimum zoom, board in the same place except for a pan). These are reference measurements, not replacement art-pixel tokens:

| Item | Measured (screen px) | Ratio / engine reference |
| --- | --- | --- |
| Card | about 50 x 58–61 | engine collider 0.42 x 0.495 units; the collider is not the sprite footprint |
| Board width | 1075 (top edge) to 1137 (bottom edge), average 1106 | engine 8.86 units; about 22.1 measured card widths |
| Board height | about 637 | engine 5.175 units |
| Top slot band (inside the board) | about 79 tall | 1.43 measured card heights |
| Slots in band | 9 slots, 67 px pitch | pack is about 1.26 measured card widths |
| Board on screen at min zoom | about 58% of screen width | --- |
| Left panel column | 344 wide, 8 px margin | 18% of screen width |
| Top-right boxes | 257 x 54 and 364 x 54 | --- |

The measured card bounds include capture/outline/shadow variation. The binding design asset remains 48 x 56 art px at prefab scale 100%; do not change the prefab dimensions to compensate for camera projection. Stacklands cards are camera-facing billboards, while the board is a perspective ground surface. That is why the board can taper while the card face remains rectangular.

The reference camera is a mild perspective view rather than a flat orthographic top-down view. The exact pitch and field of view must be calibrated from projected board corners during the Godot prototype; do not infer them from card size alone.

The current 2D implementation remains the baseline. A 3D presentation prototype is planned in [game-update.md](game-update.md), with a go/no-go gate for perspective, billboard readability, pointer projection, and performance. Ratios measured in screen space stay valid as reference targets, but the 3D prototype must be judged by projected geometry rather than by forcing every card to one screen size.

## World Canvas

```text
+--------------------------------------------------------------+  World canvas 2208 x 1288 art px
|  wasteland terrain, props, ambient animals                   |  (drag empty space to pan)
|                                                              |
|          +------------------------------------------+        |
|          | [pk][pk][pk][pk][??][??][??][??][??]      | 104    |  Top slot band, row 1
|          | [sk][sk][sk][sk][sk][sk][sk][ ][ ]      |  60    |  row 2 (skills / reserved)
|          |------------------------------------------|        |
|          |                                          |        |
|          |       CARD PLAY AREA 1104 x 480          |        |  Board 1104 x 644
|          |                                          |        |  (23 x 11.5 cards)
|          +------------------------------------------+        |
|                                                              |
+--------------------------------------------------------------+
```

| Area | Size (art px) | Position |
| --- | --- | --- |
| World canvas | 2208 x 1288 | Origin (0, 0) |
| Board | 1104 x 644 | Centered: x 552 to 1656, y 322 to 966 |
| Board rail | 3 px, inside the board bounds | --- |
| Top slot band | 1104 x 164 | Top of the board |
| Band divider | 2 px (`ink` + `steel.7`) | Bottom of the band, at y 164 |
| Play area | 1104 x 480 (the rail sits inside this) | Below the band |

**Errata E3**: the band was 104 and the play area 538. The expedition's skill tray needs a second row, so the band grows to 164. Arithmetic: `14 + 81 + 9 + 48 + 12 = 164`; the play area is `644 - 164 = 480`. See [20 Questions and Errata](20-questions-and-errata.md) and [16 Garage and Expedition Boards](16-garage-and-expedition-boards.md).

### Top Slot Band

Mirrors Stacklands' row of packs and the sell slot. What the slots do is a gameplay decision; this is only the layout.

- **Row 1**: 9 slots of 60 x 81, 4 px apart (64 px pitch). Row width 560 px, centered. Slot top at 14 px below the board top; 9 px below the slot to row 2.
- **Row 2**: 40 x 48 chips at 48 px pitch, holding the expedition skill tray or the garage's reserved slots. Top at y 104, bottom at y 152, then 12 px to the divider. See [15 Context Menus and Chips](15-context-menus-and-chips.md).
- Slot styles: active slot (dark plate, icon, price in `font.digits.small`); locked slot (`steel.2` with "???" in `font.card`).
- New-content marker: a 7 x 7 px starburst badge overlapping the slot's top-right corner, as in Stacklands. It applies to both rows.

### Card Bounds

- Cards stay inside the play area. While held, Stacklands clamps to the loose board bounds (8.86 x 5.175 units); at rest it clamps to the tight bounds, which are 0.1 units (11 px) inside. The clamp margin is 11 px from the edge.
- A card dropped outside the play area (including the world or the slot band, unless the slot accepts it) slides back inside over `motion.standard`.
- Nothing interactive is ever placed in the world outside the board.

## Camera

### Zoom

The logical board coordinate system remains art pixels. The current renderer uses a continuous `Camera2D` scale. The planned Stacklands-style renderer uses a `Camera3D` height/FOV pair and projects the same logical board onto a horizontal plane; these are two presentation implementations of the same coordinates.

For the current 2D baseline:

- screen px per art px = `z × screen_height / 540`;
- `z = 0.5` is the 100% native-art presentation at 1080p (one screen pixel per art pixel);
- the card prefab itself stays 48 x 56 at `scale = 1`; camera zoom changes its screen size globally;
- zoom targets are continuous and eased over 120 ms; named min/default/max values are calibration tokens, not prefab scale percentages.

For the 3D prototype:

- zoom changes camera height (the Stacklands model), not card `scale`;
- cards are camera-facing billboards and the board is a horizontal perspective surface;
- zoom toward the cursor is solved by intersecting the cursor ray with the board plane before and after changing height;
- pitch, FOV, and height range must be recorded from the prototype capture, then promoted to tokens only after the decision gate in [game-update.md](game-update.md).

| Presentation | Minimum reference target | Default target | Maximum target |
| --- | --- | --- | --- |
| 2D baseline `z` | 0.5 | 1.0 | 2.0 (current documented range; tune only after capture comparison) |
| 3D prototype | calibrated camera height | calibrated camera height | calibrated camera height |

Do not change the 48 x 56 prefab dimensions or add per-card scale factors to match the approximately 50 x 58–61 px reference measurements. The reference card bounds include outlines and capture variance, while the logical art footprint is the stable design token.

At 1080p and 4K, the 2D baseline's pixel-perfect setting may snap screen pixels/art pixels to whole values. In 3D, perspective means projected size can vary with board depth; nearest filtering and native card assets remain mandatory, but a uniform screen size is not a valid acceptance criterion.

### Pan

- Drag with the left mouse button on anything that isn't a card (board or world) to pan, as in Stacklands.
- WASD / arrow keys also pan, at 600 art px/s divided by z.
- Clamp: the center of the view must stay inside the board rectangle. The board can never leave the screen entirely.
- The camera never moves on its own, except a short 250 ms nudge when an event card lands out of view.

## HUD Placement

The HUD sits over the world on a 960 x 540 UI canvas (UI px), integer-scaled: 1 UI px = 2 screen px at 1080p. Full specs are in [07 Menus and HUD](07-menus-and-hud.md).

```text
UI canvas 960 x 540
+--------------+-------------------------------------------------------+
| [TabA][TabB]<|                         [Resources 128x27][Time 182x27]|
| List panel   |                                                       |
| 172 x 380    |                                                       |
|              |               (world and board show through)         |
|              |                          PAUSED                       |
|--------------|                                                       |
| Info panel   |                                                       |
| 172 x 147    |                                                       |
+--------------+-------------------------------------------------------+
```

| Element | Position (UI px) | Size |
| --- | --- | --- |
| List panel (tabs + list) | x 4, y 4 | 172 x 380 |
| Collapse button | right edge of list panel, y 6 | 18 x 18 |
| Info panel (hovered card details) | x 4, y 389 | 172 x 147 |
| Resource box | x 640, y 4 | 128 x 27 |
| Time box (with pause button) | x 774, y 4 | 182 x 27 |
| PAUSED label | screen center | `font.ui.heading` |

On screens wider than 16:9, the left column stays at the left and the top-right boxes stay at the right; the world fills the rest.

## Draw Order (Back to Front)

1. World terrain
2. World props and ambient animals
3. Board plate, rail, slot band
4. Card shadows
5. Resting cards and stacks (the most recently moved stack draws on top)
6. Timer bars above stacks
7. Held card(s) and cards in flight
8. World effects (dust, sparks, damage numbers)
9. HUD
10. PAUSED label and menus
11. Cursor
