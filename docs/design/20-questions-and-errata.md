# Questions and Errata

Two parts: **errata** — contradictions found between documents that must be fixed in the source, with the resolution — and **questions** — things only the game's designer can answer, ordered by how much design work is blocked without them.

## Errata

### E1 — The card art area is 29 rows, not 37

[04 Card System](04-card-system.md) draws the anatomy with a 37-row art area and prints the total as 64:

```text
outline 1 + header 12 + separator 1 + art 37 + footer 12 + outline 1 = 64
```

The card is 48 x **56**. The correct set that sums to 56 is:

```text
outline 1 + header 12 + separator 1 + art 29 + footer 12 + outline 1 = 56
```

[10 Validation Checklist](10-validation-checklist.md) already carries the correct version (`1 + 12 + 1 + 29 + 12 + 1`), so the checklist is right and the anatomy diagram is wrong.

The consequences in [04 Card System](04-card-system.md) that must also change:

| Location | Says | Should be |
| --- | --- | --- |
| Art area | 46 x 37, y14-50 | 46 x **29**, y14-**42** |
| Footer | y51-62 | y**43**-54 |
| Card mouth note | "up to 32 x 26" | 32 x **20** (see E2) |
| Stack height | `56 + 12 x (n - 1)`, 10-card = 164 | Unchanged — this one is already correct |

**Resolution**: the art area is **29 rows**. This is the binding constraint on every icon in the game and it is the reason [12 Vehicle Card Face](12-vehicle-card-face.md) had to choose between showing SP as a bar and showing the vehicle icon at full width — there is no room for both.

### E2 — Icon size 32 x 26 does not fit a 29-row art area

[08 Asset Production](08-asset-production.md) and [01 Visual Direction](01-visual-direction.md) both specify icons "up to 32 x 26" centered in the art area. A 26-row icon in a 29-row area leaves 1.5 rows of breathing space top and bottom, which is too tight to look intentional and leaves no room for the 1 px bottom shadow the same documents specify.

**Resolution**: the generic icon budget becomes **32 x 20**, centered in the 29-row area, leaving 4 rows top and 5 rows bottom (the extra bottom row absorbs the 1 px shadow). Families that need width more than height — vehicles, wide buildings — may use 32 x 20 fully. Families that need height — a radio mast, a standing figure — may use **20 x 24**, centered horizontally.

Both [01 Visual Direction](01-visual-direction.md) and [08 Asset Production](08-asset-production.md) are updated. The existing icon checklist's "a wide vehicle can be 32 x 18" line is superseded by this.

### E3 — `board.slot_band` 104, and the play area, change to 164 / 480

[03 Canvas and Layout](03-canvas-layout.md) lays out a 104 art px band holding one row of 9 slots of 60 x 81, leaving a 538 art px play area. The expedition needs a second row for the skill tray ([15 Context Menus and Chips](15-context-menus-and-chips.md)), which does not fit in 104.

| Token | Was | Now | Arithmetic |
| --- | --- | --- | --- |
| `board.slot_band` | 104 | **164** | `14 + 81 + 9 + 48 + 12 = 164` |
| Play area | 1104 x 538 | 1104 x **480** | `644 - 164 = 480` |
| Band divider position | y 104 | y **164** | --- |

**Resolution**: adopted, and the reasoning for putting the tray in the band rather than floating it is in [16 Garage and Expedition Boards](16-garage-and-expedition-boards.md). The cost is real: 58 art px is about one card height of working space.

[02 Design Tokens](02-design-tokens.md), [03 Canvas and Layout](03-canvas-layout.md) and [10 Validation Checklist](10-validation-checklist.md) are updated.

### E4 — "Never scale" vs. the 0.8 working-card shrink

[04 Card System](04-card-system.md) lists this as an open decision: Stacklands shrinks a working or equipped card to 0.8, and a growing stack shifts its cards, both of which conflict with [01 Visual Direction](01-visual-direction.md)'s "never scale an individual sprite".

**Resolution**: **never scale.** The 0.8 shrink is dropped and replaced with the timer bar plus a 5 x 5 work icon in the root card's header. Full reasoning and the replacement table are in [18 Asset Additions](18-asset-additions.md). [04 Card System](04-card-system.md)'s open decision is closed.

This also frees the 4 flip frames per family to be drawn without a scale compensation, so the flip asset in [08 Asset Production](08-asset-production.md) is unchanged in size.

### E5 — Grid cell 86 x 96 vs. card 48 x 56

[02 Design Tokens](02-design-tokens.md) and [06 Interaction and Motion](06-interaction-motion.md) use a 86 x 96 align-to-grid cell, which is much larger than a card. This is not an error — it is Stacklands' own grid — but it produces visible gaps and the docs do not say so.

