# Card System

All six card types share one 48 x 56 art px footprint, so stacking, dragging and hit testing work the same for every card.

## Pixel Anatomy

```text
x: 0         1 ........................ 46        47
y0   ink outline (corner pixels removed = 1 px rounded corners)
y1   +------------------------------------------+
     | HEADER  12 rows: card title, font.card    |  y1-12
y12  +------------------------------------------+
y13  separator: 1 row, darker shade of the body   y13
y14  +------------------------------------------+
     |                                          |
     |      ART AREA 46 x 37                    |  y14-50
     |      icon up to 32 x 26, centered        |
     |                                          |
y50  +------------------------------------------+
y51  | [badge L]    family cue      [badge R]   |  y51-62  FOOTER 12 rows
y62  +------------------------------------------+
y63  ink outline
```

Rows add up to 64: outline 1 + header 12 + separator 1 + art 37 + footer 12 + outline 1.
Columns add up to 48: outline 1 + content 46 + outline 1.

| Zone | Rule |
| --- | --- |
| Outline | 1 px `ink` all around. Corner pixels are transparent (1 px rounding). |
| Header | Card body color. Title in `font.card` (10 px), left-aligned at x 3, baseline so the text sits in rows 2 to 11. Max 4 CJK characters or about 8 Latin letters. No wrapping and no ellipsis: card data must provide a short name. |
| Separator | One row, the next darker shade of the body color |
| Art area | Card body color with a 1-row lighter top highlight. Icon centered, max 32 x 26; its bottom may cast a 1 px `ink` shadow. |
| Footer | Up to 2 badges and the family cue (see [05 Card Families](05-card-families.md)) |

The header is the part that stays visible in a stack, which is why the title lives there.

## Badges

Stacklands shows small values in the bottom corners (for example, sale value and health). We keep 2 badge anchors. Gameplay decides what they mean.

| Badge | Position | Size | Style |
| --- | --- | --- | --- |
| Left | x 2, y 52 | 13 x 9 | `bone` plate, `ink` outline, `ink` digits (`font.digits.small`) |
| Right | x 33, y 52 | 13 x 9 | `rust.2` plate, `ink` outline, `white` digits |
| Icon badge | Replaces a number with a 5 x 5 icon (heart, coin, bolt) | 13 x 9 | Same plates |

Up to 2 digits per badge. Values of 100 or more show as "99".

## States

Pixel art must not be scaled, so states use position offsets, outlines and palette swaps.

| State | Treatment |
| --- | --- |
| Resting | Shadow at +1, +2 |
| Hover | Card moves up 1 px (`lift.hover`); details appear in the info panel |
| Held / dragging | Card moves up 4 px (`lift.drag`), shadow at +2, +6, drawn above everything else in the world |
| Valid drop target | 1 px `cyan.3` outline drawn 1 px outside the card's outline |
| Invalid drop (held card) | Same outline in `rust.3` |
| Selected / keyboard focus | `cyan.3` outside outline, plus 3 px corner brackets 2 px outside the card |
| Busy (timer running) | Timer bar above the stack (see below) |
| Disabled / used up | Body colors swapped to the steel ramp of the same value (palette-swap shader); title stays readable |
| Hit | Whole card flashes `white` for 2 frames (`motion.frame` each) |
| New (not yet hovered) | A 3 x 3 `amber.3` dot at the header's right end, cleared on first hover |

Priority when combined: Held > Invalid > Selected > Hit > Busy > Hover > Resting.

## Timer Bar

Stacklands shows a progress bar above a stack while it is working. Same here:

- 40 x 5 px, centered 4 px above the stack's root card.
- 1 px `ink` outline, `steel.2` background, `amber.2` fill (1 px `amber.3` highlight on the top row of the fill).
- Fill grows left to right, updated every frame, no easing.

## Stacking

Terms: the **root card** is the first card of a stack, highest on screen. The **top card** is the last one added, lowest on screen and fully visible.

- Each card placed on a stack sits 12 px (`stack.offset`) lower than the one below it, so the 12-row header of every lower card stays visible.
- Stack height = 56 + 12 x (n - 1). A 10-card stack is 164 px, about 2.9 card heights.
- Open decision: in Stacklands a working or equipped card shrinks to 0.8 scale, and a stack's cards shift as it grows. Scaling pixel art by 0.8 breaks the pixel grid, so these two rules conflict. Either accept soft pixels in those two cases, or keep cards at full size and show the same states with the timer bar and a 5 x 5 work icon. Decide before drawing the card frames.
- No visible limit. Above 10 cards, a count plate (`font.digits`, `steel.2` plate) appears at the right end of the root card's header.
- Grabbing a card picks up that card and every card on top of it. Grabbing the root card moves the whole stack.
- Drop onto a stack: when the held card's rectangle overlaps another card and that card accepts it (no distance limit). It lands on the stack's last card. See [06 Interaction and Motion](06-interaction-motion.md).
- Stacks that overlap other stacks without joining them are pushed apart (see [06 Interaction and Motion](06-interaction-motion.md)).

## Card Back

Same frame. The header shows the family color with no title. The art area shows the family emblem (24 x 24). Used for unopened pack contents and event cards before they flip.

## Hit Area

The full 48 x 56 rectangle, including the transparent corner pixels. For a stack, each lower card's hit area is only its visible 12 px strip; the top card uses its full rectangle.
