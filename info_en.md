# UI and Card Information Summary

This document is used to unify the product view when designing interfaces and interactions. It extracts information from `docs/设计/已确认` (Confirmed) and `docs/设计/待验证` (To Be Verified); it does not replace `Datas` or specific rule documents.

## 1. Overall UI Model

The game has two primary operating interfaces: the **Garage** and the **Expedition Adventure**. Both use the same "card table + card inspection + drag-and-drop + state feedback" model and are essentially the same operating system; the Garage builds base facilities and long-term management features on top of it.

| Dimension | Garage | Expedition Adventure |
| --- | --- | --- |
| Core form | Card table for managing the protagonist, vehicles, equipment, and base facilities | Card table for unfolding regions, locations, and encounters, and deciding when to evacuate |
| Main purpose | Prepare for expeditions, equip skills, change equipment, repair, resupply, manage the warehouse, and purchase/rent content | Search locations, handle drops, fight, manage vehicle condition/cargo/time, and evacuate |
| Additional facilities | Hunter Camp, Warehouse, Taxi Shop, Vehicle Shop; a Vehicle Factory/Crafting Panel is also proposed | None of the fixed base facilities above |
| Protagonist card | Permanently present on the garage card table and movable; can open the skill equipping panel and driving-level details | Not used as an independent movable protagonist card; this expedition follows the skill setup locked in the garage |
| Equipment changes | Equipment configuration, changing, removal, repair, and resupply are allowed (specific responsibilities follow the vehicle maintenance plan and implementation) | Equipment configuration is locked during the expedition; in principle only transportable/equippable cards can be loaded, and equipped gear cannot be changed or removed |
| Resupply | Quick resupply with gold is available, and supply items can also be dragged | Gold-based resupply is not available; only applicable supply packs can be dragged |
| Time | Base operations that do not depend on the in-expedition countdown | Affected by region time limits, search progress bars, evacuation preparation, and combat flow |

The garage and the expedition should reuse the following interactions: card dragging, card hover inspection panel, cargo-hold panel, progress bars, explicit reasons when an action is unavailable, and immediate save or state feedback after an operation completes. Do not build two mutually inconsistent sets of card-information display rules for the garage and the expedition.

## 2. Garage Fixed Facilities

The following facilities are fixed additions to the garage relative to the expedition table. Facilities should be clickable entry points that produce clear services or decisions, not pure decoration.

| Facility | Primary function | Expected related operations |
| --- | --- | --- |
| Hunter Camp | Take bounty quests | View available targets, quest conditions, quest evidence, and rewards; specific quest fields follow quest data |
| Warehouse | Store item cards and long-term content such as equipment/blueprints | View, categorize, take out, deposit, sell, or use as crafting material; inventory capacity/sorting rules are pending further unification |
| Taxi Shop | Rent vehicles | Browse rentable vehicles, view vehicle attributes and costs, confirm rental; the full economy rules of the "Taxi Shop" are to be verified |
| Vehicle Shop | Buy vehicle supplies | Buy items, weapons, engines, C units, supplies, or other configurable content; prices come from the corresponding data tables |
| Vehicle Factory (proposed/under implementation) | Blueprint crafting and production | View recipes, insert materials, batch craft; blueprints and materials go to the warehouse, not directly onto the expedition table |

### Item Classification

Items themselves need clear categories, and the UI should support at least the following classification hierarchy:

- Weapons
  - Main cannon
  - Secondary cannon
  - S-E (special equipment)
- C units
- Engines
- Ordinary items / materials / valuables
- Supply items, such as armor packs and repair packs
- Blueprints (used for crafting; not equivalent to ordinary items)

They can appear as expedition drop cards and can also be managed in the shop or warehouse.

## 3. Expedition Adventure Interface

The expedition is a "card table without the garage's fixed facilities." The player drives a vehicle into a region unfolded by cards, and within limited in-session time searches locations, handles encounters, manages the cargo hold and vehicle condition, and decides when to leave from the evacuation point.

### Core Flow

```text
Base preparation
→ Select region
→ Unfold region content
→ Select and search locations
→ Handle drops and encounters
→ Manage vehicle condition, cargo hold, and remaining time
→ Complete evacuation at the evacuation point
→ Return to base to settle quests and loot
```

