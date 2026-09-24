# Dialogs

The gameplay brief asks for warning boxes, confirmation boxes and error boxes. The key finding is that **most of what looks like a box does not need to be one.** A game where the board is dragged a hundred times a minute is destroyed by modal interruptions, so this document first decides what is *not* a dialog, then specifies the four that are.

## The Four Tiers

| Tier | Form | Blocks input | Used for |
| --- | --- | --- | --- |
| **1. Status line** | One line of text, in a window's action bar or in a toast | No | Successes, and refusals that need no acknowledgement |
| **2. Toast** | A small panel that slides in and fades, 220 x auto UI px | No | Refusals and results that happen on the board, where there is no action bar |
| **3. Confirm dialog** | 220 x auto UI px, modal | Yes | Destructive or irreversible actions, and purchases above a threshold |
| **4. Alert dialog** | 260 x auto UI px, modal | Yes | A failure the player must read: expedition loss, save corruption, out of disk |

The decision rule, applied in order:

1. Does the player need to **read** this before continuing? → 4.
2. Could the player **regret** not having been asked? → 3.
3. Did the action happen on the **board** rather than in a window? → 2.
4. Otherwise → 1.

Two rules that follow from this and are not negotiable:

- **An expected failure is never a dialog.** "Not enough gold" during a shop drag is a toast, because the player is mid-gesture and a modal would steal the mouse. The ghost turns `rust.3`, the reason appears in the toast, and the drag continues to be cancellable.
- **A success is never a dialog.** Deposite, sell, craft, repair and scan all report in tier 1 or 2.

## Tier 1 — Status Line

Already specified in [13 Window System](13-window-system.md). This is the default for anything that happens inside a window.

| Message | Color | Dwell |
| --- | --- | --- |
| Success | `olive.2` | 3 s |
| Refusal | `rust.4` | 3 s |
| Hint | `steel.7` | While hovered |
| Idle summary | `steel.6` | Persistent |

A success message may carry an **undo** action for 3 s: `Sold 1 Scrap Plate for 8 gold   [Undo]`, with `Ctrl+Z` as the key. This covers the cheap-and-frequent sales so that tier 3 is reserved for sales above 100 gold.

## Tier 2 — Toast

For board-side events with no window open.

```text
+==========================================+
| !  Not enough gold (needs 300)           |
+==========================================+
   16 x 16 icon      text, font.ui, bone
```

| Property | Value |
| --- | --- |
| Size | 220 x auto UI px; height = 4 + 16 + 4, plus 12 per wrapped line |
| Position | Bottom-center of the screen, 24 UI px above the bottom edge, in a stack, newest at the bottom |
| Panel | `steel.2`, 1 px `ink`, 1 px `steel.3` top-left highlight, 3 x 3 corners. **No** modal overlay |
| Left icon | 16 x 16: `!` in `rust.4` for a refusal, `✓` in `olive.2` for a success, `i` in `cyan.3` for information |
| Text wrap | At 200 UI px. Never truncated |
| Life | 3 s for success, 4 s for a refusal, 6 s for information, then fades over `motion.fast` |
| Stack cap | 3. A 4th toast pushes the oldest out early (its life is cut to 500 ms rather than being killed instantly) |
| Click | Dismisses, or runs the attached action if there is one (`[Undo]`) |
| Motion | Slides up 8 UI px and fades in over `motion.fast`; the stack re-flows over `motion.standard` |
| Reduced motion | No slide, fade only over `motion.fast` |

Toasts never overlap the board's bottom-right (where the cursor often is) or the left HUD column. If the Info Panel is expanded, toasts shift right.

**What uses a toast**

| Event | Message |
| --- | --- |
| Refused drop on a chip | `Warehouse full (120/120)` |
| Refused purchase mid-drag | `Not enough gold (needs 300)` |
| A part is too heavy | `Over load limit by 6 · speed -40%` |
| Search found nothing | `Nothing left here` |
| Cargo hold full during a loot drop | `Cargo hold full (24/24) · item left behind` |
| A skill's target is invalid | `Fixed Bombardment needs a revealed location` |
| Time warning | `2 cycles remaining` in `hazard` |
| Save completed | `Saved` in `olive.2`, 2 s |

## Tier 3 — Confirm Dialog

```text
+==========================================================+
| Jettison cargo?                                          |
+----------------------------------------------------------+
| 3 x Scrap Plate, 1 x Truck Axle will be lost. 4 items,    |
| 46 kg.                                                    |
|                                                           |
|  This cannot be undone.                                   |
+----------------------------------------------------------+
|                              [ Cancel ]   [ Jettison ]    |
+==========================================================+
```

| Property | Value |
| --- | --- |
| Size | 220 x auto UI px, min 100 UI px tall, centered on screen |
| Background | `overlay.modal` (`ink` at 75%) over the whole screen, in `MenuLayer` |
| Panel | Same 9-slice as a window. A danger confirm adds a 3 px `rust.1` left bar |
| Title | `font.ui`, `bone`, at the top, 20 UI px title bar |
| Body | `font.ui`, `steel.7`, wrapping at 200 UI px. Names the **specific** thing |
| Consequence line | `This cannot be undone.` in `rust.4`, only when actually true |
| Buttons | Bottom-right, 4 UI px apart. 16 UI px tall minimum |
| Focus | **The safe option is focused first.** Always. For a non-destructive confirm, the safe option is still focused, but the confirm button is drawn as the primary |
| Cancel | Esc, right-click, and the window's `X` all cancel |
| Motion | Scale 96% → 100% and fade, `motion.standard`. Reduced motion: instant |
| Sound | A single low click, no alarm for ordinary confirms |

