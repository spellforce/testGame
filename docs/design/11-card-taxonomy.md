# Card Taxonomy

[05 Card Families](05-card-families.md) defines what a family **looks like**. This document defines **which family each gameplay card type uses**, and where the gaps were.

The gameplay brief lists 11 card types. The art spec had 6 families. They did not map onto each other: Region, Evacuation point, Item and Blueprint had no family to live in. This document closes that gap.

## Families: 6 to 8

The palette is closed at 32 colors (see [02 Design Tokens](02-design-tokens.md)), so a new family must reuse an existing ramp. Two new families, both reusing ramps that were previously world-only or generic UI:

| # | Family | Body base / shade / highlight | Title | Shape cue | Ramp was |
| --- | --- | --- | --- | --- | --- |
| 1 | Vehicle | `cyan.1` / `cyan.0` / `cyan.2` | `white` | Wheel notches: 2 x (3 x 2 px) cut into the bottom outline | unchanged |
| 2 | Enemy | `rust.2` / `rust.1` / `rust.3` | `white` | Jagged header: 3 teeth along the header's bottom edge | unchanged |
| 3 | Site | `olive.1` / `olive.0` / `olive.2` | `white` | Keyhole: 3 x 5 px mark in the footer center | unchanged |
| 4 | Event | `amber.2` / `amber.1` / `amber.3` | `ink` | Torn top: header's top edge is a 1 px zigzag | unchanged |
| 5 | Facility | `steel.2` / `steel.1` / `steel.3` | `white` | Bolts: 4 x (2 x 2 px `steel.7`) rivets in the art area corners | unchanged |
| 6 | Part | `bone` / `steel.7` / `white` | `ink` | Hazard stripe: 4 px diagonal `hazard`/`ink` stripe down the art area's left edge | unchanged |
| 7 | **Region** | `sand.1` / `sand.0` / `sand.2` | `white` | Contours: 3 x (1 px `sand.3`) contour arcs in the art area's top-left corner | was world terrain |
| 8 | **Cargo** | `steel.4` / `steel.3` / `steel.5` | `white` | Stencil band: 3 px `steel.7` band with 3 small gaps, along the art area's bottom edge | was generic UI steel |

Family 3 is renamed **Site** (was "Searchable Building") because it now covers outdoor Location cards and not only buildings.

### Why Cargo gets its own family

Ordinary items are the **most common card on the board** in a looting game, and they are the pile you scan to decide what to keep. Sharing the bone body with vehicle parts would mean every gun and every engine looks like every scrap of metal at a glance, which is the wrong trade. `steel.4` on the `steel.6` board is a 1.4 contrast ratio, low, but [02 Design Tokens](02-design-tokens.md) already establishes that cards read against the board through their `ink` outline and drop shadow, not through body value.

Facility (`steel.2`) and Cargo (`steel.4`) are 2 steps apart on the ramp, which is a clear value difference in grayscale. The two cues (corner rivets vs. bottom stencil band) are in different parts of the card. Add both to the grayscale check in [19 UI Validation](19-ui-validation.md).

### Risk

Region uses the world-terrain ramp. [01 Visual Direction](01-visual-direction.md) requires the board to read as the focus against a warmer, darker world. A `sand` card sitting on the `steel.6` board is in the same family as the terrain behind it. Mitigations: the `ink` outline, the drop shadow, and `sand.1` being 2 steps darker than terrain base `sand.2`. Review this on screen at zoom 0.5 before drawing the Region batch.

## Gameplay Type to Family

Every type from the gameplay brief, plus the three it left open.

| Gameplay type | Family | Sub-cue | Card back | Notes |
| --- | --- | --- | --- | --- |
| Region | Region | — | Region, contour emblem | Not player-draggable; the mission header (see [16 Garage and Expedition Boards](16-garage-and-expedition-boards.md)) |
| Location | Site | — | Site, crate emblem | Searches produce results; exhausted swaps to the steel ramp |
| Deck pack / region deck pack | *any* (pack presentation) | Wrapper band | Contents' family color | Not a family; see **Packs** below |
| Evacuation point | Region | Beacon | Region, contour emblem | `cyan.3` beacon triangle in the art area's top-right |
| Monster / encounter | Enemy | — | Enemy, reticle emblem | Never enters the cargo hold |
| Vehicle | Vehicle | Protagonist chip | Vehicle, wheel-in-shield | See [12 Vehicle Card Face](12-vehicle-card-face.md) |
| Item (materials, valuables, supplies) | Cargo | Supply band | Cargo, crate emblem | Supply sub-type; see below |
| Weapon (main cannon / secondary / S-E) | Part | Kind tag: shell glyph | Part, toolbox emblem | |
| Engine | Part | Kind tag: piston glyph | Part, toolbox emblem | |
| C unit | Part | Kind tag: chip glyph | Part, toolbox emblem | |
| Blueprint | Part | Kind tag: grid glyph, `cyan.3` | Part, toolbox emblem | Crafting input only; distinct from an item |
| Quest | Event | Corner cut + `rust.1` stamp | Event | Open — see decision D2 |
| Event | Event | — | Event, radio-mast emblem | |
| Facility (Hunter Camp, Warehouse, Taxi, Vehicle Shop, Vehicle Factory) | Facility | — | Facility, blueprint grid | On the table these are **chips**, not cards (see [15 Context Menus and Chips](15-context-menus-and-chips.md)) |
| Protagonist | unique | Dog-tag corner notch | n/a | `bone` body, full `cyan.3` inner outline |

