using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillContainer : MonoBehaviour
{
    [SerializeField] private List<SkillSO> skillSos;
    private List<Skill> skills;

    public event Action<SkillSO, int> OnSkillLevelAdded;

    private void Start()
    {
        skills = skillSos.Select(skillSO => skillSO.AsSkill()).ToList();
    }
    private Skill GetSkill(SkillSO skill)
    {
        return skills.Find(s => s.skillName == skill.GetName());
    }

    public List<Skill> GetSkills() => skills;

    public void AddSkillLevel(SkillSO skill, int amount)
    {
        GetSkill(skill).level += amount;
        OnSkillLevelAdded?.Invoke(skill, amount);
    }

    public void SubtractSkillLevel(SkillSO skill, int amount)
    {
        GetSkill(skill).level -= amount;
    }

    [ContextMenu("Print Skills")]
    public void PrintSkills()
    {

        Debug.Log(string.Join('\n', skills.Select(skill => $"{skill.skillName}: {skill.level}")));
        //foreach (var skill in skills)
        //{
        //    Debug.Log($"Skill: {skill.skillName}, Level: {skill.level}");
        //}
    }
}