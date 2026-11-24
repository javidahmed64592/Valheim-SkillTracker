namespace SkillTracker.Tracker;

/// <summary>Works out how much experience a vanilla <c>Skills.RaiseSkill</c> call gave and passes it to the tracker.</summary>
internal static class SkillGain
{
    /// <summary>Called after vanilla raised a skill.</summary>
    /// <param name="skills">The skills that were raised.</param>
    /// <param name="type">The skill raised.</param>
    /// <param name="factor">The raise factor, after status effect modifiers.</param>
    /// <param name="levelBefore">The skill's level before the raise, or -1 if nothing was raised.</param>
    public static void OnRaised(Skills skills, Skills.SkillType type, float factor, float levelBefore)
    {
        if (levelBefore < 0f || levelBefore >= 100f || skills.m_player == null || skills.m_player != Player.m_localPlayer)
        {
            return;
        }

        Skills.Skill skill = skills.GetSkill(type);
        float gained = skill.m_info.m_increseStep * factor * Game.m_skillGainRate;
        TrackerPanel.OnGain(skill, gained, skill.m_level > levelBefore);
    }
}
