# Asset Production

## Resolution Policy

Author all raster art at **2x** the 1920 x 1080 reference size, so it stays sharp at 1.25x zoom and on 4K screens. Godot shows it downscaled with mipmaps and linear filtering.

| Asset | Display size at 1.00x | Source size (2x) | Format |
| --- | --- | --- | --- |
| Card frame (per family) | 180 x 252 | 360 x 504 | PNG, RGBA, nine-slice-safe |
| Card art window | 164 x 128 | 328 x 256 | PNG, RGB (opaque) |
| Enemy art with breakout | 164 x 138 | 328 x 276 | PNG, RGBA (top 20 px may overlap the header) |
| Card back (per family) | 180 x 252 | 360 x 504 | PNG, RGBA |
| Category tab icon | 24 x 20 | 48 x 40 | PNG or SVG |
| Badge plates | 36 x 22 | 72 x 44 | PNG, nine-slice |
| UI icons | 24 x 24 | 48 x 48 (plus SVG master) | SVG preferred, PNG fallback |
| Resource icons | 24 x 24 | 48 x 48 | SVG preferred |
| Field background tile | 512 x 512 (tiling) | 1024 x 1024 | PNG, seamless |
| Field decals (seams, stencils, scorch) | varies | 2x | PNG, RGBA |
| Title key art | 1920 x 1080 | 3840 x 2160 | PNG or high-quality WebP |
| Panel nine-slice | varies | 2x, 24 px slice margins at 2x | PNG |
| Cursors | 32 / 48 | 32 / 48 native | PNG |
| Particles (dust, spark, ember) | 8 to 32 | 16 to 64 | PNG, grayscale, tinted in engine |

## Layered Card Construction

Build cards from layers at runtime instead of baking a unique image per card. This keeps family frames consistent and lets content scale.

```text
Card (Control, 180 x 252)
 |- Shadow           (shared soft shadow texture, offset by state)
 |- FrameBase        (family frame, nine-slice)
 |- ArtWindow        (per-card illustration, clipped to 164 x 128)
 |- ArtOverlay       (optional: smoke, damage, desaturation via shader)
 |- WearMask         (shared grunge overlay at alpha.wear.max, randomized UV offset per card instance)
 |- Header           (category tab + title Label)
 |- TypeLine         (Label)
 |- InfoPanel        (Label or ProgressBar or pip row)
 |- Footer           (badge L, family rail, badge R)
 |- CornerSlot       (state glyph)
 |- StateOutline     (selection, target, invalid; drawn last)
```

So artists deliver 6 frame sets, 6 backs, and one illustration per card. They never deliver finished composite cards.

## Art Creation Rules

1. **Value first**: block the illustration in 3 grayscale values and check it at 164 x 128 before adding color. The subject must read as a silhouette.
2. **Palette**: sample midtones from the dust, rust, olive, and steel families in the tokens. Keep saturated cyan, amber, and red for light sources and markings only, under 10% of the illustration area.
3. **Light**: key light from upper left (late-afternoon sun), cool rim light from the right. This keeps the whole card set consistent.
4. **Edges**: the illustration fills the art window completely (no vignette baked in; the frame provides the edge).
5. **Grain**: do not bake heavy grain into the art. Global grain is applied by the frame layer.
6. **Text**: no text in illustrations, except wear-level stencils that are illegible by design.

## Frame Material Recipe

1. Base: flat gunmetal `surface.panel`.
2. Bevel: 1 px highlight top/left at 18% white, 1 px shadow bottom/right at 30% black.
3. Rivets: 4 at the corners (6 px at 1x), plus family-specific additions.
4. Paint layer: the family accent as chipped enamel on the tab and rail, with 15 to 25% of the paint area chipped along the edges.
5. Wear: dust along the lower edge, scratches near the grab areas (the sides at mid-height).
6. Export the frame without any text or icons; those are live layers.

## Naming Convention

```text
<domain>_<family-or-group>_<name>_<variant>.png

card_frame_vehicle.png
card_frame_enemy.png
card_back_event.png
card_art_vehicle_scout_buggy.png
card_art_enemy_raider_truck.png
ui_icon_fuel.png
ui_panel_steel_9slice.png
fx_particle_dust_01.png
field_tile_plate_a.png
field_decal_scorch_02.png
```

All raster sources are 2x by policy, so filenames carry no resolution suffix. Do not use an `@` suffix such as `@2x`. Use lowercase snake_case with no spaces. Card IDs in art filenames must match future card data IDs.

## Godot Import Settings

| Asset type | Settings |
| --- | --- |
| Card art, frames, backs, UI | `compress/mode = Lossless` (or VRAM Compressed for large sets later), `mipmaps/generate = true`, filter linear with mipmaps |
| Pixel-precise icons (if pixel style) | Not applicable; this project uses painterly and vector icons |
| Field tile | `repeat = enabled`, mipmaps on |
| SVG icons | Import scale 2.0, so they rasterize at 2x |
| Particles | Lossless, mipmaps off |

Set the project default texture filter to `Linear Mipmap` so zoomed-out cards stay clean.

## Texture Atlases

Start with individual files. Once there are more than about 80 card illustrations, pack them into atlases by family (4096 x 4096, 2 px padding). Frames, backs, and UI share one atlas.

## Audio Hooks (For Coordination)

Motion events that need a sound hook: grab, drop (metal clack), stack merge, invalid, search tick, reveal flip, hit (light/heavy), destruction, event arrival (radio static plus stamp), equipment attach (bolt ratchet), UI click, UI back, and toast. The names should mirror the particle and animation names.

## First Production Batch (Priority Order)

1. Field background tile plus 3 decals: it sets the whole value range.
2. Six card frames plus six card backs (grayscale plus accent versions).
3. The vehicle and enemy illustration templates: one final illustration each, which serve as the style bar for later art.
4. One illustration each for searchable building, event, fixed building, and equipment.
5. Badge plates, count plate, corner glyphs (timer, lock, new, alert, cross).
6. Panel nine-slice, primary, secondary, and danger buttons, toggle, slider.
7. The 24 px UI and resource icon set (about 20 icons).
8. Particle sprites: dust, spark, ember, smoke.
9. Cursors.
10. Title key art (last, once the style is proven on cards).

Review the batch at 0.80x, 1.00x, and 1.25x on a real 1080p and a 1440p or 4K display before producing more content.
