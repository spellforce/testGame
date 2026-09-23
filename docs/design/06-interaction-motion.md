# Interaction and Motion

Motion shows cause and effect: the player grabs a heavy steel card, it lifts, it lands, something happens. Cards should feel weighty and mechanical, not floaty. Every duration below comes from the motion tokens in [02 Design Tokens](02-design-tokens.md).

## Motion Character

- **Weight**: cards accelerate quickly and settle with a small overshoot (at most 4%), like a steel plate dropped onto felt over metal.
- **Directness**: a card under the pointer follows it with no lag. Smoothing applies only to tilt and shadow.
- **Locality**: effects stay on or next to the card that caused them. Only the tabletop-wide events described below may shake the camera.
- **Priority**: at most one looping animation on the board at a time (`motion.loop`), reserved for the most urgent item.

## Input Model (Mouse-First)

| Input | Result |
| --- | --- |
| Hover card | Hover state after 0 ms; tooltip after 500 ms |
| Left press on card | Grab immediately (no drag threshold for lift visuals); becomes a drag after 4 px of movement |
| Left press + release under 4 px | Click: select card, or run its primary action (for example, opening a pack or searching), as gameplay defines |
| Drag | Card(s) follow the pointer at the grab offset |
| Release over valid target | Snap and merge into the stack |
| Release over empty board | Drop in place, then push apart any overlap (see below) |
| Release outside tabletop | Return to the last valid position |
| Right click / long press | Inspect view |
| Mouse wheel | Zoom (0.80x to 1.25x) |
| Middle drag or Space + drag | Pan the board |
| Esc | Cancel drag (return card); otherwise open pause menu |
| Space | Pause/resume time, if the game has real-time progression |

Controller and keyboard navigation: a focus cursor jumps between cards by direction (nearest card within a 60 degree cone). A button grabs; the stick moves the held card at 900 px/s, with target snapping when within 64 px. The focus cursor uses the same cyan corner brackets as the selected state.

## Core Choreography

### Grab / Lift

1. Frame 0: z-order moves to `CardTransient`.
2. Over `motion.fast` (140 ms, ease-out): scale 1.0 to `scale.lift`, y offset to `offset.lift`, shadow `shadow.card` to `shadow.lift`.
3. If a sub-stack is grabbed, cards beneath follow with a 20 ms delay each (up to 4), giving a slight trailing feel.

### Drag

- Position tracks the pointer 1:1.
- Tilt: `rotation = clamp(velocity.x * 0.004, -4 deg, 4 deg)`, damped toward 0 with a 90 ms time constant.
- Valid target under the card: the target shows the cyan outline and brackets within `motion.instant`.
- Invalid target: the held card's outline turns `state.damage` and a cross glyph appears in its corner slot.

### Drop / Snap

1. Over `motion.standard` (220 ms, cubic-out): move to the snap position, scale back to 1.0, rotation to 0.
2. Last 60 ms: a 2 px downward settle and a quick shadow tighten (the landing).
3. Play a dust-puff particle at the card's lower edge (6 to 10 particles, `material.dust`, 300 ms lifetime).
4. Audio hook: a metal "clack", pitch varied by ±6% per drop.

### Return (Cancelled or Invalid Drop)

Move back to the origin over `motion.standard` with a slight arc (12 px upward midpoint). No dust puff. Invalid drops add one 3 px horizontal shake (2 cycles over `motion.impact`) before returning.

### Overlap Push-Apart

When a dropped card overlaps a non-accepting card by more than 30% of its area, nudge the dropped card toward the nearest free space over `motion.standard`. The push distance is the overlap plus 12 px. Never move the card that was already there.

### Stack Merge

When a stack accepts a card, the cascade re-lays out: each card moves to its new slot over `motion.standard`, staggered 15 ms per card. The count plate pops (`scale.impact`) when the number changes.

### Search / Reveal (Searchable Buildings, Card Backs)

