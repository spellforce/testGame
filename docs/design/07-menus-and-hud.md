# Menus and HUD

All UI is pixel art on a 960 x 540 UI canvas, integer-scaled to the screen (2x at 1080p, 3x at 1440p, 4x at 4K). Layout positions are in [03 Canvas and Layout](03-canvas-layout.md).

Stacklands uses cream paper panels with black ink outlines. We use steel plates with enamel labels.

## Panel Kit

All panels are 9-slice sprites (see [08 Asset Production](08-asset-production.md)).

| Component | Look |
| --- | --- |
| Panel | `steel.2` body, 1 px `ink` outline, 1 px `steel.3` inner highlight on top and left, 2 x 2 rivets (`steel.5`) in the corners |
| Light plate (list rows, headers) | `bone` body, `ink` text, 1 px `ink` outline |
| Inset (lists, text boxes) | `steel.1` body, 1 px `ink` inner shadow on top and left |
| Primary button | `amber.2` body, `ink` text, 2 px `amber.1` bottom lip; pressed: lip gone, content 1 px lower |
| Normal button | `steel.3` body, `bone` text, 2 px `steel.1` lip |
| Danger button | `steel.3` body, `rust.4` text, 2 px `rust.1` left bar |
| Tab | Active: `bone` with `ink` text, connected to the panel. Inactive: `steel.3` with `steel.7` text |
| Checkbox | 9 x 9, `ink` outline, `cyan.3` check mark |
| Slider | 3 px `steel.1` track, `cyan.2` fill, 5 x 9 `steel.7` handle |
| Scrollbar | 4 px wide, `steel.1` track, `steel.5` thumb |
| Focus | 1 px `cyan.3` outline 1 px outside the element |

Minimum button height: 16 UI px (32 screen px at 1080p).

## In-Game HUD

Like Stacklands: a left column for the task list and hovered card info, and small boxes at the top right.

### List Panel (Left, Top)

- 172 x 380 UI px.
- 2 tabs across the top (for example "Tasks" and "Ideas" / recipes), 18 UI px tall, `font.ui`.
- Collapse button (18 x 18) with a `<` arrow at the right edge. Collapsed, only the button stays on screen.
- List rows: 12 px checkbox + text, 16 px line height, wraps to more lines when needed. Group headers use the light plate with a `-` / `+` toggle.
- Footer message under the list (for example, "Complete 2 more tasks to unlock a pack") in `amber.2`.

### Info Panel (Left, Bottom)

- 172 x 147 UI px.
- Hovered card: title bar in the card's body color with its title, then the description in `font.ui`, `bone` on `steel.2`.
- Empty when nothing is hovered.

### Resource Box (Top Right)

- 128 x 27 UI px, 3 counters with a 9 x 9 icon each (for example, food, money, card count "4/20").
- A counter that is over its limit shows its number in `rust.4` and blinks the icon twice.

### Time Box (Top Right)

- 182 x 27 UI px: day/moon count text on the left, a thin `amber.2` progress bar along the bottom showing the cycle, and a 16 x 16 pause/play button at the right.

## Screens

```text
Title -> New Game / Continue -> Game
     \-> Settings, Credits, Quit
Game -> Pause Menu -> Resume / Settings / Save and Quit
Game -> Game Over / Win Screen -> Title
```

### Title Screen

- The world canvas with the board in the center, zoomed out, camera drifting slowly (4 art px/s). This is the key art; no separate painting needed.
- Logo top center: pixel lettering, about 320 x 64 UI px, `bone` with `rust.2` shadow and rivets.
- Button column under the logo, 120 UI px wide, 20 UI px tall, 4 UI px gaps: Continue (primary, only with a save), New Game, Settings, Credits, Quit.
- Version number bottom right, `steel.7`.

### Pause Menu

- `overlay.modal` over the whole screen.
- Centered panel 200 x 180 UI px: Resume (primary), Settings, Save and Quit (normal), Quit to Desktop (danger, asks for confirmation).

### Settings

Panel 400 x 300 UI px with tabs: Game, Display, Audio, Controls.

- **Display**: window mode, resolution, VSync, pixel-perfect zoom (on/off), UI scale (auto, or a fixed whole number).
- **Audio**: master, music, sound effects, ambient.
- **Game**: reduced motion, camera shake, pan with keys, pan with screen edge.
- **Controls**: key list with rebind buttons.

### Confirmation Dialog

- 220 x auto UI px, centered.
- Message in `font.ui`, 2 buttons at the bottom right. The safe option is focused first.

### Game Over / Win

- `overlay.modal`, centered panel 280 x 200 UI px.
- Stamp-style headline (`font.ui.heading`): "CONVOY LOST" in `rust.3` or "ROUTE SECURED" in `cyan.3`, appearing in 2 frames (large → final position, as drawn frames).
- A few stats, counting up one after another.
- Buttons: New Game (primary), Title.

## Cursor

Hardware cursor, pixel art, drawn at 1x and provided at 2x and 4x.

| State | Sprite (1x) |
| --- | --- |
| Default | 9 x 12 arrow |
| Over a card | 11 x 11 open hand |
| Holding a card | 11 x 11 closed hand |
| Panning | 11 x 11 closed hand with 4 arrows |

## Keyboard / Controller

- Menus: arrow keys / D-pad move focus; Enter / A confirms; Esc / B goes back.
- Board navigation with a controller: a focus cursor jumps to the nearest card in the stick direction; A picks up and drops; the left stick moves a held card; the right stick pans. This is a later goal. The first version is mouse only.
