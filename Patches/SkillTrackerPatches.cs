using HarmonyLib;

using SkillTracker.Tracker;

namespace SkillTracker.Patches;

[HarmonyPatch(typeof(Hud), nameof(Hud.Awake))]
internal static class Hud_Awake_TrackerPatch
{
    private static void Postfix(Hud __instance)
    {
        TrackerPanel.Create(__instance);
    }
}

/// <summary>Every skill gain goes through here, including Dodge, which skips <c>Player.RaiseSkill</c>.</summary>
[HarmonyPatch(typeof(Skills), nameof(Skills.RaiseSkill))]
internal static class Skills_RaiseSkill_TrackerPatch
{
    private static void Prefix(Skills __instance, Skills.SkillType skillType, out float __state)
    {
        __state = skillType == Skills.SkillType.None ? -1f : __instance.GetSkill(skillType).m_level;
    }

    private static void Postfix(Skills __instance, Skills.SkillType skillType, float factor, float __state)
    {
        SkillGain.OnRaised(__instance, skillType, factor, __state);
    }
}
