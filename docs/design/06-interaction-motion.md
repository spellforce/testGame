# Interaction and Motion

The behavior below is taken from the shipped Stacklands code (Steam build, `GameScripts.dll`: `Draggable`, `GameCard`, `WorldManager`, `Boosterpack`, `GameCamera`). Values are the game's own numbers, converted to art pixels.

**Unit conversion.** Stacklands works in 3D world units. Its card collider is 0.42 x 0.495 units, so **1 unit = 114 art px** (48 / 0.42). All values below are given in engine units and in art px.

**3D to 2D.** Stacklands' board is horizontal, so "position" is (x, z) and "height" is y. In our 2D port, (x, z) becomes (x, y) on screen and height becomes a visual upward offset. Colliders become plain rectangles.

**3D presentation option.** The current port maps the logical board directly to 2D coordinates. If the 3D camera prototype is approved, keep all interaction rules and saved positions in the same logical art-pixel coordinate system, then map `(x, board_y)` to `(x, board_height, z)` on a horizontal board plane. Screen input must be converted by a ray/plane intersection using the active `Camera3D`; never divide by a guessed zoom value. Cards face the camera as billboards, while depth sorting uses logical board depth plus an explicit tie-breaker for stacks and held cards. See [game-update.md](game-update.md) for the migration gate.


| Constant | Engine value | Our value | Notes |
| --- | --- | --- | --- |
| Gravity | 30 units/s² | 3429 px/s² | Only while a card is airborne |
| Air drag (horizontal) | x0.93 per 0.02 s physics step | same | About 2.6% of speed left after 1 s |
| Bounciness | 0.6 | same | Applied to vertical speed on landing |
| Bounce horizontal loss | x0.8 | same | On each landing |
| Position lerp (settle) | 20 per second | same | `pos = lerp(pos, target, dt * 20)` |
| Stack child lerp | 20 per second | same | Same rate as settling |
| Drag threshold | 0.2 s AND 0.1 units (11 px) | same | Below both, a press is a click |
| Card-target snap | 0.2 units (23 px) | same | Packs and sell box |
| Auto-stack radius | 2.0 units (228 px) | same | Only for cards sent out by machines |
| Push rate | 2.0 units/s (228 px/s) | same | See Push-Apart |
| Clamp margin | 0.1 units (11 px) | same | From the board edge |
| Hover lift | +0.06 units (7 px) | same | |
| Drag lift | +0.1 units (11 px) | same | Also used for spirits/advisors (0.25 units) |
| Flip time | 0.1 s | same | Face-down to face-up |
| Face rotation lerp | 14 per second | not needed | 90° face up, 270° face down |
| Grid cell | 0.75 x 0.85 units | 86 x 96 px | For the align action |
| Stack overlay | 0.1 units | 12 px | `CardOverlayOffset` |
| Collapsed stack overlay | 0.02 units | 2 px | `CollapsedCardOverlayOffset` |
| Camera zoom range | 3 to 11 units | our zoom 0.5 to 2.0 | See [03 Canvas and Layout](03-canvas-layout.md) |
| Zoom per wheel notch | 0.2 units | x1.15 | |
| Combat spacing | 0.2 units (23 px) | same | `HorizonalCombatOffset` |
| Combat row offset | 1.0 unit (114 px) | same | `CombatOffset` |

## Mass

Pushing uses mass, so heavy things shove light things.

| Card | Engine mass | Our mass |
| --- | --- | --- |
| Any card | 1 | 1 |
| Plus, if it is a creature/unit ("Mob") | +50 | +50 |
| Plus, if it is a building-type structure | +8 | +8 |
| Plus, if it is a heavy foundation | +1000 | +1000 |
| Plus the mass of every card stacked on it | recursive | recursive |

A stack's mass is therefore the sum of its cards, and a stack of 3 buildings (3 x 9 = 27) barely moves when a lone card (mass 1) pushes it.

The push share is:

```
pushShare = 1 - myMass / (myMass + otherMass)
```