Monster and Enemy are the same thing; the brief's "monster/encounter" is the Enemy family.

## Sub-Cues

A sub-cue distinguishes cards **inside** one family. It must not change the family cue.

### Part — kind tag

A 9 x 5 px `steel.2` plate at the art area's bottom-left, holding a 5 x 5 `steel.7` glyph:

| Kind | Glyph |
| --- | --- |
| Main cannon | Large shell |
| Secondary cannon | Small shell |
| S-E | 4-point star |
| Engine | Piston |
| C unit | Chip (square with 2 pins per side) |
| Blueprint | Grid (3 x 3 lattice), drawn in `cyan.3` |

### Cargo — supply band

A 2 px `amber.2` band immediately under the header separator marks a **supply** (armor pack, repair pack, anything directly consumable). Bulk material and valuables have no band. This is the only amber on a Cargo card, so it reads instantly in a stack.

### Region — beacon

The evacuation point carries a 7 x 9 px `cyan.3` triangle beacon in the art area's top-right. It is the only Region card with cyan on the face, so an evac point is findable at zoom 0.5 without reading the title.

## Badge Defaults

[04 Card System](04-card-system.md) leaves the 2 badge anchors to gameplay. These are the defaults; a specific card may override one with an icon badge.

| Family | Left badge (`bone` plate, `ink` digits) | Right badge (`rust.2` plate, `white` digits) |
| --- | --- | --- |
| Vehicle | SP as a percentage (0-99) | Cards in the cargo hold |
| Enemy | Level | HP |
| Site | Searches remaining | — |
| Region | Time limit in cycles | — |
| Event | — | Countdown in seconds |
| Part | Weight | — |
| Cargo | Stack count | Unit value |
| Facility | — | Cost, or `!` when a service is unlocked and unused |

Reminder from [04 Card System](04-card-system.md): 2 digits maximum, 100 or more shows as "99", and the enemy's right badge switches to a `steel.2` plate so it stays visible on the `rust.2` body.

## Packs

A pack is a presentation, not a family. Any card can be a pack:

- **Frame**: the card back of its **contents'** family color, so a gear pack reads as parts and a region deck pack reads as Region.
- **Wrapper**: a 6 px `bone` horizontal band across the middle of the back, with a 1 px `ink` top and bottom edge. This is the "pack" cue and it is present on every pack.
- **Count plate**: a `steel.2` plate at the right end of the header showing cards remaining, in `font.digits`. Turns `rust.4` when 3 or fewer are left.
- **Opening**: existing behavior in [06 Interaction and Motion](06-interaction-motion.md) — cards are thrown out one at a time around a circle, ~0.1 s apart, aimed at an identical free card within 228 px.
- The pack card itself is removed when empty.

## The Protagonist Card

A single card, so it does not need a family:

- Body `bone`, header title in `ink` — same as Part, deliberate: the protagonist reads as "yours".
- A full 1 px `cyan.3` inner outline, 1 px inside the `ink` outline. No other card has that.
- A **dog-tag notch**: the top-left corner pixel block (3 x 3 px) is cut out and replaced with the board color, so the silhouette alone identifies it.
- The art area holds the avatar (custom upload, see [12 Vehicle Card Face](12-vehicle-card-face.md) for the avatar pipeline).
- Left badge: driving level. No right badge.

## Card States for All Families

The 9 states in [04 Card System](04-card-system.md) apply unchanged. Two additions:

| State | Treatment |
| --- | --- |
| Pinned | 2 x (3 x 3 px) `steel.5` clips in the header's top-left and top-right, drawn inside the frame. Only the region card and layout-locked cards use it. A grabbed pinned card shakes 1 px left-right twice. |
| Overloaded (vehicle) | Body outline unchanged, plus a 5 x 5 `hazard` chevron in the footer center and the right badge in `rust.4`. |

## Open Decisions

| ID | Question | Current position |
| --- | --- | --- |
| D2 | Is a Quest card an Event card (amber, corner cut), or a Facility card issued by the Hunter Camp? | Event family; quests are objectives with timers |
| D3 | Does the Region card stay on the table for the whole expedition, and is it pinned? | Yes, pinned to the play area's top-left at board cell (1,1) |
| D4 | Can a Region card ever be more than a mission header — e.g. can the player hold an unused one in the warehouse? | No in this version |
