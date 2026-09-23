# Godot Handoff

Target: **Godot 4.x** (GDScript). This document maps the design to project structure so the visual rules live in shared resources rather than being scattered across gameplay code.

## Project Settings

| Setting | Value |
| --- | --- |
| `display/window/size/viewport_width` | 1920 |
| `display/window/size/viewport_height` | 1080 |
| `display/window/stretch/mode` | `canvas_items` |
| `display/window/stretch/aspect` | `expand` (extra width or height becomes field bleed; see [03 Tabletop Layout](03-tabletop-layout.md)) |
| `rendering/textures/canvas_textures/default_texture_filter` | `Linear Mipmap` |
| `gui/theme/custom` | `res://themes/ui_theme.tres` |

With `expand`, anchor the HUD to the centered 1920 x 1080 safe region, not to the viewport edges, so ultrawide screens keep the HUD inside the 1728 px safe width.

## Directory Layout

```text
res://
  art/
    cards/
      frames/        card_frame_<family>.png
      backs/         card_back_<family>.png
      illustrations/ card_art_<family>_<id>.png
    ui/
      icons/
      panels/
      cursors/
    field/           tiles and decals
    fx/              particle sprites
    materials/       grunge, wear, and noise textures
  fonts/
  themes/
    ui_theme.tres
    card_style_library.tres
    motion_tokens.tres
    card_styles/
      vehicle.tres  enemy.tres  searchable.tres
      event.tres    fixed_building.tres  equipment.tres
  shaders/
    card_dissolve.gdshader
    card_state.gdshader      (desaturate, flash, outline)
  scenes/
    main/            Main.tscn, Boot.tscn
    board/           Tabletop.tscn, FieldBackground.tscn, BoardCamera.tscn
    cards/           Card.tscn, CardStack.tscn, CardPreview.tscn, CardTooltip.tscn
    ui/
      hud/           TopBar.tscn, BottomTray.tscn, ResourceCounter.tscn, Toast.tscn
      menus/         TitleScreen.tscn, RunSetup.tscn, PauseMenu.tscn, Settings.tscn, RunResult.tscn
      common/        ModalPanel.tscn, SteelButton.tscn, IconButton.tscn
    fx/              DustPuff.tscn, Sparks.tscn, DamageNumber.tscn
  scripts/
    ui/  cards/  board/  fx/  data/
```

## Scene Tree: Main Run Screen

```text
Main (Node)
 |- Tabletop (Node2D)                      world space, zoomable
 |   |- BoardCamera (Camera2D)             zoom 0.80 to 1.25, pan limits = tabletop bounds + 240 px
 |   |- FieldBackground (Node2D)           tiles + decals (layer 1 to 2)
 |   |- CardGround (Node2D)                fixed buildings (layer 3)
 |   |- CardWorld (Node2D)                 stacks (layer 4), y-sort off, z managed by script
 |   |- CardTransient (Node2D)             held or arriving cards (layer 5)
 |   |- FieldFx (Node2D)                   particles and damage numbers (layer 6)
 |- HudLayer (CanvasLayer, layer 10)       not affected by the camera
 |   |- SafeArea (Control, 1920 x 1080 centered)
 |       |- TopBar
 |       |- BottomTray
 |       |- ToastStack
 |       |- OffscreenMarkers
 |- OverlayLayer (CanvasLayer, layer 20)   pause, modal, inspect, settings
 |- CursorLayer (CanvasLayer, layer 30)    custom cursor and drag ghost if needed
```

Cards are `Node2D`-rooted scenes in world space (so the camera zoom scales them), but their internal layout uses `Control` children with a fixed 180 x 252 root. The Tabletop camera zoom must not affect HUD layers.

## Card.tscn Structure

```text
Card (Node2D)                    script: card_view.gd
 |- Shadow (Sprite2D)
 |- Body (Control, 180 x 252, pivot at center)
     |- Frame (NinePatchRect)
     |- Art (TextureRect, clip)       material: card_state.gdshader
     |- ArtOverlay (TextureRect)
     |- Wear (TextureRect)
     |- Header (HBoxContainer)
     |   |- CategoryTab (TextureRect)
     |   |- Title (Label)
     |   |- CornerSlot (TextureRect)
     |- TypeLine (Label)
     |- InfoPanel (PanelContainer)
     |   |- InfoLabel / Progress / Pips
     |- Footer (Control)
     |   |- BadgeLeft  |- Rail  |- BadgeRight
     |- StateOutline (Control, custom _draw)
 |- HitArea (Area2D + CollisionShape2D 192 x 264)
```