So a mass-1 card pushing a mass-27 stack moves itself 0.96 of the displacement and the stack 0.04. A 50-mass unit pushing a 1-mass card moves the card almost the whole way, which is why in Stacklands a chicken can shove your mine.

## Push-Apart

This is the "cards bumping into each other" rule. It runs every frame, after movement.

Rules, in order:

1. A card being dragged never pushes and is never pushed.
2. Only a **root card** (a card that is not stacked on another) pushes. Cards inside a stack do nothing.
3. A card that is busy working or equipped does not push and is not pushed.
4. Packs push cards but are never pushed by cards.
5. Cards that are children or parents of each other (in the same stack) ignore each other.
6. A card that is airborne toward a target (`BounceTarget`) does not push.
7. Heavy foundations only push and get pushed by other heavy foundations.

When a root card overlaps something it can push:

```
direction  = normalize(other.position - myPosition)     # in the ground plane
share      = 1 - myMass / (myMass + otherMass)
myTarget  -= direction * share * 2.0 * delta            # 228 px/s, scaled by share
```

Key details that differ from a naive implementation:

- The push moves the **target position**, not the current position. The card then lerps toward it at the normal 20/s rate, so the slide is smooth and can be interrupted.
- Only **one** push per card per frame: the loop breaks after the first valid overlapping object (the order comes from the physics overlap query, so it is not sorted by distance).
- Because a root card pushes with its **root** collider but overlaps the other stack's **leaf** collider, a stack pushes from the position of its first card and detects contact at its last card. Port this as: push uses the root card's position, overlap test uses the root and leaf rectangles together.
- The result is a slow, continuous separation, not a bounce. Cards never gain speed from being pushed.

## Pick Up

1. `StartDragging`: clear any `Velocity`, clear any pending bounce target, set `BeingDragged`.
2. The grabbed card is removed from its parent stack (`SetParent(null)`), and every card above it in the stack is also marked as dragged, so the whole sub-stack moves with the cursor.
3. Cards below keep their position; the stack above collapses down to close the gap.
4. Sound: pickup group sound, pitch between 1.0 and 1.2.
5. While held, the card's target is `mouseWorldPosition - grabOffset`, so it keeps the grab point offset. It lifts 11 px and its shadow spreads.
6. Held cards clamp to the **loose** board bounds (`WorldBounds`); resting cards clamp to the **tight** bounds (`TightWorldBounds`), which is 0.1 units inside. This is why you can drag a card partly over the board edge but it settles inside.

## Drop

Dropping is simpler than most people assume, and it is **not** a snap-back:

1. On release, the game looks for a valid stack target under the card.
2. If found, the card joins that stack.
3. If not found, **the card simply stays where it was released** and settles by lerping to its position (20/s). It is never sent back to where it was picked up. The only correction is the clamp: if the release point is outside the tight board bounds, the card slides to the nearest legal spot.
4. Sound: drop-on-stack sound at pitch 0.8 to 1.2 if it landed on a stack, otherwise the card's pickup sound group at 0.8 to 1.0.

Also, right-click drops immediately (`DropCard` on right-mouse press).

## Drop Target Search

The target is found by **collider overlap**, not by a fixed snap radius:

1. Take every card overlapping the dropped card's rectangle.
2. Skip itself, its own stack children, cards in combat, and cards that already have something stacked on them.
3. Keep only targets where the game's `CanHaveCardOnTop` check passes (that is a gameplay rule, not a presentation one).
4. If a target in the stack has a timing status effect, skip it unless that card allows stacking while it runs.
5. Among the candidates, pick the one whose **center is nearest** to the dropped card's center.
6. Stack onto that card's **leaf** (last) card.

There is no maximum distance in this path: if the rectangles overlap, it stacks. The 23 px snap radius applies only to special drop zones (packs and the sell box), where the held card is pulled onto the zone's center.

This means a card dropped mostly off another card still stacks, as long as the rectangles touch. Port it exactly that way.

## Cards Sent Out by the Game

When a machine, pack or building sends a card out (the `SendIt` path):

```
velocity = randomUnitCircleDirection * 4.5 units/s, plus 5 units/s upward
```

