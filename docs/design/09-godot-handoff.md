# Godot Handoff

Target: Godot 4.3 or newer, GDScript. 4.3 or newer is needed for the `TileMapLayer` node used for the world terrain.

The HUD root is 960 x 540 UI px at 16:9. On other screen shapes it is larger (for example, 1280 x 720 UI px on a 1280 x 720 window at 1x), so anchor HUD panels to their corners instead of placing them at fixed coordinates.

## Pixel Rendering Approach

The world uses continuous zoom (like Stacklands), so it is **not** rendered into a fixed low-resolution viewport. Instead:

- The world is drawn directly at screen resolution with a `Camera2D`. Textures use nearest-neighbor filtering, so art pixels stay sharp squares.
- At whole-number zoom factors (1, 2, 3, 4 screen px per art px), every art pixel is exactly the same size. In between, some art pixels are 1 screen px wider than others. At these small ratios it is barely visible. The optional pixel-perfect zoom setting removes it.
- The UI is on a separate `CanvasLayer` with its own integer scale.

## Project Settings

| Setting | Value | Why |
| --- | --- | --- |
| `display/window/size/viewport_width` | 1920 | Base resolution |
| `display/window/size/viewport_height` | 1080 | |
| `display/window/stretch/mode` | `disabled` | World and UI scaling are handled by script (see below) |
| `rendering/textures/canvas_textures/default_texture_filter` | `Nearest` | Sharp pixels |
| `rendering/2d/snap/snap_2d_transforms_to_pixel` | `true` | Removes sub-pixel shimmer |
| `rendering/2d/snap/snap_2d_vertices_to_pixel` | `true` | |
| `rendering/anti_aliasing/quality/msaa_2d` | `Disabled` | |
| `application/run/max_fps` | 0, VSync on | |
| Texture import (all art) | Compress mode `Lossless`, mipmaps off | Avoids blur and color changes |

### World Scaling

```gdscript
# board_camera.gd
var zoom_level := 0.5  # 0.5 (min) to 2.0 (max), see 02-design-tokens.md

func _apply_zoom() -> void:
    var screen_px_per_art_px := zoom_level * get_viewport_rect().size.y / 540.0
    if Settings.pixel_perfect_zoom:
        screen_px_per_art_px = maxf(1.0, roundf(screen_px_per_art_px))
    zoom = Vector2.ONE * screen_px_per_art_px
```

### UI Scaling

```gdscript
# main.gd
func _update_ui_scale() -> void:
    var s := get_viewport().get_visible_rect().size
    var ui_scale: int = maxi(1, floori(minf(s.x / 960.0, s.y / 540.0)))
    if Settings.ui_scale_override > 0:
        ui_scale = Settings.ui_scale_override
    $HudLayer.transform = Transform2D.IDENTITY.scaled(Vector2.ONE * ui_scale)
    $HudLayer/Root.size = s / ui_scale  # 960 x 540 at 1080p
```

Call both on start and whenever the window size changes.

## Folders

```text
res://
  art/
    palette/       iron_horizon_32.gpl
    cards/         frames/  backs/  icons/  states/
    board/
    world/         tiles/  props/  animals/
    fx/
    ui/            panels/  buttons/  icons/  cursors/
    fonts/
    src/           .aseprite sources (excluded from export)
  data/
    cards/         one .tres per card
  scenes/
    main/          Main.tscn
    world/         World.tscn, Board.tscn, BoardCamera.tscn
    cards/         Card.tscn, CardStack.tscn
    ui/            Hud.tscn, ListPanel.tscn, InfoPanel.tscn, ResourceBox.tscn, TimeBox.tscn
    menus/         TitleScreen.tscn, PauseMenu.tscn, Settings.tscn, GameOver.tscn, ConfirmDialog.tscn
    fx/            DustPuff.tscn, Sparks.tscn, DamageNumber.tscn
  scripts/
  themes/
    ui_theme.tres
  shaders/
    card_state.gdshader
```

## Main Scene Tree

```text
Main (Node)
 |- World (Node2D)
 |   |- BoardCamera (Camera2D)
 |   |- Terrain (TileMapLayer)
 |   |- Props (Node2D)
 |   |- Board (Node2D)                 board plate, rail, slot band; also the play-area rectangle
 |   |- Cards (Node2D)                 all stacks; draw order set by script
 |   |- Held (Node2D)                  cards being dragged or in flight
 |   |- Fx (Node2D)
 |- HudLayer (CanvasLayer, layer 10)
 |   |- Root (Control, 960 x 540 UI px)
 |       |- ListPanel  |- InfoPanel  |- ResourceBox  |- TimeBox  |- PausedLabel
 |- MenuLayer (CanvasLayer, layer 20)
```

## Card.tscn

```text
Card (Node2D)                       card_view.gd; origin = top-left corner of the card
 |- Shadow (ColorRect 48 x 56, ink 45%)
 |- Body (Node2D)                   moved up for hover/drag lift
     |- Frame (Sprite2D)            material: card_state.gdshader
     |- Icon (Sprite2D)
     |- Title (Label, font.card)
     |- BadgeLeft (Node2D)  |- BadgeRight (Node2D)
     |- NewDot (Sprite2D)
     |- Outline (Sprite2D)          cyan / red state outline
```

- Put the card's origin at its top-left corner and keep all child offsets whole numbers.
- Lift moves `Body`; the `Shadow` stays put and gets a bigger offset. The card's logical position never changes during lift.
- `card_state.gdshader`: white flash (mix to white), and palette swap to the steel ramp for the disabled state.

## Card Data

```gdscript
class_name CardData
extends Resource

enum Family { VEHICLE, ENEMY, SEARCHABLE, EVENT, FIXED_BUILDING, EQUIPMENT }

@export var id: StringName          # matches icon file name: card_icon_<family>_<id>.png
@export var family: Family
@export var title: String           # short: max 4 CJK characters / about 8 Latin letters
@export var full_name: String       # shown in the info panel
@export_multiline var description: String
@export var icon: Texture2D
@export var badge_left: int = -1    # -1 = hidden
@export var badge_right: int = -1
@export var grade: StringName = &"standard"
```

Gameplay fields are added later. The view only reads these.

`CardFamilyStyle` resources (one per family) hold the frame, back, flip strip and title color, so the view never branches on family in code.

## Scripts

| Script | Job |
| --- | --- |
| `card_view.gd` | Shows a `CardData`; states (hover, held, outline, flash, disabled, new) |
| `card_stack.gd` | Keeps the list of cards, lays them out 12 px apart, shows timer bar and count |
| `drag_controller.gd` | Press, click-or-drag, pan-or-drag, drop target search (24 px), return to play area |
| `repel.gd` | Pushes overlapping root cards apart each physics frame, scaled by mass |
| `board_camera.gd` | Zoom toward cursor, pan, clamp, event nudge |
| `motion.gd` (autoload) | Shared tweens: lift, land, arc launch, bump, shake. Reads motion tokens and the reduced-motion setting. |
| `settings.gd` (autoload) | Saves and loads settings |

Gameplay sends signals (`card_spawned`, `attack`, `card_destroyed`, `event_arrived`...). The view reacts with the animations in [06 Interaction and Motion](06-interaction-motion.md). Gameplay code never runs tweens.

## Performance

- Target: 60 FPS with 200 cards on the board.
- Push check runs per root card per frame and stops at the first valid overlap, as in the engine. Use a spatial grid (cells of 64 x 64 art px) so 200 cards do not test every pair.
- Pool damage numbers, dust and sparks.
- One shared `ShaderMaterial` for all cards; per-card values through instance uniforms.
