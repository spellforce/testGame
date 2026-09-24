# Facility Windows

Each fixed facility from [02 of the gameplay brief] is a **window definition** from [13 Window System](13-window-system.md). This document gives the ones that need more than the generic layout: what is specific, and what is deliberately generic.

Read [13 Window System](13-window-system.md) first. Nothing here repeats the list zone, the action bar behavior, or the drag rules.

## Warehouse

The warehouse is the reference implementation of the window. Everything in [13 Window System](13-window-system.md) applies with no changes; this section only adds its numbers.

| Setting | Value |
| --- | --- |
| Window ID | `warehouse` |
| Default size | 480 x 420 |
| Filters | `All / Gear / Cargo / Blueprint` |
| Default sort | Type, ascending |
| Capacity | `warehouse.slots`, a global from `Datas` |
| dragOut scope | `extended` |

Zones that are **hidden** here: none. The warehouse uses every zone.

**Capacity** is the one facility-specific element. It appears in the title bar right as `084 / 120` in `font.ui` and in the summary as a bar:

- 40 UI px wide, 5 art px tall: `steel.1` track, 1 px `ink` border, `cyan.2` fill.
- Above 90% the fill goes `hazard`; at capacity it goes `rust.3` and the title count goes `rust.4`.
- At capacity, `Deposit` is disabled with the reason `Warehouse full (120/120)`. The deposit attempt from the board — dragging a card onto the warehouse facility chip — fails with the same reason in a status toast, and the card returns to where it was lifted.

**Blueprint handling**: blueprints are a filter tab and are stored in the warehouse, per [02 of the gameplay brief]. They are **not** ordinary items: depositing a blueprint into the warehouse is always allowed even at full capacity, because a blueprint is knowledge, not cargo. This is a deliberate exception and it must be stated in the `?` help. The blueprint row shows a `∞` in the unit column instead of a weight.

## Cargo Hold

The cargo hold is the warehouse window with a **vehicle binding**.

| Setting | Value |
| --- | --- |
| Window ID | `cargo.<vehicleId>` |
| Default size | 420 x 380 |
| Filters | `All / Supply / Material / Valuables` |
| Default sort | Weight, descending (heaviest first — this is what you dump first) |
| Capacity | The bound vehicle's `cargoMax` |
| dragOut scope | `extended`, **or** `locked` during an expedition |

**Binding**: opening a cargo hold always identifies its vehicle. Three things make the link visible:

1. The title is `<Vehicle short name> Cargo`.
2. The bound vehicle card on the board gets a 1 px `cyan.3` outline for as long as the window is open — the same outline style as "valid drop target", at a lower priority so a real drop-target outline overrides it.
3. The vehicle's detail sheet, if also open, highlights the `CARGO` stat row.

**Locked during an expedition**: [01 of the gameplay brief] says equipment configuration is locked during an expedition, but the cargo hold is explicitly not — items can be loaded and unloaded. So:

| Action | Garage | Expedition |
| --- | --- | --- |
| Take out | Yes | Yes |
| Put in | Yes | Yes |
| Use | Yes | Yes, if the item's `usableIn` includes expedition |
| Jettison | Yes, asks for confirmation | Yes, asks for confirmation, and is styled as `Danger` |
| Sell | Yes | Disabled — `You cannot sell during an expedition` |
| Deposit to warehouse | Yes | Disabled — `No warehouse access outside the garage` |

**Jettison** is the only destructive container action. It confirms, because the board is a place where a mis-drop is easy: the confirm dialog names the item and its weight, and the safe option is focused.

## Vehicle Shop

The shop introduces one concept the warehouse does not have: **a price on a card**.

| Setting | Value |
| --- | --- |
| Window ID | `shop.vehicle` |
| Default size | 480 x 420 |
| Filters | `Weapons / Engines / C units / Cargo / Blueprints` |
| Default sort | Name, ascending |
| dragOut scope | `deferred` |
| Currency | `gold`, shown in the title bar right |

**Price display.** In a shop window, the row's unit column switches from weight to **price**, in `amber.2` when affordable and `rust.4` when not. There are two ways to buy:

1. **Buy button** on the action bar, acting on the selected row(s). `Buy 5` is offered when the item is stackable and the player can afford 5+.
2. **Drag the card out.** Because the scope is `deferred`, the ghost is a preview: while it is out, its right badge shows the **price** instead of its value, the title bar's gold count shows the projected balance (`1240 → 940`), and if the player cannot afford it the ghost's outline is `rust.3` and the drop is refused with the reason `Not enough gold (needs 300)`.

Dropping a `deferred` ghost on the board commits the purchase; the card is placed where it was dropped. Dropping it back on the window cancels. This makes "drag the gun onto my vehicle" a single gesture that buys and equips.

**Selling from the board**: drag any card from the board onto the open shop window. The window's action bar has `Sell`, so the drop means sell. The price is shown on the dragged card during the whole drag, and the sale confirms only above 100 gold (`Sell Scrap Plate for 8 gold?` is a status-line action with a 2 s undo; above 100 it is a dialog).