Scale, tilt, and lift animate `Body` and `Shadow`, never the root. This keeps the logical card position stable for layout and snapping.

## Resources

### `CardFamilyStyle` (Resource), one `.tres` per family

```gdscript
class_name CardFamilyStyle
extends Resource

@export var family_id: StringName          # &"vehicle", &"enemy", &"searchable", &"event", &"fixed_building", &"equipment"
@export var frame_texture: Texture2D
@export var back_texture: Texture2D
@export var tab_texture: Texture2D
@export var rail_texture: Texture2D
@export var accent_color: Color
@export var frame_margins: Vector4i = Vector4i(24, 24, 24, 24)   # nine-slice at 2x
@export var art_breakout_px: int = 0       # enemies: 10
@export var shadow_scale: float = 1.0      # fixed buildings: 0.7 (tighter)
@export var can_lift: bool = true          # fixed buildings: gameplay may override
```

### `CardStyleLibrary` (Resource)

A dictionary from `family_id` to `CardFamilyStyle`, plus shared textures (wear masks, badge plates, glyphs).

### `MotionTokens` (Resource)

Exposes every motion token from [02 Design Tokens](02-design-tokens.md) as exported floats and `Tween.TransitionType` / `Tween.EaseType` pairs, plus a `reduced_motion: bool` that the settings screen toggles. All animation code reads from here; no durations are hardcoded.

### `CardPresentationData` (Resource or plain data)

The minimal contract between future gameplay data and the card view:

```gdscript
class_name CardPresentationData
extends Resource

@export var card_id: StringName
@export var family_id: StringName
@export var title: String
@export var type_line: String
@export var art: Texture2D
@export var info_text: String = ""
@export var badge_left: String = ""        # empty = hidden
@export var badge_right: String = ""
@export var badge_left_style: StringName = &"primary"   # primary | secondary | threat
@export var grade: StringName = &"standard"
```

Runtime state (selected, busy progress, damaged, disabled, new) is set on the view through methods such as `set_state()` and `set_progress()`, not stored in this resource.

### Theme (`ui_theme.tres`)

Define type variations for `SteelButtonPrimary`, `SteelButtonSecondary`, `SteelButtonDanger`, `PanelSteel`, `PanelInset`, `LabelDisplayLg`, `LabelDisplayMd`, `LabelUiMd`, `LabelUiSm`, and `LabelBadge`. Colors, fonts, and StyleBoxes come from the tokens; controls reference type variations, never inline overrides.

## Scripts (Responsibilities)

| Script | Responsibility |
| --- | --- |
| `card_view.gd` | Applies `CardPresentationData` + `CardFamilyStyle`; owns visual states and their tweens |
| `card_stack_view.gd` | Cascade layout (`offset.stack`), count plate, hover fan |
| `drag_controller.gd` | Pointer and controller grab, drag, drop, snap target search (64 px), return, push-apart |
| `board_camera.gd` | Zoom clamp and step, pointer-centered zoom, pan, event nudge |
| `motion.gd` (autoload) | Helper functions: `lift()`, `settle()`, `shake()`, `pop()`, `flip()`; they honor reduced motion |
| `fx_spawner.gd` | Pools particles and damage numbers |
| `hud_*.gd` | Binds HUD widgets to game state signals |
| `ui_focus.gd` | Default focus, back navigation, focus restoration after modals |

Gameplay code emits signals (`card_attacked`, `search_completed`, `event_arrived`, and so on). The presentation layer listens and plays the choreography from [06 Interaction and Motion](06-interaction-motion.md). Gameplay never calls tweens directly.

## Performance Notes

- Target 60 FPS with 150 cards on the board on mid-range hardware.
- Pool particle and damage-number nodes.
- Keep per-card shaders to one material; share a single `ShaderMaterial` where parameters are identical and use instance uniforms for per-card values.
- Card tooltips and inspect views are single shared instances that are repositioned, not created per card.