**Resolution**: keep 86 x 96 and document the gap as intended. The grid's job is tidiness, not packing. A card-aligned grid would remove the gaps but also remove the ability to see stack headers without overlap. [06 Interaction and Motion](06-interaction-motion.md) already notes this; no change beyond a cross-reference from [02 Design Tokens](02-design-tokens.md).

### E6 — `stack.offset` 12 vs. "the whole header stays visible"

[02 Design Tokens](02-design-tokens.md) sets `stack.offset` to 12 and [04 Card System](04-card-system.md) says the offset exists "so the 12-row header of every lower card stays visible". A 12 px offset on a 12 px header leaves the header exactly visible with **zero** margin, and the 1 px separator row is then covered.

**Resolution**: `stack.offset` becomes **13** — 12 for the header plus 1 for the separator. The stack height formula becomes `56 + 13 x (n - 1)`:

| Cards | Was | Now |
| --- | --- | --- |
| 9 (full vehicle) | 152 | **160** |
| 10 (count plate) | 164 | **173** |

This is a 1 px change, and it is exactly the kind of 1 px that is invisible in a spec and glaring on screen. Flagged for the first art review rather than changed unilaterally: if the separator reading as covered is acceptable, 12 stands and this erratum is withdrawn. Listed as **Q9** below.

### E7 — The "34 x 28 cell" in the detail sheet's equipment rows

[12 Vehicle Card Face](12-vehicle-card-face.md) specifies an equipment row's icon cell as 34 x 28. The row is 18 UI px tall ([13 Window System](13-window-system.md)), which cannot contain a 28 px cell.

**Resolution**: the cell is **20 x 16 UI px**, matching the mount grid's slot cell so one asset serves both. Corrected in [12 Vehicle Card Face](12-vehicle-card-face.md).

### E8 — Reference camera model is perspective 3D, not a card-size change

The reference capture has a board whose projected top and bottom edges differ in width, while the cards remain readable rectangular faces. The decompiled Stacklands code also uses a camera-height zoom and camera-facing card rotations. The correct interpretation is a perspective ground surface plus billboard cards, not a request to enlarge the native card asset from 48 x 56.

**Resolution**: keep the design card footprint at **48 x 56 art px** and treat that as 100% prefab scale (`scale = 1`). Keep the current 2D renderer as a rollback baseline, then prototype a `Camera3D`/billboard renderer. Promote it to production only after projected-geometry, input, depth, and performance checks pass. The implementation sequence is in [game-update.md](game-update.md).

### E9 — Camera calibration must be measured before changing zoom tokens

The measured 25 wheel steps and the min/initial/max screenshots are useful calibration evidence, but they do not by themselves determine a 3D camera's FOV, pitch, or height range. Card screen size is not a safe proxy because cards face the camera and their capture bounds include outlines/shadows.

**Resolution**: do not change card dimensions, per-card scale, or named zoom tokens to compensate for an uncalibrated camera. Record the 3D camera's pitch, FOV, height range, cursor anchoring, and projected board corners in a prototype capture, then update the tokens in a separate approved change.


Ordered by how much design is blocked. Q1-Q4 should be answered before the art batch in [18 Asset Additions](18-asset-additions.md) step 5, because they can change the card faces.

### Q1 — The source design documents are not in this repository

`info_en.md` states it extracts from `docs/设计/已确认` (Confirmed) and `docs/设计/待验证` (To Be Verified). Neither directory exists in this repo; `docs/design/` contains only the UI/art package.

Those two directories are the actual gameplay source of truth — the mechanics, economy and content that `info_en.md` explicitly says it does not replace. Every "to be verified" marker in the brief resolves to a document I cannot read.

**Needed**: the contents of `docs/设计/`, or a decision that the gameplay design is being written fresh from here.

### Q2 — How does combat actually resolve?

This is the largest UI-shaping unknown. The brief implies a lot without stating it: enemies have "attack interval", "speed" and a "combat state"; C units provide an "AP, attack count, hit/evasion/defense" and affect a "combat command pool"; weapons have cooldowns, "interception" and "range/attributes"; and [06 Interaction and Motion](06-interaction-motion.md) ports Stacklands' fixed combat line-up (teams spread 23 px apart, opposing teams 114 px apart).

Those point in different directions:

| Model | What it means for the UI |
| --- | --- |
| **Auto-resolve with a command pool** (Stacklands-like) | The line-up is a spectator view; the UI's job is showing the pool, the turn order and the log |
| **Player-issued commands** | Needs a selection UI, an ability bar, a target cursor, and a turn indicator on every card |
| **Real-time with cooldowns** | Needs per-weapon cooldown rings on the face, and a much denser HUD |

[16 Garage and Expedition Boards](16-garage-and-expedition-boards.md) currently assumes the board is not blocked during combat and that skill chips are dragged onto targets mid-fight. That assumption is wrong under at least the second model.