**Buyback tab.** The brief proposes this and it is cheap: the shop keeps a **buyback list** of the last 12 sales, priced at what they sold for, available only in the same shop visit and cleared when the window closes. It appears as a `Buyback` filter tab. This prevents the "I sold the quest item by accident" problem without needing a full undo system.

## Taxi Shop

Renting vehicles is the shop with a different verb and a duration.

| Setting | Value |
| --- | --- |
| Window ID | `shop.taxi` |
| Default size | 460 x 400 |
| Filters | `Light / Heavy / Special` |
| Columns | Vehicles profile: SP%, cargo, weight class |
| dragOut scope | `deferred` |

The detail zone for a rental shows a **rental block** above the description:

```text
RENTAL TERMS
  Rate      120 gold / trip
  Deposit   200 gold (refunded on safe return)
  Fuel      included, 40 units
  Time      1 region cycle
  Loss      deposit is forfeit if the vehicle is wrecked
```

Numbers come from `Datas`; [02 of the gameplay brief] marks the taxi economy as to-be-verified, so the layout is fixed and the values are placeholders. The action is `Rent`, and the rented vehicle arrives on the board as a real vehicle card with a 1 px `amber.2` inner outline (a rented marker no other card has) and a `hazard` right badge showing remaining time.

The rented vehicle **cannot be sold** and **cannot have its equipment permanently removed** — removing a part from a rental gives it back to the warehouse, which is correct, but the vehicle itself has no sell action, with the reason `This vehicle is a rental`.

## Vehicle Factory

The factory is the one facility where the generic window is not enough — it needs a recipe view and a queue. It uses the window shell and replaces the list zone's contents for the `Recipes` tab.

| Setting | Value |
| --- | --- |
| Window ID | `factory` |
| Default size | 560 x 440 |
| Filters | `Recipes / Materials` |
| dragOut scope | `extended` (materials and finished goods) |
| Bottom strip | Craft queue, 22 UI px, present on both tabs |

**Recipes tab** — the list zone holds blueprint rows (profile: material count, craft seconds) and the detail zone holds the recipe:

```text
+--------------------------------------------+
| [blueprint card render]                    |
| Repair Pack                                |
| Part · consumes all materials              |
+--------------------------------------------+
| MATERIALS                                  |
|  [icon] Scrap Plate          2 / 3   rust.4|
|  [icon] Rubber Strip         1 / 1   olive.2|
|  [icon] Copper Wire          0 / 2   rust.4|
+--------------------------------------------+
| OUTPUT            TIME      CRAFT          |
|  [icon] x1        12 s      [Craft][All]   |
+--------------------------------------------+
```

- A material row's count is `have / need`, colored `olive.2` when satisfied and `rust.4` when not.
- `Craft` is disabled unless every material row is satisfied, with the reason naming the first missing material: `Missing 2 Copper Wire`.
- `Craft all` computes the maximum craftable count from the warehouse stock and offers it in the button's label: `Craft x4`.
- Crafting consumes from the **warehouse**, not from the board. Materials must be deposited first. This is stated in the `?` help and is the reason the factory sits behind the warehouse in the player's mental model.
- The output goes to the warehouse ([02 of the gameplay brief]: blueprints and materials go to the warehouse, not directly onto the expedition table).

**Queue strip** — the bottom strip shows in-progress and pending crafts:

```text
QUEUE  [Repair Pack ▓▓▓▓░░░░ 6s] [Armor Pack ░░░░░░░░ 12s] [x]
```

Each entry: a 2 px `cyan.2` progress line, name, remaining seconds in `font.digits`, and an `x` to cancel. Cancelling refunds materials; cancelling an item already in progress refunds 100% in v1 (simplest rule, and there is no exploit because crafting has no output randomness).

**Materials tab** — the materials needed by the currently selected recipe, with counts, so the player can see the shopping list without hunting. Read-only; it deposits nothing.

## Hunter Camp

Quests are not cards the player owns, so the Hunter Camp is the one facility window that is mostly **read-only**.

| Setting | Value |
| --- | --- |
| Window ID | `hunter_camp` |
| Default size | 480 x 420 |
| Filters | `Open / Accepted / Done` |
| Columns | Quest profile: `reward`, `evidence n/m` |
| dragOut scope | `closed` |

The detail zone for a quest:

```text
+--------------------------------------------+
| [quest card render, Event family]          |
| Ruined Convoy Cull                         |
| Bounty · Region: Dried Seabed              |
+--------------------------------------------+
| OBJECTIVE                                  |
|  Destroy 3 Raider Trucks                   |
|  Progress                      2 / 3       |
+--------------------------------------------+
| EVIDENCE                                   |
|  [icon] Truck Axle          1 / 1   olive.2|
|  [icon] Fuel Cell           0 / 2   rust.4 |
+--------------------------------------------+
| REWARD        240 gold, 1 blueprint        |
| CONDITIONS    Must evacuate successfully   |
+--------------------------------------------+
```