In 2D px: 514 px/s horizontally in a random direction, 571 px/s of hop height.

The card then arcs: horizontal speed decays at 0.93 per 0.02 s, vertical speed falls at 30 units/s². On landing it bounces to 0.6 of its vertical speed, loses 20% of its horizontal speed, wobbles, and if it still has a pending landing target it stacks there; otherwise it slides to a stop (when horizontal speed is under 0.01 units/s and it is near the ground).

A pack specifically launches cards around a circle so they spread evenly: the angle steps 360° / cards-in-pack for each card, at 4.5 u/s sideways and 6 u/s up. Cards appear one at a time, about 0.1 s apart.

If a card is sent out and an identical card is free within 2.0 units (228 px), it is aimed at that card instead and stacks on landing. That is the "sent cards stack themselves" behavior.

## Timers

A working card shows a progress bar; when it finishes, the bar clears and the result card spawns using the SendIt path. There is no easing; the bar tracks the timer directly.

## Card Face and Wobble

- A card spawned face down turns face up after 0.1 s.
- Rotation interpolates at 14/s toward 90° (up) or 270° (down). In 2D, port this as the flip animation frames.
- `RotWobble` is a decaying wobble: `offset = amplitude * sin(t * speed) * t`, where `t` decays with a springiness factor. It is triggered by sending, dragging, and impacts. Cards only wobble; they never tilt or shear.
- Cards also idle-wobble on their own (`AutoRotWobble`) at a low amplitude. Use this sparingly: a slow, small 1 px sway on newly landed cards reads as physical without breaking the pixel grid.

## Depth Sorting

A card's depth is its position: closer to the bottom of the board draws in front. The engine derives the vertical offset as `y = -z * 0.001`. In 2D, set `z_index` from the card's y position. Held cards go to the top layer; a card in a stack always draws in front of the card it sits on.

## Align to Grid

A dedicated action snaps every free root card to a grid of 0.75 x 0.85 units (86 x 96 px) by rounding its position to the nearest cell. Cards in stacks and creatures are skipped, and only cards that are not being dragged and are essentially at rest (velocity under 0.01) are moved. A faint grid overlay flashes on and fades out at 3/s.

This is a convenience feature, and it is the reason Stacklands boards look tidy. Make it a keybind (Stacklands defaults it to an action in the input map) and give it the same fading grid overlay. Note: the grid cell (86 x 96 px) is larger than a card (48 x 56), so aligned cards have visible gaps.

## Camera

- Zoom is a camera height between 3 and 11 units, adjusted 0.2 units per wheel notch, easing toward the target.
- Zooming keeps the pointer roughly fixed by lerping halfway between the pointer and screen center.
- Panning is clamped so the board's bounds stay on screen, plus a 0.1-unit margin.
- Screen shake is a random offset scaled by a `Screenshake` value that decays 1 unit/s, and it is skipped when the accessibility setting is off.

Events that shake the screen in Stacklands include opening a pack (0.3). Use it for impacts and keep it optional.

## Combat Positioning

Fights are not free-form; the game puts both sides on fixed lines:

- All combatants in a fight are placed relative to the conflict's start position.
- Cards on the same team are spread along one axis by 0.2 units (23 px) each, centered on the team (`index - (count - 1) / 2`).
- The opposing team is offset 1.0 unit (114 px) along the other axis.
- The conflict rectangle is clamped inside the tight board bounds, so fights near the edge shift inward rather than spilling out.
- Inside a fight, cards lerp to their slot at 20/s, with a small perlin wobble (`Perlin(conflictTime * 10) * 0.01`) so the line is not perfectly rigid.

## Pause

Pausing sets the time scale to 0. Cards can still be dragged and dropped while paused; only the world simulation stops.

## Reduced Motion (Optional Setting)

Stacklands has a screen-shake toggle, not a full reduced-motion mode. If we add one, keep the physics identical and only change presentation: no camera shake, no idle wobble, no hop arcs (cards slide instead), and no flip animation (instant swap).
