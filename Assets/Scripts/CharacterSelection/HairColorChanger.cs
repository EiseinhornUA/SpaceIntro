using System.Collections;
using UnityEngine;

public class HairColorChanger : MonoBehaviour
{
    [SerializeField] private Material hairMaterial;
    [SerializeField] private HairColorContainerSO colorContainer;
    private int currentColorIndex;

    internal void SetColor(Color color) => hairMaterial.color = color;
    
    internal void ChangeColor()
    {
        currentColorIndex = (currentColorIndex + 1) % colorContainer.colors.Count;
        SetColor(colorContainer.GetColor(currentColorIndex));
    }
}
