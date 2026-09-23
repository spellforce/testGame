# Card System

## Shared Footprint

| Property | Value at 1.00x |
| --- | --- |
| Outer size | 180 x 252 px |
| Aspect ratio | 5:7 |
| Corner radius | 6 px (`radius.card`) |
| Outer frame | 8 px steel frame, 1 px `line.steel` edge light |
| Inner content area | 164 x 236 px |
| Hit area | Outer rect + 6 px forgiving margin |
| Source art resolution | 2x: 360 x 504 px card, 328 x 256 px art window |

All six families share this footprint so stacking, dragging, snapping, and hit testing stay uniform. Family identity comes from frame accents, tabs, and art, never from a different card size.

## Anatomy

```text
+------------------------------------+  y=0
| [cat tab]              [corner slot]|  0..28   Header strip (28 px)
| TITLE NAME (display.md / ui.md)    |
+------------------------------------+  28
|                                    |
|                                    |
|          ART WINDOW                |  28..156  (164 x 128 px)
|          164 x 128                 |
|                                    |
+------------------------------------+  156
| [type line / subtitle ui.sm]       |  156..176
+------------------------------------+
|                                    |
|   INFO PANEL (inset, 164 x 48)     |  176..224  Short descriptor or progress
|                                    |
+------------------------------------+  224
| [badge L]   [family rail]  [badge R]|  224..252  Footer (28 px) value badges
+------------------------------------+  252
```

Measurements are from the outer card top; horizontal content respects the 8 px frame.

### Zone Rules

- **Header strip**: title left aligned after the 24 x 20 px category tab. Titles fit one line at `type.display.md` (20 px); if too long, step down once to `type.ui.md` (15 px) and then truncate with an ellipsis. Never wrap to two lines.
- **Corner slot** (top-right, 22 x 22 px): at most one state glyph such as timer, lock, new, or alert.
- **Art window**: fixed crop. Art must keep its focal subject inside the central 140 x 108 px safe area. Enemies may break the upper art boundary by up to 10 px into the header behind the title plate.
- **Type line**: `type.ui.sm`, `text.secondary`. Family name plus optional subtype.
- **Info panel**: reserved for a short descriptor, progress bar, or a two-icon summary. Maximum 2 lines of `type.ui.sm`. Details beyond that go to tooltip/inspect view.
- **Footer**: up to two 36 x 22 px value badges at left and right anchors. The center holds the family rail graphic. Badges never move; unused badge anchors remain empty.

## Badges

| Badge | Shape | Default color |
| --- | --- | --- |
| Primary value (left) | Stamped plate, 3 px radius | `material.bone` fill, `surface.inset` text |
| Secondary value (right) | Stamped plate | `surface.elevated` fill, `text.primary` text |
| Damage/threat value | Chipped plate with notch | `state.damage` fill, `text.primary` text (5.4:1) |
| Timer / progress | Thin 4 px bar in info panel or 22 px corner radial | `accent.amber` |
| Count (stack) | Hex nut plate, 26 x 22 px, top-right of stack | `surface.elevated`, `text.primary` |

Badge values use `type.ui.xs` bold numerals, minimum 11 px. Future gameplay decides what the values mean; presentation only guarantees slots.

## Visual States

| State | Treatment | Motion token |
| --- | --- | --- |
| Resting | `shadow.card`; frame at normal value | --- |
| Hover | Scale `scale.hover`, frame edge light +10% value, tooltip timer starts | `motion.instant` |
| Pressed / grabbed | Scale `scale.lift`, `offset.lift`, `shadow.lift`, z to `CardTransient` | `motion.fast` |
| Dragging | Maintains lift; tilt up to 4 degrees toward drag velocity, returns to 0 at rest | continuous, damped |
| Valid drop target | Target shows 2 px `accent.cyan` outline plus corner brackets | `motion.instant` |
| Invalid drop | Held card outline `state.damage` 2 px; small cross glyph at corner slot | `motion.instant` |
| Selected | 2 px `accent.cyan` outline + corner brackets, persists until deselect | `motion.fast` |
| Busy / working | Info panel progress bar fills; frame unchanged | Linear per progress |
| Disabled / exhausted | Art desaturated 60%, text `text.muted`, frame unchanged | `motion.standard` |
| Damaged | Brief hit feedback, then persistent chipped-frame segment if applicable | `motion.impact` |
| Destroyed | Dissolve sequence (see motion doc) | `motion.slow` |
| New / unseen | Corner slot shows amber dot; clears on first hover | --- |

States combine by priority: Dragging > Invalid > Selected > Damaged > Busy > Hover > Resting.

## Stacking

- Stacks cascade straight down with a 32 px offset (`offset.stack`). The root card sits highest on screen and each added card lies over it 32 px lower, exposing the 28 px header strip of every card beneath. That is why title and category tab live in the header.
- The last card added is fully visible at the bottom of the cascade.
- Show up to 8 cascaded cards. Beyond 8, compress the oldest cards under the root and show the count plate on the root.
- Grabbing any card lifts that card plus every card stacked on top of it (Stacklands-like). Grabbing the root moves the whole stack. Pulling a single card out of the middle is a gameplay/input decision (for example, a modifier key).
- Optional: hovering a stack for 450 ms widens the cascade to `offset.stack.fan` (44 px) so the headers are easier to read; it collapses on exit. Drop targets use the collapsed geometry.
- Snap onto a stack when the held card's center is within 64 px of the center of the stack's last (fully visible) card and the stack accepts it. The held card joins at the bottom of the cascade.

## Card Back

Card backs use the same frame with a full-face emblem panel. Each family has a back variant (see [05 Card Families](05-card-families.md)) with its family tab visible so undrawn or unrevealed cards still communicate category.

## Inspect View

Right-click, controller Y, or long-press opens an inspect view: the card at 2x (360 x 504 px) centered over a `surface.scrim`, with an adjacent 420 px wide detail panel. The inspect view is the only place full descriptions appear.

## Tooltip

After 500 ms hover on a resting card, show a 280 px max-width tooltip at the card's right edge (left if near viewport edge), 12 px offset. Content: title, family, up to 3 short lines. Hide immediately on drag.