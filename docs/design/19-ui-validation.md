# Validation Checklist

Extends [10 Validation Checklist](10-validation-checklist.md). That document covers the board, cards and pixel art; this one covers the interface system added in documents 11 to 19. Check both lists on every build.

## Errata Applied First

- [ ] **Errata applied**: `board.slot_band` is 164 (was 104), the play area is 1104 x 480 (was 538), and the card art area is 29 rows ([04 Card System](04-card-system.md) says 37). See [20 Questions and Errata](20-questions-and-errata.md). If any document or `.tres` still holds the old number, the build is wrong.

## Taxonomy

- [ ] All 8 families are distinguishable in a **grayscale** screenshot by shape cue alone, at 1 screen px per art px.
- [ ] Facility (`steel.2`) and Cargo (`steel.4`) are told apart in grayscale. This is the pair most likely to fail.
- [ ] The Region card (`sand.1`) does not vanish against the `sand.2` world terrain behind the board at zoom 0.5.
- [ ] The Region card reads as a card and not as board terrain against the `steel.6` plate.
- [ ] Every gameplay type from the brief maps to exactly one family with no orphans.
- [ ] A pack is recognizable as a pack (wrapper band) regardless of its contents' family.
- [ ] Part sub-cues: the 6 kind glyphs are distinguishable at 1x without the title.
- [ ] The Supply band on a Cargo card is visible in a 10-card stack.
- [ ] The evac beacon is findable on a board of 20 cards at zoom 0.5 without reading any title.
- [ ] Only the protagonist card has the dog-tag notch; no other silhouette has a cut corner.

## Vehicle Card Face

- [ ] Card rows add up: 1 + 12 + 1 + 29 + 12 + 1 = 56.
- [ ] Vehicle icons are bottom-aligned to row a26 and a row of 4 vehicles on the board stands on one ground line.
- [ ] The vehicle icon never overlaps the cargo band (rows a27-a28) or the protagonist chip.
- [ ] The protagonist chip appears only when a protagonist card is in the stack, and hovering it selects the protagonist, not the vehicle.
- [ ] The cargo band is hidden when the hold is empty, and turns `rust.3` at capacity.
- [ ] The SP plate changes color at 60% and 30%, and the number is legible at zoom 0.5.
- [ ] The overload chevron appears only when cargoUsed > cargoMax.
- [ ] A 9-card vehicle stack is 152 art px tall and the equipment's indent marks are visible.

## Stacks

- [ ] Grabbing a protagonist card in a vehicle stack takes it and the equipment above it, leaving the vehicle.
- [ ] Dropping a part on a vehicle inserts it at the correct group position, not on top.
- [ ] Dropping a 6th weapon is refused with a reason naming the limit.
- [ ] A damaged part shows a 1 px `rust.3` bar and a destroyed part a full-width one, and this reads correctly in a 9-card stack.
- [ ] The count plate appears at 10 cards and not at 9.

## Window System

- [ ] A 500-item warehouse opens in under 2 frames and scrolls without creating nodes.
- [ ] The window's 4 zones reflow upward when a zone is hidden, leaving no empty bordered box.
- [ ] Every disabled button keeps its footprint and gives a specific reason on hover.
- [ ] No button is ever hidden where it could have been disabled with a reason.
- [ ] Two windows can be open and dragged apart; opening a third closes the oldest.
- [ ] Moving a window never fully occludes the top slot band or the pinned region card.
- [ ] A window dragged mostly off screen can still be recovered by its title bar.
- [ ] The list and card modes show the same items, in the same order.
- [ ] Sorting is stable: scrolling and re-sorting never makes two equal-named rows swap.
- [ ] Multi-select swaps the detail zone to a summary and disables single-target actions with a reason.
- [ ] The status line's priority order (error > hint > idle) holds when messages arrive together.

## Dragging Out

- [ ] A 120 ms hold or a 5 UI px drag starts the ghost; a click selects without dragging.
- [ ] The ghost pushes root cards, can join a stack, and can be dropped on empty board.
- [ ] In the shop (`deferred` scope) the ghost shows its **price** badge and the projected gold balance, and refuses when unaffordable.
- [ ] Dropping a board card on an open window runs the window's action bar verb.
- [ ] Right-click cancels a ghost and the source row returns to normal.
- [ ] Dragging over a window does not pan the board.

