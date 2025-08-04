using System.Collections;
using UnityEngine;

public class HairColorChanger : MonoBehaviour
{
    [SerializeField] private Material hairMaterial;
    [SerializeField] private ColorContainerSO hairColorContainer;
    private int currentIndex;

    internal void ChangeColor()
    {
        currentIndex = (currentIndex + 1) % hairColorContainer.colors.Count;
        hairMaterial.color = hairColorContainer.GetColor(currentIndex);
    }
}
