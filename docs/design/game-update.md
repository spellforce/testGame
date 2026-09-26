# Camera and World-Rendering Update Plan

Status: proposed implementation plan; documentation and code are intentionally not switched in this file.

## Purpose

The reference captures and decompiled Stacklands code show two different rendering behaviors:

- The board is a perspective ground surface. Its top and bottom edges have different screen widths.
- Cards are camera-facing 3D billboards. Their faces remain rectangular and readable while their screen position changes.
- Stacklands zooms by changing a perspective camera's height, not by changing an orthographic size.

The current Iron Horizon prototype uses a `Node2D` world and `Camera2D`. That is still a valid presentation baseline, but it cannot reproduce ground-plane perspective, camera-height zoom, or true depth occlusion. The recommended change is therefore a measured 3D prototype followed by a go/no-go gate, rather than an immediate replacement of the working 2D path.

## Reference facts to preserve

- Card art remains a native **48 x 56 art-pixel** asset at prefab scale 100% (`scale = 1`). The measured 50-ish by 59-ish pixels in the reference captures include outline/shadow and capture variance; they are not a reason to change the design token.
- Board art remains 1104 x 644 art px and the world remains 2208 x 1288 art px.
- Card art is never individually scaled or sheared. A camera change is allowed to change its screen size globally.
- UI remains a separate 2D `CanvasLayer` and must not inherit world perspective.
- Card interaction remains in board/world coordinates. Screen-to-world conversion must continue to use the active camera transform, never a hand-written zoom formula.
- The 2D implementation and `InteractionSelfTest` remain the rollback and behavior reference until the 3D path passes.

## Decision gate

Do not delete the 2D camera yet. Build the 3D slice behind a feature flag or alternate scene and compare it against the reference captures.

Switch the production path to 3D only if all of these are true:

1. A 1080p reference capture reproduces the board's measured top-to-bottom taper within an agreed tolerance (initial target: ±2 percentage points).
2. Cards remain axis-aligned, readable billboards at the board center and near the screen edges.
3. A card at the default camera setting has the intended 100% prefab footprint and a 10-card stack preserves readable headers.
4. Cursor zoom keeps the board point under the cursor within 1 art px after the eased transition.
5. Drag, stack, clamp, grid-align, pause, and depth ordering behavior passes the existing self-test plus the 3D-specific checks below.
6. The 3D slice does not make the 200-card target materially worse than the current 2D baseline.

If any gate fails, keep `BoardCamera : Camera2D`, record the failing measurement, and defer the migration rather than changing card metrics to compensate for camera behavior.

## Implementation phases

### Phase 0 — Baseline and instrumentation

1. Freeze the current 2D behavior as the baseline.
2. Add a debug capture mode that records viewport size, camera transform, zoom/height, card world position, projected card rectangle, and board corner projections.
3. Run `InteractionSelfTest` and save a baseline screenshot at minimum, default, and maximum zoom.
4. Do not change `Metrics.CardW`, `Metrics.CardH`, stack offset, board dimensions, or interaction constants in this phase.

Likely files:

- `Godot/GodotProject/TheGame/GameScripts/Board/BoardCamera.cs`
- `Godot/GodotProject/TheGame/GameScripts/Design/Metrics.cs`
- `Godot/GodotProject/TheGame/GameScripts/Debug/InteractionSelfTest.cs`
- `Godot/GodotProject/TheGame/GameScripts/Game/GameBoard.cs`

### Phase 1 — 3D rendering slice

1. Add a `BoardWorld3D`/prototype scene with a `Node3D` root, a horizontal board plane, a perspective `Camera3D`, directional lighting, and a board material using nearest-neighbor art.
2. Add a prototype card representation using a `Sprite3D` or a camera-facing `MeshInstance3D` quad. The card must use a 48 x 56 texture at native prefab scale and billboard toward the camera.
3. Keep card logical positions as a 2D board coordinate (`x`, `y`) and map them to 3D (`x`, `height`, `z`). Do not move gameplay logic into the pure framework layer.
4. Put the 3D renderer behind an explicit mode switch so the same `CardData`, `CardStack`, `BoardGeometry`, and interaction rules can be exercised by either renderer.
5. Render the HUD through the existing `CanvasLayer`, not through world geometry.

Likely new/changed files:

- `GameScripts/Board/BoardCamera3D.cs`
- `GameScripts/Board/BoardView3D.cs`
- `GameScripts/Cards/CardView3D.cs` or a renderer adapter used by `CardView`
- `GameScripts/Game/GameBoard.cs`
- an alternate prototype scene under `TheGame/DebugTools/` or `TheGame/Scenes/`

### Phase 2 — Camera model and zoom

