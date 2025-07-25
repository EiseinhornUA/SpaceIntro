using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skill
{

    [field: SerializeField] public string skillName { get; set; }
    [field: SerializeField] public int level { get; set; }

    public Skill(SkillSO skillSO)
    {
        skillName = skillSO.GetName();
        level = skillSO.GetLevel();
    }
}