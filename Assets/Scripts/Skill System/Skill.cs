using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public string skillName { get; set; }
    public int level { get; set; }

    public Skill(SkillSO skillSO)
    {
        skillName = skillSO.GetName();
        level = skillSO.GetLevel();
    }
}