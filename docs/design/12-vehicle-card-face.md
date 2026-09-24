# Vehicle Card Face

The vehicle card carries the most state of any card. The gameplay brief asks directly: what goes on the face, where do the numbers go, how do 4-5 weapons plus 2 engines plus 2 C units fit, and where does the protagonist go?

Answer in three layers. The **face** carries only what must be readable in a stack at zoom 0.5. The **stack** carries the equipment as real cards. The **detail sheet** carries the full numbers and the mount grid.

## Layer 1 — The Face

The art area is only 29 rows (see **Errata** in [20 Questions and Errata](20-questions-and-errata.md) — [04 Card System](04-card-system.md) says 37, which does not fit in 56 rows). That is the budget the whole design has to fit in.

```text
x: 0    1 ..................................... 46   47
y0    ink outline
y1    +------------------------------------------+
      | TITLE                     (12 rows)      |  header
y12   +------------------------------------------+
y13   | separator                                |
y14   +------------------------------------------+
      | a0  [  chip  ]                           |  a0  free
      | a1-a6              (protagonist chip)     |  a1..a6  reserved for the chip
      |                                          |
      | a7-a26   VEHICLE ICON, up to 32x20,      |  a7..a26  icon band, 20 rows
      |          centered, bottom-aligned        |
      |                                          |
      | a27-a28  CARGO BAND, 46 x 2             |  a27..a28  cargo fill
y42   +------------------------------------------+
y43   | [SP%]      wheel notches     [cargo]     |  footer 12 rows
y54   +------------------------------------------+
y55   ink outline
```

Rows: 1 + 12 + 1 + 29 + 12 + 1 = 56. Columns: 1 + 46 + 1 = 48.

### Icon band

Vehicle icons follow a stricter spec than other families:

- Up to **32 x 20**, horizontally centered, **bottom-aligned to row a26**.
- The shared ground line at a26 means a row of vehicles on the board stands on one line, which is what makes a convoy read as a convoy.
- The vehicle's own wheels never cross a27; the cargo band owns that strip.
- A 3/4 view or a side view both work. Tracks may extend to the full 32 width.

This is narrower than the generic 32 x 26 in [08 Asset Production](08-asset-production.md) and it is deliberate: a wide, low profile is what a vehicle looks like from the side, and it frees the top strip for the chip.

### Protagonist chip

A 9 x 5 px `bone` plate with the protagonist's **silhouette** in `ink`, at the art area's top-right, inset 2 px from the art area's top and right edges (so its rectangle is 9 x 5 at art-area offset x 35, y 2).

- Present whenever a protagonist card is in the vehicle's stack. Absent means unmanned.
- It is a **mirror**, not a second copy of state. Nothing about the protagonist is stored on the vehicle; the chip reads "a protagonist card is in this stack" and the avatar comes from the protagonist's own data (see [04 of the gameplay brief's caution about not duplicating state]).
- Drawn above the icon, always. Vehicle icons are authored knowing the top-right 12 x 7 of the art area may be covered.
- Hovering the chip makes the **protagonist's** card the hover target, so the info panel shows the protagonist, not the vehicle. This is the only hover redirect on a card face; it needs a 1 px `bone` outline flash on the chip so the redirect is not confusing.

### Cargo band

A 46 x 2 px bar on rows a27-a28:

- Track: `steel.1` at 1 px, then `steel.2`.
- Fill: `amber.2`, width = round(cargoUsed / cargoMax x 46) art px, growing left to right, no easing.
- Hidden entirely when the vehicle is unmanned or cargoUsed is 0.
- At cargoMax or over, the fill becomes `rust.3` and the last 3 px blink twice (`motion.frame` each) when the value changes.

This is the single most-consulted number in the game — how full am I — so it gets the widest, thinnest, least intrusive element on the card, and the exact number stays in the badge.

### Footer badges

| Badge | Meaning | Plate | Digits |
| --- | --- | --- | --- |
| Left | SP as a percentage, 0-99 | `bone` above 60%, `hazard` at 30-60%, `rust.2` below 30% | `ink` on bone/hazard, `white` on rust |
| Right | Cargo hold used, 0-99 ("99" at 100 or more) | `rust.2`, or `rust.1` with `white` digits when overloaded | `white` |

