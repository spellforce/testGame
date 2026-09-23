# Design Tokens

These values are the authoritative visual constants for the first Godot implementation. Store them in `ui_theme.tres`, `card_style_library.tres`, and `motion_tokens.tres` as described in [09 Godot Handoff](09-godot-handoff.md).

## Color Tokens

| Token | Hex | Role |
| --- | --- | --- |
| `surface.field` | `#1B211F` | Tabletop base graphite-green |
| `surface.panel` | `#252A28` | HUD, menu, card frame dark steel |
| `surface.inset` | `#111514` | Card inner panel and input recess |
| `surface.elevated` | `#353A36` | Raised controls and active tool plates |
| `surface.scrim` | `#090B0AE0` | Modal and pause overlay |
| `line.steel` | `#66706A` | Default frame edge and dividers |
| `line.dim` | `#3D4642` | Secondary separations |
| `text.primary` | `#EEF0DF` | Main readable label and number |
| `text.secondary` | `#B8BEB0` | Secondary copy |
| `text.muted` | `#7F8A80` | Disabled and low-priority copy |
| `accent.cyan` | `#8ECED1` | Player ownership, focus, system state |
| `accent.cyan.deep` | `#3D878E` | Cyan inset and pressed state |
| `accent.amber` | `#E9A93A` | Actions, heat, energy, alerts |
| `accent.amber.deep` | `#A7641F` | Amber inset and pressed state |
| `accent.olive` | `#98A66B` | Neutral sites, search state, navigation |
| `accent.yellow` | `#D5C15A` | Equipment identifiers and utility marking |
| `state.threat` | `#C94D3C` | Enemy and threat accent (fills, tabs, rails; not for small text) |
| `state.threat.text` | `#EC8B78` | Threat-colored text on dark surfaces (5.9:1 on panel, 4.7:1 on elevated) |
| `state.damage` | `#A83B32` | Damage and destructive condition |
| `state.success` | `#A9C478` | Confirmed positive state; use sparingly |
| `material.bone` | `#D7D5BF` | Enamel plates and stenciled marks |
| `material.dust` | `#968A70` | Environmental dust and low-priority texture |

## Alpha and Depth

| Token | Value | Use |
| --- | --- | --- |
| `alpha.grain` | 0.05 | General grain overlay |
| `alpha.dust` | 0.10 | Environmental dust overlay |
| `alpha.wear.max` | 0.18 | Maximum local chip/soot mask |
| `shadow.card` | `0 10 18 #07090873` | Resting card depth |
| `shadow.lift` | `0 20 34 #05070699` | Held/dragged card depth |
| `shadow.modal` | `0 28 80 #000000B3` | Modal depth |
| `outline.focus` | 2 px | Selected card and keyboard focus |
| `outline.invalid` | 2 px | Invalid drop indication |

Do not use broad background gradients. Material variation comes from textures, inset planes, shadows, and controlled edge light.

## Spacing and Shape

Base unit: 4 px.

| Token | Value |
| --- | --- |
| `space.1` | 4 px |
| `space.2` | 8 px |
| `space.3` | 12 px |
| `space.4` | 16 px |
| `space.5` | 20 px |
| `space.6` | 24 px |
| `space.8` | 32 px |
| `space.10` | 40 px |
| `radius.card` | 6 px |
| `radius.panel` | 6 px |
| `radius.control` | 4 px |
| `radius.badge` | 3 px |
| `line.default` | 1 px |
| `line.strong` | 2 px |

Cards and operational panels stay angular and compact. Do not use soft, oversized rounded containers.

## Typography Scale

| Token | Size / line height | Use |
| --- | --- | --- |
| `type.display.xl` | 42 / 46 px | Title screen game name |
| `type.display.lg` | 28 / 32 px | Menu titles, major overlays |
| `type.display.md` | 20 / 24 px | Panel headings, card title at full scale |
| `type.ui.lg` | 18 / 24 px | Primary menu action |
| `type.ui.md` | 15 / 20 px | Standard labels, HUD values |
| `type.ui.sm` | 13 / 17 px | Card support labels, tabs |
| `type.ui.xs` | 11 / 14 px | Badge values, status captions |

Contrast checked against WCAG: `text.primary`, `text.secondary`, `accent.cyan`, `accent.amber`, and `accent.olive` pass 4.5:1 on `surface.panel` and `surface.inset`. `text.muted` (4.1:1 on panel) is only for disabled or non-essential copy. Never set text in `state.threat`; use `state.threat.text`.

Use normal letter spacing. Minimum interactive label size is `type.ui.md`; `type.ui.sm` is informational only.

## Motion Tokens

| Token | Duration | Curve | Use |
| --- | --- | --- | --- |
| `motion.instant` | 80 ms | ease-out | Hover border, cursor response |
| `motion.fast` | 140 ms | ease-out | Lift, press, tooltip appear |
| `motion.standard` | 220 ms | cubic-out | Snap, panel transition, card return |
| `motion.slow` | 380 ms | cubic-in-out | Event arrival, modal open |
| `motion.impact` | 120 ms | ease-out | Hit shake, badge pop |
| `motion.loop` | 2200 ms | sine-in-out | Single urgent state pulse |
| `scale.hover` | 1.015 | --- | Card hover only |
| `scale.lift` | 1.035 | --- | Held card |
| `scale.impact` | 1.06 then 1.0 | --- | Local feedback |
| `offset.lift` | -12 px y | --- | Held card vertical lift |
| `offset.stack` | 32 px y | --- | Vertical stack cascade (28 px header + 4 px gap) |
| `offset.stack.fan` | 44 px y | --- | Hover-expanded stack cascade |

Reduced motion replaces scale, shake, and slide with 80 ms opacity/outline changes. Refer to [06 Interaction and Motion](06-interaction-motion.md).