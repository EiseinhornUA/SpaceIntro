using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(AskView))]
public class RobotChat : MonoBehaviour
{
    private const string apiUrl = "https://e-spaceintroai-chatbot.onrender.com/chat";
    [SerializeField] private List<PhraseCharacterPair> conversationHistory = new();

    private AskView askView;
    private RobotChatHistoryView robotChatHistoryView;
    
    [Header("Characters")]
    [SerializeField] private DialogueCharacter playerCharacter;
    [SerializeField] private DialogueCharacter robotCharacter;

    private void Start()
    {
        askView = GetComponent<AskView>();
        robotChatHistoryView = FindObjectOfType<RobotChatHistoryView>(true);
        askView.askButton.onClick.AddListener(OnAskButtonClicked);
    }

    private void OnAskButtonClicked()
    {
        string question = askView.GetQuestionText();
        SendChatRequest(question).Forget();
    }

    private async UniTask SendChatRequest(string question)
    {
        if (string.IsNullOrEmpty(question)) return;

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
            askView.SetResponse("Try again");
            Debug.LogError("No response received from the server.");
        }
    }

    private void SaveQuestionAnswer(string question, string answer)
    {
        conversationHistory.Add(new(question, playerCharacter));
        conversationHistory.Add(new(answer, robotCharacter));
        robotChatHistoryView.UpdateChatHistory(conversationHistory);
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


