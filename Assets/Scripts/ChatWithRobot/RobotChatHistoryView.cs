using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobotChatHistoryView : MonoBehaviour
{
    [SerializeField] private MessageView messageViewPrefab;
    [SerializeField] private Transform messagesParent;

    private List<MessageView> messages = new();
    private List<PhraseCharacterPair> conversationHistory = new();

    private void OnEnable()
    {
        UpdateRectTransform().Forget();
    }

    private async UniTask UpdateRectTransform()
    {
        await UniTask.WaitForEndOfFrame(this);

        LayoutRebuilder.ForceRebuildLayoutImmediate(messagesParent.GetComponent<RectTransform>());
    }

    public void UpdateChatHistory(List<PhraseCharacterPair> conversationHistory)
    {
        //if (conversationHistory.Count == this.conversationHistory.Count) return;

        this.conversationHistory = conversationHistory;

        int existingCount = messages.Count;

        for (int i = existingCount; i < conversationHistory.Count; i++)
        {
            var messageView = MessageView.Create(conversationHistory[i], messageViewPrefab, messagesParent);

            messages.Add(messageView);
        }
    }

}