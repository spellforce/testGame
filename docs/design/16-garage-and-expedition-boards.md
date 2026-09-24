# Garage and Expedition Boards

[03 Canvas and Layout](03-canvas-layout.md) defines the board's geometry. This document defines what **sits in it** on each of the two boards, and the one geometry change the rest of the system requires.

## One Board, Two Layouts

The garage and the expedition are the same board with different slot band contents and different fixture sets. [01 of the gameplay brief] requires them to be the same operating system with reused interactions; this is that requirement in layout terms.

| | Garage | Expedition |
| --- | --- | --- |
| Slot band rows | 1 (facilities) | 2 (region + skill tray) |
| Pinned fixtures | Protagonist home cell, facility chips | Region card, evac point area |
| Board background | Deck plate, calm | Deck plate, plus the region's terrain tint at 12% and a dust overlay that thickens as the time limit drops |
| Window availability | All 6 facility windows | Cargo hold, Pickup, Skills (read-only) |
| Time pressure | None | Region timer, shown in the Time Box |

## Slot Band Revision

[03 Canvas and Layout](03-canvas-layout.md) has a 104 art px band holding one row of 9 slots of 60 x 81. The skill tray needs a home in the expedition, and the cleanest one is a second row inside the band rather than a new floating element.

**Revised band: 164 art px, two rows.**

```text
+--------------------------------------------------------------+  Board 1104 x 644
|  +------+ +------+ +------+ ... +------+   row 1: 60 x 81    |
|  |      | |      | |      |     |      |   14 + 81 + 9 = 104 |
|  |      | |      | |      |     |      |                      |
|  +------+ +------+ +------+ ... +------+                      |
|  +----+ +----+ +----+ +----+            row 2: 40 x 48        |
|  |    | |    | |    | |    |            104 + 48 + 12 = 164   |
|  +----+ +----+ +----+ +----+                                  |
|==============================================================|  divider
|                                                              |
|            CARD PLAY AREA 1104 x 480                          |
|                                                              |
+--------------------------------------------------------------+
```

| Item | Value |
| --- | --- |
| Band height | 164 art px (was 104) |
| Row 1 | 9 slots of 60 x 81, 64 px pitch, top at y 14, bottom at y 95 |
| Gap | 9 art px |
| Row 2 | Skill tray, 40 x 48 chips at 8 px pitch (48 px pitch), top at y 104, bottom at y 152 |
| Below row 2 | 12 art px to the divider at y 164 |
| Play area | 1104 x 480 (was 538) |

`board.slot_band` becomes **164**. This is an erratum against [02 Design Tokens](02-design-tokens.md) and [03 Canvas and Layout](03-canvas-layout.md); both are updated. See [20 Questions and Errata](20-questions-and-errata.md).

**Why not float the tray?** A floating tray overlaps cards, and the play area is already 8.6 card heights — losing 58 art px (about 1 card height) is a real cost. Three things make it the right trade:

1. The tray's chips are standing abilities, not cards. Putting them in the band beside the other non-card fixture (the facility chips) teaches the "band is for places, board is for things" rule with zero text.
2. Row 2 has 23 chip slots at 48 px pitch across 1104 px. A player has at most 7 equipped skills, so the row is mostly empty — see the next point.
3. The empty right side of row 2 is where **region pack cards** land during an expedition. Packs are drawn one at a time and disappear; giving them a home in the band keeps them out of the player's working space. In the garage, that space holds the locked `???` facility slots at 8 and 9.

**Play area consequence**: 1104 x 480 is 23 x 8.6 cards, still over Stacklands' 21.1 x 10.5 ratio on the horizontal axis and slightly under vertically. A full 9-card vehicle stack is 152 tall, so it fits 3 times over. Acceptable.

The band's **new-content marker** ([03 Canvas and Layout](03-canvas-layout.md), a 7 x 7 starburst) applies to both rows.

## Garage Layout

