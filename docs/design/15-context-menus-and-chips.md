# Context Menus and Chips

The brief asks whether a right-click shortcut menu is redundant. Short answer: **not redundant, but tight.** It is redundant exactly where it duplicates a chip, and it is the only way to reach a few actions. This document draws that line, then defines the chips.

## The Decision

A right-click menu earns its place when all three hold:

1. The action is **frequent** (used most turns), or the action is **destructive** and benefits from a deliberate second step.
2. The action has **no chip** and no obvious drag gesture.
3. The action applies to **one card**, not to a stack or a selection.

Where all three fail, there is no menu entry. Where a chip already does it, there is no menu entry either — see the duplication policy below.

A menu must never be the only path to something. Every menu entry has a keyboard route and, where one exists, a drag route.

## Duplication Policy

The brief lists four candidate menu actions for a garage vehicle: one-click repair (gold), refill SP, open cargo hold, unfold equipment. Three of those are also proposed as fixed chips. Both cannot own them.

| Concern | Owner | Why |
| --- | --- | --- |
| **Card-specific verbs** (repair this, unload this, disembark this) | The **context menu** | The menu is already aimed at one card. Putting them on chips would need a "which card?" step |
| **Ambient modes and panels** (open the warehouse, open skills, see the shopping list) | The **chip** | These are not aimed at anything; they are places |
| **The drag targets** (deposit here, sell here, craft with this) | The **chip**, by dropping onto it | This is the fastest interaction in the game and must live on a persistent target |
| **Skills that act on a location** (scan, bombard) | The **chip**, in the expedition's skill tray | They need a target on the board and a chip is the thing the player drags onto that target |

So: repair is on the menu; the warehouse is on a chip; deposit is a drop onto the warehouse chip. No overlap.

## Menu Anatomy

```text
+-------------------------------+
| Repair                240 G   |
| Repair all           1,240 G  |
| ---------- (separator)        |
| Refill SP             180 G   |
| Unload cargo            8 / 24|
| ----------                    |
| Open cargo hold          >    |
| Equipment                >    |
| ----------                    |
| Disembark                    |
| ----------         (danger)   |
| Sell                   1,800 G|
+-------------------------------+
```

| Part | Spec |
| --- | --- |
| Panel | 9-slice steel panel, `steel.2`, 1 px `ink`, 1 px `steel.3` top-left highlight, 3 x 3 rounded corners |
| Width | Auto: widest label + 8 px right gutter for the value column, minimum 88 UI px, maximum 160 UI px |
| Row height | 16 UI px (same as a button, so the rows are the same target size) |
| Label | `font.ui` at 12 px, `bone`, left-aligned at x 6 |
| Value | Right-aligned in the gutter, `amber.2` for a cost, `steel.7` for a state (`8 / 24`), `rust.4` when the action is unavailable |
| Separator | 1 UI px row, `steel.3`, spans x 4 to width-4 |
| Submenu | A `>` chevron in the value column; opens after a 150 ms hover dwell, to the right, aligned to the parent row |
| Disabled row | 60% wash, `steel.4` text, and the reason in the status line on hover. Same rule as buttons: disabled, never hidden |
| Danger row | `rust.4` text, and after the separator, at the bottom. Requires a confirm dialog |
| Focus | `cyan.3` 1 px outline, 1 px outside. Keyboard: up/down, Enter, Esc |
| Position | At the pointer, offset +2, +2. Flips up/left if it would overflow the screen. Never covers the pointer |
| Dismiss | Any click outside, Esc, another right-click, board pan, or 4 s of no pointer movement with the menu unfocused |

Motion: opens in `motion.fast` (120 ms) as a 1 px vertical expand with a fade. No scale — scaling a 9-slice panel breaks its corners. Reduced motion: instant, with a 1-frame `white` top edge.

Sound: one short click on open and on select, from the UI group. Pitch 1.0.

## Menus Per Family