1. Define camera pitch, field of view, height range, target position, and board clamp in one design/configuration surface.
2. Treat zoom as a continuous camera-height target. Preserve the 25-notch measurement as a calibration input; do not silently equate a wheel notch with a `Camera2D.zoom` multiplier.
3. Zoom toward the cursor by projecting the cursor ray onto the board plane before and after changing height, then translating the camera so the same board point remains under the cursor.
4. Ease current height toward target height over 120 ms. `SetZoomLevel` must not immediately overwrite the current value when a transition is requested.
5. Keep min/default/max as named design tokens. Their final numeric values are decided by the reference-capture comparison, not by the prefab's 100% scale.
6. Add a pixel-art validation pass. Perspective can make different cards occupy slightly different screen sizes; that is expected. Individual card textures must remain nearest-filtered and must not be rescaled by card logic.

### Phase 3 — Interaction and depth parity

1. Replace 2D inverse-zoom conversion in the prototype with a ray-plane intersection against the board plane.
2. Keep the same logical `BoardGeometry` bounds and clamp margins in art-pixel coordinates.
3. Map logical board depth to render order using 3D position plus a deterministic tie-breaker for stacks. A held card uses a dedicated render layer or depth offset.
4. Revalidate stack hit regions, mid-stack pickup, overlap pushing, special drop zones, and grid alignment.
5. Ensure the card billboard does not rotate with the card's logical wobble. Wobble may be represented as a small visual rotation around the camera-facing normal, subject to the pixel-art gate.
6. Confirm that shadows, board rail, slot band, and world props have an intentional depth relationship. Do not rely on node insertion order after switching to 3D.

### Phase 4 — Performance and visual acceptance

1. Compare 2D and 3D at 50, 100, and 200 cards.
2. Measure frame time, draw calls, texture memory, and input latency.
3. Capture minimum/default/maximum zoom at 1280x720, 1920x1080, and 3840x2160.
4. Run the existing interaction self-test and the new camera projection checks.
5. Review grayscale family distinguishability and UI readability with the HUD over the 3D world.
6. Only after the gate passes, make the 3D scene the production scene and retire the 2D camera through a separate cleanup change.

## New 3D-specific verification checks

- The projected board corners form the intended mild trapezoid; the board is not accidentally rotated around its normal.
- A card's projected face remains a rectangle with the expected aspect ratio at the board center, top, and bottom.
- A card's screen size changes as expected with camera height and board depth; card logic never changes `scale` to correct it.
- The same screen cursor position maps to the same board point before and after an eased zoom, within tolerance.
- A card dropped at each board edge clamps in logical board coordinates, not screen coordinates.
- A stack's visual order is stable when two roots share the same board depth.
- The HUD size and anchors are unchanged by camera pitch or height.
- Switching the feature flag back to 2D restores the baseline behavior without changing saved card positions.

## Risks and mitigations

| Risk | Mitigation |
| --- | --- |
| Perspective makes pixel art uneven or blurry | nearest filtering, no mipmaps, billboard cards, capture-based acceptance test |
| 3D depth changes stack ordering | explicit depth sort and held-card render layer |
| Mouse picking becomes inconsistent | ray-plane projection; reuse logical board coordinates |
| Camera zoom changes card readability | calibrate camera height/FOV separately from prefab scale |
| Performance regression | prototype gate at 200 cards before migration |
| Migration blocks gameplay work | keep the 2D renderer and scene until the gate passes |
| Save compatibility breaks | persist logical 2D board coordinates, not camera-space coordinates |

## Definition of done

- The design docs distinguish the **logical art coordinate system** from the optional **3D presentation camera**.
- The 3D prototype can be toggled without changing card data or interaction rules.
- The camera calibration has recorded pitch/FOV/height values and reference screenshots.
- All listed validation checks pass, or the project explicitly keeps the 2D baseline with the failed checks documented.
- A later cleanup commit, separate from the prototype, removes obsolete camera code only after the production decision.

## Files to update during implementation

- `docs/design/02-design-tokens.md` — camera tokens and the invariant native card size.
- `docs/design/03-canvas-layout.md` — reference camera model and 2D/3D presentation options.
- `docs/design/04-card-system.md` — native prefab scale and billboard rule.
- `docs/design/06-interaction-motion.md` — 3D coordinate mapping and camera-height zoom.
- `docs/design/09-godot-handoff.md` — renderer alternatives and 3D handoff contract.
- `docs/design/10-validation-checklist.md` — projection and parity checks.
- `docs/design/20-questions-and-errata.md` — record the camera-model decision as a question/erratum.
- `Godot/GodotProject/TheGame/GameScripts/Board/BoardCamera.cs` — 2D baseline; do not replace until the gate.
- `Godot/GodotProject/TheGame/GameScripts/Game/GameBoard.cs` — renderer selection and prototype wiring.
- `Godot/GodotProject/TheGame/GameScripts/Design/Metrics.cs` — only after measured calibration is approved.
