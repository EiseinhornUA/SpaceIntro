using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine.TextCore.Text;
public class DialogueView : Popup
{
    [SerializeField] private TextMeshProUGUI message;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private Image characterPortrait;
    [SerializeField] private Button nextButton;
    [SerializeField] private List<Button> choiceButtons;
    public static DialogueView Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        Hide();
        foreach (var button in choiceButtons)
            button.gameObject.SetActive(false);
    }

    public void SetMessage(string messageText) => message.text = messageText.Translate();
    public void SetCharacterName(string name)
    {
        characterName.text = (name == "Player") ? PlayerPrefs.GetString("CharacterName", "Player") : name;
    }

    public void ChangeCharacterPortrait(Sprite portrait) => characterPortrait.sprite = portrait;

    internal async UniTask WaitForClick()
    {
        await nextButton.OnClickAsync();
    }

    internal async UniTask<int> WaitForChoice()
    {
        var tasks = new List<UniTask>();

        foreach (var button in choiceButtons)
        {
            tasks.Add(button.OnClickAsync());
        }

        int selectedIndex = await UniTask.WhenAny(tasks);

        nextButton.gameObject.SetActive(true);

        return selectedIndex;
    }


    internal void ShowChoices(List<string> choiceMessages)
    {
        for (int i = 0; i < choiceMessages.Count; i++)
        {
            choiceButtons[i].gameObject.SetActive(true);
            choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = choiceMessages[i].Translate();
        }
        nextButton.gameObject.SetActive(false);
    }

    private List<UniTaskCompletionSource> completionSources = new();

    internal async UniTask WaitForHide()
    {
        if (!gameObject.activeInHierarchy) return;
        var taskCompletionSource = new UniTaskCompletionSource();
        completionSources.Add(taskCompletionSource);
        await taskCompletionSource.Task;
    }

    private void OnDisable()
    {
        completionSources.ForEach(tcs => tcs.TrySetResult());
        completionSources.Clear();
    }
}
