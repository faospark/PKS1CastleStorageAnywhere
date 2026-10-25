using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using PKS1CastleStorageAnywhere.Patches;
using UnityEngine;

namespace PKS1CastleStorageAnywhere;

[BepInPlugin("faospark.pks1castlestorageanywhere", "PKS1 Castle Storage Anywhere", "1.0.0")]
[BepInDependency("faospark.pkcore", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("d3xMachina.suikoden_fix", BepInDependency.DependencyFlags.SoftDependency)]
public class Plugin : BasePlugin
{
    public static Plugin Instance { get; private set; }
    public static new ManualLogSource Log;
    public static new ModConfiguration Config;

    public override void Load()
    {
        Instance = this;
        Log = base.Log;

        Log.LogInfo("Loading PKS1CastleStorageAnywhere (Toran Castle Vault Access Anywhere)...");

        Config = new ModConfiguration(base.Config);
        Config.Init();

        // Register custom MonoBehaviour to drive the Unity Update loop
        try
        {
            Il2CppInterop.Runtime.Injection.ClassInjector.RegisterTypeInIl2Cpp<PKS1CastleStorageMainLoop>();

            var obj = new GameObject("PKS1CastleStorageMainLoop");
            GameObject.DontDestroyOnLoad(obj);
            var component = obj.AddComponent<PKS1CastleStorageMainLoop>();
            obj.hideFlags = HideFlags.HideAndDontSave;

            PKS1CastleStorageMainLoop.Instance = component;

            Log.LogInfo("Initialized PKS1CastleStorageMainLoop for Update events.");
        }
        catch (System.Exception ex)
        {
            Log.LogError($"Failed to register PKS1CastleStorageMainLoop: {ex.Message}");
        }

        if (Config.EnableQuickStorage.Value)
        {
            var harmony = new Harmony("faospark.pks1castlestorageanywhere");
            harmony.PatchAll(typeof(S1CastleStoragePatch));
            Log.LogInfo("PKS1CastleStorageAnywhere patches applied successfully!");
        }

        Log.LogInfo("PKS1CastleStorageAnywhere loaded successfully!");
    }

    public void Update()
    {
        if (Config.EnableQuickStorage.Value)
        {
            S1CastleStoragePatch.Update();
        }
    }
}

/// <summary>
/// Helper MonoBehaviour to drive the Unity Update loop for PKS1CastleStorageAnywhere.
/// </summary>
public class PKS1CastleStorageMainLoop : MonoBehaviour
{
    public static PKS1CastleStorageMainLoop Instance { get; set; }

    public void Update()
    {
        if (Plugin.Instance != null)
        {
            Plugin.Instance.Update();
        }
    }
}