- SP is a percentage because the raw range is 1-50000 and a badge holds 2 digits. The raw value lives in the detail sheet.
- The plate color is the at-a-glance health read; the number is the precise read. No SP bar, because a bar would have to live in the art area and would fight the icon.
- **Overloaded** (cargoUsed > cargoMax) adds a 5 x 5 `hazard` chevron in the footer's center, next to the wheel notches. Overloaded is a legal state — the vehicle is slower and takes a push penalty — so it is a warning, not a block.

### Where the equipment is NOT

Not on the face. Five weapons, two engines and two C units is a maximum of nine items on one 48 x 56 card, and any attempt to show them produces a card that reads as noise at zoom 0.5, which is the zoom most of the game is played at.

## Layer 2 — The Stack

Equipment is a real card in the vehicle's stack, so loading and unloading use the same drag rules as everything else.

### Stack order

Fixed, and enforced on drop by `CanHaveCardOnTop`:

| Position | Card | Max |
| --- | --- | --- |
| 1 (root) | Vehicle | 1 |
| 2 | Protagonist | 1 |
| 3 | C unit | 2 |
| 4 | Engine | 2 |
| 5 | Weapon | 5 |

- The protagonist sits directly under the vehicle so the stack reads vehicle → driver → equipment, and so disembarking is a one-grab action in the garage.
- A full vehicle stack is 9 cards: 56 + 12 x 8 = **152 art px** tall, about 2.7 card heights. In the play area (480 tall) it fits 3 times over.
- At 9 cards the count plate appears on the root card's header (existing rule, [04 Card System](04-card-system.md) — 10 cards triggers it, so a full vehicle is one short of it; a vehicle with a protagonist and 5 weapons is 8 and stays clean).

### Indent marks

So that a long equipment stack is readable without hovering each card, each equipment card draws a **1 px `steel.5` indent mark** at the left end of its visible 12 px strip, at x 2, 6 px long. Depth in the stack is legible from the edge alone. Only vehicle-stack equipment gets this mark; ordinary stacks do not.

### What a stack cannot tell you

Damage. A damaged card's face looks like an intact card's face. Damage is carried by:

- The **timer bar position** — a damaged engine or weapon shows a persistent bar at 1 px of width in `rust.3` instead of a progress bar. A destroyed one shows a `rust.3` 1 px full-width bar.
- This reuses the existing timer bar asset and the existing "n px above the root card" placement, so it costs no new art.

## Layer 3 — The Detail Sheet

Hovering the vehicle (or opening it from a window) shows the vehicle detail sheet, which is the generic Card Detail Sheet from [13 Window System](13-window-system.md) with two vehicle-specific blocks: the mount grid and the stat pairs.

### Mount grid

```text
+-----------------------------------------------+
| [vehicle card]   LINEUP  (48x56 at 1:1)       |
| 48x56            full_name                    |
|                  heavy hauler · grade         |
+-----------------------------------------------+
| MOUNTS                                        |
|  [W1][W2][W3][W4][W5]          SP  04200/04200|
|  [E1][E2]      [P]             LOAD 018/024   |
|  [C1][C2]                      WGT  00034     |
|                                DEF  00012     |
+-----------------------------------------------+
| TRAITS   [rugged] [thirsty] [welded]          |
+-----------------------------------------------+
```

- Each mount slot is **20 x 16 UI px**: `steel.1` inset, 1 px `ink` border, holding a 16 x 12 native-size icon of the equipped item.
- Empty slot: the border becomes a 1 px dashed `steel.4` box with a 5 x 5 kind glyph (`steel.4`) at the center. Dashed = "can hold something", solid = "holds something".
- Slot states: filled (border `steel.5`), damaged (plus a 3 x 3 `hazard` corner mark at the top-right), destroyed (icon drawn through the disabled palette swap plus a 5 x 5 `rust.3` X).
- Hover: the slot's border goes `cyan.3` and the detail sheet's body swaps to that item's sheet with a 120 ms cross-fade. Moving off returns to the vehicle sheet.
- Click: pins the item's sheet, so the mouse can travel to a button. Click the pinned slot again to unpin.
- Slot order is fixed: W1-W5 left to right is main cannon, secondary cannon, S-E, then 2 free; E1-E2; C1-C2. The kind glyph on an empty slot tells the player what belongs there.
- The `[P]` slot is the protagonist: 20 x 16, showing the protagonist's 16 x 12 avatar. Empty means unmanned.