Only these exist. A card with no entries shows no menu at all — not an empty panel.

### Vehicle

| Row | Value | Available when | Reason when not |
| --- | --- | --- | --- |
| `Repair` | cost | At least 1 damaged part | `Nothing to repair` |
| `Repair all` | cost | 2+ damaged parts | `Nothing to repair` |
| `Refill SP` | cost | Garage, SP below max | `SP is already full` / `Quick resupply is garage only` |
| `Resupply ammo` | cost | Garage, any weapon below full ammo | `All magazines are full` |
| `Unload cargo` | `n / m` | cargoUsed > 0 | `Cargo hold is empty` |
| `Open cargo hold` | `>` | Always | Opens the cargo window |
| `Equipment` | `>` | Always | Submenu, see below |
| `Disembark` | — | Garage, manned | `You can only disembark in the garage` |
| `Sell` | price | Garage, not the active expedition vehicle | `This vehicle is on the expedition` |

**Equipment submenu** — the same mount list as the detail sheet, flattened into menu rows so a specific part can be acted on without opening a window:

```text
 Equipment                  >
     Main: 76mm Cannon  >      ->  Repair / Replace / Remove / Details
     Sec:  --
     S-E:  Smoke Discharger >
     Eng: V8 Diesel     >
     C:   Mk2 Servo     >
```

This is the brief's "unfold equipment" idea, and it stays a menu rather than a panel because it is a path to another card's menu, not a place.

### Part (weapon, engine, C unit, blueprint)

| Row | Available when |
| --- | --- |
| `Repair` | 1+ damage level |
| `Remove` | Garage, or the part is not equipped |
| `Details` | Always |
| `Sell` | Garage, not equipped |
| `Use as material` | In the factory window |

`Remove` on an equipped part in the expedition: disabled, `Equipment is locked during the expedition`.

### Cargo (item)

| Row | Available when |
| --- | --- |
| `Use` | The item is usable, and `usableIn` includes the current board |
| `Move to warehouse` | Garage, warehouse not full — or the item is a blueprint (always) |
| `Move to cargo hold` | Garage, a vehicle is on the table |
| `Jettison` | Always. Danger, confirms |
| `Sell` | Garage. Confirms above 100 gold |
| `Details` | Always |

The value column shows the gold price on `Sell`, and the item count when hovering a stack (`x3`) so `Use` and `Jettison` can be scoped. When the right-clicked card is in a stack of 3, `Use` acts on **one** and `Jettison` offers `Jettison 1` / `Jettison all 3`.

### Enemy

| Row | Available when |
| --- | --- |
| `Inspect` | Always — opens the detail sheet pinned |
| `Engage` | The vehicle is adjacent, and it is the player's turn to act |
| `Flee` | In combat |

No destructive rows. An enemy is never a target of a card verb.

### Site, Region, Event, Facility

| Row | Available when |
| --- | --- |
| `Inspect` | Always |
| `Search` | Site, searchable, vehicle adjacent |
| `Set as target` | Region, Event — marks the card with a crosshair so skills can refer to it |

### Protagonist

| Row | Available when |
| --- | --- |
| `Open skills` | Garage |
| `Disembark` | Garage, mounted |
| `Details` | Always |
| `Change portrait` | Garage. Opens the file picker (see [18 Asset Additions](18-asset-additions.md)) |

## Chips

A **chip** is a card-like fixture on the board that is not a card. The brief calls this out and the distinction matters: a chip is a *place* or a *standing ability*, a card is a *thing*. They must be visually unmistakable.

| | Card | Chip |
| --- | --- | --- |
| Size | 48 x 56 art px | 40 x 48 art px |
| Body | Family color | `steel.2` with a 1 px `bone` inner outline |
| Header | Title row with text | No text row — a 24 x 24 icon, centered |
| Footprint | Free, draggable, stacks, pushes | Pinned to a band slot or to a fixed board cell; never pushes, never stacks |
| Shadow | Yes, `shadow.card` | No shadow — a chip is *part of the deck*, not lying on it |
| Hover | Lifts 1 px | Does **not** lift. Instead, a 1 px `cyan.3` inner outline and the name in `bone` appears in the chip's own 12 px caption strip |

