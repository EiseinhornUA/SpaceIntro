using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HairColors", menuName = "ScriptableObjects/CharacterSelection/HairColors")]
public class HairColorContainerSO : ScriptableObject
{
    [field: SerializeField] public List<Color> colors {  get; private set; }

    public Color GetColor(int index) => colors[index];
}