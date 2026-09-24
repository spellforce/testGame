# Window System

Every facility, panel and container in the game is one component: the **window**. The gameplay brief proposes this and it is the right call — a warehouse, a vehicle shop, a crafting panel and a cargo hold are all "a list of cards, a detail view, and some actions".

This document defines the window, its four zones, how zones are disabled per context, and how windows behave when they overlap the board.

## Why one window, not five

The alternative is five bespoke screens. That fails for three reasons specific to this game:

1. The player drags cards **between** containers constantly (warehouse → vehicle, cargo → warehouse, vehicle → shop). Two bespoke screens need a bridging UI; one shared window needs none.
2. `info_en.md` requires that the garage and the expedition **not** grow two inconsistent sets of card-information rules. Five screens is the fastest way to break that.
3. Card inspection, hover, badges, states and sorting are ~90% of the work in each. Writing once and parameterizing the other 10% is the whole point.

## Anatomy

A window is a `CanvasLayer`-hosted Control tree, centered over the board on first open.

```text
+==============================================================+
| # Warehouse                                    [_] [?] [X]  |  TITLE BAR  20 UI px
+===========================+==================================+
| [Search ................. ]|  +------------------------+     |
| [All][Gear][Cargo][BP]    |  |   detail card render   |     |  DETAIL  zone
|                           |  |        48 x 56         |     |
|  [L] Name        0001  W  |  |        at 3x           |     |
|  [L] Name        0002  W  |  +------------------------+     |
|  [L] Name        0003  W  |  Full Name                    |
|  [L] Name        0004  W  |  Type: Engine · Grade: standard|  LIST  zone   DETAIL zone
|  [L] Name        0005  W  |  ----------------------------  |
|  [L] Name        0006  W  |  Description text, wrapped     |
|  [L] Name        0007  W  |  in the detail pane.           |
|  [L] Name        0008  W  |                                |
|                           |  LOAD 024   DEF 012   WGT 034  |
+---------------------------+----------------------------------+
| [Sell][Deposit][Craft]   |  status: 18 items · 214 kg       |  ACTION BAR  18 UI px
+===========================+==================================+
```

Columns: list 240 UI px, detail fills the rest. Minimum window 360 x 320, default 420 x 400, maximum 720 x 560. All integer UI px so 9-slices land on whole pixels.

### Title bar — 20 UI px

| Element | Behavior |
| --- | --- |
| Icon | 9 x 9 facility icon, matching the facility chip |
| Title | `font.ui`, `bone` |
| `?` | Opens the context help window (a window about the window). 12 x 12, `steel.3` |
| `_` | **Collapse** — shrinks the window to just the title bar. State persists per window ID for the session |
| `X` | Close. Returns cards? No — closing a container window never moves cards; that is the point of the shared drag model |
| Drag area | The whole title bar except the 3 buttons. Drags the window, clamped so at least the title bar stays on screen |
| Resize | The bottom-right 8 x 8 corner, `steel.5` diagonal hatch. Drag to resize, both axes independent, clamped to min/max |

Reordering: there is **no** tab strip and no docking. Two windows can be open at once and dragged apart; the most recently clicked one is on top. Simultaneous windows are capped at 2; opening a third closes the oldest. This cap is the main defense against the screen becoming a stack of panels.

### List zone — 240 UI px wide

Three stacked parts:

**Search row** (16 UI px): a text field, 1 px `steel.5` dashed border that goes solid `cyan.3` on focus, with a 9 x 9 magnifier at the left. Esc clears. No live fuzzy matching in v1 — prefix and substring match on the short name and the full name.

**Filter tabs** (16 UI px): 3-4 tabs drawn from the window's `filter_set`, using the existing tab style. The default filter set for containers is `All / Gear / Cargo / Blueprint`. A tab with a zero count is drawn `steel.4` but stays clickable.

**Group header bar** (14 UI px, toggled by `[≡]` / `[▤]`):

| Mode | Rendering |
| --- | --- |
| `[▤] list` | The dense rows above. Default |
| `[≡] card` | A 3-wide grid of the same cards, drawn at native 48 x 56 art px in a 52 x 60 cell. Same sort, same filters, same selection |

The **rows** are the workhorse. Each row is 18 UI px tall (16 px content + 2 px gap):

