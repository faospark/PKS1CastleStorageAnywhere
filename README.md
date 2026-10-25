# PKS1CastleStorageAnywhere (Suikoden I Castle Vault Access Anywhere)

A **BepInEx 6 (IL2CPP)** mod for **Suikoden I & II HD Remaster** (Steam) that enables remote, instant access to Rock's Toran Castle Vault from anywhere across towns, dungeons, and the world map.

---

## Overview & Background: Why This Mod Is Essential

Unlike *Suikoden II* (which introduced Barbara's dedicated party convoy) or modern RPGs, **Suikoden I has no shared party inventory bag**. 

Instead, the original 1995 game engine assigns inventory strictly on an individual character basis:
- **Severe Pocket Limitations**: Each character only possesses a tiny personal pocket carrying capacity (a handful of item slots), several of which are already consumed by permanent equipment, armor, and rune pieces.
- **The "Locked-Out Items" Dilemma**: If a companion holding valuable armor, rare runes, antiques, or restorative items is swapped out of your active 6-person party, their carried items are **completely locked away and inaccessible**. You cannot view, trade, or retrieve those items in the field. To get them back, you are forced to make a tedious pilgrimage back to Toran Castle, find the party organizer, swap that character back into your active lineup, strip the items from their inventory into someone else's pockets, and swap your original team back in.
- **Dungeon Loot Suffocation**: Exploring long dungeons quickly turns into inventory juggling. Picking up treasure chests or farming rare drops frequently forces hard choices to discard useful consumables or pass up valuable gear simply because pockets are full.

### How PKS1CastleStorageAnywhere Solves This

**PKS1CastleStorageAnywhere** turns Rock's Toran Castle warehouse into a seamless, accessible-anywhere vault:
- **Instant Remote Storage**: Open Rock's 60-slot Toran Castle warehouse directly from any dungeon, town, or world map screen at the push of a button (**Right Stick Up** or **[F6]**).
- **Acts as the Missing Party Bag**: Store rare runes, overflow equipment, and treasures immediately upon finding them, keeping your active party's pockets lean and uncluttered.
- **No More Backtracking**: Eliminates the frustrating, repetitive loop of trekking back to base just to dump items or juggle gear between benchwarmers.
- **Native Engine Hook with Safeguards**: Invokes the HD Remaster's native warehouse routine (`D_azukar_c.azukari_demo`) while suppressing room background textures, freezing player movement, and blocking random encounter checks so the menu functions reliably outside Toran Castle.

---

## Features

- **Direct Castle Vault Access Anywhere**: Open Toran Castle's item deposit & withdrawal warehouse directly from any town, dungeon, or world map location.
- **Controller & Keyboard Triggers**:
  - **Gamepad**: Push **Right Stick Up** or press **R3** (Right Stick Click).
  - **Keyboard**: Press **[F6]**.
- **Instant Opening & Seamless Field HUD**:
  - Automatically suppresses Rock's warehouse background image (`uiAzukari.bg_obj`) so your current environment remains fully visible behind the menu.
  - Bypasses async background image loading routines (`UIAzukari.Initialize` / `IsCompleteInitialize`) for instantaneous frame-0 UI display.
  - Authentic Suikoden I chime sound effects upon opening and closing.
- **Configurable Rock Recruitment Check**:
  - **`RequireRock = false`** (Default): Access vault storage immediately from the start of your journey without castle restrictions.
  - **`RequireRock = true`**: Restricts vault access until Rock has been officially recruited to Toran Castle (`shiro.level > 0`).
- **Engine-Level Softlock & Crash Safeguards**:
  - Strictly blocked during active combat (`BattleChapter`), loading screens, scene transitions, and event dialogues.
  - Locks out player movement (`Village_c.player_move`, `Fmain_c.userMove`) and wandering monster encounters while the storage window is active.
  - Graceful cancel and exit handling via gamepad back/cancel buttons or toggling the shortcut again.

---

## Controls

| Device | Trigger / Input | Action |
| :--- | :--- | :--- |
| **Gamepad** | **Right Stick Up** or **R3** (Right Stick Click) | Open / Close Toran Castle Vault |
| **Keyboard** | **[F6]** | Open / Close Toran Castle Vault |

---

## Requirements

1. **Suikoden I & II HD Remaster** (Steam edition, 64-bit Windows)
2. **BepInEx 6.0 (Unity IL2CPP x64)**
   - Download **[BepInEx-Unity.IL2CPP-win-x64-6.0.0-pre.2.zip](https://github.com/BepInEx/BepInEx/releases/download/v6.0.0-pre.2/BepInEx-Unity.IL2CPP-win-x64-6.0.0-pre.2.zip)**.

---

## Installation Guide (Steam)

### 1. Locate Game Root Directory
In your Steam client:
1. Open your **Library** and right-click **Suikoden I & II HD Remaster**.
2. Select **Manage** → **Browse local files**.
3. A file explorer window will open at your game's root directory:
   ```text
   ...\steamapps\common\Suikoden I and II HD Remaster\
   ```
   *(This folder contains `Suikoden I and II HD Remaster.exe` and game files).*

---

### 2. Install BepInEx 6 (If not already installed)
1. Download **[BepInEx-Unity.IL2CPP-win-x64-6.0.0-pre.2.zip](https://github.com/BepInEx/BepInEx/releases/download/v6.0.0-pre.2/BepInEx-Unity.IL2CPP-win-x64-6.0.0-pre.2.zip)**.
2. Extract the contents of the ZIP archive directly into your **`Suikoden I and II HD Remaster`** root folder.
3. You should see `doorstop_config.ini`, `winhttp.dll` (or `version.dll`), and the `BepInEx/` directory alongside the game executable.
4. **Launch the game once via Steam**, then quit to desktop.
   - This generates necessary IL2CPP interop assemblies and configuration directories.

---

### 3. Install PKS1CastleStorageAnywhere
The release package is bundled with the complete directory structure for instant drag-and-drop installation.

1. Download **`PKS1CastleStorageAnywhere-vX.X.X.zip`** from Releases (or grab `PKS1CastleStorageAnywhere.dll`).
2. Open the ZIP archive. You will see a `BepInEx` folder.
3. **Drag and drop the `BepInEx` folder directly into your game's root directory**:
   ```text
   Suikoden I and II HD Remaster/
   ├── BepInEx/
   │   ├── config/
   │   │   └── faospark.pks1castlestorageanywhere.cfg (created automatically on first run)
   │   └── plugins/
   │       └── PKS1CastleStorageAnywhere.dll
   ├── doorstop_config.ini
   └── Suikoden I and II HD Remaster.exe
   ```
4. If prompted by Windows, choose **Replace the files in the destination**.

---

### 4. Verify Installation
1. Start the game through Steam.
2. Check `BepInEx/LogOutput.log` (or console if enabled). You will see:
   ```text
   [Info   :   BepInEx] Loading [PKS1 Castle Storage Anywhere 1.0.0]
   [Info   :PKS1 Castle Storage Anywhere] Loading PKS1CastleStorageAnywhere (Toran Castle Vault Access Anywhere)...
   [Info   :PKS1 Castle Storage Anywhere] PKS1CastleStorageAnywhere patches applied successfully!
   [Info   :PKS1 Castle Storage Anywhere] PKS1CastleStorageAnywhere loaded successfully!
   ```
3. Load a Suikoden 1 save file, explore any town or dungeon, and press **Right Stick Up** or **[F6]** to open Rock's Vault.

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

## Building from Source

Prerequisites:
- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) or newer
- BepInEx 6.x IL2CPP installed in your game directory

To compile:
```bash
dotnet build -c Release
```

---

## Community & Support

Join the **Moogles & Mods** Discord community for updates, mod releases, technical support, and discussions:
- **Discord**: [Moogles & Mods Discord](https://discord.gg/bSnpVBV)

---

## Credits & Acknowledgments

- **Konami**: Original creators and developers of the legendary *Suikoden* series and *Suikoden I & II HD Remaster*.
- **faospark**: Creation of PKS1CastleStorageAnywhere, native delegate loop integration (`D_azukar_c.azukari_demo`), zero-delay background suppression, and encounter/movement guard architecture.
- **Moogles & Mods Discord Community**: For ongoing support, research, collaboration, and keeping the Suikoden modding scene thriving.
- **Framework**: [BepInEx](https://github.com/BepInEx/BepInEx) & [HarmonyX](https://github.com/BepInEx/HarmonyX).
- **Suikoden Community**: For keeping the history and spirit of Suikoden alive.