The main decisions during an expedition are: where to go, how deep to search, what to take, whether to fight, and when to evacuate. Each search of a location produces one result after a progress bar completes; when a location is fully searched, it remains on the table and is marked as exhausted. A region deck pack disappears after all its cards have been drawn. Only a successful evacuation brings cargo-hold loot and quest evidence into base settlement; running out of time, having the vehicle destroyed, or voluntarily abandoning all count as failures, and cargo-hold items are lost.

### States and Entry Points in the Expedition

- Search and evacuation use stackable progress bars.

## 4. Protagonist Card and Skills

### Protagonist Card

- The protagonist appears only in the garage as a permanent, movable protagonist card on the garage card table.
- The card image uses the protagonist's avatar, which supports custom upload.
- The card displays the driving level; clicking it opens the level details.
- The card has a skill button; clicking it opens a secondary skill-equipping panel overlaid on the garage. Closing it returns to the garage without switching the top-level scene.
- The protagonist card can be equipped onto a vehicle card; disembarking is only possible in the garage interface, not in the expedition interface.

### Skill Equipping Panel

Uses a two-list layout:

- Left: skills the protagonist has already learned.
- Right: currently unlocked skill slots.
- The left side offers "All / Combat / Non-combat" filters.
- Unlearned skills are not shown; unlocked slots show their unlock conditions but cannot be filled yet.
- Selecting a skill shows its name, description, usage scenario, how it takes effect, trigger conditions, cost, restrictions, and specific effects.
- You can select a skill first and then click a slot, or drag as a shortcut; dragging must not be the only way to do it.
- Clicking an equipped skill removes it; replacing an existing skill must show the skill being replaced and require confirmation.
- The first version uses unified skill slots with `SlotCost` fixed at 1; the same skill cannot be equipped twice; slots may be left empty.
- Equipping, removing, and replacing save immediately without an extra "Apply" button; failures show the specific reason.

After an expedition starts, the skill setup is locked. During combat and non-combat phases, you can only view the setup for the current expedition and cannot equip, remove, or replace; the lock is released after returning to the garage. Skills are only classified by usage scenario into combat and non-combat; there are no cross-scenario skills.

### Command Points (CP)

Command points are a resource exclusive to the protagonist card; they do not belong to the vehicle and are not persisted across expeditions.

| Rule | Value/Behavior |
| --- | --- |
| At expedition start | 100 |
| Absolute cap | 150 |
| Effective cap in non-combat | 100 |
| Non-combat recovery | +1 per in-session second, stops at 100 |

Combat skill examples: fixed-point bombardment, field emergency repair, emergency armor patching, overload burst fire, close-range breakthrough, full broadside barrage, adverse-conditions fire control. Non-combat skill examples: sign scanning, directional engine, field assembly, on-site repair, hazardous emergency repair, major-damage overhaul, ammunition-type allocation, temporary modification. The above skills and their values, materials, and conditions are currently to-be-verified design; the UI should be data-table driven and must not judge effects from skill names or description text.

Skills can only act on vehicles, weapons, ammunition magazines, C units, SP, parts, the cargo hold, materials, location searches, encounters, and evacuation preparation.

The protagonist can get on and off the vehicle in the garage interface.

## 5. Vehicle Card Information

### Detailed Information

The vehicle card inspection panel should show:

- SP (1-50000).
- Load: `LoadUsed / LoadMax`.
- Cargo hold.
- Defense and self weight.
- Number of equipment slots, number of engines, number of C units.
- Vehicle traits and current state.
- Equipment details:
  - Weapons: name, attack, magazine/current ammunition, cooldown, and main-cannon/secondary-cannon/S-E slot.
  - Engines: load cap, defense, and state.
  - C units: hit, evasion, defense, and state.

Equipment states must distinguish at least intact, damaged, and destroyed; destroyed equipment cannot be used, while damaged equipment can be used but its traits are disabled. SP reaching zero destroys the vehicle/fails the expedition; the vehicle's own SP, ammunition, armor, and other states do not automatically recover after an expedition ends.

### Relationship Between Vehicle and Protagonist Cards

The protagonist card is an independent character card, but it can also be displayed as associated information/attached content on the vehicle card. This association should not duplicate a copy of the protagonist's state; the protagonist's skill setup and CP are still maintained by protagonist data, and the vehicle card only displays the currently associated protagonist and the available skill entry point (the specific card-face layout is to be verified).

