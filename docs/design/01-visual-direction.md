# Visual Direction

## Tone

Iron Horizon is harsh but not monochrome. It is the visual language of a convoy maintained with scavenged parts: sun-bleached paint, welded repairs, scorched exhaust, dust trapped in seams, and small points of deliberate light. The tabletop feels dependable enough to command from, even when the world beyond it is not.

The target is illustrated industrial realism, not photorealism. Forms are simplified into clear planes and silhouettes so every card reads at tabletop scale.

## Material Hierarchy

| Layer | Purpose | Treatment |
| --- | --- | --- |
| Field surface | Playable tabletop | Dusty charcoal plates, seams, faded stencils, sparse bolt lines |
| Card outer frame | Object identity and durability | Dark gunmetal with 1 px edge light and category accent insert |
| Card inset | Information surface | Near-black enamel or dark canvas; flatter and calmer than the frame |
| Art window | Category image | Matte painted illustration with a controlled film-grain pass |
| Data plates | Functional labels and badges | Pale enamel, brass/amber marker, or stamped steel; no glassy pills |
| Urgency layer | Danger, heat, new event | Localized amber/red glow, not a full-card color wash |

Wear must follow form: edge chipping at corners, horizontal dust along lower edges, soot around vents, and scratches near handles or mount points. Keep the title bar, icons, and value badges free of destructive texture.

## Color Roles

Color values live in [02 Design Tokens](02-design-tokens.md). Their meaning is fixed here.

- Coal and graphite establish the board, card frame, and modal foundation.
- Bone enamel provides readable labels and strongest text.
- Industrial cyan marks player ownership, selected-object focus, and system navigation.
- Signal amber marks action readiness, power, heat, and attention requiring a decision.
- Field olive marks neutral locations, systems, and search-ready state.
- Threat red marks hostile identity, damage, and irreversible warnings.
- Dust beige belongs only to environmental texture and secondary dividers; it must not become the dominant UI surface.

Never encode category or critical status by color alone. Pair each color role with a distinct tab profile, icon, border treatment, or label.

## Illustration Rules

Each card illustration uses a three-value composition:

1. A dark environmental mass for depth.
2. A readable midtone subject silhouette.
3. A selective high-value edge, headlamp, metal reflection, or dust break that points toward the title and badges.

The subject occupies 55% to 70% of the art window. Keep horizon lines in the upper third or omit them. Avoid tiny debris fields, busy settlements, wide cinematic skies, and detailed figures at this scale.

### Subject Direction

- Vehicles: 3/4 angle, low camera, one dominant chassis, visible wheel/tread mass, identifiable weapon or utility profile.
- Enemies: confrontational angle or abrupt intrusion, stronger contrast around the threat-facing edge, uneven or improvised silhouette.
- Searchable buildings: one clear facade, breach point, door, loot hatch, or antenna; scene remains quieter than vehicle art.
- Events: graphic symbol plus a single environmental cue such as a sand plume, flare, radio mast, or blackout stripe.
- Fixed buildings: grounded, low-center composition with foundation, pipes, gantry, or defensive wall visibly contacting the lower edge.
- Equipment: product-like object portrait on dark technical backdrop; one physical mount, cable, casing, or tool head makes function plausible.

## Typography

Use two families when production begins.

- Display: a condensed, squared industrial face for card names, section labels, and numerals. Use all caps sparingly for major labels and never letter-space text negatively.
- Interface: a high-legibility neutral sans for descriptions, menus, tooltips, and accessibility-sensitive values.

Names use compact upper/lower title case where local writing allows it. Avoid long all-caps strings inside cards. Text hierarchy and exact sizes are defined in [02 Design Tokens](02-design-tokens.md).

## Icons and Marks

Use simple, solid, familiar pictograms with a 2 px visual stroke at card scale: wheel, reticle, shield plate, fuel can, wrench, radio mast, crate, tower, warning triangle, and skull-like threat marker only when thematically appropriate. Icons sit in stamped plates or painted panels; they never float as glossy rounded rectangles.

Create category marks with both shape and color distinction:

- Vehicle: horizontal cyan rail and tread notch.
- Enemy: jagged red tab and broken edge.
- Searchable: olive locator tag with keyed notch.
- Event: torn dispatch band.
- Fixed building: broad low rivet plate.
- Equipment: narrow yellow technical stripe.

## Texture and Lighting

Texture overlays should be subtle: 3% to 8% opacity for grain, 6% to 14% for dust masks, and no more than 18% for localized wear. Use multiply-like dark dirt only on broad background surfaces; artwork maintains its intended value hierarchy.

Global lighting is late-afternoon hard light filtered through dust. Cyan is emissive only for owned-system indicators. Amber and red emit softly from a local source, with no full-screen neon bloom.

## Contrast and Accessibility

- Standard body text on dark surfaces targets at least 4.5:1 contrast.
- Large display text targets at least 3:1 contrast.
- Selected state combines cyan color, 2 px outline, and a corner-bracket icon.
- Damage combines threat red, fractured border segment, and a damage mark icon.
- Searchable state combines olive, locator tag, and an open/closed hatch pictogram.
- Reserve continuous pulsing for one urgent item at a time; use a 2.2 s cycle at most.

See [10 Validation Checklist](10-validation-checklist.md) for visual review criteria.