using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "ScriptableObjects/Skill")]
public class SkillSO : ScriptableObject
{
    [SerializeField] private float minLevel;
    [SerializeField] private float maxLevel;

    public string GetName() => name;
    [SerializeField] private int level;
    public int GetLevel() => level;
    public void AddLevel(int amount) => level += amount;
    public void SubtractLevel(int amount) => level -= amount;
    public void ResetLevel() => level = 0;

    public float GetMinLevel() => minLevel;
    public float GetMaxLevel() => maxLevel;
}
