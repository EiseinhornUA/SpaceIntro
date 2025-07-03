using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterContainer", menuName = "ScriptableObjects/CharacterContainer", order = 1)]
public class CharacterContainer : ScriptableObject
{
    [SerializeField] private List<GameObject> characters = new List<GameObject>();
    public int Count => characters.Count;
    public GameObject GetCharacter(int index) => characters[index];
    public List<GameObject> GetCharacters() => characters;

}