```text
+---+-----------------------------+--------+--------+----+
|icn| Short name                  |  val1  |  val2  |unit|
+---+-----------------------------+--------+--------+----+
 20    1fr                              right-aligned
```

- Native icon 16 x 14 in a 20 x 16 cell. Native size, never scaled — this is what keeps the list sharp.
- `val1` / `val2` are defined by the **column profile**, not by the row. See below.
- Alternating rows use `steel.1` / `steel.2` at 1 px of vertical inset so a long list stays scannable.
- Hover: row background `steel.3`, and the detail zone swaps to that card.
- Selected (clicked): row background `steel.3` plus a 2 px `cyan.3` left bar. Selection is what the action bar acts on.
- Multi-select: Ctrl+click and Shift+click. Selecting more than one swaps the detail zone to a summary (`3 items selected · 46 kg · 210 value`) and disables the single-target actions with a reason.
- Disabled rows (a card that cannot be moved in this context, e.g. a locked blueprint): 60% `steel.1` body wash over the row, `steel.4` text, and an `!` at the far right whose tooltip gives the reason.

**Column profiles** per window type, so the columns are right without per-window code:

| Profile | val1 | val2 | unit |
| --- | --- | --- | --- |
| Weapons | ATK | ammo `cur/max` | kg |
| Parts (generic) | LOAD/DEF/HIT/EVA, whichever applies | bonus `+n` | kg |
| Cargo | count | unit value | kg |
| Blueprints | material count | craft seconds | — |
| Vehicles | SP% | cargo `n/m` | t |

**Sort**: a 4-item control in the group header bar — `Name`, `Type`, `Value`, `Weight`, each with a `^`/`v` toggle. Default per window (warehouse sorts by type, shop by name). Sorting is stable and tie-breaks on name, so lists never shuffle under the player's hand.

### Detail zone

Two states, never both at once:

**A card is hovered or selected** — the card's full detail sheet (see [Card Detail Sheet](#card-detail-sheet)). Hover previews and is lost when the mouse leaves; click pins. A pinned detail survives hovering other rows. Pinning shows a 3 x 3 `cyan.3` corner mark in the detail pane's top-right and clears on Esc.

**Nothing is hovered** — the window's own summary: total item count, total weight, total value, capacity bar if the container is limited, and the 3 most recent changes (`+ Scrap Plate x2`). The summary never jumps in height, so the pane does not flicker when the mouse crosses empty space.

### Action bar — 18 UI px

Left: context actions for the selected row(s), as buttons. Right: the status line, `steel.7` text, 1 line.

The status line carries three kinds of message, in priority order:

1. **Error**, `rust.4` — a refused action's reason, held for 3 s (`Cannot deposit: warehouse full (120/120)`).
2. **Hint**, `steel.7` — what is hovered and what it will do (`Deposit 1 item · double-click to deposit the stack`).
3. **Idle**, `steel.6` — `18 items · 214 kg · 1,240 value`.

Every action bar has, at minimum: `[Sell]`, `[Deposit]`, and the window-specific one. A button whose action is unavailable is **disabled with a reason**, never hidden — hiding makes the layout shift and teaches the player nothing. Disabled buttons keep their footprint, draw at 60%, and put the reason in the status line on hover.

## Disable, Hide or Show

The brief asks for a generic page that disables, shows or hides per context. Three mechanisms, in order of preference:

| Mechanism | When | Examples |
| --- | --- | --- |
| **Parameterize** | The zone is meaningful but its contents differ | Columns, filters, sort default, title, icon, which summary fields |
| **Disable with reason** | The action is legal in this window type but not right now | Sell in the cargo hold during an expedition → `You cannot sell during an expedition` |
| **Hide** | The zone has no meaning here at all and would be a lie | Search box in a 6-slot cargo hold; filter tabs in a 3-row pickup list |

Rules:

- **Hide the row, disable the button.** A zone that is a whole zone (search, filters, the card/list toggle, the tab strip) is hidden when inert, and the remaining zones reflow upward. A single **button** is disabled with a reason.
- Never leave an empty bordered box where a hidden zone was. Reflow, do not reserve.
- Anything hidden must be recoverable: the `?` help window lists what this window hides and why.

