# Card Families

> **Partially superseded.** This document defines the look of the 6 original families. [11 Card Taxonomy](11-card-taxonomy.md) now covers **8** families, maps every gameplay card type onto one, and renames family 3 from "Searchable Building" to **Site**. Read that document for the full set and for the sub-cues inside a family. Everything below about a family's body color, title color and shape cue still stands.

All families use the anatomy in [04 Card System](04-card-system.md). They differ in body color, one **shape cue** (so they can be told apart without color), icon style and card back.

Stacklands tells card types apart mostly by body color (cream for villagers, dark gray for resources). We do the same and add a shape cue.

## Summary

| Family | Body ramp (base / shade / highlight) | Title color | Shape cue |
| --- | --- | --- | --- |
| Vehicle | `cyan.1` / `cyan.0` / `cyan.2` | `white` | Wheel notches: 2 notches (3 x 2 px) cut into the bottom outline |
| Enemy | `rust.2` / `rust.1` / `rust.3` | `white` | Jagged header: 3 small teeth along the header's bottom edge |
| Searchable building | `olive.1` / `olive.0` / `olive.2` | `white` | Keyhole: 3 x 5 px keyhole mark in the footer center |
| Event | `amber.2` / `amber.1` / `amber.3` | `ink` | Torn top: header's top edge is a zigzag (1 px steps) |
| Fixed building | `steel.2` / `steel.1` / `steel.3` | `white` | Bolts: 4 rivets (2 x 2 px `steel.7`) at the art area's corners |
| Equipment | `bone` / `steel.7` / `white` | `ink` | Hazard stripe: 4 px diagonal `hazard` / `ink` stripe along the left edge of the art area |

## Vehicle

- **Role**: the player's convoy units.
- **Icon**: side or 3/4 view, wheels or tracks clearly separate from the body. Headlamp glint in `amber.3` (1 to 2 px).
- **Footer**: wheel notches in the bottom outline, between the two badges.
- **Card back**: `cyan.1` with a wheel-in-shield emblem.

## Enemy

- **Role**: raiders, drones, mutated machines.
- **Icon**: faces the viewer or charges, uneven silhouette, a single `amber.3` or `white` eye/light pixel cluster.
- **Header**: 3 teeth pointing down from the header into the separator row.
- **Card back**: `rust.2` with a crossed-reticle emblem.
- **Extra**: the right badge (health in most designs) uses the `rust.2` plate by default. On an enemy it switches to a `steel.2` plate so it stays visible against the red body.

## Searchable Building

- **Role**: ruins, wrecks, depots and bunkers to explore.
- **Icon**: front view with a clear entrance (door, hatch, breach).
- **Footer**: keyhole cue in the center.
- **States**: a sealed one shows the keyhole in `ink`; searching shows the timer bar; emptied swaps to the steel ramp (the disabled state).
- **Card back**: `olive.1` with a crate emblem.

## Event

- **Role**: storms, radio signals, ambushes, supply drops. Usually temporary.
- **Icon**: symbol-like, strong silhouette (radio mast, storm cloud, flare, parachute crate).
- **Header**: torn zigzag top edge.
- **Timer**: an event with a countdown shows the timer bar.
- **Arrival**: lands face down and flips (see [06 Interaction and Motion](06-interaction-motion.md)).
- **Card back**: `amber.2` with `ink` diagonal hazard stripes and a radio-mast emblem.

## Fixed Building

- **Role**: garage, turret, workshop, fuel depot.
- **Icon**: front view, wide at the bottom (grounded).
- **Cue**: 4 rivets in the art area corners.
- **Behavior**: draggable only if gameplay allows. A locked building that is grabbed shakes 1 px left-right twice and shows a 5 x 5 lock icon.
- **Card back**: rarely shown; `steel.2` with a blueprint grid.

## Equipment

- **Role**: guns, armor plates, engines, scanners.
- **Icon**: single object, side view.
- **Cue**: diagonal hazard stripe on the left edge of the art area.
- **Attached**: equipment joins a vehicle stack like any card (it stacks under the vehicle, 12 px offset). Gameplay decides where in the stack it may go.
- **Card back**: `bone` with a toolbox emblem.

## Grades (Optional)

If gameplay needs rarity or grades, show them with the outline only. Never change size or body color.

| Grade | Outline |
| --- | --- |
| Standard | `ink` |
| Rare | `ink` with a second 1 px `amber.3` line inside it, top edge only |
| Prototype | `ink` with the inner line in `cyan.3`, full border |

## Checking Families Without Color

Convert a screenshot of all six families to grayscale. Each must still be recognizable from its shape cue alone at 1 screen px per art px.
