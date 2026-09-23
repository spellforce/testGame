# Iron Horizon: UI and Art Design Spec (Pixel Art)

## Purpose

This package defines the visual and interface system for a pixel-art tabletop card game about combat vehicles in a "wasteland and steel" world. Controls, animation behavior, card-to-board proportions and the canvas/board structure follow Stacklands. Colors, materials and art are original.

It covers presentation only. Card mechanics, economy and content are out of scope.

"Iron Horizon" is a placeholder title.

## Core Decisions

| Item | Decision |
| --- | --- |
| Art style | Pixel art, 32-color palette, 1 px dark outlines, no scaling or rotation of individual sprites |
| Art unit | 1 **art px** = one pixel in the source art. All world sizes are in art px. |
| Card | 48 x 56 art px |
| Board (card interaction area) | 1104 x 644 art px = 23 card widths x 11.5 card heights (engine: 8.86 x 5.175 units) |
| World canvas | 2208 x 1288 art px, board centered; drag empty space to pan |
| Camera | Straight top-down, orthographic. Stacklands' board is a horizontal 3D plane viewed at a tilt; we flatten that to a 2D grid. |
| Zoom | Continuous, 0.5 to 2.0 (engine camera height 3 to 11), x1.15 per wheel notch. At minimum zoom on 1080p, 1 art px = 1 screen px. |
| UI | Pixel UI on a 960 x 540 logical canvas, integer-scaled (2x at 1080p, 4x at 4K) |
| Fonts | CJK-capable pixel fonts: 12 px for UI, 10 px for card titles |
| Engine | Godot 4.3 or newer |

## Document Map

| Document | Authority |
| --- | --- |
| [01 Visual Direction](01-visual-direction.md) | Pixel-art rules, materials, world and board look |
| [02 Design Tokens](02-design-tokens.md) | Palette, fonts, spacing, motion values |
| [03 Canvas and Layout](03-canvas-layout.md) | Stacklands measurements, world/board sizes, zoom, pan, HUD placement |
| [04 Card System](04-card-system.md) | Card pixel anatomy, badges, states, stacking |
| [05 Card Families](05-card-families.md) | The six card types |
| [06 Interaction and Motion](06-interaction-motion.md) | Controls and animations |
| [07 Menus and HUD](07-menus-and-hud.md) | HUD panels, menus, pause, settings |
| [08 Asset Production](08-asset-production.md) | Asset list, sizes, Aseprite workflow, naming |
| [09 Godot Handoff](09-godot-handoff.md) | Project settings, pixel rendering pipeline, scenes, resources |
| [10 Validation Checklist](10-validation-checklist.md) | Review criteria |

Reference screenshots: `1.jpeg` and `2.jpeg` in the project root (Stacklands at minimum zoom, before and after panning).

Use the named values in [02 Design Tokens](02-design-tokens.md). Do not add colors outside the palette.
