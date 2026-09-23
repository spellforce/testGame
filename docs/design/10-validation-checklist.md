# Validation Checklist

Use this list when reviewing mockups, the first Godot prototype, and each art batch. Each item should pass or have a written exception.

## Documentation Consistency

- [ ] Every color, spacing, type size, and duration used in other documents or in the theme exists in [02 Design Tokens](02-design-tokens.md).
- [ ] No near-duplicate colors outside the token list were introduced in art or theme files.
- [ ] Vertical layout budget sums to 1080: 136 + 16 + 746 + 16 + 118 + 48.
- [ ] Card anatomy zones sum to 252 px tall: 28 + 128 + 20 + 48 + 28.

## Layout and Scaling

- [ ] At 1920 x 1080, the HUD bands never overlap the tabletop (y 152 to 898).
- [ ] At 1280 x 720, all text stays at or above the token minimums after scaling, and nothing is clipped.
- [ ] At 1920 x 1200 (16:10), extra height becomes field bleed; card size relative to the board is unchanged.
- [ ] At 2560 x 1080 (ultrawide), HUD content stays inside the centered 1728 px safe width.
- [ ] At 390 x 844 (compact mode), the board pans, overlays go full height, and no text falls below the minimum.
- [ ] Zoom clamps at 0.80x and 1.25x, and zoom centers on the pointer.
- [ ] 8 to 10 separate cards, or 5 to 7 small stacks, fit at 1.00x without crowding.

## Cards

- [ ] Each of the 6 families is identifiable at 0.80x in **grayscale** (shape cues only).
- [ ] Titles fit or truncate cleanly at 20 px, then 15 px, and never wrap.
- [ ] In a cascade of 8 cards, every card's title and category tab are readable.
- [ ] Badges stay at fixed anchors; an empty anchor leaves no gap artifact.
- [ ] Enemy art breakout does not cover the title text.
- [ ] Equipment attached to a vehicle shows its header and left strip, and the cyan connector is visible.
- [ ] Card backs show their family tab.
- [ ] Grade inserts never change card size or family color.

## Art Quality

- [ ] Every illustration passes the 3-value grayscale test at 164 x 128.
- [ ] The subject fills 55 to 70% of the art window, and the focal point lies inside the 140 x 108 safe area.
- [ ] Saturated cyan, amber, and red cover less than 10% of any illustration.
- [ ] Lighting direction is consistent: key upper-left, rim right.
- [ ] Wear never covers the title, badges, or icons.
- [ ] Assets follow the naming convention and are delivered at 2x.
- [ ] Art is sharp at 1.25x on a 1440p or 4K display and clean (no shimmer) at 0.80x.

## Interaction and Motion

- [ ] Grab lift begins on the same frame as the press.
- [ ] Drag follows the pointer 1:1, with tilt limited to 4 degrees.
- [ ] Valid and invalid targets are distinguishable without color (brackets vs. cross glyph).
- [ ] Snap distance is 64 px; releasing outside the tabletop returns the card.
- [ ] Push-apart never moves the card that was already there.
- [ ] No animation blocks input for more than 400 ms; grabbing mid-animation completes the animation instantly.
- [ ] At most one `motion.loop` pulse is active on the board.
- [ ] Reduced motion removes scale, shake, tilt, and camera moves while keeping every state readable.
- [ ] Heavy-hit camera shake can be turned off on its own.

## Menus and HUD

- [ ] Every screen is fully usable with mouse only, keyboard only, and controller only.
- [ ] Every screen opens with a sensible default focus; Esc or B goes back one level.
- [ ] Destructive actions require confirmation and are never the default focus.
- [ ] Toasts show at most 2 at a time and never cover the tabletop center.
- [ ] The card-capacity counter changes state at 90% and 100% with both color and icon.
- [ ] UI scale (90 to 125%) affects HUD and menus only.

## Accessibility

- [ ] Body text contrast is at least 4.5:1; large display text at least 3:1 (check `text.secondary` on `surface.panel` and `surface.inset`).
- [ ] The colorblind-safe accent option keeps all shape cues.
- [ ] No flashing faster than 3 times per second.
- [ ] Interactive hit areas are at least 44 x 44 px at 1080p.
- [ ] Every state conveyed by color is also conveyed by shape, icon, or text.

## Performance

- [ ] 60 FPS with 150 cards on the board on target hardware.
- [ ] No per-card node creation during drag; particles and damage numbers come from pools.
