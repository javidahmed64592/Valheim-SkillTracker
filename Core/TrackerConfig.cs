using BepInEx.Configuration;

namespace SkillTracker.Core;

/// <summary>Personal display settings; nothing here affects gameplay, so none of it is synced.</summary>
internal static class TrackerConfig
{
    public static ConfigEntry<bool> ShowSkillTracker;
    public static ConfigEntry<bool> ShowXpDrops;
    public static ConfigEntry<float> TrackerFadeDelay;

    public static void Bind(ConfigFile config)
    {
        ShowSkillTracker = config.Bind("General", "ShowSkillTracker", true,
            "Shows the skills you earn experience in below the minimap, with their level and progress towards the next level.");
        ShowXpDrops = config.Bind("General", "ShowXpDrops", true,
            "Shows the experience each gain gives, rising beside the skill tracker.");
        TrackerFadeDelay = config.Bind("General", "FadeDelay", 6f,
            new ConfigDescription("Seconds a skill stays in the tracker after its last gain before fading out.",
                new AcceptableValueRange<float>(1f, 30f)));
    }
}
