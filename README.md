# P.K. - Castle Storage Anywhere

**By [faospark](https://github.com/faospark)**

A **BepInEx 6 (IL2CPP)** mod for **Suikoden I & II HD Remaster** that gives you instant access to Rock's Toran Castle Vault from anywhere in **Suikoden I** (towns, dungeons, and the world map).

## Features

- **Castle Vault Anywhere (`EnableQuickStorage`)**: Open and close Rock's item deposit/withdrawal storage from any field map with **Right Stick Up** / **R3** (gamepad) or **`F6`** (keyboard).
- **Seamless Field Overlay**: Opens instantaneously over your current map without Rock's warehouse background, complete with Suikoden I open/close chimes.
- **Optional Recruitment Gate (`RequireRock`)**: Available from the start of the game by default (`false`), or set to `true` to require recruiting Rock first.
- **Softlock & Encounter Safeguards**: Automatically blocked during battles, loading screens, scene transitions, and dialogue events, while pausing movement and random encounters while open.

## Configuration (`BepInEx/config/faospark.pks1castlestorageanywhere.cfg`)

```ini
[Suikoden 1 Castle Storage]
EnableQuickStorage = true   # Open/close Castle Vault with Right Stick Up, R3, or F6
RequireRock = false         # Require Rock to be recruited before allowing remote vault access
```

## Requirements

- **BepInEx 6.0.0-pre.2 IL2CPP**
- **Suikoden I & II HD Remaster**
- **Recommended:** [Suikoden Fix](https://github.com/d3xMachina/SuikodenFix) by d3xMachina

## Installation

1. Install [BepInEx 6.0 (Unity IL2CPP x64)](https://github.com/BepInEx/BepInEx/releases/download/v6.0.0-pre.2/BepInEx-Unity.IL2CPP-win-x64-6.0.0-pre.2.zip) into your game root directory.
2. Place `PKS1CastleStorageAnywhere.dll` into `BepInEx/plugins/`.
3. Launch the game once to generate `BepInEx/config/faospark.pks1castlestorageanywhere.cfg`.

## Credits & License

- **[faospark](https://github.com/faospark)** — Primary Author & Lead Researcher
- **[d3xMachina](https://github.com/d3xMachina/Suikoden-Fix)** — Creator of Suikoden-Fix & BepInEx IL2CPP foundation
- **[asilverthorn](https://github.com/asilverthorn/suikoden_ref)** — Reference tables, internal IDs, and save structure research
- **Moogles & Mods Discord** — Community support & research ([Invite](https://discord.com/invite/bSnpVBV))
- **Framework:** [BepInEx](https://github.com/BepInEx/BepInEx) & [HarmonyX](https://github.com/BepInEx/HarmonyX)

Distributed under the [MIT License](LICENSE).
