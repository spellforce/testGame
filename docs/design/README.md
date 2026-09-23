# Iron Horizon UI and Art Direction

## Purpose

This package defines the visual and interface system for a tactical tabletop card game about combat vehicles crossing a hostile wasteland. It takes inspiration from the tactile, dense, direct-manipulation rhythm of tabletop card games: cards are large enough to read at a glance, stack naturally, lift when held, and occupy a generous open play surface. Its art, palette, materials, information framing, and interaction language are original to Iron Horizon.

This is a presentation specification, not a rules document. It establishes how future vehicle, enemy, searchable building, event, fixed building, and equipment content should look and behave before the game-specific fields are finalized.

## North Star

The player is a convoy commander working from a battered field table. The screen should feel like a physical planning surface set into an armored dashboard: dusty, practical, scarred, and legible under pressure. Every visual choice must serve recognition, action, or atmosphere.

- Cards are the primary objects, not list entries.
- The tabletop is a workspace, not a background illustration.
- Materials communicate ownership and danger before text does.
- Wear is controlled and readable; it never hides functional information.
- Motion confirms cause and effect, then gets out of the way.

## Design Principles

1. Read the board in one second. Player assets, threats, interactable sites, and temporary events have unique silhouettes and frame grammar.
2. Preserve physical handling. Cards retain a 5:7 footprint, clear overlap rules, and immediate lift/snap feedback.
3. Make steel feel used, not decorative. Use rivets, enamel, paint chips, dust, rubber, and heat marks as restrained functional materials.
4. Keep status local. Badges occupy fixed anchors and never shift the title or illustration.
5. Earn emphasis. Amber is action or heat, cyan is friendly/system information, olive is neutral field information, and red signals threat or damage.
6. Design for future data. Presentation components accept category, state, rarity/grade, art, title, and badge data without embedding gameplay logic.

## Document Map

| Document | Authority |
| --- | --- |
| [01 Visual Direction](01-visual-direction.md) | Art language, imagery, typography, texture, contrast |
| [02 Design Tokens](02-design-tokens.md) | Named colors, spacing, type, depth, motion tokens |
| [03 Tabletop Layout](03-tabletop-layout.md) | Reference resolution, board zones, scaling, z-order |
| [04 Card System](04-card-system.md) | Shared card anatomy, sizing, states, stack behavior |
| [05 Card Families](05-card-families.md) | Category-specific frames, silhouettes, card backs |
| [06 Interaction and Motion](06-interaction-motion.md) | Input feedback, animation choreography, reduced motion |
| [07 Menus and HUD](07-menus-and-hud.md) | Navigation, overlays, top bar, lower tray, modals |
| [08 Asset Production](08-asset-production.md) | Asset list, source sizes, exports, naming, batches |
| [09 Godot Handoff](09-godot-handoff.md) | Proposed scenes, resources, scripts, directory layout |
| [10 Validation Checklist](10-validation-checklist.md) | Visual, responsive, accessibility, and QA criteria |

## Scope Boundary

Included: UI geometry, visual roles, category presentation, asset direction, animation language, responsive behavior, and handoff architecture.

Deferred: card mechanics, economy, combat resolution, content taxonomy below the six card families, localization copy, narrative writing, and final platform input mapping.

## Reference Snapshot

| Item | Standard |
| --- | --- |
| Authoring viewport | 1920 x 1080 logical px |
| Core tabletop | 1728 x 746 px at 100% scale |
| Standard card | 180 x 252 px, 5:7 ratio |
| Default zoom | 1.00x; range 0.80x to 1.25x |
| Primary type | Condensed industrial display face plus neutral UI sans |
| Main material set | Oxidized steel, faded enamel, charcoal rubber, canvas dust |
| First-play intent | Desktop mouse-first; controller and touch remain structurally supported |

Use the named values in [02 Design Tokens](02-design-tokens.md) rather than creating near-duplicate colors, spacings, or animation durations.