At UI scale 1 each slot is 20 x 16 screen px, which is small but the icons are native-size pixel art and stay sharp. The mount grid is a map, not an inventory: the player looks at it to see *where* the damage is, then hovers that slot for the numbers.

### Stat pairs

Right of the mount grid, label/value rows at 16 UI px line height: `steel.7` label left, `bone` value right-aligned, values in `font.ui` with tabular digit spacing.

| Row | Value format | Notes |
| --- | --- | --- |
| SP | `04200 / 04200` | Zero-padded so the column does not jump |
| LOAD | `018 / 024` | `rust.4` when over |
| WGT | `00034` | Self weight |
| DEF | `00012` | |
| CARGO | `018 / 024` | Over shows `rust.4` and appends `OVER` in `hazard` |
| SLOTS | `5 / 2 / 2` | Weapons, engines, C units. Each part turns `rust.4` when its own limit is exceeded |
| CP | `100 / 150` | Only shown when manned |
| STATE | text | `Ready`, `Damaged`, `Crippled`, `Wrecked` from the SP thresholds |
| TRAITS | chips | One 3-9 char chip each, max 4 shown then `+n` |

TRAITS chips are the same component as the region and quest trait chips: `steel.3` body, 1 px `ink` outline, `bone` text, 12 UI px tall. A disabled trait (because the card is damaged) draws with `steel.4` strikethrough text.

### Equipment rows

Below the stat pairs, one row per equipped card, so the sheet doubles as a parts list. Each row reuses the list row from [13 Window System](13-window-system.md): native icon 20 x 16 cell (the same asset as a mount-grid slot), name, then 3 right-aligned numeric columns that change per kind.

| Kind | Columns |
| --- | --- |
| Main / secondary cannon | ATK, ammo `cur/max`, cooldown in s |
| S-E | ATK, charges, cooldown |
| Engine | LOAD cap, DEF, bonus `+n` |
| C unit | HIT, EVA, DEF |

### Unavailable actions

The vehicle sheet's action bar is where the brief's "explicit reasons when an action is unavailable" lands. Each disabled button carries a reason in the status bar on hover, in `steel.7`, and the same reason appears as a `!` tooltip. Never a dead button with no explanation.

| Action | Available when | Reason shown when not |
| --- | --- | --- |
| Repair | At least one damaged or destroyed part, and a repair pack or enough gold | `Needs a repair pack or 200 gold` |
| Resupply | In the garage only | `Quick resupply is garage only` |
| Unload cargo | cargoUsed > 0 | `Cargo hold is empty` |
| Disembark | In the garage, manned | `You can only disembark in the garage` |
| Refuel SP | In the garage, SP below max | `SP is already full` |
| Sell | The vehicle is not the active expedition vehicle | `This vehicle is on the expedition` |
| Open cargo hold | Always | Opens the cargo hold window (see [14 Facility Windows](14-facility-windows.md)) |

## The Three Zooms This Must Survive

Check the vehicle card at each, before drawing it once:

| Zoom | Screen px per art px | What must still read |
| --- | --- | --- |
| 0.5 | 1 | Family (cyan), SP plate color, cargo band present or not, protagonist chip present or not |
| 1.0 | 2 | Everything above, plus the 2-digit values |
| 2.0 | 4 | Everything above, plus trait chips and the indent marks in a loaded stack |

At 0.5 the badge digits are 3 x 5 art px, which is readable only because they are a dedicated sprite font and not scaled text. This is the reason the digits are hand-drawn rather than a Label.
