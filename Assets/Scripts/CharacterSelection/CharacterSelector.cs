using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelector : MonoBehaviour
{
    #region SerializeFields
    [SerializeField] private CharacterContainer characterContainer;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI characterDescriptionText;
    [SerializeField] private TMP_InputField characterNameInputField;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button selectButton;
    [SerializeField] private Button changeHairColorButton;
    [SerializeField] private Button changeSkinColorButton;
    [SerializeField] private HairColorChanger hairColorChanger;
    [SerializeField] private SkinChanger skinChanger;
    [SerializeField] private GameObject loadingText;
    [SerializeField] private GameObject background;
    #endregion // SerializeField

    private List<GameObject> characters = new();
    private List<CharacterRoleSO> roles = new();

    private int selectedIndex = 0;

    private void Start()
    {
        nextButton.onClick.AddListener(SelectNext);
        previousButton.onClick.AddListener(SelectPrevious);
        selectButton.onClick.AddListener(Select);
        changeHairColorButton.onClick.AddListener(HairChangeColor);
        changeSkinColorButton.onClick.AddListener(ChangeSkinColor);

        foreach (var character in characterContainer.GetCharacters())
        {
            GameObject instance = Instantiate(character, transform);
            characters.Add(instance);
            instance.SetActive(false);
        }
        roles.AddRange(characterContainer.GetRoles());
        characters[selectedIndex].SetActive(true);
        SetCharacterName(roles[selectedIndex].GetName());
        SetDescription(roles[selectedIndex].GetDescription());
    }

    private void Update()
    {
        if (IsNameEmpty())
        {
            selectButton.gameObject.SetActive(false);
            return;
        }

        selectButton.gameObject.SetActive(true);
    }

    private bool IsNameEmpty()
    {
        return string.IsNullOrWhiteSpace(characterNameInputField.text);
    }

    private void HairChangeColor()
    {
        hairColorChanger.ChangeColor();
    }

    private void ChangeSkinColor()
    {
        skinChanger.ChangeColor();
    }

    private void SelectNext()
    {
        Hide(selectedIndex);
        selectedIndex = (selectedIndex + 1) % characterContainer.Count;
        Show(selectedIndex);
        SetCharacterName(roles[selectedIndex].GetName());
        SetDescription(roles[selectedIndex].GetDescription());
    }

    private void SelectPrevious()
    {
        Hide(selectedIndex);
        selectedIndex = (selectedIndex - 1 + characterContainer.Count) % characterContainer.Count;
        Show(selectedIndex);
        SetCharacterName(roles[selectedIndex].GetName());
        SetDescription(roles[selectedIndex].GetDescription());
    }

    private void Select() => SelectAsync().Forget();

    private async UniTaskVoid SelectAsync()
    {
        PlayerPrefs.SetString("CharacterName", characterNameInputField.text);
        PlayerPrefs.SetInt("SelectedCharacter", selectedIndex);

        FindObjectOfType<PortraitSaver>().SaveImage();
        ShowLoadingScreen();
        await SceneManager.LoadSceneAsync("3DSci-fiScene");
        //CloseLoadingScreen();
    }

    private void ShowLoadingScreen()
    {
        loadingText.SetActive(true);
        background.SetActive(true);
    }

    private void CloseLoadingScreen()
    {
        loadingText.SetActive(false);
        background.SetActive(false);
    }

    private void SetCharacterName(string name) => characterNameText.text = name;
    private void SetDescription(string text) => characterDescriptionText.text = text;


    private void Show(int index) => characters[index].SetActive(true);
    private void Hide(int index) => characters[index].SetActive(false);
}
