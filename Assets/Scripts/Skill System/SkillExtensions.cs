public static class SkillExtensions
{
    public static Skill AsSkill(this SkillSO skillSO)
    {
        return new Skill(skillSO);
    }
}