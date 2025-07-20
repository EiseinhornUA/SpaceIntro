using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterRole", menuName = "ScriptableObjects/CharacterRole", order = 1)]
public class CharacterRoleSO : ScriptableObject
{
    [TextArea(3, 10)]
    [SerializeField] private string description;
    public string GetName() => name;
    public string GetDescription() => description;
}
