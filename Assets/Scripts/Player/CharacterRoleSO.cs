using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterRole", menuName = "ScriptableObjects/CharacterRole", order = 1)]
public class CharacterRoleSO : ScriptableObject
{
    [TextArea(3, 10)]
    [SerializeField] private string description;
    [SerializeField] private List<SkillSOLevelPair> skills;
    public List<Skill> GetInitialSkills()
    {
        return skills.ConvertAll(s => new Skill(s.skillSO.GetName(), s.level));
    }
    public string GetName() => name;
    public string GetDescription() => description;
}

[System.Serializable]
public class SkillSOLevelPair
{
    public SkillSO skillSO;
    public float level;
}