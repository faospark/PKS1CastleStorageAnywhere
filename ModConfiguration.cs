using BepInEx.Configuration;

namespace PKS1CastleStorageAnywhere;

public sealed class ModConfiguration
{
    private readonly ConfigFile _config;

    // Suikoden 1 Castle Storage Anywhere Settings
    public ConfigEntry<bool> EnableQuickStorage { get; private set; }
    public ConfigEntry<bool> RequireRock { get; private set; }

    public ModConfiguration(ConfigFile config)
    {
        _config = config;
    }

    public void Init()
    {
        EnableQuickStorage = _config.Bind(
            "Suikoden 1 Castle Storage",
            "EnableQuickStorage",
            true,
            "Press Right Stick Up (or R3 / [F6] on keyboard) to open Rock's Castle Vault directly from anywhere on the field."
        );

        RequireRock = _config.Bind(
            "Suikoden 1 Castle Storage",
            "RequireRock",
            false,
            "When enabled, Castle Vault requires Rock to be recruited (Toran Castle warehouse unlocked) before it can be accessed. Set to false to allow opening anytime without recruitment or castle restrictions."
        );
    }
}

