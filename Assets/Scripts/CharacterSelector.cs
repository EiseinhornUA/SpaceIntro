using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelector : MonoBehaviour
{
    [SerializeField] private CharacterContainer characterContainer;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button selectButton;
    private List<GameObject> characters = new();

    private int selectedIndex = 0;

    private void Start()
    {
        nextButton.onClick.AddListener(SelectNext);
        previousButton.onClick.AddListener(SelectPrevious);
        selectButton.onClick.AddListener(Select);

        foreach (var character in characterContainer.GetCharacters())
        {
            GameObject instance = Instantiate(character, transform);
            characters.Add(instance);
            instance.SetActive(false);
        }
        characters[selectedIndex].SetActive(true);
    }

    private void SelectNext()
    {
        Hide(selectedIndex);
        selectedIndex = (selectedIndex + 1) % characterContainer.Count;
        Show(selectedIndex);
    }

    private void SelectPrevious()
    {
        Hide(selectedIndex);
        selectedIndex = (selectedIndex - 1) % characterContainer.Count;
        Show(selectedIndex);
    }

    private void Select()
    {
        PlayerPrefs.SetInt("SelectedCharacter", selectedIndex);
        SceneManager.LoadScene("MainScene");
    }

    private void Show(int index) => characters[index].SetActive(true);
    private void Hide(int index) => characters[index].SetActive(false);
}
