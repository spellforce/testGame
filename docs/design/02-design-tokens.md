# Design Tokens

These are the only colors, fonts, sizes and timings to use. [09 Godot Handoff](09-godot-handoff.md) explains where they live in the project.

## Palette (32 Colors)

Save the palette as `art/palette/iron_horizon_32.gpl` (GIMP palette, which Aseprite also reads) and lock every sprite to it.

| Token | Hex | Main uses |
| --- | --- | --- |
| `ink` | `#0E1110` | All outlines, dark text, pixel shadows |
| `steel.1` | `#1B211F` | Deepest shadow on steel |
| `steel.2` | `#252A28` | Fixed-building card body, UI panel body |
| `steel.3` | `#353A36` | UI raised controls, dark metal |
| `steel.4` | `#4B524D` | Board rail inner line, stencils, muted UI text on bone |
| `steel.5` | `#66706A` | Board tread and seams |
| `steel.6` | `#8E968D` | **Board base** |
| `steel.7` | `#B8BEB0` | Metal highlights, board rail, secondary light text |
| `bone` | `#D7D5BF` | Equipment card body, light UI plates, main UI text |
| `white` | `#EEF0DF` | Light text on cards, brightest highlights, hit flash |
| `sand.0` | `#3B3328` | Deep world shadow |
| `sand.1` | `#5E5240` | World shadows |
| `sand.2` | `#857657` | **World terrain base** |
| `sand.3` | `#B3A485` | World highlights, dust particles |
| `sand.4` | `#D8CBA6` | Sun-bleached highlights (sparingly) |
| `rust.0` | `#3A1D17` | Rust shadow |
| `rust.1` | `#6E2F22` | Rust, enemy card shading |
| `rust.2` | `#A83B32` | **Enemy card body**, health badge |
| `rust.3` | `#C94D3C` | Enemy highlights, invalid outline |
| `rust.4` | `#EC8B78` | Red text on dark UI, damage numbers |
| `amber.0` | `#5C3514` | Amber shadow |
| `amber.1` | `#A7641F` | Event card shading, pressed buttons |
| `amber.2` | `#E9A93A` | **Event card body**, timer bars, primary buttons |
| `amber.3` | `#F5D27A` | Headlamps, sparks, amber highlights |
| `olive.0` | `#2E3522` | Olive shadow |
| `olive.1` | `#56613C` | **Searchable-building card body** |
| `olive.2` | `#98A66B` | Olive highlights, positive numbers |
| `cyan.0` | `#1E3B40` | Cyan shadow |
| `cyan.1` | `#2B5F65` | **Vehicle card body** |
| `cyan.2` | `#3D878E` | Vehicle highlights |
| `cyan.3` | `#8ECED1` | Selection and focus outline, friendly UI accent |
| `hazard` | `#D5C15A` | Equipment hazard stripe, warnings |

### Semi-Transparent Exceptions

| Token | Value | Use |
| --- | --- | --- |
| `shadow.card` | `ink` at 45% | Card drop shadow |
| `overlay.pause` | `ink` at 35% | Dims the world while paused |
| `overlay.modal` | `ink` at 75% | Behind modal menus |

### Checked Contrast (WCAG)

| Pair | Ratio |
| --- | --- |
| `white` on `cyan.1` (vehicle title) | 6.2 |
| `white` on `rust.2` (enemy title) | 5.4 |
| `white` on `olive.1` (searchable title) | 5.7 |
| `ink` on `amber.2` (event title) | 9.2 |
| `white` on `steel.2` (fixed-building title) | 12.6 |
| `ink` on `bone` (equipment title) | 12.8 |
| `bone` on `steel.2` (UI text on panels) | 9.8 |
| `rust.4` on `steel.2` (red UI text) | 5.9 |

Card bodies against the board are 1.4 to 4.8. Cards stand out from the board through their `ink` outline and drop shadow, not body color alone. Stacklands does the same with its cream cards.

## Fonts

Use free pixel fonts that cover Chinese. Both options below are released under the SIL Open Font License; confirm the license of the exact version you download.

