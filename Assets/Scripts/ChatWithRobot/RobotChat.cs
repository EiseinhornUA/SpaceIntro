using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(AskView))]
public class RobotChat : MonoBehaviour
{
    private const string apiUrl = "https://e-spaceintroai-chatbot.onrender.com/chat";
    private List<string> conversationHistory = new List<string>();

    private AskView askView;

    private void Start()
    {
        askView = GetComponent<AskView>();
        askView.askButton.onClick.AddListener(OnAskButtonClicked);
    }

    private void OnAskButtonClicked()
    {
        string question = askView.GetQuestionText();
        if (!string.IsNullOrEmpty(question))
        {
            SendChatRequest(question);
        }
    }

    private async UniTask SendChatRequest(string question)
    {
        ChatRequest requestData = new ChatRequest
        {
            user_id = SystemInfo.deviceUniqueIdentifier,
            question = question,
            conversation_history = conversationHistory
        };

        string json = JsonUtility.ToJson(requestData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        askView.SetResponse("Thinking...");

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;
            ResponseData responseData = JsonUtility.FromJson<ResponseData>(jsonResponse);

            if (responseData != null && !string.IsNullOrEmpty(responseData.answer))
            {
                askView.SetResponse(responseData.answer);
                askView.ShowAskButton();
                conversationHistory.Add(question);
                conversationHistory.Add(responseData.answer);
            }
            else
            {
                askView.SetResponse("No response received from the server.");
                Debug.LogError("No response received from the server.");
            }
        }
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
    public List<string> conversation_history;
}