- `Accept` is enabled only when the player has space for the quest (a quest slot cap from `Datas`) and the region is available.
- `Claim` is enabled only when the quest is complete **and** the player is in the garage. During an expedition it is disabled with `Settle your expedition first` — this follows [01 of the gameplay brief]: only a successful evacuation brings quest evidence into base settlement.
- Evidence rows show live counts against the cargo hold and the warehouse, so the player can see what is still missing. Rows for evidence in the world are drawn `steel.4` with a small `?` instead of a count.
- Hovering an evidence row that exists in the warehouse highlights that row's counterpart in an open warehouse window. This is the one cross-window hover link in the game; it needs no new component, only an ID match.

## Skills Panel

The skills panel is a window with the **two-list layout** from [04 of the gameplay brief], which is a different shape from every other window. It uses the shell, the title bar, the action bar and the drag system, and replaces both the list zone and the detail zone.

| Setting | Value |
| --- | --- |
| Window ID | `skills` |
| Default size | 560 x 400 |
| Filters | `All / Combat / Non-combat` (on the left list only) |
| dragOut scope | `extended`, but the drop targets are the slots |

```text
+=== Skills ==========================================[_][?][X]=====+
| LEARNED (24)                 |  SLOTS           5 / 7          |
| [All][Combat][Non-combat]    |  +--------+ +--------+ +------+ |
|                              |  | [icon] | | [icon] | |  --  | |
|  [icon] Fixed Bombardment    |  | Combat | | Combat | | Lock | |
|  [icon] Field Repair         |  +--------+ +--------+ +------+ |
|  [icon] Sign Scanning        |  +--------+ +--------+ +------+ |
|  [icon] Directional Engine   |  |  --    | |  --    | | Lock | |
|  ...                         |  | empty  | | empty  | |      | |
|                              |  +--------+ +--------+ +------+ |
|------------------------------|---------------------------------- |
| SELECTED                                                     G  |
| Fixed Bombardment · Combat                                   F  |
| ----------------                                             S  |
| Cost 1 slot · Trigger: combat round start                    S  |
| Effect  Deal 40 damage to a chosen location on the board.    |
|         Costs 25 CP.                                         |
+-----------------------------------------------------------------+
| [Equip]  [Unequip]     status: slot 1 of 7 · click a slot to equip
+=================================================================
```

Layout numbers:

- Left list: 200 UI px, the same row component as a window list (native icon, name, 18 UI px rows), with the filter tabs above it.
- Right slot grid: 3 columns of 72 x 72 UI px slot cells. Slot cell = `steel.1` inset, 1 px `ink`, holding a 32 x 32 native icon and a 2-line label (`name` wrapped to 8 chars, then `Combat`/`Non-combat` in `steel.7`).
- A **locked** slot draws with a 5 x 5 `steel.4` lock icon and its unlock condition in `steel.4` at the bottom of the cell, truncated; the full condition is in the detail block on hover.
- The right column of single letters in the ASCII above is the **slot cost column** (`G`/`F`/`S`) — replaced in the real layout by a 12 x 12 `SlotCost` number in the cell's bottom-right corner. Shown here for completeness; with `SlotCost` fixed at 1 in v1 the number is a small `1` in `steel.7`.

Interaction, following [04 of the gameplay brief] exactly:

| Action | Behavior |
| --- | --- |
| Select then click a slot | Equips. The selected skill's row gets the `cyan.3` left bar, and clicking an empty slot fills it |
| Drag | A shortcut, never the only path. Dragging a learned skill onto a slot equips it |
| Click an equipped slot | Removes the skill, no confirmation |
| Equip onto a filled slot | Replaces. A confirm dialog names **both** skills: `Replace Field Repair with Fixed Bombardment?` The safe option (Cancel) is focused |
| Same skill twice | Refused, `Already equipped in slot 2`. The target slot flashes `rust.3` once |
| No free slot | Refused, `No free slots (7/7 equipped)` |
| Locked slot | Refused, `Locked: reach driving level 4`. The slot flashes `rust.3` once |
| Save | Immediate on every change. No `Apply` button. Failures show the reason in the status line |

**Lock states**:

| State | Left list | Right slots | Drag out of a slot |
| --- | --- | --- | --- |
| Garage | Editable | Editable | Yes |
| Expedition, combat or non-combat | Read-only, unlearned skills hidden, `steel.4` rows | Read-only, 1 px `steel.5` border instead of interactive | No |
| Expedition, viewing | A `Viewing only` banner across the top of both zones, 14 UI px, `steel.3` with `steel.7` text | | |

The banner is not optional: the player will try to change a skill mid-expedition, and a silent refusal reads as a bug. During an expedition the window's action bar has no buttons at all, and the status line reads `Locked during the expedition. Returns to editable in the garage.`

## See Also

- [15 Context Menus and Chips](15-context-menus-and-chips.md) — the quick actions these windows duplicate
- [16 Garage and Expedition Boards](16-garage-and-expedition-boards.md) — how the windows sit on each board