1. The progress bar fills linearly in the info panel over the gameplay-defined duration.
2. At completion: a 120 ms amber flash on the hatch icon.
3. The new card appears from behind the source card: it slides 40 px up and out, flips (scale.x 1 to 0 to 1 over 260 ms, swapping from back to face at the midpoint), then drops to an adjacent free position using Drop / Snap.
4. Several spawned cards stagger by 120 ms each.

### Work in Progress (Timers)

- Progress bar: linear fill, no easing, updated every frame.
- On completion: the bar flashes `accent.amber` for 120 ms, then clears.
- A card with an active timer does not pulse. The bar is enough.

### Combat Impact (Hit)

1. Attacker: moves 18 px toward the target over 90 ms (ease-in), returns over 160 ms (ease-out).
2. Target at contact: `scale.impact` pop, 4 px shake (3 cycles, `motion.impact`), and a 60 ms white-to-`state.damage` flash overlay at 40% opacity.
3. Spark particles at the contact edge (8 to 14, amber to red, 250 ms).
4. A damage number floats up 24 px and fades over 600 ms (`type.display.md`, `state.threat.text`, 1 px dark outline).
5. Heavy hits (gameplay-defined) add a camera shake: 3 px, 180 ms, decaying. Disabled under reduced motion.

### Destruction

1. 0 to 120 ms: the card flashes to `state.damage` at 50%.
2. 120 to 380 ms: burn-dissolve shader moves from the damaged edge across the card; soot-colored embers rise.
3. Leftovers (drops, salvage) spawn with Search / Reveal step 3.
4. The card leaves a faint scorch decal on the field for 4 s, then fades out.

### Event Arrival

1. Background: a 200 ms, 8% amber tint wash over the tabletop only (not the HUD).
2. The event card enters from the nearest horizontal edge of the tabletop, face down, sliding over `motion.slow` (380 ms, cubic-in-out) to a free spot near the center.
3. It flips to reveal (260 ms). The dispatch band stamps in with `scale.impact`.
4. If the landing spot is off-camera, the camera nudges toward it by at most 240 px over `motion.slow`.
5. Audio hook: radio static burst, then a stamp.

### Equipment Attach

1. The equipment card snaps behind the host, offset per [03 Tabletop Layout](03-tabletop-layout.md), over `motion.standard`.
2. A cyan connector tick draws between the two over 120 ms.
3. The host's relevant badge pops (`scale.impact`) if its value changed.

### Enemy Spawn

Enemy cards rise from under a dust burst at the tabletop edge: scale 0.85 to 1.0 plus opacity 0 to 1 over `motion.slow`. The red tab drops in 4 px after 80 ms. Do not flip; enemies arrive face up.

## UI Motion

| Element | Enter | Exit |
| --- | --- | --- |
| Tooltip | Fade + 4 px rise, `motion.fast` | Fade, `motion.instant` |
| Modal | Scrim fade `motion.standard`; panel scale 0.96 to 1.0 plus fade, `motion.slow` | Reverse, `motion.standard` |
| Pause menu | Scrim fade `motion.standard`; panel slides from left 24 px | Reverse |
| Screen change | 220 ms fade through `surface.inset` | --- |
| Button press | Content moves down 1 px, fill to `.deep` token, `motion.instant` | Reverse |
| HUD value change | Number rolls to the new value over 300 ms; gains are cyan, losses red, 400 ms flash | --- |

## Reduced Motion

A settings toggle, which can also follow the OS preference where Godot exposes it.

- Replace scale, tilt, shake, arcs, and camera moves with 80 ms opacity and outline changes.
- Drag still follows the pointer and lift still happens, but through the shadow only (no scale).
- Keep the flip, reduced to a 120 ms crossfade.
- Particles drop to 30% count and never flash.
- `motion.loop` pulses become a static accent outline.

## Timing Budget

No interaction should block input for more than 400 ms. The player can grab another card while any animation above is playing. If the player grabs a card mid-animation, the animation completes instantly to its final state before the lift begins.
