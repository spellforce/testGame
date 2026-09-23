# Tabletop Layout

## Reference Frame

Author all desktop-first layouts at 1920 x 1080 logical pixels. Godot should scale the interface uniformly from this reference rather than independently stretching controls.

```text
0,0                                                                     1920,0
+-------------------------------------------------------------------------+
|  96 px margin    HEADER / STATUS BAND: 136 px high          96 px     |
|  [Run marker] [resources / notices]                  [pause/settings] |
+-------------------------------------------------------------------------+
|                                                                         |
|                                                                         |
|            CORE TABLETOP: 1728 x 746 px                               |
|            x=96..1824, y=152..898                                     |
|                                                                         |
|     Camera target, cards, stacks, event arrivals, ambient field marks |
|                                                                         |
+-------------------------------------------------------------------------+
|  LOWER ACTION / INVENTORY BAND: 118 px high                            |
|  [context action] [queue/inventory slots] [zoom] [utility icons]       |
+-------------------------------------------------------------------------+
0,1080                                                               1920,1080
```

Vertical budget (sums to 1080): header 136 + gap 16 + tabletop 746 + gap 16 + lower band 118 + bottom safe margin 48. The 16 px gaps keep the HUD from touching cards; the 48 px bottom margin absorbs OS taskbars, overscan, and field bleed. The board reads as a broad work area rather than a collection of cards inside a framed window.

## Zones

| Zone | Bounds at 1920 x 1080 | Size | Role |
| --- | --- | --- | --- |
| `HudTop` | x 96 to 1824, y 0 to 136 (content inset to y 16 to 120) | 1728 x 136 | Persistent run status, resources, high-priority notices |
| `Tabletop` | x 96 to 1824, y 152 to 898 | 1728 x 746 | Direct card manipulation and world presentation |
| `HudBottom` | x 96 to 1824, y 914 to 1032 | 1728 x 118 | Context action, inventory/queue, zoom, secondary tools |
| Bottom safe margin | y 1032 to 1080 | 48 | Field bleed only; no interactive content |
| `Overlay` | Full viewport | 1920 x 1080 | Menus, pause, modal, tutorial, result layer |
| `Cursor` | Full viewport | 1920 x 1080 | Custom cursor and drag ghost, top-most interactive layer |

Keep all permanent HUD elements outside `Tabletop`; do not sacrifice tabletop area for sidebars during a run.

## Card Placement

- Standard card: 180 x 252 px at 1.00x zoom.
- Minimum board-edge clearance for a freely placed card: 24 px.
- Recommended free-card separation: 12 px.
- Card title-safe edge when obscured by another card: 68 px of upper card face remains visible.
- Stack cascade: 32 px straight down per card (`offset.stack`), so every card's header strip stays readable. A stack of n cards is 252 + 32 x (n - 1) px tall; show up to 8 cascaded cards (476 px), then compress the rest under the top of the stack and add a count plate.
- Equipment attachment offset: 28 px below the host lower edge or 22 px right of a host side socket, chosen by the future gameplay layout.
- Drag collision and hit testing use the visual card rectangle plus a 6 px forgiving border, not illustration-alpha hit testing.

## Camera and Zoom

The desktop tabletop defaults to 1.00x. Mouse wheel or the lower-band zoom controls clamp from 0.80x to 1.25x in 0.05 steps. Zoom centers on pointer location when the pointer is inside the tabletop; keyboard/controller zoom centers on the selected card or board midpoint.

At 1.00x, expose roughly 8 to 10 well-separated cards or 5 to 7 small stacks without visual crowding. Do not auto-zoom on every card interaction. Use a brief camera nudge only when an event enters outside the current visible board region.

## Layer Order

1. `FieldBackground`: plate texture, dust, quiet stencils.
2. `FieldDecor`: non-interactive environmental marks and cast shadows.
3. `CardGround`: fixed buildings and grounded object bases.
4. `CardWorld`: vehicles, enemies, searchable sites, standard stacks.
5. `CardTransient`: held card, incoming event, resolving card.
6. `FieldFx`: dust burst, hit sparks, short-lived danger marker.
7. `HudTop` and `HudBottom`.
8. `Overlay`.
9. `Cursor` and accessibility focus proxy.

A held card always appears above all other cards but below HUD, menus, and the cursor.

## Responsive Rules

| Viewport shape | Treatment |
| --- | --- |
| 16:9 at 1280 x 720 or greater | Uniformly scale the 1920 x 1080 reference. Maintain zone proportions. |
| 16:10 | Center 16:9 working region vertically; use extra height as field bleed, never taller cards. |
| Ultrawide 21:9 | Center a 1920-wide working board. Extend only the field background and optional quiet decorative side plates. HUD content remains inside the 1728 px safe width. |
| Narrow portrait / 390 x 844 | Enter compact tabletop mode: header 92 logical px, lower band 104 logical px, camera starts at 0.80x, use horizontal scrolling or pan for the board, and open context panels as full-height overlays. Never shrink text below the token minimum. |

Godot project settings should begin with `viewport_width=1920`, `viewport_height=1080`, `stretch_mode=canvas_items`, and a proportional content-scale strategy. Exact project configuration is documented in [09 Godot Handoff](09-godot-handoff.md).

## Field Background

The field is full-bleed beyond all zones. Its job is depth, not decoration: use dark plated steel, three to five large seams, limited dust at the lower third, faded coordinate marks, and small regions of paint abrasion. Keep its contrast lower than a resting card by at least one value step.