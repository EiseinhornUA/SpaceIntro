using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillContainer : MonoBehaviour
{
    [SerializeField] private List<SkillSO> skillSos;
    private List<Skill> skills;

    public event Action OnSkillLevelChanged;

    private void Awake()
    {
        skills = skillSos.Select(skillSO => skillSO.AsSkill()).ToList();
    }
    private Skill GetSkill(SkillSO skill)
    {
        return skills.Find(s => s.skillName == skill.GetName());
    }

    public List<Skill> GetSkills() => new List<Skill>(skills);

    public void AddSkillLevel(SkillSO skill, float amount)
    {
        GetSkill(skill).level += amount;
        OnSkillLevelChanged?.Invoke();
    }

    public void AddSkillLevel(Skill skill)
    {
        Skill originalSkill = skills.Find(s => s.skillName == skill.skillName);
        originalSkill.level += skill.level;
        OnSkillLevelChanged?.Invoke();
    }

    public void SubtractSkillLevel(SkillSO skill, float amount)
    {
        GetSkill(skill).level -= amount;
        OnSkillLevelChanged?.Invoke();
    }

    [ContextMenu("Print Skills")]
    public void PrintSkills()
    {
        Debug.Log(string.Join('\n', skills.Select(skill => $"{skill.skillName}: {skill.level}")));
    }

    public void SetLoadedSkills(Dictionary<string, float> loadedSkills)
    {
        foreach (var skill in loadedSkills)
        {
            Skill existingSkill = this.skills.Find(s => s.skillName == skill.Key);
            if (existingSkill != null)
            {
                existingSkill.level = skill.Value;
            }
            else
            {
                Debug.LogError($"Skill '{skill.Key}' not found in SkillContainer.");
            }
        }
        OnSkillLevelChanged?.Invoke();
    }

    internal Dictionary<string, float> GetDictionarySkills()
    {
        return skills.ToDictionary(skill => skill.skillName, skill => skill.level);
    }
}