```text
+--------------------------------------------------------------+
| [WH][HC][TX][VS][VF][SK][ P][??][??]      row 1 facilities     |
| [  ][  ][  ][  ][  ][  ][  ][  ][  ]      row 2 empty         |
|==============================================================|
|                                                              |
|  (1,1)          (3,1)               (6,1)                    |
|  +-------+      +-------+           +-------+                |
|  |REGION |      |SKILLS |           |SHOP   |   <- not cards; |
|  | slot  |      | test  |           | bench |      see below  |
|  +-------+      +-------+           +-------+                |
|                                                              |
|        +--------+                                            |
|        |PROTAG  |  <- protagonist home cell, cyan.3 dashed    |
|        |  home   |                                            |
|        +--------+                                            |
|                                     +--------+               |
|                                     | VEHICLE|               |
|                                     |  (1)   |               |
|                                     +--------+               |
+--------------------------------------------------------------+
```

The garage's play area has **no fixed fixtures** other than the protagonist's home cell. The three boxes marked above in the ASCII are illustrative of where a player's own layout drifts, not spec. The one rule the garage enforces:

- **Protagonist home cell** — at board cell (11, 7), roughly center-low. It is where the protagonist card returns when the player clicks the protagonist chip, or presses `H`. It is drawn as a 1 px `cyan.3` **dashed** outline, 48 x 56, always present while the protagonist is unmounted, and it disappears while the protagonist is on a vehicle. A dashed outline is this design's "an empty thing can go here" cue (the same cue as an empty mount slot in [12 Vehicle Card Face](12-vehicle-card-face.md)).

Everything else in the garage is placed by the player, and the garage remembers it: card positions in the garage persist across save/load, because a base the player has arranged and then lost is worse than no arrangement.

**Not persisted**: the expedition board. It is rebuilt from the region every time.

## Expedition Layout

```text
+--------------------------------------------------------------+
| [PK][PK][EV][  ][  ][  ][  ][  ][  ]      row 1                |
| [SK][SK][SK][SK][  ][  ][  ][  ][  ]      row 2 skill tray     |
|==============================================================|
| +--------+                                                   |
| | REGION |  pinned, cell (1,1)                               |
| |  CARD  |                                                   |
| +--------+                                                   |
|                                                              |
|      (4,3) +--------+        (7,2) +--------+                |
|            |  SITE  |              |  SITE  |                |
|            +--------+              +--------+                |
|                                                              |
|                 (5,6) +--------+        (9,5) +-------+      |
|                       | ENEMY  |              | EVAC  |      |
|                       +--------+              +-------+      |
|                                                              |
|  (3,8) +--------+                                            |
|        |VEHICLE |                                            |
|        +--------+                                            |
+--------------------------------------------------------------+
```

| Fixture | Position | Behavior |
| --- | --- | --- |
| Region card | Pinned to board cell (1,1), top-left of the play area | Not draggable (state: Pinned). Hovering shows the region detail sheet. Its right badge counts down the time limit |
| Region pack | Row 1, band slot 1 | Drawn one card at a time. Refills only when a location produces a new pack |
| Evac pack | Row 1, band slot 3 | Empty until an evacuation point has been revealed; then it holds the evac card until the player takes it out |
| Skill chips | Row 2, left to right in slot order | See [15 Context Menus and Chips](15-context-menus-and-chips.md) |
| Spawn slots | Row 1, slots 2 and 4-9 | Where newly revealed cards land before the player moves them, so a reveal does not interrupt an in-progress drag |
| Search slots | Row 2, right of the skill chips | Where search results land |

**Placement of revealed cards**: a card revealed by a location search is thrown out using the existing pack launch path (514 px/s sideways, 571 up, [06 Interaction and Motion](06-interaction-motion.md)) aimed at the nearest free spot in the play area, preferring a spot inside the camera view. It does not land in a band slot unless the play area is so full that there is nowhere else, in which case it lands in a spawn slot and the slot shows a `!` in `hazard`. The band slots are a **backstop**, not the normal path.

**Region tint**: the board's deck plate is tinted toward the region's palette color at 12%, so the Dried Seabed is warm and a Rustbelt region is redder. This is a single `modulate` on the board sprite and costs nothing. At below 25% of the time limit, a 16 x 16 dust overlay tiles across the board at 20% opacity and the Time Box's bar goes `rust.3`.

## Pin State

"Pinned" is a card state added in [11 Card Taxonomy](11-card-taxonomy.md). A pinned card:

