using System;
using System.Collections;
using UnityEngine;

public class HairColorChanger : MonoBehaviour
{
    [SerializeField] private Material hairMaterial;
    [SerializeField] private ColorContainerSO hairColorContainer;
    private int currentIndex;

    private void Start()
    {
        ResetColor();
    }

    private void ResetColor()
    {
        hairMaterial.color = hairColorContainer.GetColor(0);
    }

    internal void ChangeColor()
    {
        currentIndex = (currentIndex + 1) % hairColorContainer.colors.Count;
        hairMaterial.color = hairColorContainer.GetColor(currentIndex);
    }
}