## Chips and Menus

- [ ] A chip is never mistaken for a card: no shadow, no title row, and it never lifts on hover.
- [ ] A chip is never draggable and never stacks.
- [ ] Dropping a card on all 7 garage chips does the documented thing; the Hunter Camp refuses with a reason.
- [ ] A card with no available menu entries shows **no menu**, not an empty panel.
- [ ] Every menu row that is disabled gives a specific reason.
- [ ] The safe option is focused in every confirm.
- [ ] Every menu row has a keyboard equivalent and every chip has a number key.
- [ ] The skill tray is present only in the expedition; the facility chips only in the garage.
- [ ] A skill chip's cooldown arc is visible at zoom 0.5.

## Dialogs

- [ ] No dialog appears for a success, and none for an expected failure during a drag.
- [ ] Tier assignment holds: `Deposit`, `Withdraw`, `Buy`, `Craft`, `Equip`, `Search`, `Repair` never open a dialog.
- [ ] Every confirm body names **specific** items and counts. `Are you sure?` appears nowhere.
- [ ] The irreversible line appears only when the action is actually irreversible.
- [ ] A toast stack of 4 never exceeds 3 visible, and the 4th pushes the oldest out gracefully.
- [ ] A toast never covers the board's bottom-right or the left HUD column.
- [ ] `Ctrl+Z` restores a sub-100-gold sale, a jettison and a stack removal within their windows.
- [ ] In the expedition, a confirm dialog shows the remaining region time in its header.
- [ ] Every refusal produces a visual marker **and** a reason string, and no state change.
- [ ] Every reason string matches the `<what is wrong> (<numbers>)` form. In a debug build, an unfilled reason key prints `Missing reason key: <id>`.

## Availability and Locking

- [ ] During an expedition, the skills panel is read-only, shows the `Viewing only` banner, and has no action bar buttons.
- [ ] An equipment change attempt during an expedition is refused with `Equipment is locked during the expedition`.
- [ ] A cargo hold is still fully usable during an expedition, including jettison.
- [ ] `Sell` is disabled in the cargo hold during an expedition with the correct reason.
- [ ] A blueprint can always be deposited, even at warehouse capacity, and the `?` help says so.
- [ ] A rental vehicle has no sell action and shows the rental marker.
- [ ] `Claim` on a complete quest is disabled during an expedition with `Settle your expedition first`.

## Boards

- [ ] `14 + 81 + 9 + 48 + 12 = 164` for the band; the divider is at y 164.
- [ ] The play area is 1104 x 480 and a 9-card vehicle stack fits 3 times over.
- [ ] The protagonist's home cell is dashed while unmounted and absent while mounted.
- [ ] Garage card positions survive a save, load and quit.
- [ ] The expedition board rebuilds from the region every time and persists nothing.
- [ ] Returning to the garage restores every hidden card to its exact previous position.
- [ ] The region card cannot be dragged, is skipped by align-to-grid, and cannot be selected with select-all.
- [ ] The region tint is subtle enough that card bodies still read against the plate.
- [ ] The settlement window has no `X`, and `Review loot` opens the cargo hold behind it.
- [ ] On a failed run, the settlement window shows every lost item, struck through.

## Performance and Scale

- [ ] 60 FPS with 200 cards on the board.
- [ ] No node creation during a drag.
- [ ] A window opens in under 2 frames with 500 rows, and only the visible rows exist.
- [ ] Two windows plus a full board hold 60 FPS.
- [ ] The push check uses the spatial grid, not all-pairs.

## Accessibility

- [ ] Every window, menu and dialog is fully keyboard navigable with a visible focus ring.
- [ ] Every zone toggle (`[≡]` / `[▤]`), filter tab and sort control has a keybinding.
- [ ] Error messages reach the notification element, not only the status line.
- [ ] Reduced motion removes window scale, toast slide, dialog scale, chip hover transitions and idle wobble, while keeping the physics identical.
- [ ] No flashing exceeds 3 per second; the cargo band blink and the evac beacon pulse are both within it.
- [ ] Every interactive target is at least 16 x 16 UI px.
- [ ] A card title never wraps and never ellipsizes; long names are shortened in data.
- [ ] The player's uploaded portrait passes the contrast check and does not vanish on the card.
- [ ] The avatar import never applies a color filter, and the `?` help says so.