The two are told apart at a glance by the missing shadow and the missing title row. That is enough, and it costs nothing.

### Facility Chips (Garage)

One chip per facility, in the top slot band, in a fixed order. Clicking opens the window; dropping a card on one runs the facility's default verb.

| Slot | Chip | Click | Card drop |
| --- | --- | --- | --- |
| 1 | Warehouse | Open warehouse | **Deposit** |
| 2 | Hunter Camp | Open Hunter Camp | Shows the deposit verb refused: `The Hunter Camp takes no goods` |
| 3 | Taxi Shop | Open Taxi Shop | **Rent** the dropped vehicle (if it is rentable) |
| 4 | Vehicle Shop | Open shop | **Sell** at the shop price |
| 5 | Vehicle Factory | Open factory | **Add as material** to the selected recipe |
| 6 | Skills | Open skills panel | **Equip** the dropped skill into the first free slot |
| 7 | Protagonist | Returns the protagonist card to its home cell | **Mount** the dropped card onto a vehicle? No — dropping on the protagonist chip does nothing |

Slots 8 and 9 are locked `???` plates, reserved. They exist so the band is not rebuilt when content is added.

Every chip's drop behavior is confirmed by a 120 ms status line (`Deposited 1 Scrap Plate · 84/120`), not a dialog. The two exceptions keep a dialog: dropping a card on slot 4 to sell above 100 gold, and dropping a *quest item* anywhere (see [17 Dialogs](17-dialogs.md)).

### Protagonist Skill Chips (Expedition)

The brief notes skills could be fixed cards at the top that the player drags onto the board. That is the right call for the **expedition**, where a skill is an instant action aimed at a target, and the wrong call for the garage, where skills are a loadout edited in a panel.

So: a skill tray, only in the expedition, only for the equipped skills.

- 40 x 48 chips in the top slot band, one per equipped skill, in slot order.
- They do **not** consume band slots 1-3 (which are the region pack and spawn slots). The tray occupies a **second row** inside the band: the band grows from 104 to 164 art px, with row 1 keeping its 60 x 81 facility slots and row 2 holding 40 x 48 chips. See the band revision in [16 Garage and Expedition Boards](16-garage-and-expedition-boards.md).
- Each chip shows the skill icon, a `SlotCost` number, and a **CP cost** in `amber.2` in its caption strip.
- A chip whose CP cost exceeds current CP draws at 60% with the cost in `rust.4`.
- Combat skills appear in the tray during combat; non-combat during non-combat. Both are visible, the other group at 60%, so the player can see what is coming.
- **Drag a skill chip onto a target** (a location to search, a vehicle to repair, an enemy to bombard) to use it. The target validates, the chip's cost is shown on the target, and the release commits.
- A chip is not consumed. It has a cooldown, shown as a 2 px `cyan.2` arc around the chip's outline, drawn clockwise.

The visual separation the brief asks for is therefore: facilities are chips with a window behind them in the garage band; skills are chips with an instant effect in the expedition band. They never appear on the same board.

## Keyboard Equivalents

Every menu row and chip needs one.

| Target | Key |
| --- | --- |
| Context menu | `E` on the hovered card (or the `Menu` key) |
| Warehouse | `1` |
| Hunter Camp | `2` |
| Taxi Shop | `3` |
| Vehicle Shop | `4` |
| Vehicle Factory | `5` |
| Skills | `6` |
| Skill tray slot `n` | `Shift` + `1`-`7` |
| Search / interact | `Space` on the hovered card |

Keys `1`-`6` are contextual: in the garage they open facilities, in the expedition they are unbound. `Shift`+digit is only bound in the expedition. This is stated in the controls screen and in the `?` help.