**Needed**: which of the three, or a description of a combat round.

### Q3 — What is the region time unit, and how does it map to real seconds?

The brief says regions have "time limits" and [16 of the brief is silent on units]. CP recovery is specified precisely — "+1 per in-session second, stops at 100" — so the loop is in real seconds, but a region limit is described as "cycles" in my documents and as a "countdown" in the brief.

**Needed**: one region limit in real seconds (or a stated cycle length), plus whether the clock runs during dialogs and during combat.

### Q4 — Is the game Chinese-first, English-first, or both?

This decides more than font choice. [02 Design Tokens](02-design-tokens.md) budgets a card title at "4 CJK characters or about 8 Latin letters", and [14 Facility Windows](14-facility-windows.md) is built on a 240 UI px list column that fits about 12 CJK characters or 24 Latin ones.

If both languages ship, every card needs a title that fits in **both** budgets, and the CJK budget (4 characters) is the binding one — which means English card names must be short enough to survive translation. That is a data-authoring constraint, not a rendering one, and it is much cheaper to decide now than to retrofit.

**Needed**: ship languages, and whether Chinese is the primary.

### Q5 — Skill slots: how many, and what unlocks them?

[04 of the brief] says `SlotCost` is fixed at 1 and slots may be empty, and [14 Facility Windows](14-facility-windows.md) shows 7 slots with unlock conditions, but nothing states:

- How many slots exist at start, and the total at max driving level
- What exactly unlocks a slot (driving level alone, quests, purchases?)
- Whether slots unlock per-slot in a fixed order or by category (combat slots vs. non-combat)

This is the number that decides how much of the skills panel's right side is locked at any moment, which is the whole shape of the screen.

### Q6 — Driving level: curve and rewards

The protagonist card shows a driving level and clicking it opens level details. Nothing in the brief says what a level is worth, how XP is earned, or what the levels unlock. The skill slot question (Q5) is probably part of the answer.

**Needed**: the level range, the XP source, and a list of what each level grants.

### Q7 — Warehouse capacity and sorting rules

The brief marks these "pending further unification". [14 Facility Windows](14-facility-windows.md) assumes a slot-count capacity for the warehouse (`warehouse.slots`) and an unbounded cargo-weight model for vehicles (`cargoMax` in weight). Those are two different currencies and the shop sells both. The "blueprints bypass capacity" exception in my design is invented — it needs confirmation or replacement.

**Needed**: is warehouse capacity a slot count, a weight, or uncapped? Does it grow, and how?

### Q8 — Save structure

[16 Garage and Expedition Boards](16-garage-and-expedition-boards.md) assumes garage card positions persist across save/load, which implies the save stores per-card board positions. Whether an expedition can be saved mid-run is not stated anywhere and it is a large implementation difference — resuming a timed run is a very different system from settling one.

**Needed**: number of save slots, and whether mid-expedition saves exist.

### Q9 — The 1 px separator in a stack (see E6)

Does covering the separator row on every lower card in a stack matter? If it does, `stack.offset` is 13 and three documents change. If it does not, it stays 12. This is answerable from the first art test, not from design.

### Q10 — What replaces "Iron Horizon"?

It is marked as a placeholder in [README](README.md). The title affects the logo asset (about 320 x 64) and nothing else, so it is low priority, but the logo is in the first asset batch.

## Errata Applied To

The following source documents carry corrections from this list. Each has an inline **Errata** note pointing here.

| Document | Errata |
| --- | --- |
| [01 Visual Direction](01-visual-direction.md) | E2 — icon budget 32 x 20 |
| [02 Design Tokens](02-design-tokens.md) | E3 — `board.slot_band` 164; E5 — grid cell note; E6 — `stack.offset` pending |
| [03 Canvas and Layout](03-canvas-layout.md) | E3 — band and play area |
| [04 Card System](04-card-system.md) | E1 — art area 29 rows; E4 — the 0.8 shrink is closed |
| [08 Asset Production](08-asset-production.md) | E2 — icon budget |
| [10 Validation Checklist](10-validation-checklist.md) | E3 — band arithmetic; E1 already correct |
| [12 Vehicle Card Face](12-vehicle-card-face.md) | E7 — equipment row cell 20 x 16 |
| [03 Canvas and Layout](03-canvas-layout.md) | E8/E9 — perspective reference and camera calibration gate |
| [04 Card System](04-card-system.md) | E8 — native 48 x 56 prefab and billboard option |
| [06 Interaction and Motion](06-interaction-motion.md) | E8/E9 — logical coordinates and 3D ray-plane input |
| [09 Godot Handoff](09-godot-handoff.md) | E8/E9 — 2D baseline plus 3D prototype |
| [10 Validation Checklist](10-validation-checklist.md) | E8/E9 — 3D camera acceptance checks |
