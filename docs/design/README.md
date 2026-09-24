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
| [05 Card Families](05-card-families.md) | The six original card families (partially superseded by 11) |
| [06 Interaction and Motion](06-interaction-motion.md) | Controls and animations |
| [07 Menus and HUD](07-menus-and-hud.md) | HUD panels, menus, pause, settings |
| [08 Asset Production](08-asset-production.md) | Asset list, sizes, Aseprite workflow, naming |
| [09 Godot Handoff](09-godot-handoff.md) | Project settings, pixel rendering pipeline, scenes, resources |
| [10 Validation Checklist](10-validation-checklist.md) | Review criteria for the board, cards and pixel art |

### Interface and Content Design (this pass)

These cover the gameplay-facing UI: taxonomy, the card faces, the window system and the dialogs.

| Document | Authority |
| --- | --- |
| [11 Card Taxonomy](11-card-taxonomy.md) | 8 families, every gameplay card type mapped to one, packs, sub-cues, badge defaults |
| [12 Vehicle Card Face](12-vehicle-card-face.md) | The vehicle's three layers: face, stack, detail sheet. The highest-information card |
| [13 Window System](13-window-system.md) | The one window component: warehouse, cargo, shops, factory, camp, skills, pickup |
| [14 Facility Windows](14-facility-windows.md) | What each facility adds on top of the generic window |
| [15 Context Menus and Chips](15-context-menus-and-chips.md) | Whether right-click is redundant; chips vs. cards; the skill tray |
| [16 Garage and Expedition Boards](16-garage-and-expedition-boards.md) | The two board layouts, the band revision, pinning, settlement |
| [17 Dialogs](17-dialogs.md) | The four tiers, and what must never open a dialog |
| [18 Asset Additions](18-asset-additions.md) | New assets, the batch order, the avatar upload pipeline |
| [19 UI Validation](19-ui-validation.md) | Review criteria for documents 11 to 18 |
| [20 Questions and Errata](20-questions-and-errata.md) | Contradictions found and resolved; open questions for the designer |

**Read [20 Questions and Errata](20-questions-and-errata.md) before acting on any number in documents 01 to 10.** It records seven corrections to the original measurements, two of which change the board geometry.

Reference screenshots: `1.jpeg` and `2.jpeg` in the project root (Stacklands at minimum zoom, before and after panning).

Use the named values in [02 Design Tokens](02-design-tokens.md). Do not add colors outside the palette.
