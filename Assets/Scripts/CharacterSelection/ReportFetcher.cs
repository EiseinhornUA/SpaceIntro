using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;   // Make sure you have the Newtonsoft.Json package

public class ReportFetcher : MonoBehaviour
{
    private const string apiUrl = "https://e-spaceintroai-chatbot.onrender.com/report";

    public async UniTask<string> FetchReportAsync(string name, List<Skill> skills, string playerId = "", bool useLLM = false)
    {
        // Convert skills list into dictionary
        var scoresDict = new Dictionary<string, float>();
        foreach (var skill in skills)
        {
            scoresDict[skill.skillName] = skill.level;
        }

        // Build request data
        var requestData = new ReportRequest
        {
            name = name,
            player_id = playerId,
            scores = scoresDict,
            use_llm = useLLM
        };

        // Serialize to JSON (dictionary works here)
        string jsonInput = JsonConvert.SerializeObject(requestData, Formatting.None);

        using var request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonInput);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        var op = await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var responseJson = request.downloadHandler.text;
            var response = JsonConvert.DeserializeObject<ReportResponse>(responseJson);
            return response.report;
        }
        else
        {
            Debug.LogError($"Error: {request.error}");
            return "Failed to fetch report.";
        }
    }

    private class ReportRequest
    {
        public string name { get; set; }
        public string player_id { get; set; }
        public Dictionary<string, float> scores { get; set; }
        public bool use_llm { get; set; }
    }

    private class ReportResponse
    {
        public string name { get; set; }
        public Strength strength { get; set; }
        public Opportunity opportunity { get; set; }
        public string report { get; set; }
    }

    private class Strength
    {
        public string metric { get; set; }
        public string label { get; set; }
        public string[] occupations { get; set; }
    }

    private class Opportunity
    {
        public string metric { get; set; }
        public string label { get; set; }
    }
}
