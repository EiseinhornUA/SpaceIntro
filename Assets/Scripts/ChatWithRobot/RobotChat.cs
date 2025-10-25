using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using static Cinemachine.CinemachineFreeLook;

[RequireComponent(typeof(AskView))]
public class RobotChat : MonoBehaviour
{
    private const string apiUrl = "https://e-spaceintroai-chatbot.onrender.com/chat";
    [SerializeField] private List<PhraseCharacterPair> conversationHistory = new();

    private AskView askView;
    [SerializeField] private RobotChatHistoryView robotChatHistoryView;
    
    [Header("Characters")]
    [SerializeField] private DialogueCharacter playerCharacter;
    [SerializeField] private DialogueCharacter robotCharacter;

    public event Action<List<PhraseCharacterPair>> OnChatHistoryUpdated = delegate { };

    private void Start()
    {
        askView = GetComponent<AskView>();
        askView.askButton.onClick.AddListener(OnAskButtonClicked);
    }

    private void OnAskButtonClicked()
    {
        string question = askView.GetQuestionText();
        SendChatRequest(question).Forget();
    }

    private async UniTask SendChatRequest(string question)
    {
        if (string.IsNullOrEmpty(question))
        {
            askView.ShowAskButton();
            return;
        }

        ChatRequest requestData = new ChatRequest
        {
            user_id = SystemInfo.deviceUniqueIdentifier,
            question = question,
        };

        string json = JsonUtility.ToJson(requestData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new(apiUrl, "POST")
        {
            uploadHandler = new UploadHandlerRaw(bodyRaw),
            downloadHandler = new DownloadHandlerBuffer()
        };
        request.SetRequestHeader("Content-Type", "application/json");

        askView.SetResponse("...");

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;
            ResponseData responseData = JsonUtility.FromJson<ResponseData>(jsonResponse);

            if (responseData != null && !string.IsNullOrEmpty(responseData.answer))
            {
                askView.SetResponse(responseData.answer);
                askView.ShowAskButton();
                askView.ClearInput();
                SaveQuestionAnswer(question, responseData.answer);
                return;
            }
            askView.SetResponse("Try again.");
            Debug.LogError("No response received from the server.");
        }
    }

    private void SaveQuestionAnswer(string question, string answer)
    {
        conversationHistory.Add(new(question, playerCharacter));
        conversationHistory.Add(new(answer, robotCharacter));
        robotChatHistoryView.UpdateChatHistory(conversationHistory);
        OnChatHistoryUpdated.Invoke(conversationHistory);
    }

    public List<(string phrase, string characterName)> GetChatHistory()
    {
        return conversationHistory.Select(pair => (pair.phrase, pair.character.GetName())).ToList();
    }

    public void SetChatHistory(List<(string phrase, string characterName)> conversation)
    {
        conversationHistory = conversation.Select(kvp => new PhraseCharacterPair(phrase: kvp.phrase, character: GetCharacter(kvp.characterName))).ToList();
        robotChatHistoryView.UpdateChatHistory(conversationHistory);
        OnChatHistoryUpdated.Invoke(conversationHistory);
    }

    private DialogueCharacter GetCharacter(string name)
    {
        if (name == playerCharacter.GetName()) return playerCharacter;

        return robotCharacter;
    }
}

[System.Serializable]
internal class ResponseData
{
    public string answer;
}

[System.Serializable]
public class ChatRequest
{
    public string user_id;
    public string question;
}


