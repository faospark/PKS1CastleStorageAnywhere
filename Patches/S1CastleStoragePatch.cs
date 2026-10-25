extern alias S1;

using System;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PKS1CastleStorageAnywhere.Patches;

/// <summary>
/// Suikoden I: Remote Castle Vault Access Patch (Party Item Storage Anywhere).
/// Solves Suikoden 1's lack of a shared party inventory bag by allowing players to open
/// Rock's Toran Castle Vault anywhere on the field via Right Stick Up, R3, or [F6].
/// Prevents items from being permanently locked out in benched characters' personal pockets
/// and eliminates tedious backtracking to Toran Castle just to manage inventory.
/// </summary>
public static class S1CastleStoragePatch
{
    public static bool IsQuickStorageOpen { get; private set; } = false;

    public static bool IsAnyQuickWindowOpen => IsQuickStorageOpen;

    public static bool IsInBattle { get; internal set; } = false;

    public static S1::Village_c CurrentVillage { get; private set; }
    public static S1::Fmain_c CurrentField { get; private set; }

    private static S1::SHIRO_FUNC_DELEGATE _activeStorageDelegate = null;
    private static float _lastToggleTime = 0f;
    private static bool _wasRightStickUpPrev = false;

    /// <summary>
    /// Checks if AppRoot/Map/MapBackGround/ has at least one child, ensuring Tir is loaded on the area.
    /// </summary>
    public static bool HasMapBackgroundChild()
    {
        try
        {
            var mapBg = GameObject.Find("AppRoot/Map/MapBackGround");
            return mapBg != null && mapBg.transform != null && mapBg.transform.childCount > 0;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Checks if the game is currently in a loading screen, scene change, or screen fade.
    /// </summary>
    public static bool IsGameInLoadingOrTransition(out string reason)
    {
        reason = null;

        try
        {
            // 1. Loading UI overlay
            var uiManager = global::UIManager.Instance;
            if (uiManager != null && uiManager.loading != null && uiManager.loading.gameObject.activeInHierarchy)
            {
                reason = "Loading screen is currently displayed";
                return true;
            }

            // 2. Active UILoadingAnim resource loading
            if (S1::UILoadingAnim.IsLoadingAnimeResources)
            {
                reason = "Loading animation resources active";
                return true;
            }

            // 3. Map background asset loading in village / field
            if (CurrentVillage != null && CurrentVillage.bg_manager != null && CurrentVillage.bg_manager.IsLoading)
            {
                reason = "Map background asset loading in progress";
                return true;
            }

            // 4. World map transition in progress (only valid when actively on world map)
            if (CurrentVillage == null && CurrentField != null)
            {
                if (CurrentField.loadVillage != 0 || CurrentField.field_exit_flg != 0)
                {
                    reason = "World map transition in progress";
                    return true;
                }
            }
        }
        catch { }

        return false;
    }

    /// <summary>
    /// Per-frame update called on the Unity main thread via Plugin.Update().
    /// </summary>
    public static void Update()
    {
        if (CurrentVillage == null && CurrentField == null)
            return;

        if (Plugin.Config == null || !Plugin.Config.EnableQuickStorage.Value)
            return;

        // Check if Right Stick Up, R3 (Right Stick Press), or [F6] was pressed
        bool triggerPressed = false;
        try
        {
            var pad = Gamepad.current;
            if (pad != null)
            {
                // Right stick up detection (analog threshold + edge detection)
                float stickY = pad.rightStick.y.ReadValue();
                bool isStickUp = stickY > 0.65f;
                bool stickUpPressed = isStickUp && !_wasRightStickUpPrev;
                _wasRightStickUpPrev = isStickUp;

                if (stickUpPressed || pad.rightStickButton.wasPressedThisFrame)
                {
                    triggerPressed = true;
                }
            }
            else
            {
                _wasRightStickUpPrev = false;
            }

            var kb = Keyboard.current;
            if (kb != null && kb.f6Key.wasPressedThisFrame)
            {
                triggerPressed = true;
            }
        }
        catch { }

        // If storage is currently active, advance the native state machine
        if (IsQuickStorageOpen)
        {
            // Allow pressing trigger again to close early
            if (triggerPressed && Time.time - _lastToggleTime > 0.4f)
            {
                _lastToggleTime = Time.time;
                CloseStorageSafely();
                return;
            }

            StepStorageLoop();
            return;
        }

        // Attempt to open storage on trigger press
        if (triggerPressed && Time.time - _lastToggleTime > 0.4f)
        {
            _lastToggleTime = Time.time;

            if (CanOpenStorage(out string failReason))
            {
                OpenStorage();
            }
            else
            {
                Plugin.Log.LogInfo($"[S1CastleStorage] Cannot open vault right now: {failReason}");
            }
        }
    }

    /// <summary>
    /// Comprehensive safety checks to guarantee immunity to softlocks, crashes, and narrative breaks.
    /// </summary>
    public static bool CanOpenStorage(out string failReason)
    {
        failReason = null;

        // 0. Ensure map background has loaded Tir on the area
        if (!HasMapBackgroundChild())
        {
            failReason = "Map background has not loaded Tir on the area";
            return false;
        }

        // 1. Loading screen / scene transition guard
        if (IsGameInLoadingOrTransition(out failReason))
        {
            return false;
        }

        var gw = S1::OldSrcBase.game_work;
        if (gw == null)
        {
            failReason = "Game session not active";
            return false;
        }

        // Ensure castle data and warehouse buffer are present
        var shiro = gw.shiro_data;
        if (shiro == null)
        {
            shiro = new S1::SHIRO_DATA();
            gw.shiro_data = shiro;
        }
        if (shiro.azukari == null)
        {
            shiro.azukari = new Il2CppStructArray<byte>(60);
        }
        if (shiro.azukari_kantei == null)
        {
            shiro.azukari_kantei = new Il2CppReferenceArray<Il2CppStructArray<byte>>(60);
        }

        // Rock recruitment requirement
        if (Plugin.Config.RequireRock.Value)
        {
            if (shiro.level <= 0)
            {
                failReason = "Rock is not yet recruited (Toran Castle vault locked)";
                return false;
            }
        }

        // Rejection: In Battle
        if (IsInBattle)
        {
            failReason = "Currently in battle";
            return false;
        }

        // Rejection: In Village during an active cutscene, event dialogue, or transition
        if (CurrentVillage != null)
        {
            try
            {
                if (!CurrentVillage.isEnableOpenMenu)
                {
                    failReason = "Scene is currently busy with event, dialog, or cutscene";
                    return false;
                }
            }
            catch { }
        }

        // Rejection: WindowManager uninitialized
        if (S1::WindowManager.Instance == null)
        {
            failReason = "UI WindowManager is not available";
            return false;
        }

        return true;
    }

    /// <summary>
    /// Opens the native HD Remaster storage UI.
    /// </summary>
    private static void OpenStorage()
    {
        try
        {
            Plugin.Log.LogInfo("[S1CastleStorage] Invoking Rock's Castle Vault via Right Stick Up...");

            var initResult = S1::D_azukar_c.azukari_demo();
            if (initResult.Item1 && initResult.Item2 != null)
            {
                _activeStorageDelegate = initResult.Item2;
                IsQuickStorageOpen = true;
                SuppressRockBackground();
                PlaySoundOpen();
                Plugin.Log.LogInfo("[S1CastleStorage] Vault opened successfully.");
            }
            else
            {
                Plugin.Log.LogWarning("[S1CastleStorage] azukari_demo() returned inactive state.");
                IsQuickStorageOpen = false;
                _activeStorageDelegate = null;
            }
        }
        catch (Exception ex)
        {
            Plugin.Log.LogError($"[S1CastleStorage] Exception opening storage: {ex}");
            IsQuickStorageOpen = false;
            _activeStorageDelegate = null;
        }
    }

    /// <summary>
    /// Steps the native storage frame delegate each Unity update.
    /// </summary>
    private static void StepStorageLoop()
    {
        if (_activeStorageDelegate == null)
        {
            CloseStorageSafely();
            return;
        }

        try
        {
            var result = _activeStorageDelegate.Invoke();
            if (result.Item1 && result.Item2 != null)
            {
                _activeStorageDelegate = result.Item2;
            }
            else
            {
                // Native storage routine finished cleanly (user pressed Back/Cancel)
                Plugin.Log.LogInfo("[S1CastleStorage] Storage session completed cleanly.");
                PlaySoundClose();
                IsQuickStorageOpen = false;
                _activeStorageDelegate = null;
            }
        }
        catch (Exception ex)
        {
            Plugin.Log.LogError($"[S1CastleStorage] Exception in storage step: {ex}");
            CloseStorageSafely();
        }
    }

    /// <summary>
    /// Hides Rock's warehouse background image so the current field stays visible behind the vault UI.
    /// </summary>
    private static void SuppressRockBackground()
    {
        try
        {
            var eventC = S1::Village_c.event_c;
            if (eventC != null)
            {
                var uiAzukari = eventC.uiAzukari;
                if (uiAzukari != null && uiAzukari.bg_obj != null)
                {
                    uiAzukari.bg_obj.enabled = false;
                    if (uiAzukari.bg_obj.gameObject != null)
                    {
                        uiAzukari.bg_obj.gameObject.SetActive(false);
                    }
                }
            }
        }
        catch { }
    }

    /// <summary>
    /// Safely forces the storage window to close if interrupted or toggled off.
    /// </summary>
    public static void CloseStorageSafely()
    {
        if (IsQuickStorageOpen)
        {
            PlaySoundClose();
        }

        try
        {
            S1::D_azukar_c.demo_exit();
        }
        catch { }

        try
        {
            if (S1::WindowManager.Instance != null)
            {
                S1::WindowManager.Instance.CloseStorageWindowUI(
                    Share.UI.Window.StorageMenuType.Deposit,
                    Share.UI.Window.StorageWindowType.Command);
            }
        }
        catch { }

        IsQuickStorageOpen = false;
        _activeStorageDelegate = null;
        Plugin.Log.LogInfo("[S1CastleStorage] Storage closed and player control restored.");
    }

    /// <summary>
    /// Plays the authentic Suikoden 1 menu open chime.
    /// </summary>
    public static void PlaySoundOpen()
    {
        try
        {
            int seId = S1::G_cmd_c.SE_OK;
            if (seId <= 0) seId = S1::D_azukar_c.SE_OK;
            if (seId <= 0) seId = 1;
            S1::P_sound_c.Qsd_call(seId);
        }
        catch { }
    }

    /// <summary>
    /// Plays the authentic Suikoden 1 menu cancel/close chime.
    /// </summary>
    public static void PlaySoundClose()
    {
        try
        {
            int seId = S1::G_cmd_c.SE_CANCEL;
            if (seId <= 0) seId = S1::D_azukar_c.SE_CANCEL;
            if (seId <= 0) seId = 2;
            S1::P_sound_c.Qsd_call(seId);
        }
        catch { }
    }

    // =========================================================================
    // HARMONY HOOKS (Player Control, Movement & Encounter Guards)
    // =========================================================================

    [HarmonyPatch(typeof(S1::Village_c), nameof(S1::Village_c.village_main))]
    [HarmonyPrefix]
    public static void Village_village_main_Prefix(S1::Village_c __instance)
    {
        CurrentVillage = __instance;
        CurrentField = null;
        IsInBattle = false;
    }

    [HarmonyPatch(typeof(S1::Fmain_c), nameof(S1::Fmain_c.fieldMain))]
    [HarmonyPrefix]
    public static void Fmain_fieldMain_Prefix(S1::Fmain_c __instance)
    {
        CurrentField = __instance;
        CurrentVillage = null;
        IsInBattle = false;
    }

    // --- Battle State Tracking ---

    [HarmonyPatch(typeof(S1::BattleChapter), nameof(S1::BattleChapter.Start))]
    [HarmonyPrefix]
    public static void BattleChapter_Start_Prefix()
    {
        IsInBattle = true;
    }

    [HarmonyPatch(typeof(S1::BattleChapter), nameof(S1::BattleChapter.BattleEnd))]
    [HarmonyPostfix]
    public static void BattleChapter_BattleEnd_Postfix()
    {
        IsInBattle = false;
    }

    [HarmonyPatch(typeof(S1::BattleChapter), nameof(S1::BattleChapter.OnDestroy))]
    [HarmonyPostfix]
    public static void BattleChapter_OnDestroy_Postfix()
    {
        IsInBattle = false;
    }

    // --- Movement Blockers (Towns, Dungeons, Castles) ---

    [HarmonyPatch(typeof(S1::Village_c), nameof(S1::Village_c.NewPlayerMove))]
    [HarmonyPrefix]
    public static bool Village_NewPlayerMove_Prefix()
    {
        return !IsAnyQuickWindowOpen;
    }

    [HarmonyPatch(typeof(S1::Village_c), nameof(S1::Village_c.OldPlayerMove))]
    [HarmonyPrefix]
    public static bool Village_OldPlayerMove_Prefix()
    {
        return !IsAnyQuickWindowOpen;
    }

    [HarmonyPatch(typeof(S1::Village_c), nameof(S1::Village_c.playerMoveBase1))]
    [HarmonyPrefix]
    public static bool Village_playerMoveBase1_Prefix()
    {
        return !IsAnyQuickWindowOpen;
    }

    [HarmonyPatch(typeof(S1::Village_c), nameof(S1::Village_c.playerMoveBase2))]
    [HarmonyPrefix]
    public static bool Village_playerMoveBase2_Prefix()
    {
        return !IsAnyQuickWindowOpen;
    }

    [HarmonyPatch(typeof(S1::Village_c), nameof(S1::Village_c.playerMoveBase3))]
    [HarmonyPrefix]
    public static bool Village_playerMoveBase3_Prefix()
    {
        return !IsAnyQuickWindowOpen;
    }

    [HarmonyPatch(typeof(S1::Village_c), nameof(S1::Village_c.player_move))]
    [HarmonyPrefix]
    public static bool Village_player_move_Prefix(ref byte __result)
    {
        if (IsAnyQuickWindowOpen)
        {
            __result = 0;
            return false;
        }
        return true;
    }

    [HarmonyPatch(typeof(S1::Village_c), nameof(S1::Village_c.CheckArrivedEvent))]
    [HarmonyPrefix]
    public static bool Village_CheckArrivedEvent_Prefix(ref bool __result)
    {
        if (IsAnyQuickWindowOpen)
        {
            __result = false;
            return false;
        }
        return true;
    }

    [HarmonyPatch(typeof(S1::Village_c), nameof(S1::Village_c.playerOP_move))]
    [HarmonyPrefix]
    public static bool Village_playerOP_move_Prefix()
    {
        return !IsAnyQuickWindowOpen;
    }

    // --- Movement & Input Blockers (World Map) ---

    [HarmonyPatch(typeof(S1::Fmain_c), nameof(S1::Fmain_c.userMove))]
    [HarmonyPrefix]
    public static bool Fmain_userMove_Prefix()
    {
        return !IsAnyQuickWindowOpen;
    }

    [HarmonyPatch(typeof(S1::Fmain_c), nameof(S1::Fmain_c.userMoveSet))]
    [HarmonyPrefix]
    public static bool Fmain_userMoveSet_Prefix()
    {
        return !IsAnyQuickWindowOpen;
    }

    [HarmonyPatch(typeof(S1::Fmain_c), nameof(S1::Fmain_c.padReaction))]
    [HarmonyPrefix]
    public static bool Fmain_padReaction_Prefix()
    {
        return !IsAnyQuickWindowOpen;
    }

    // --- Encounter Lockouts ---

    [HarmonyPatch(typeof(S1::Village_c), nameof(S1::Village_c.v_wanderingMonsterChk))]
    [HarmonyPrefix]
    public static bool Village_MonsterChk_Prefix(ref int __result)
    {
        if (IsAnyQuickWindowOpen)
        {
            __result = 0;
            return false;
        }
        return true;
    }

    [HarmonyPatch(typeof(S1::Fmain_c), nameof(S1::Fmain_c.f_wanderingMonsterChk))]
    [HarmonyPrefix]
    public static bool Fmain_MonsterChk_Prefix(ref int __result)
    {
        if (IsAnyQuickWindowOpen)
        {
            __result = 0;
            return false;
        }
        return true;
    }

    // --- Rock Background Image Suppression & Instant Opening ---

    [HarmonyPatch(typeof(S1::D_azukar_c.UIAzukari), nameof(S1::D_azukar_c.UIAzukari.Initialize))]
    [HarmonyPrefix]
    public static bool UIAzukari_Initialize_Prefix(S1::D_azukar_c.UIAzukari __instance, ref Il2CppSystem.Collections.IEnumerator __result)
    {
        if (IsQuickStorageOpen)
        {
            if (__instance != null)
            {
                __instance.is_complete_initialize = true;
            }
            __result = null;
            return false; // Skip async background image loading entirely for zero-delay opening
        }
        return true;
    }

    [HarmonyPatch(typeof(S1::D_azukar_c.UIAzukari), nameof(S1::D_azukar_c.UIAzukari.IsCompleteInitialize))]
    [HarmonyPrefix]
    public static bool UIAzukari_IsCompleteInitialize_Prefix(ref bool __result)
    {
        if (IsQuickStorageOpen)
        {
            __result = true;
            return false; // Ready instantly on frame 0
        }
        return true;
    }

    [HarmonyPatch(typeof(S1::D_azukar_c.UIAzukari), nameof(S1::D_azukar_c.UIAzukari.Terminate))]
    [HarmonyPrefix]
    public static bool UIAzukari_Terminate_Prefix(S1::D_azukar_c.UIAzukari __instance)
    {
        if (IsQuickStorageOpen)
        {
            if (__instance != null)
            {
                __instance.is_complete_initialize = false;
            }
            return false;
        }
        return true;
    }

    [HarmonyPatch(typeof(S1::D_azukar_c.UIAzukari), nameof(S1::D_azukar_c.UIAzukari.SetFadeBright))]
    [HarmonyPrefix]
    public static bool UIAzukari_SetFadeBright_Prefix(S1::D_azukar_c.UIAzukari __instance)
    {
        if (IsQuickStorageOpen)
        {
            try
            {
                if (__instance != null && __instance.bg_obj != null)
                {
                    __instance.bg_obj.enabled = false;
                    if (__instance.bg_obj.gameObject != null)
                        __instance.bg_obj.gameObject.SetActive(false);
                }
            }
            catch { }
            return false;
        }
        return true;
    }
}

