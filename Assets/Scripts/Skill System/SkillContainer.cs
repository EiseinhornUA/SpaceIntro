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

    private void Start()
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
}
