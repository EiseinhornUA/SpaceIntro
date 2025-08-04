using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterColors", menuName = "ScriptableObjects/CharacterSelection/CharacterColors")]
public class ColorContainerSO : ScriptableObject
{
    [field: SerializeField] public List<Color> colors {  get; private set; }
    public Color GetColor(int index) => colors[index];
}