The body **must** name specifics, not categories. `Are you sure?` is banned. `3 x Scrap Plate, 1 x Truck Axle will be lost` is the standard.

### Confirmations in the game

| Action | Body | Safe button |
| --- | --- | --- |
| Jettison 1 item | `<n> x <name> will be lost. <n> items, <n> kg.` + irreversible line | Cancel |
| Jettison all of a stack | Same, with the total count | Cancel |
| Sell above 100 gold | `<name> will be sold for <n> gold.` | Cancel |
| Sell the last of a quest item | `<name> is needed for <quest>. Selling it will not fail the quest, but the evidence must be found again.` | Cancel |
| Replace an equipped skill | `Replace <current> with <new>?` — names **both** | Cancel |
| Abandon a quest | `<quest> will be returned to the camp. Progress (2/3) is kept.` | Cancel |
| Rent a vehicle | `Rent <name> for <n> gold, plus a <n> gold deposit?` | Cancel |
| Quit to desktop mid-expedition | `Your expedition and its cargo will be lost.` + irreversible line | Cancel |
| Delete a save | `<save name>, <playtime>, <date>.` + irreversible line | Cancel |
| Overwrite a save | `<save name> will be overwritten.` | Cancel |

Note that `Deposit`, `Withdraw`, `Buy`, `Craft`, `Equip`, `Search` and `Repair` are **not** on this list. They are all reversible or cheap, so they go to tier 1.

### Undo

Three actions get a real undo with `Ctrl+Z`, for 10 s or until the board changes:

| Action | Undo restores |
| --- | --- |
| A sale under 100 gold | The card, to its previous position, and the gold |
| Jettison, if the card has not been destroyed | The card, to its previous position |
| Removing a card from a stack | The card, to its previous position in the stack |

Jettison's undo is offered in the confirm's own body as a checkbox-free promise: `You can undo this for 10 seconds (Ctrl+Z)`. If the item's destruction is the whole point of a mechanic, the confirm drops the undo line and shows the irreversible line instead.

## Tier 4 — Alert Dialog

Same panel as tier 3, 260 UI px wide, with these differences:

- One button only (`OK`), or two where a choice exists.
- A 16 x 16 icon at the top-left: `!` in `hazard` for a warning, `X` in `rust.3` for a failure.
- No `X` in the corner. The player must read it.

| Alert | Body | Buttons |
| --- | --- | --- |
| Expedition lost (time out) | `The expedition ran out of time. <vehicle> and 18 items were lost.` | `Settle`, `Title` |
| Expedition lost (vehicle destroyed) | `SP reached zero. <vehicle> was wrecked. 18 items were lost.` | `Settle`, `Title` |
| Save failed | `Could not write the save file. <path>. Check free disk space.` | `Retry`, `OK` |
| Save version mismatch | `This save was made with a newer version. Loading it may lose data.` | `Load anyway` (danger), `Cancel` |
| A mod or data table failed to load | Names the file and the line | `OK` |
| The player is out of a critical resource with no path forward | `No repair packs and no gold. The expedition cannot be repaired and continuing is not recommended.` | `Evacuate now`, `Continue` |

The last one is the only dialog in the game that advises rather than asks. It exists because the alternative is a silent dead end, which is the worst possible outcome in a run-based game.

## Error Boxes: The Refusal Contract

The brief asks for error boxes. The design answer is that there are **no error boxes for refused actions** — there is the refusal contract:

Every refusal in the game produces, together:

1. A **visual** marker on the thing that refused: the card's outline flashes `rust.3` for 2 frames, or the target slot/row flashes, or the button draws at 60%.
2. A **textual** marker with the specific reason, in the right tier for the context.
3. **No state change**: a refused action leaves the world exactly as it was. No partial deposits, no half-sold stacks.

The reason string must always be in the form `<what is wrong> (<the numbers>)`:

| Good | Bad |
| --- | --- |
| `Warehouse full (120/120)` | `Cannot deposit` |
| `Not enough gold (needs 300)` | `Purchase failed` |
| `Equipment is locked during the expedition` | `Unavailable` |
| `Locked: reach driving level 4` | `Locked` |
| `Missing 2 Copper Wire` | `Missing materials` |
| `Cargo hold full (24/24)` | `No space` |

This is enforceable: the reason strings are keyed data, not free text, so a missing key shows `Missing reason key: <id>` in a debug build. That is deliberate — an unfilled reason is a bug, and it should look like one.

## Dialogs and the Paused Board

A dialog does not pause the game's simulation, but it does block board input. In the expedition, where time is a resource, this matters:

- Tier 3 and 4 dialogs in the expedition are shown with the region timer **continuing**. The confirm dialog shows the remaining time in its header (`DRIED SEABED · 1.4 cycles left`) so the player can see the cost of deliberating.
- The exception is the settlement window, which stops the timer because the run is over.

This is a deliberate design position and worth a playtest: a player who opens a confirm at 2 seconds left and loses the run will feel cheated. The mitigation is that tier 3 dialogs are only reached from deliberate actions, never from a timer, and the timer warning toast fires at 2 cycles, well before.

## Accessibility

- Every dialog's body text is also pushed to the notification element and to the status line, so a player not looking at the center of the screen is not missed.
- Focus is trapped in the dialog and returns to the previously focused element on close.
- The safe option is focused, and it is the one under `Enter`. The destructive option is never at the focus's default position.
- Reduced motion: no scale, no fade-in order change; both instant, with a 1-frame border flash.