## Window Definitions

The version 1 set. `dragOut` = cards can be dragged out of the list onto the board.

| Window | Title | Filters | Columns | Actions | dragOut | Notes |
| --- | --- | --- | --- | --- | --- | --- |
| **Warehouse** | Warehouse | All / Gear / Cargo / Blueprint | By type | Deposit, Withdraw, Sell, Use, Craft here | Yes | Capacity-limited; the summary shows the capacity bar |
| **Cargo hold** | `<Vehicle> Cargo` | All / Supply / Material / Valuables | Cargo | Take out, Put in, Use, Jettison | Yes | Attached to a vehicle; showing it also outlines that vehicle card on the board with a `cyan.3` 1 px outline so the link is visible |
| **Vehicle Shop** | Vehicle Shop | Weapons / Engines / C units / Cargo / Blueprints | By type | Buy, Buy 5, Sell (from cargo), Buyback | Yes | Prices from `Datas`; the gold counter sits in the title bar right |
| **Taxi Shop** | Taxi Shop | Light / Heavy / Special | Vehicles | Rent, Inspect | Yes (fuel, packs) | Rental terms shown in the detail zone |
| **Vehicle Factory** | Vehicle Factory | Recipes / Materials | Blueprints | Add material, Remove, Craft 1, Craft all | Yes | Recipe list left, material slots in the detail zone |
| **Hunter Camp** | Hunter Camp | Open / Accepted / Done | Quests | Accept, Abandon, Claim | No | Quest cards show as rows; the evidence items show in the detail zone |
| **Skills** | Skills | All / Combat / Non-combat | Skills | Equip, Unequip | Yes (to a slot) | See [16 Skills Panel](#skills-panel-note) note in [07 of the gameplay brief] |
| **Pickup / Loot** | `<source>` | None | Compact | Take all, Take selected, Leave | Yes | Small: 320 x 260 default, 4 columns of 3 card cells. Opens next to the source card on the board |
| **Craft queue** | Queue | None | Recipes | Cancel, Reorder | No | Appears inside the Vehicle Factory window as a bottom strip rather than its own window |

The **Pickup** window is the exception to the row layout: it uses the card grid mode as its **only** mode, because the decision there is "which of these 8 things do I want" and a 3-wide card grid answers it faster than a list. It therefore hides the search box, the filter tabs and the list toggle.

## Dragging Out

Cards in a list or grid can be dragged onto the board, which is the mechanic the brief asks for.

1. **Press and hold on a row for 120 ms** (`motion.fast`), or drag more than 5 UI px, starts the drag. The drag threshold is longer than on the board because a list row is also a click target and a list is scrollable, so the drag cannot be instant.
2. On drag start, a **ghost** of the card is created on the board layer: the real `Card.tscn`, face up, held and lifted, with the pointer at its grab point.
3. The ghost is real. It pushes other root cards aside (existing push rules, [06 Interaction and Motion](06-interaction-motion.md)) and it can be dropped on a stack, into a slot, or on empty board.
4. **Scope**: the boundary between a window and the board is a **ghost scope**:

| Ghost scope | Windows | Can be dragged where |
| --- | --- | --- |
| `extended` | Warehouse, Cargo hold, Pickup, Factory, Skills | Anywhere on the board. Dropping on empty board leaves the card there — legal, surfaced as a relocation |
| `deferred` | Vehicle Shop, Taxi Shop | Dragging out **preauthorizes a purchase**. The card is a preview; dropping it commits the transaction at the shown price. The card's right badge shows the price instead of its value while the drag is in `deferred` scope |
| `closed` | Hunter Camp quests | Rows are not draggable. The cursor shows the plain arrow, not the open hand |

The `deferred` scope is the answer to "cards can be dragged to the shop to sell": the **reverse direction** also works — drag a card from the board onto any open window whose action bar has `Sell` and the card sells, with the price shown on the card during the drag. Both directions use one rule: **the window's action bar decides what a drop means**.

5. Right-click during a drag cancels it (existing rule) and the ghost returns to the list.
6. While a ghost is out, the source window stays open and marks the source row `steel.4` for the duration, so the player can see what they have taken.

## Overlapping the Board

Windows open centered over the board, 8 UI px apart in a diagonal cascade if one is already open.

- The board stays **interactive** where a window does not cover it. There is no modal dim by default. This is what makes dragging between a window and the board feel continuous.
- A window that covers a card the player is trying to see can be dragged, collapsed (`_`) or closed (`X`). All three are always available.
- The **card table's drop zones must never be fully occluded**: when a window would cover the top slot band or the pinned region card, the window's initial position shifts down to clear it. This is checked once at open time, not continuously.
- Dragging over a window does **not** pan the board (existing rule: only empty board or world pans). Dropping a ghost on a window that does not accept it returns the ghost to its source row and flashes that row's left bar `rust.3` once.

## Modal Windows

Some things must block. Those use `overlay.modal` (`ink` at 75%) and the `MenuLayer`:

- Confirmation and warning dialogs (see [17 Dialogs](17-dialogs.md)).
- Save / load, settings, game over.
- Anything the player must answer before the game continues.

A modal window disables board input, including pan and zoom, and the cursor is confined to the modal's focus order for keyboard navigation. Cards can still be dragged *within* a modal if it contains a board — but no v1 window does.

## Movement and Feedback

| Action | Motion |
| --- | --- |
| Open | The window scales from 96% to 100% and fades 0→1, `motion.standard` (200 ms), ease-out. It does **not** slide from an edge — a slide implies a direction the window does not have |
| Close | Reverse, `motion.fast` (120 ms) |
| Collapse / expand | Height animates, `motion.standard`. The title bar stays fixed |
| Resize | Immediate, no easing. The resize must track the pointer exactly |
| Move | Immediate while dragging, with a 4 art px snap to other windows' edges and to the screen halves |
| Hover a row | `motion.instant` (60 ms) background change |
| Status line message | `motion.fast` in, holds, `motion.fast` out |

Reduced motion: no scale on open/close, no height easing — instant, with a 1-frame `white` flash on the border so the change is still marked. The resize and move behavior is unchanged because it is direct manipulation.

## Performance

- A window builds its rows **once** on open and re-uses them on scroll (virtualized: only rows intersecting the viewport exist). A 500-item warehouse must open in under 2 frames.
- Only the hovered row's detail sheet is built. Building a 500-row detail cache is the classic way to make this slow.
- Windows are freed on close, not kept hidden, except the two most recent (so re-opening the same window is instant).

## Card Detail Sheet

The shared detail renderer, used by the window's detail zone **and** by the in-world Info Panel ([07 Menus and HUD](07-menus-and-hud.md)). One layout, two sizes:

| Context | Card render | Text |
| --- | --- | --- |
| Window detail zone | 48 x 56 at 3x = 144 x 168 art px | `font.ui` (12 px), full description |
| Info Panel (in-world, 172 x 147 UI px) | 48 x 56 at 1x, top-left | `font.ui`, description truncated with `…` and `[more]` |

Sections, in order, each omitted entirely when empty:

1. Card render + short title in the family's title color on the family body color
2. `full_name` and a type line (`Engine · Grade: standard`)
3. `description`
4. **Stat pairs** — the same `steel.7` label / `bone` value rows as the vehicle sheet
5. **Effect list** — one line per effect, `cyan.3` for gains and `rust.4` for costs, with the triggering condition in `steel.7` after a dash
6. **Traits** — chips
7. **Source** — where this card comes from (`Found in: Ruined Convoy, Sandstorm Cache`). Data-driven; the list is empty in v1 and the section is hidden

The player must never have to infer an effect from the skill's or item's name. [04 of the gameplay brief] says the UI must be data-table driven and must not judge effects from names or description text; this layout is how that requirement is met — section 5 is rendered from effect data, never parsed from prose.

## Accessibility

- Every window is fully keyboard navigable: Tab moves between zones, arrows move within a list, Enter selects, Space toggles, Esc closes or unpins. Focus rings use the existing `cyan.3` 1 px outline.
- The status line's error messages are also pushed to the notification element so they are not missed by a player looking at the board.
- Minimum interactive target is 16 x 16 UI px (32 x 32 screen px at 1080p), from [07 Menus and HUD](07-menus-and-hud.md).
- Row text is never truncated without an ellipsis, and the full string is in the detail zone.
