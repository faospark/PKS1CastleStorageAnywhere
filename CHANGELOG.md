# Changelog - PKS1CastleStorageAnywhere

All notable changes to PKS1CastleStorageAnywhere will be documented in this file.

---

## [1.0.0] - 2026-10-26

### Added
- **Toran Castle Vault Access Anywhere (S1) ([QuickStoragePatch.cs](Patches/QuickStoragePatch.cs))**:
  - Direct access to Rock's Castle Vault (warehouse inventory) from anywhere on the field, world map, towns, and dungeons in Suikoden I.
  - **Native Input Triggers**:
    - **Gamepad**: Right Stick Up or R3 (`Pad.PadRRead()`).
    - **Keyboard**: `[F6]` hotkey.
  - **Native Delegate Loop Integration**:
    - Invokes `D_azukar_c.azukari_demo` directly within the native engine loop, preserving authentic UI windows, item icons, and deposit/withdraw mechanics.
  - **Zero-Delay Background Suppression**:
    - Suppresses player movement, camera shifts, and random wandering monster encounters while the storage window is active.
- **Configuration Options ([ModConfiguration.cs](ModConfiguration.cs))**:
  - `EnableQuickStorage` (default: `true`): Master toggle for the quick storage shortcut.
  - `RequireRock` (default: `false`): When enabled, requires Rock to be recruited and Toran Castle warehouse unlocked before vault access is permitted. When false, allows opening anytime without story or recruitment restrictions.