- Cannot be dragged. Grabbing it shakes it 1 px left-right twice and shows a 5 x 5 `steel.5` clip icon.
- Still a valid drop target and still hoverable.
- Still pushes and is pushed — it is a root card.
- Is excluded from **Align to Grid** ([06 Interaction and Motion](06-interaction-motion.md)) — it must not be snapped off its cell.
- Is excluded from **select all**.

Only the region card and the two band rows' contents use it.

## Transitions Between Boards

Going to an expedition and coming back are the two biggest state changes in the game. The visuals follow Stacklands' pattern of not hiding the board.

| Transition | Sequence |
| --- | --- |
| Garage → Expedition | 1. The player confirms the region. 2. The vehicle and its mounted stack lift and slide to the board's bottom-center over `motion.slow` (350 ms). 3. Everything else on the garage board — facilities are chips and do not move; the player's other cards do not move — is **hidden**, not shuffled. 4. The band rebuilds: row 1 swaps to the region setup, row 2 fills with the skill tray. 5. The vehicle lands. 6. The region card drops in pinned, from the top, with a 1-frame `white` flash. Total 900 ms |
| Expedition → Garage | Reverse, plus the settlement window (see below) |
| Failed expedition | The vehicle is **dissolved** (existing dissolve mask, 6 frames) rather than slid, and the return is immediate with a `rust.3` flash on the board edge |

Cards that were on the garage board but are not going on the expedition are **hidden** rather than moved. When the player returns, they reappear exactly where they were. This avoids the "where did all my stuff go" problem of shuffling them into the warehouse automatically, and it means a player can leave a repair job half-finished.

The garage board is hidden, not unloaded: `visible = false`. Picking it back up is instant and needs no rebuild.

## Settlement

Returning from an expedition opens the **Settlement window** — a modal window (see [13 Window System](13-window-system.md)) that is the one place the game reports.

```text
+=== Expedition Result ==================================[_][?][X]====+
| ROUTE SECURED                                       (cyan.3 heading)|
+---------------------------------------------------------------------+
| DRIED SEABED                    in 4 cycles / limit 6               |
|                                                                     |
| CARGO RETRIEVED                                        18 items      |
|   [icon] Scrap Plate x6        [icon] 76mm Cannon x1                |
|   [icon] Fuel Cell x3          [icon] Truck Axle x1                 |
|                                       ... and 7 more                |
+---------------------------------------------------------------------+
| QUESTS                                                              |
|   Ruined Convoy Cull          COMPLETE      240 gold, 1 blueprint    |
|   Fuel Run                  1 / 2            not claimed            |
+---------------------------------------------------------------------+
| VALUE  1,240 gold        SP  4,020 / 4,200        REPAIRS  3 needed  |
+---------------------------------------------------------------------+
|                                        [Settle]  [Review loot]      |
+======================================================================
```

- The heading is the existing stamp-style headline from [07 Menus and HUD](07-menus-and-hud.md) — `ROUTE SECURED` in `cyan.3` or `CONVOY LOST` in `rust.3`.
- `Settle` moves the cargo into the warehouse (respecting capacity; overflow stays on the vehicle and the window says so) and returns everything to the garage. It is the only way out; there is no `X` on this window.
- `Review loot` opens the cargo hold window **behind** the settlement window so the player can inspect before committing. This is the reason the settlement window is 2 panes rather than a single list.
- On a failure, the CARGO RETRIEVED block reads `LOST` in `rust.4` and the item rows are drawn at 60% with a strikethrough. It is important that the player sees exactly what they lost.

## Camera Per Board

| | Garage | Expedition |
| --- | --- | --- |
| Default zoom | 1.0 | 0.5 (min) |
| Why | The garage is about arranging, so see detail | The expedition is about the region, so see the whole board |
| Auto-pan | None | 250 ms nudge when a revealed card lands off-screen (existing rule) |
| Region card nudge | — | Yes, on reveal |
| Clamp | Board stays on screen (existing rule) | Same |

The expedition's default zoom of 0.5 is the zoom at which a card is 48 x 56 screen px at 1080p — the same as the reference screenshots. It is the zoom most of the game is played at, which is exactly why [12 Vehicle Card Face](12-vehicle-card-face.md) treats 0.5 as the readability floor rather than an edge case.
