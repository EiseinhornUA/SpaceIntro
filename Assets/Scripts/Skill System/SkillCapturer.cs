using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class SkillCapturer : MonoBehaviour
{
    [SerializeField] private List<Skill> capturedSkills;

    public void CaptureSkills(List<Skill> skills)
    {
        
        capturedSkills = new List<Skill>();
        foreach (Skill skill in skills)
        {
            capturedSkills.Add(new Skill(skill.skillName, skill.level));
        }
    }

    public List<Skill> GetSkills() => capturedSkills;
}