## 6. Other Card Types and Their Functions

The card types explicitly covered by the current document are as follows. Counted by type, there are **11 types**; weapons are further divided by slot into main cannon, secondary cannon, and S-E, which are weapon subcategories and do not count as separate data card types.

| Type | Function | Main inspection info/interactions |
| --- | --- | --- |
| Region card | Select and unfold an expedition region, providing time limits and regional drop groups | Region identity, time limit, resource theme, main risks; clicking generates locations, evacuation points, and other results |
| Location card | Search to produce items, equipment, monsters, or new locations | Remaining search count, time per single search; drag a vehicle to the location or perform a search; disappears once fully searched |
| Deck pack / region deck pack | Carries the undrawn card results within a region | Name, description, and operation hints; drawn one by one, disappears when empty |
| Evacuation point card | The evacuation target that ends the expedition | Whether it is occupied, preparation seconds, interruption rules, evacuation cost; only an appeared, usable evacuation point can start evacuation |
| Monster/encounter card | Enters combat and produces loot on death | HP, attack, defense, attack interval, level, speed, experience, combat state; does not enter the cargo hold |
| Vehicle card | The player's carrier for movement, combat, and cargo during the expedition | SP, structure, load, cargo hold, equipment, and state; can be dragged, search, evacuate, engage combat, and load |
| Item card | Materials, valuables, supplies, and other consumable content | Weight, cargo-hold occupancy; can be placed in the cargo hold, dragged onto the vehicle, or used on applicable targets |
| Weapon card | Main cannon, secondary cannon, or S-E equipped on the vehicle | Attack, defense, weight, magazine, slot, cooldown, interception, range/attributes; can be loaded during the expedition but not freely removed |
| Engine card | Provides load, mobility, defense, and engine traits | Load cap, defense, weight, bonus type/value, traits; installation position must match the vehicle's slot |
| C unit card | Provides AP, attack count, hit/evasion/defense, and C unit traits | Hit, evasion, defense, weight, traits; state affects the combat command pool |
| Blueprint card | Unlocks factory recipes for making supplies, parts, or other products | Product, aggregated material requirements, craft time, whether all materials are consumed; goes to the warehouse and is not crafted directly during the expedition |

Drop sources are mainly: region cards generate initial content; location cards generate items, equipment, monsters, or new locations; monster deaths generate loot.

# Content Needing Design

### Pop-up Design

Pop-up animation, pop-up style, background, size.
Pop-up content types.
One idea I had is:
The window has two columns: the left column is a list (also provide a button to switch to card display) (sorting, search, categorization), the right column is the details (which may overlap with the detail box at the bottom-left), and below the right column there is an area for an action bar usable by all facilities, like a Windows or Mac file manager.

The warehouse and cargo hold also open as pop-ups; cards inside the warehouse/cargo-hold pop-up can be dragged directly onto the table, so these pop-ups may also need to be movable or resizable.

Cards can be dragged to the shop to sell; the shop may also need a "buyback" tab.

So I feel we might need to design a generic pop-up content page that disables, shows, or hides certain features depending on the context — though of course the main design is up to you; I'm just offering some suggestions.

### Warning Boxes, Confirmation Boxes

The design of these warning boxes, confirmation boxes, and error boxes.

### Card Design

The final number of card types may also include quest cards, event cards, and so on.
The vehicle card carries the most information. What goes on the vehicle card face, where do the numbers go, and with that much equipment (the most might be 4-5 weapon slots, 2 engine slots, 2 C unit slots), where does it all go? When the protagonist is equipped onto the vehicle, where does that go? How does it open?

### Whether Cards Provide a Right-Click Shortcut Menu

Is it redundant design?
If not, the shortcut menu's pop-up and styling also need to be considered.
For example, right-clicking a vehicle card in the garage interface can bring up a menu for one-click repair (consumes gold), refilling SP, opening the cargo hold, and unfolding equipment; right-clicking an already-equipped card can also bring up repair.
Right-clicking a vehicle card in the garage interface may also show some of the protagonist's out-of-session skills, such as repair (consuming repair packs and CP).
Of course, the protagonist's skills and these operations could also be made into fixed cards at the top, which the player drags and drops cards onto to interact (but facility cards such as shops and protagonist skills must be clearly separated).
