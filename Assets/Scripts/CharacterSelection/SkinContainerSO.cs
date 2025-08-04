using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkinContainer", menuName = "ScriptableObjects/CharacterSelection/SkinContainer")]
public class SkinContainerSO : ScriptableObject
{
    [field: SerializeField] public List<Skin> skins {  get; private set; }

    public Skin GetSkin(int index) => skins[index];
}