using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
public class DialogueView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI message;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private Image characterPortrait;
    [SerializeField] private Button nextButton;
    [SerializeField] private List<Button> choiceButtons;
    private UniTaskCompletionSource taskCompletionSource;

    public void ChangeMessage(string messageText) => message.text = messageText;
    public void ChangeCharacterName(string name) => characterName.text = name;
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

        return selectedIndex;
    }


    internal void ShowChoices(List<string> choiceMessages)
    {
        for (int i = 0; i < choiceMessages.Count; i++)
        {
            choiceButtons[i].gameObject.SetActive(true);
            choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = choiceMessages[i];
        }
    }

    internal void Show() => gameObject.SetActive(true);
    internal void Hide() => gameObject.SetActive(false);
    internal async UniTask WaitForHide()
    {
        taskCompletionSource = new UniTaskCompletionSource();
        await taskCompletionSource.Task;
    }

    private void OnDisable()
    {
        taskCompletionSource?.TrySetResult();
    }

}
