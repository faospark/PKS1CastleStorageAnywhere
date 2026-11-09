# P.K. - Castle Storage Anywhere

A **BepInEx 6 (IL2CPP)** mod for **Suikoden I & II HD Remaster** (Steam) that enables remote, instant access to Rock's Toran Castle Vault from anywhere across towns, dungeons, and the world map.

---

## Overview & Features

Unlike *Suikoden II* or modern RPGs, **Suikoden I has no shared party inventory bag**. Inventory is strictly per-character, causing companion items to become locked away whenever party members are swapped out, and quickly filling pockets in long dungeons.

**PKS1CastleStorageAnywhere** turns Rock's Toran Castle warehouse into a seamless, accessible-anywhere party vault:
- **Direct Castle Vault Access Anywhere**: Open Toran Castle's item deposit & withdrawal warehouse directly from any town, dungeon, or world map location at the push of a button.
- **Controller & Keyboard Triggers**:
  - **Gamepad**: Push **Right Stick Up** or press **R3** (Right Stick Click).
  - **Keyboard**: Press **[F6]**.
- **Instant Opening & Seamless Field HUD**:
  - Automatically suppresses Rock's warehouse background image (`uiAzukari.bg_obj`) so your current environment remains fully visible behind the menu.
  - Bypasses async background image loading routines (`UIAzukari.Initialize` / `IsCompleteInitialize`) for instantaneous frame-0 UI display.
  - Authentic Suikoden I chime sound effects upon opening and closing.
- **Configurable Rock Recruitment Check**:
  - `RequireRock = false` (Default): Access vault storage immediately from the start of your journey without castle restrictions.
  - `RequireRock = true`: Restricts vault access until Rock has been officially recruited to Toran Castle (`shiro.level > 0`).
- **Engine-Level Softlock & Crash Safeguards**:
  - Strictly blocked during active combat, loading screens, scene transitions, and event dialogues.
  - Locks out player movement and wandering monster encounters while the storage window is active.
  - Graceful cancel and exit handling via gamepad back/cancel buttons or toggling the shortcut again.

---

## Controls

| Device | Trigger / Input | Action |
| :--- | :--- | :--- |
| **Gamepad** | **Right Stick Up** or **R3** (Right Stick Click) | Open / Close Toran Castle Vault |
| **Keyboard** | **[F6]** | Open / Close Toran Castle Vault |

---

## Configuration Options

Edit `BepInEx/config/faospark.pks1castlestorageanywhere.cfg` with Notepad or any text editor:

```ini
[Suikoden 1 Castle Storage]

## Press Right Stick Up (or R3 / [F6] on keyboard) to open Rock's Castle Vault directly from anywhere on the field.
# Setting type: Boolean
# Default value: true
EnableQuickStorage = true

## When enabled, Castle Vault requires Rock to be recruited (Toran Castle warehouse unlocked) before it can be accessed. Set to false to allow opening anytime without recruitment or castle restrictions.
# Setting type: Boolean
# Default value: false
RequireRock = false
```

---

## Installation

- **Prerequisite**: [BepInEx 6.0 (Unity IL2CPP x64)](https://github.com/BepInEx/BepInEx/releases/download/v6.0.0-pre.2/BepInEx-Unity.IL2CPP-win-x64-6.0.0-pre.2.zip) installed in your game root directory.
- **Install**: Extract the release ZIP directly into your game's root directory (or place `PKS1CastleStorageAnywhere.dll` into `BepInEx/plugins/`).
- **Verify**: Launch the game; `BepInEx/LogOutput.log` will confirm `[PKS1CastleStorageAnywhere] loaded successfully!`.

---

## Credits & Acknowledgments

- **[d3xMachina](https://github.com/d3xMachina/Suikoden-Fix)**: Creator of [Suikoden-Fix](https://github.com/d3xMachina/Suikoden-Fix). Bootstrapped the foundational BepInEx IL2CPP implementation and provided early inspiration for Remaster modding.
- **[asilverthorn](https://github.com/asilverthorn/suikoden_ref)**: Creator of [suikoden_ref](https://github.com/asilverthorn/suikoden_ref). Invaluable early reference tables, internal IDs, and save structure research.
- **[faospark](https://github.com/faospark)**: Primary Author & Lead Researcher.
- **Moogles & Mods Discord Community**: For ongoing support, research, collaboration, and keeping the Suikoden modding scene thriving. Join us on the [Moogles & Mods Discord](https://discord.com/invite/bSnpVBV).
- **Framework**: [BepInEx](https://github.com/BepInEx/BepInEx) & [HarmonyX](https://github.com/BepInEx/HarmonyX).

---

## License

Distributed under the [MIT License](LICENSE).
