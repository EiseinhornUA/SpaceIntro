using System;
using System.Collections;
using UnityEngine;

public class SkinChanger : MonoBehaviour
{
    [SerializeField] private Material skinMaterial;
    [SerializeField] private Material beardMaterial;
    [SerializeField] private Material lipsMaterial;
    [SerializeField] private SkinContainerSO skinContainer;
    private int currentIndex;

    private void Start()
    {
        ResetSkin();
    }

    private void ResetSkin()
    {
        skinMaterial.color = skinContainer.GetSkin(0).skinColor;
        beardMaterial.color = skinContainer.GetSkin(0).beardColor;
        lipsMaterial.color = skinContainer.GetSkin(0).lipsColor;
    }

    internal void ChangeColor()
    {
        currentIndex = (currentIndex + 1) % skinContainer.skins.Count;

        skinMaterial.color = skinContainer.GetSkin(currentIndex).skinColor;
        beardMaterial.color = skinContainer.GetSkin(currentIndex).beardColor;
        lipsMaterial.color = skinContainer.GetSkin(currentIndex).lipsColor;
    }
}
