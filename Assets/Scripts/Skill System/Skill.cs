using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skill
{

    [field: SerializeField] public string skillName { get; set; }
    [field: SerializeField] public float level { get; set; }

    public Skill(SkillSO skillSO)
    {
        skillName = skillSO.GetName();
        level = skillSO.GetLevel();
    }

    public Skill(string skillName, float level)
    {
        this.skillName = skillName;
        this.level = level;
    }
}