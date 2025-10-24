using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelector : Popup
{
    #region SerializeFields
    [SerializeField] private CharacterContainer characterContainer;
    [SerializeField] private Transform characterParent;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI characterDescriptionText;
    [SerializeField] private TMP_InputField characterNameInputField;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button selectButton;
    [SerializeField] private Button changeHairColorButton;
    [SerializeField] private Button changeSkinColorButton;
    [SerializeField] private Button backButton;
    [SerializeField] private HairColorChanger hairColorChanger;
    [SerializeField] private SkinChanger skinChanger;
    [SerializeField] private GameObject loadingText;
    [SerializeField] private GameObject background;
    #endregion // SerializeField

    private List<GameObject> characters = new();
    private List<CharacterRoleSO> roles = new();

    private int selectedIndex = 0;

    public UnityEvent OnCharacterSelected { get; private set; } = new();
    public UnityEvent OnBackButtonClicked { get; private set; } = new();

    private void Start()
    {
        #region Listeners
        nextButton.onClick.AddListener(SelectNext);
        previousButton.onClick.AddListener(SelectPrevious);
        selectButton.onClick.AddListener(Select);
        changeHairColorButton.onClick.AddListener(HairChangeColor);
        changeSkinColorButton.onClick.AddListener(ChangeSkinColor);
        characterNameInputField.onValueChanged.AddListener(OnNameChanged);
        backButton.onClick.AddListener(() => OnBackButtonClicked.Invoke());
        #endregion // Listeners

        ClearCharacters();
        InstantiateCharacters();
        roles.AddRange(characterContainer.GetRoles());
        characters[selectedIndex].SetActive(true);
        SetCharacterName(roles[selectedIndex].GetName());
        SetDescription(roles[selectedIndex].GetDescription());
    }

    public void InstantiateCharacters()
    {
        foreach (var character in characterContainer.GetCharacters())
        {
            GameObject instance = Instantiate(character, characterParent);
            characters.Add(instance);
            instance.SetActive(false);
        }
    }

    public void ClearCharacters()
    {
        foreach (Transform character in characterParent)
        {
            Destroy(character.gameObject);
        }
        characters.Clear();
    }

    private void OnNameChanged(string name)
    {
        selectButton.gameObject.SetActive(!string.IsNullOrWhiteSpace(name));
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
        HideCharacter(selectedIndex);
        selectedIndex = (selectedIndex + 1) % characterContainer.Count;
        ShowCharacter(selectedIndex);
        SetCharacterName(roles[selectedIndex].GetName());
        SetDescription(roles[selectedIndex].GetDescription());
    }

    private void SelectPrevious()
    {
        HideCharacter(selectedIndex);
        selectedIndex = (selectedIndex - 1 + characterContainer.Count) % characterContainer.Count;
        ShowCharacter(selectedIndex);
        SetCharacterName(roles[selectedIndex].GetName());
        SetDescription(roles[selectedIndex].GetDescription());
    }

    private void Select() => SelectAsync().Forget();

    private async UniTaskVoid SelectAsync()
    {
        PlayerPrefs.SetString("CharacterName", characterNameInputField.text);
        PlayerPrefs.SetInt("SelectedCharacter", selectedIndex);

        OnCharacterSelected?.Invoke();

        await SceneManager.LoadSceneAsync("3DSci-fiScene");
        //CloseLoadingScreen();
    }

    private void SetCharacterName(string name) => characterNameText.text = name;
    private void SetDescription(string text) => characterDescriptionText.text = text;


    private void ShowCharacter(int index) => characters[index].SetActive(true);
    private void HideCharacter(int index) => characters[index].SetActive(false);
}
