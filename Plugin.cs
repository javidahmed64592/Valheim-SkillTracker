using BepInEx;

using HarmonyLib;

using Jotunn.Utils;

using SkillTracker.Core;

namespace SkillTracker;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
[BepInDependency(Jotunn.Main.ModGuid)]
[NetworkCompatibility(CompatibilityLevel.NotEnforced, VersionStrictness.None)]
public class SkillTrackerPlugin : BaseUnityPlugin
{
    public const string PluginGUID = "javidahmed64592.skilltracker";
    public const string PluginName = "Skill Tracker";
    public const string PluginVersion = "0.1.1";

    private Harmony _harmony;

    private void Awake()
    {
        TrackerConfig.Bind(Config);
        _harmony = new Harmony(PluginGUID);
        _harmony.PatchAll();
    }

    private void OnDestroy()
    {
        _harmony?.UnpatchSelf();
    }
}
