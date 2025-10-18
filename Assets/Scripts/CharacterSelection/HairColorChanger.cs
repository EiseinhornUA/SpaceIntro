using System;
using System.Collections;
using UnityEngine;

public class HairColorChanger : MonoBehaviour
{
    [SerializeField] private Material hairMaterial;
    [SerializeField] private ColorContainerSO hairColorContainer;
    private int currentIndex;

    private const string HairColorKey = "HairColor";

    private void Start()
    {
        LoadColor();
    }

    internal void ChangeColor()
    {
        currentIndex = (currentIndex + 1) % hairColorContainer.colors.Count;
        hairMaterial.color = hairColorContainer.GetColor(currentIndex);
        SaveColor(currentIndex);
    }

    private void SaveColor(int index)
    {
        PlayerPrefs.SetInt(HairColorKey, index);
        PlayerPrefs.Save();
    }

    public void LoadColor()
    {
        int index = PlayerPrefs.GetInt(HairColorKey, 0);
        hairMaterial.color = hairColorContainer.GetColor(index);
    }
}