| Token | Font | Size | Use |
| --- | --- | --- | --- |
| `font.card` | Fusion Pixel Font 10px | 10 px | Card titles |
| `font.ui` | Fusion Pixel Font 12px or Ark Pixel Font 12px | 12 px, line height 16 | All UI text |
| `font.ui.heading` | `font.ui` drawn at 2x | 24 px | Menu titles, PAUSED label |
| `font.title` | `font.ui` drawn at 4x, or a custom logo | 48 px | Title screen |
| `font.digits.small` | Custom 3 x 5 digit sprite font | 5 px tall | Card badges |
| `font.digits` | Custom 5 x 7 digit sprite font | 7 px tall | Damage numbers, stack count |

Rules:

- Only draw fonts at their native size or whole multiples.
- A card title fits 4 Chinese characters or about 8 Latin letters. Longer names must be shortened in the card data (full name shows in the info panel).

## Spacing

World (art px): base unit 4. UI (UI px): base unit 4.

| Token | Value |
| --- | --- |
| `space.1` | 1 px |
| `space.2` | 2 px |
| `space.4` | 4 px |
| `space.8` | 8 px |
| `space.12` | 12 px |
| `space.16` | 16 px |

## Card and Board Constants (Art px)

| Token | Value |
| --- | --- |
| `card.size` | 48 x 56 |
| `card.header` | 12 (title row) |
| `stack.offset` | 12 (the header stays visible). **Pending E6**: a 12 px offset exactly covers the 12-row header, so the 1 px separator of every lower card is hidden. If that matters, the value is 13 and three documents change. Decide at the first art review — see [20 Questions and Errata](20-questions-and-errata.md) |
| `board.size` | 1104 x 644 |
| `board.slot_band` | 164 (top row of slots + skill tray row) — see **E3** in [20 Questions and Errata](20-questions-and-errata.md) |
| `world.size` | 2208 x 1288 |
| `drop.snap_radius` | 23 (special drop zones only; stacking uses overlap) |
| `grid.cell` | 86 x 96 (align-to-grid action; deliberately larger than a card, so aligned cards show gaps — see **E5** in [20 Questions and Errata](20-questions-and-errata.md)) |

## Motion

| Token | Value | Use |
| --- | --- | --- |
| `motion.instant` | 60 ms | Hover response |
| `motion.fast` | 120 ms | Lift, UI press |
| `motion.standard` | 200 ms | UI panel moves |
| `motion.slow` | 350 ms | Card launch from a pack, panel slide |
| `motion.frame` | 83 ms | One frame of a 12 fps sprite animation (flashes, dissolve) |
| `lift.hover` | 7 px | Card moves up on hover |
| `lift.drag` | 11 px | Card moves up while held; shadow stays on the ground |
| `shadow.rest` | +1, +2 px | Shadow offset at rest (x, y) |
| `shadow.drag` | +2, +6 px | Shadow offset while held |
| `zoom.min` / `zoom.default` / `zoom.max` | 0.5 / 1.0 / 2.0 | See [03 Canvas and Layout](03-canvas-layout.md); these are presentation targets, not prefab scale percentages |
| `zoom.step` | x1.15 per wheel notch, eased over 120 ms | 2D baseline token; 3D camera-height calibration is gated by [game-update.md](game-update.md) |
| `push.speed` | 228 art px/s | Root cards push overlapping cards apart, scaled by mass |
| `pickup.snap` | 23 art px | Snap radius for packs and the sell box |
| `send.speed` | 514 art px/s sideways, 571 up | Cards thrown out by packs and machines |

Easing, gravity, drag and bounce are the game's real physics values, listed in [06 Interaction and Motion](06-interaction-motion.md). Cards settle with `lerp(pos, target, dt * 20)`; thrown cards use gravity 3429 px/s2, air drag x0.93 per 20 ms and bounciness 0.6.

`motion.*` durations are for UI only. Card motion is driven by the physics constants, not by fixed durations.
