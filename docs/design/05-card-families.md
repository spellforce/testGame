# Card Families

All families share the 180 x 252 px footprint and anatomy from [04 Card System](04-card-system.md). Each family differs in category tab, frame accent, rail, art direction, and card back. A player should identify any family at 0.80x zoom from shape cues alone, without color.

## Summary Matrix

| Family | Tab shape | Accent token | Frame accent | Footer rail | Behavior class |
| --- | --- | --- | --- | --- | --- |
| Vehicle | Rectangle with tread notch | `accent.cyan` | Heavy lower fender plate | Horizontal cyan rail | Movable, player-owned |
| Enemy | Jagged downward tab | `state.threat` | Asymmetric chipped frame | Broken red dash rail | Movable, hostile |
| Searchable building | Tag with keyed notch | `accent.olive` | Locator bolt at each top corner | Olive hatch rail | Usually stationary, interactable |
| Event | Torn dispatch strip across full top | `accent.amber` | Torn paper upper edge | None (dispatch band replaces it) | Transient |
| Fixed building | Broad low plate | `line.steel` + `material.bone` | Riveted thick base, 12 px | Rivet row | Stationary, placed |
| Equipment | Narrow vertical strip at left edge | `accent.yellow` | Thin technical frame, mount notches | Yellow/black hazard ticks | Attachable |

## Vehicle

- **Identity**: the player's convoy. Must feel sturdy, owned, and ready.
- **Frame**: standard 8 px steel frame, lower 16 px becomes a heavier fender plate with two bolt heads. Cyan 3 px ownership rail across the footer center.
- **Tab**: 24 x 20 px cyan rectangle with a small tread notch cut from its lower edge. Icon: wheel or chassis silhouette.
- **Art**: low 3/4 angle, chassis fills 60% to 70% of art window, headlamp or weapon catches light. Background: dusty plain in muted olive/beige, low horizon.
- **Title plate**: pale serial stencil option (e.g., small `MK-II`-style suffix in `type.ui.xs` next to the name) supported but optional.
- **Distinct states**: selected/owned glow in cyan; damaged state adds persistent frame scratches and a smoke wisp overlay on art (optional art layer).
- **Card back**: dark steel with cyan convoy emblem (stylized wheel inside a shield).

## Enemy

- **Identity**: hostile raiders, drones, mutated machines. Immediately threatening.
- **Frame**: same steel frame with one or two chipped/broken segments (fixed per art, not random each frame). Upper-right corner has a 10 px notch cut.
- **Tab**: jagged red tab hanging down 4 px lower than other families. Icon: reticle or threat mark.
- **Art**: confrontational angle, subject faces or charges the viewer, stronger contrast and rim light. Subject may break the art-window upper edge by up to 10 px.
- **Footer**: broken red dash rail; left badge defaults to threat-style chipped plate.
- **Distinct states**: aggressive/attacking uses an amber-red corner pulse (`motion.loop`), only for the most urgent enemy.
- **Card back**: rust-red emblem of a crossed reticle on scorched steel.

## Searchable Building

- **Identity**: ruins, wrecks, depots, and bunkers that can be explored or looted.
- **Frame**: steel frame with a small olive locator bolt at each top corner.
- **Tab**: olive tag shape with a keyed notch on the right side. Icon: crate, hatch, or magnifier-style scanner.
- **Art**: single facade or wreck, clear point of entry (door, hatch, breach). Quieter value range than vehicles and enemies.
- **Info panel**: search progress bar (amber) or remaining-search pips (up to 5, 8 px each).
- **Distinct states**: `sealed` (hatch icon closed), `searching` (progress bar fills, dust particles at base), `depleted` (art desaturated 60%, hatch icon open and empty).
- **Card back**: olive-gray with stenciled grid coordinates and a hatch emblem.

## Event

- **Identity**: storms, radio signals, ambushes, supply drops. Temporary and attention-grabbing.
- **Frame**: upper edge is a torn dispatch band spanning full card width (the tab is integrated into this band). Band color `accent.amber` with dark stencil text.
- **Art**: graphic, high-contrast composition — a large central symbol plus one environmental cue. More poster-like than other families.
- **Footer**: no rail; may show a countdown badge.
- **Distinct states**: arrival (see motion doc) and expiry countdown via corner radial timer.
- **Card back**: amber-and-charcoal hazard-striped back with a radio mast emblem. Events are often revealed from this back with a flip.

## Fixed Building

- **Identity**: player-constructed or permanent structures: garage, turret, workshop, fuel depot.
- **Frame**: heavier base; lower 12 px is a thick riveted plate with 5 visible rivets, visually "bolted to the table".
- **Tab**: broad, low 36 x 14 px plate in `material.bone` with a dark icon. Icon: wrench, tower, or tank silhouette.
- **Art**: low center of mass, structure touches the lower art-window edge, pipes/gantries/walls give grounding.
- **Shadow**: uses a tighter, darker contact shadow than other cards (they do not lift as high).
- **Behavior presentation**: can be dragged only if gameplay allows; when locked, drag attempts show a small 3 px shake and lock glyph.
- **Card back**: rarely seen; bone-and-steel blueprint pattern.

## Equipment

- **Identity**: weapons, armor plates, engines, scanners attached to vehicles.
- **Frame**: thinner 6 px technical frame; left edge carries a 10 px vertical yellow strip with two mount notches.
- **Tab**: integrated into the left strip. Icon: bolt, gun barrel, plate, or gear.
- **Art**: object portrait on dark technical backdrop, subtle blueprint grid at 6% opacity.
- **Footer**: yellow/black hazard tick rail; badges show equipment stats.
- **Attachment presentation**: when attached to a vehicle, the card sits behind the vehicle with its header and left strip visible (28 px offset per [03 Tabletop Layout](03-tabletop-layout.md)). A cyan connector tick links host and equipment.
- **Card back**: yellow-and-steel crate stencil.

## Grades / Rarity (Optional Hook)

If future design needs grades, express them with a frame edge insert only:

| Grade | Treatment |
| --- | --- |
| Standard | No insert |
| Refitted | 1 px `material.bone` inner line |
| Military | 1 px `accent.amber` inner line + corner rivets |
| Prototype | 1 px `accent.cyan` inner line + subtle scanline overlay at 5% |

Do not change card size or family color based on grade.