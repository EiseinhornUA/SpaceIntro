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

    private const string SkinColorKey = "SkinColor";

    private void Start()
    {
        LoadSkin();
    }

    internal void ChangeColor()
    {
        currentIndex = (currentIndex + 1) % skinContainer.skins.Count;

        skinMaterial.color = skinContainer.GetSkin(currentIndex).skinColor;
        beardMaterial.color = skinContainer.GetSkin(currentIndex).beardColor;
        lipsMaterial.color = skinContainer.GetSkin(currentIndex).lipsColor;
        SaveSkin(currentIndex);
    }

    private void SaveSkin(int index)
    {
        PlayerPrefs.SetInt(SkinColorKey, index);
        PlayerPrefs.Save();
    }

    public void LoadSkin()
    {
        int index = PlayerPrefs.GetInt(SkinColorKey, 0);
        skinMaterial.color = skinContainer.GetSkin(index).skinColor;
        beardMaterial.color = skinContainer.GetSkin(index).beardColor;
        lipsMaterial.color = skinContainer.GetSkin(index).lipsColor;
    }
}
