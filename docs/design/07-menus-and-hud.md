# Menus and HUD

Menus look like the convoy's command console: stamped steel panels, enamel labels, toggle switches, and stenciled headings. They must stay quick to navigate. Every screen works with mouse, keyboard, and controller.

## Shared Panel Kit

| Component | Spec |
| --- | --- |
| Panel | `surface.panel` fill, 1 px `line.steel` edge, `radius.panel`, 4 corner rivets (6 px), `alpha.grain` texture |
| Inset | `surface.inset`, 1 px inner shadow, used for lists and fields |
| Primary button | 280 x 52 px, `accent.amber` fill, `surface.inset` text, `type.ui.lg`, `radius.control`, 2 px darker bottom lip |
| Secondary button | 280 x 48 px, `surface.elevated` fill, `text.primary`, 1 px `line.steel` |
| Danger button | Secondary style with a `state.threat` 3 px left bar and `state.threat.text` label |
| Icon button | 44 x 44 px minimum hit area, 24 px icon |
| Toggle | Industrial lever switch: 52 x 28 px, cyan when on |
| Slider | 4 px track in `line.dim`, filled with `accent.cyan`, 16 x 24 px handle plate |
| Tab | Stenciled label on a steel tab; the active tab connects to the panel below and uses `text.primary` |
| Focus ring | 2 px `accent.cyan` outline 3 px outside the element, plus corner brackets on large elements |

Button hover: fill brightens 8% over `motion.instant`. Pressed: 1 px down, `.deep` fill. Disabled: 40% opacity, no hover.

## Screen Flow

```text
Boot/Splash -> Title -> (Continue | New Run -> Run Setup) -> Tabletop (run)
                    \-> Settings, Collection, Credits, Quit
Tabletop -> Pause -> (Resume | Settings | Save & Quit to Title)
Tabletop -> Run Result -> Title | New Run
```

## Title Screen

- Full-bleed key art: a convoy silhouette on a ridge at dusk, dust haze, and one warm headlamp beam. Slow parallax dust at 3 px/s.
- The game logo sits upper left at 96 px margins, with the `type.display.xl` wordmark treatment.
- The menu stack is left-aligned under the logo at x = 96, with 12 px gaps: Continue (primary; only if a save exists), New Run, Collection, Settings, Credits, Quit.
- The version string is bottom right in `type.ui.xs`, `text.muted`.
- On first load, the menu fades in 300 ms after the art. No button bounces.

## Run Setup

- Centered panel, 1120 x 720 px.
- Left, 520 px wide: starting convoy or board options, as a list of selectable plates.
- Right: a preview of 3 to 5 starting cards at 1.00x card scale on a mini tabletop surface.
- Bottom right: Start Run (primary). Bottom left: Back (secondary).

## In-Run HUD

### Top Band (1728 x 136, content y 16 to 120)

```text
[Day/Cycle plate] [Phase timer bar ...........]   [Fuel] [Scrap] [Ammo] [Food]   [Card cap 18/20] [||] [gear]
 left group                                          center resource group            right group
```

- **Left**: cycle/day plate (stenciled number in a 64 x 64 bone enamel plate) and a 360 x 10 px phase timer bar in amber. The end of the bar carries a small marker for the next event.
- **Center**: up to 5 resource counters. Each counter is a 24 px icon plus a `type.ui.md` bold value on an inset plate, 112 x 40 px. Resource names and icons stay generic until gameplay defines them.
- **Right**: card-capacity counter, which turns amber at 90% and red at 100%. Then pause (44 x 44) and settings (44 x 44).
- **Notices**: short toasts drop from under the band's center, 480 px max width, and stay 3 s. At most 2 visible; older ones slide up and out.

### Bottom Band (1728 x 118)

```text
[Context action panel 520 w] ....... [Queue / tray slots x 6] ....... [Zoom - 100% +] [speed x1 x2] [log]
```

- **Context action panel** (left, 520 x 96): shows the selected card's name, family icon, and 1 to 2 context buttons (for example, Sell or Scrap). It is empty and dimmed when nothing is selected.
- **Tray slots** (center): 6 slots, 72 x 96 px each (a card-back silhouette at 0.4x). Use them for purchasable packs, a build queue, or incoming reinforcements, as gameplay decides.
- **Right tools**: zoom stepper, speed toggle (if real-time), and an event log button.

### On-Board Overlays

- Card tooltips and the inspect view are specified in [04 Card System](04-card-system.md).
- Off-screen threat markers: a red chevron at the tabletop edge pointing toward an enemy outside the camera view, 28 px, with a distance-based opacity of 60 to 100%.
- Tutorial callouts: a bone enamel note panel with a cyan leader line to the target, 320 px max width, blocking only the target area.

## Pause Menu

- Scrim `surface.scrim` over everything except the cursor.
- Left-anchored panel, 420 px wide and full height minus 96 px: Resume (primary), Settings, Controls, Save & Quit to Title, Quit to Desktop (danger, with confirmation).
- The tabletop stays visible but dimmed under the scrim, so the player keeps context.

## Settings

Tabbed panel, 960 x 680 px. Tabs: Gameplay, Display, Audio, Controls, Accessibility.

- **Display**: resolution, window mode, UI scale (90% to 125%, affects HUD and menus only, never card size relative to the board), VSync, frame cap.
- **Audio**: master, music, SFX, UI, ambient sliders.
- **Controls**: rebind list; each row shows action name, primary binding, and secondary binding.
- **Accessibility**: reduced motion, screen shake on/off, colorblind-safe accents (swap cyan/red pair for a blue/orange pair with shape cues unchanged), text size (+0, +1, +2 steps), hold-to-drag vs click-to-pick-up.

## Confirmation Modal

- 520 x auto px (min 220) panel, centered.
- Heading `type.display.md`, body `type.ui.md`, max 3 lines.
- Buttons bottom right: the safe choice is focused by default. Destructive actions use the danger style and never sit in the default focus position.

## Run Result

- Full-screen panel over the dimmed final board.
- Headline in `type.display.lg`: a stenciled "CONVOY LOST" (threat stamp) or "ROUTE SECURED" (cyan stamp), stamped in with `scale.impact`.
- Stat rows count up one by one (150 ms stagger).
- Buttons: New Run (primary), Return to Title.

## Cursor

| State | Cursor |
| --- | --- |
| Default | Small steel arrow, 24 px, dark outline |
| Hover grabbable card | Open gauntlet hand |
| Dragging | Closed gauntlet hand |
| Invalid target | Closed hand plus a small red cross |
| Pan | Four-way arrow plate |
| UI hover | Default arrow; buttons show their own hover state |

Provide cursors at 32 px and 48 px (for high-DPI and UI scale), with the hotspot at the fingertip or arrow tip.

## Focus and Navigation Rules

- Every menu opens with a sensible default focus (the primary button).
- The focus order follows visual order: top to bottom, then left to right.
- Esc or controller B always goes back one level; on the title screen it does nothing.
- Menus never trap focus in a hidden element; closing a modal returns focus to the element that opened it.
