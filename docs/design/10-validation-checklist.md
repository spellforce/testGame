# Validation Checklist

Check these when reviewing art, the first Godot build, and every new batch.

## Numbers

- [ ] Card rows add up to 56: 1 + 12 + 1 + 29 + 12 + 1. Columns add up to 48: 1 + 46 + 1.
- [ ] Board is 1104 x 644 art px (23 cards wide, 11.5 cards tall; ratio 1.714).
- [ ] Slot band: 14 + 81 + 9 + 48 + 12 = 164, and the divider sits at y 164 (**E3** in [20 Questions and Errata](20-questions-and-errata.md)). Pack slots are 60 x 81, skill tray chips are 40 x 48.
- [ ] Play area is 1104 x 480 (644 - 164).
- [ ] 9 slots at 64 px pitch = 560 px, centered in 1104.
- [ ] UI layout: list panel 4 + 380 + 5 + info panel 147 + 4 = 540.

## Match With Stacklands (Compare With 1.jpeg / 2.jpeg)

- [ ] At minimum zoom on 1080p, a card is 48 x 56 screen px and the board takes about 58% of the screen width.
- [ ] Dragging empty board or world pans; dragging a card moves the card.
- [ ] Grabbing a card in the middle of a stack takes it and the cards on top of it.
- [ ] Only the header of lower cards shows in a stack, and every title in a 10-card stack is readable.
- [ ] Overlapping stacks drift apart on their own, slowly, and the lighter stack moves more.
- [ ] A dragged card never pushes and is never pushed; only a root card pushes at all.
- [ ] A dropped card stays where it was released, and is never sent back to where it was picked up.
- [ ] Releasing outside the card area slides the card to the nearest legal spot (11 px inside the edge).
- [ ] The align-to-grid action snaps free cards to an 86 x 96 px grid and flashes a faint grid overlay.
- [ ] Opened packs throw cards out one at a time in arcs.
- [ ] Pausing shows "PAUSED" and cards can still be moved.
- [ ] The board can't be panned fully off screen.

## Camera and 3D prototype

- [ ] The 48 x 56 card prefab is at 100% native scale (`scale = 1`); no card logic changes its scale to match a screenshot.
- [ ] The 2D baseline still passes its existing interaction self-test before the 3D prototype is enabled.
- [ ] If 3D mode is enabled, the board is a horizontal perspective surface and its projected top/bottom width matches the reference within the agreed tolerance.
- [ ] 3D cards are camera-facing billboards with readable rectangular faces at the board center and near both depth extremes.
- [ ] Cursor zoom preserves the board point under the cursor during the eased camera-height transition.
- [ ] Screen picking uses the active camera ray and board-plane intersection; no guessed zoom conversion is used.
- [ ] Logical card positions, clamp margins, stacks, and saves are identical after switching between 2D and 3D render modes.
- [ ] Depth ordering is deterministic for roots, stacks, held cards, and effects.
- [ ] The 3D prototype meets the 200-card frame-time target before it replaces the 2D baseline.


- [ ] Every asset uses only the 32 palette colors.
- [ ] No blurry pixels anywhere: check textures use Nearest filtering and no mipmaps.
- [ ] No card is ever scaled or rotated (check flip, spawn, hit, destroy).
- [ ] Cards at rest sit on whole art pixels.
- [ ] At zoom 0.5, 1.0, 1.5 and 2.0 on 1080p every art pixel is the same size.
- [ ] In between those zooms, uneven pixels are barely visible; the pixel-perfect setting removes them.
- [ ] Text is drawn only at native size or whole multiples in the UI.
- [ ] Icons pass the checklist in [08 Asset Production](08-asset-production.md).

## Cards

- [ ] All 6 families are recognizable in a grayscale screenshot, by shape cue alone.
- [ ] Every card title fits in the header with no wrapping.
- [ ] Badges stay readable on every family body (especially the enemy's right badge).
- [ ] Disabled cards still show a readable title.

## UI

- [ ] UI stays sharp at 1280 x 720 (1x), 1920 x 1080 (2x), 2560 x 1440 (2x) and 3840 x 2160 (4x).
- [ ] On 21:9 screens the left column stays on the left, the top-right boxes on the right, and the world fills the middle.
- [ ] Every button is at least 16 UI px tall.
- [ ] Every menu can be used with the keyboard.
- [ ] Quitting and other destructive actions ask for confirmation, with the safe option focused.

## Accessibility

- [ ] Card titles meet 4.5:1 contrast (values in [02 Design Tokens](02-design-tokens.md)).
- [ ] No flashing more than 3 times per second.
- [ ] Reduced motion and camera shake settings work.

## Performance

- [ ] 60 FPS with 200 cards on the board.
- [ ] No new nodes created during a drag.
