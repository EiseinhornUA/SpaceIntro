using System.Collections;
using UnityEngine;

public class SkinChanger : MonoBehaviour
{
    [SerializeField] private Material skinMaterial;
    [SerializeField] private Material beardMaterial;
    [SerializeField] private Material lipsMaterial;
    [SerializeField] private SkinContainerSO skinContainer;
    private int currentIndex;

    internal void ChangeColor()
    {
        currentIndex = (currentIndex + 1) % skinContainer.skins.Count;

        skinMaterial.color = skinContainer.GetSkin(currentIndex).skinColor;
        beardMaterial.color = skinContainer.GetSkin(currentIndex).beardColor;
        lipsMaterial.color = skinContainer.GetSkin(currentIndex).lipsColor;
    }
}
