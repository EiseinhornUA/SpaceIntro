using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "CharacterContainer", menuName = "ScriptableObjects/CharacterContainer", order = 1)]
public class CharacterContainer : ScriptableObject
{
    [SerializeField] private List<CharacterEntry> characters = new();
    public int Count => characters.Count;
    public GameObject GetCharacter(int index) => characters[index].character;
    public List<GameObject> GetCharacters() => characters.Select(c => c.character).ToList();
    public List<CharacterRoleSO> GetRoles() => characters.Select(c => c.role).ToList();

}

[Serializable]
public class CharacterEntry
{
    public GameObject character;
    public CharacterRoleSO role;
}