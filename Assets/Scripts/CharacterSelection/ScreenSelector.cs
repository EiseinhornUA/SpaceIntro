using System;
using UnityEngine;

public class ScreenSelector : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] private CharacterSelector characterSelector;
    [SerializeField] private CharacterProfileScreen profile;
    [SerializeField] private LoadingScreen loadingScreen;
    [SerializeField] private MainMenu mainMenu;

    private void Start()
    {
        HideAll();
        mainMenu.Show();

        characterSelector.OnCharacterSelected.AddListener(() =>
        {
            HideAll();
            loadingScreen.Show();
        });
        mainMenu.OnGameStarted.AddListener(() =>
        {
            HideAll();
            characterSelector.Show();
        });
        mainMenu.OnProfileButtonClicked.AddListener(() =>
        {
            HideAll();
            profile.Show();
        });
        profile.onBackButtonClicked.AddListener(() =>
        {
            HideAll();
            mainMenu.Show();
        });
        characterSelector.OnBackButtonClicked.AddListener(() =>
        {
            HideAll();
            mainMenu.Show();
        });

        if (GameStateProvider.IsGameCompleted())
        {
            HideAll();
            profile.Show();
        }
        //mainMenu.OnGameStarted.AddListener(() =>
        //{
        //    HideAll();
        //    characterSelector.Show();
        //});
    }


    private void HideAll()
    {
        characterSelector.Hide();
        profile.Hide();
        loadingScreen.Hide();
        mainMenu.Hide();
    }
}