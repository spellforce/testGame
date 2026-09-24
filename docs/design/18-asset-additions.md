# Asset Additions

[08 Asset Production](08-asset-production.md) lists the original batch. This document adds what the interface design now requires, in the same format, and marks the four items in the original list that change.

## Additions

| Asset | Size (px) | Frames | Notes |
| --- | --- | --- | --- |
| Card frame, Region | 48 x 56 | 1 | `sand.1` body, contour cue |
| Card frame, Cargo | 48 x 56 | 1 | `steel.4` body, stencil band cue |
| Card back, Region | 48 x 56 | 1 | Contour emblem |
| Card back, Cargo | 48 x 56 | 1 | Crate emblem |
| Card back, pack variant | 48 x 56 | 1 | Any family's back + the 6 px `bone` wrapper band. Draw 3: Region (region deck pack), Cargo (supply pack), Part (gear pack) |
| Protagonist card frame | 48 x 56 | 1 | `bone` body, `cyan.3` inner outline, dog-tag corner notch |
| Protagonist card back | — | — | Not needed. The protagonist card is never face down |
| Protagonist chip plate | 9 x 5 | 1 | `bone`, `ink` outline, for the vehicle face's top-right |
| Avatar frame | 32 x 32 | 1 | Square, drawn over the uploaded portrait + the 4 mask corners (see below) |
| Kind tag plate | 9 x 5 | 1 | `steel.2`, `ink` outline |
| Kind tag glyphs | 5 x 5 | 1 each | 6: large shell, small shell, star, piston, chip, grid |
| Supply band | 46 x 2 | 1 | `amber.2` with a 1 px `steel.2` underline, for Cargo faces |
| Evac beacon | 7 x 9 | 2 | `cyan.3` triangle, 2-frame pulse at 1 fps. Only the 2-frame pulse is on the card; the static one is frame 1 |
| Clip marker (pinned) | 3 x 3 | 1 | `steel.5`, drawn twice per pinned card's header |
| Indent mark | 6 x 1 | 1 | `steel.5`, for vehicle-stack equipment strips |
| Cargo band track | 46 x 2 | — | 9-slice horizontally: `steel.1` frame, `amber.2` fill |
| Overload chevron | 5 x 5 | 1 | `hazard` |
| Damage corner mark | 3 x 3 | 1 | `hazard`, drawn at a mount slot's top-right |
| Destroyed X | 5 x 5 | 1 | `rust.3` |
| Lock icon | 5 x 5 | 1 | `steel.4`, for locked slots and chips |
| Chip plate | 40 x 48 | — | 9-slice: `steel.2` body, `ink` outline, `bone` 1 px inner outline, no shadow |
| Chip caption strip | 40 x 12 | — | `steel.2` lower half, for the chip name on hover |
| Chip drop-target outline | 42 x 50 | 1 | `cyan.3` and `rust.3` versions |
| Skill cooldown arc | 44 x 52 | — | 9-slice; `cyan.2`, drawn clockwise. 4 corner pieces + 4 edge pieces |
| Slot cell (mount grid) | 20 x 16 | 3 | Filled, empty (dashed `steel.4`), locked |
| Panel: window 9-slice | 24 x 24 source, 4 px corners | — | Same steel panel; drawn larger than the existing 16 x 16 UI panel |
| Icon: magnifier | 9 x 9 | 1 | Search |
| Icon: `!`, `✓`, `i` | 16 x 16 | 1 each | Toasts and alerts |
| Icon: `?` | 12 x 12 | 1 | Window help |
| Icon: `_`, `X` | 12 x 12 | 1 each | Window collapse, close |
| Icon: `≡`, `▤` | 9 x 9 | 1 each | Card view / list view toggle |
| Icon: sort arrows | 5 x 5 | 2 | `^` and `v` |
| List row alternates | — | — | Not an asset. Two `steel.1` / `steel.2` ColorRects |
| Badge plate: hazard | 13 x 9 | 1 | Added to the existing bone / rust / steel set |
| Progress bar (craft queue) | 2 px height | — | 9-slice; `cyan.2` fill |
| Capacity bar | 5 px height | — | 9-slice; `steel.1` track, `cyan.2` / `hazard` / `rust.3` fills |
| Facility chip icons | 24 x 24 | 1 each | 6: warehouse, hunter camp, taxi, shop, factory, skills |
| Skill category glyphs | 7 x 7 | 2 | Combat (crossed barrel), non-combat (wrench) |
| Tooltip panel | 16 x 16 source | — | 9-slice, small variant of the steel panel |
| Dialog head icons | 16 x 16 | 3 | `!` hazard, `X` rust, `?` cyan |
| Region tint overlay | board-sized | 1 | Flat white; tinted by code. Reuses the board sprite |
| Dust overlay | 16 x 16 | 4 | Tiling, loops at 4 fps, for the low-time warning |

### Wrapper band on a pack back

Draw once as a 48 x 8 overlay and composite it on any back at y 24:

```text
y24  +------------------------------------------+
     | bone band, 1 px ink top and bottom edge   |  6 px tall
y30  +------------------------------------------+
```

## Answers to Open Questions

### The opened-stack sprite question

[04 Card System](04-card-system.md) leaves this open: Stacklands shrinks a working card to 0.8 and shifts a growing stack, which conflicts with "never scale pixel art".

**Decision: never scale. Use the timer bar and a work icon instead.** The 0.8 shrink is the one Stacklands behavior this design drops outright, because the alternative is soft pixels on the busiest object on the board, and the whole art direction rests on 1 art px = a whole number of screen px.

What replaces it:

| Stacklands cue | Our replacement |
| --- | --- |
| A working card shrinks to 0.8 | The card keeps its size and shows the timer bar above the root card (existing) |
| "This card is in use" | A 5 x 5 `steel.7` **work icon** (a gear) drawn in the root card's header, right end, at 60% opacity |
| A stack shifts as it grows | The existing 12 px offset, unchanged. The stack grows downward, not sideways |
| An equipped card shrinks | The 6 x 1 indent mark on the visible strip, plus the mount grid in the detail sheet |

This needs one new asset (the work icon, 5 x 5, `steel.7`) and removes the flip-frame scaling problem, so the 4 flip frames per family can be drawn straight.

### The avatar upload pipeline

[04 of the gameplay brief] requires the protagonist's portrait to support custom upload. Pixel art cannot be scaled with filtering, so the pipeline is explicit:

1. The player picks a file: PNG or JPG, any size, from a native file dialog. Godot's `FileDialog` with `access = ACCESS_FILESYSTEM`.
2. The image is loaded, then **cropped to a centered square** on the shorter axis, then resampled with **nearest-neighbor** (`Image.resize(w, h, Image.INTERPOLATE_NEAREST)`) to 32 x 32.
3. No color quantization is applied. The palette rule in [01 Visual Direction](01-visual-direction.md) governs authored art; a player's portrait is data, not art, and quantizing it to 32 colors would make every upload look like mud. This is a deliberate exception and it is stated in the `?` help.
4. The result is stored as `<save_dir>/avatars/<save_id>.png`, outside the res:// pack.
5. It is drawn inside the 32 x 32 avatar frame, which masks the corners to 1 px rounding so the portrait sits in the card's language. The frame is drawn **over** the portrait.
6. A **contrast check**: if the portrait's mean luminance is within 15% of bone (`#D7D5BF`), the frame switches to a `steel.2` inner plate behind the portrait, so a white-on-white upload does not vanish. Computed once at import and stored beside the file.
7. Fallback: a default 32 x 32 silhouette, `ink` on `bone`, which is also the asset the protagonist chip uses when the portrait is unset.

The same 32 x 32 result feeds three places: the protagonist card's art area, the protagonist chip on the vehicle face, and the `[P]` mount slot in the detail sheet. One import, three consumers.

## Changes to Existing Assets

| Asset | Change |
| --- | --- |
| Card frames | Now **8**, not 6. Region and Cargo added; "Searchable building" renamed **Site** (art unchanged, name only) |
| Card backs | Now **8**, plus 3 pack variants |
| Badge plates | Add the `hazard` plate |
| Digit fonts | The `3 x 5` small font uses 1 px strokes at 1 px height, which is at the limit. Draw the `1` and `7` with the same width as the other digits so right-aligned columns do not jitter |
| Board rail | Unchanged, but the band revision changes the **band divider** position. The divider asset (2 px, `ink` + `steel.7`) is drawn at y 164, not y 104 |
| Slot plates | Unchanged (60 x 81). New: **skill tray cells**, 40 x 48, same style, no price row |
| World props | Add 3 to 4 props that read as **ruined vehicles**, because the Enemy family's icons are raider trucks and the board's world should hint at where they come from. Reuse the enemy icon's silhouette language |
| Cursors | Add: over a chip (open hand, same as over a card), over a list row (open hand), over a resize corner (`↘`) |

## Batch Order

Insert into the batch order in [08 Asset Production](08-asset-production.md). The revised order:

1. Palette, board tile, seam, rail, band divider at the new y.
2. **8** card frames and **8** card backs.
3. 1 icon per family (**8** icons): the style reference.
4. Badge plates, digit fonts, state outlines, timer bar, work icon.
5. **The vehicle face test**: one vehicle card, one equipment card, one protagonist chip, the cargo band and the SP plate, placed in a 9-card stack at zoom 0.5 / 1.0 / 2.0 on 1080p. This is the single highest-risk asset in the package and it is cheap to test early. Do not proceed until it reads.
6. World terrain, 10 props.
7. **UI panel kit including the window 9-slice**, buttons, list rows, the slot cell, the chip plate, UI icons, cursors.
8. **The window test**: build one warehouse window with 12 rows and check it at UI scale 1, 2 and 4 (1280x720, 1080p, 4K) before drawing the other five windows.
9. Dialogs, toasts, the capacity bar, the craft queue bar.
10. Effects: dust, sparks, flip, dissolve, the skill cooldown arc.
11. Ambient animals, logo, avatar frame.

Two checkpoints added to the existing one, both before their expensive dependents. Steps 5 and 8 are the two places where a wrong call costs a whole batch of redrawn art or a rebuilt window kit.
