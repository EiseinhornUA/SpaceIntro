using Cysharp.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    private DatabaseReference userReference;
    private string userID;

    private void Start()
    {
        FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(true);

        InitializeAsync().Forget();
    }

    private async UniTask InitializeAsync()
    {
        await Login();
        InitDatabase();
        await SaveName();

        FindObjectOfType<SkillContainer>(true).OnSkillLevelChanged += OnSkillLevelChanged;
        FindObjectOfType<RobotChat>(true).OnChatHistoryUpdated += OnChatHistoryUpdated;
    }


    private async UniTask Login()
    {
        var result = await FirebaseAuth.DefaultInstance.SignInAnonymouslyAsync();
        userID = result.User.UserId;
        Debug.Log($"Logged in as {userID}");
    }

    private void InitDatabase()
    {
        userReference = FirebaseDatabase.DefaultInstance
            .RootReference
            .Child("users")
            .Child(userID);
    }

    private void OnSkillLevelChanged(Skill skill)
    {
        SaveSkill(skill).Forget();
    }
    private void OnChatHistoryUpdated(List<PhraseCharacterPair> conversationHistory)
    {
        SaveChatHistory(conversationHistory).Forget();
    }

    private async UniTask SaveChatHistory(List<PhraseCharacterPair> conversationHistory)
    {
        try
        {
            var wrapper = new ChatHistoryWrapper(conversationHistory);
            await userReference.Child("chatHistory").SetRawJsonValueAsync(JsonConvert.SerializeObject(wrapper));
            Debug.Log("Saved chat history");
        }
        catch (Exception e)
        {
            Debug.LogError($"SaveChatHistory failed: {e.Message}");
        }
    }


    private async UniTask SaveSkill(Skill skill)
    {
        try
        {
            await userReference.Child("metrics")
                .Child(skill.skillName)
                .SetValueAsync(skill.level);
            Debug.Log($"Saved {skill.skillName}: {skill.level}");
        }
        catch (Exception e)
        {
            Debug.LogError($"SaveSkill failed: {e.Message}");
        }
    }

    private async UniTask SaveName()
    {
        try
        {
            string name = PlayerPrefs.GetString("CharacterName", "Name");
            await userReference.Child("name").SetValueAsync(name);
            Debug.Log($"Saved name: {name}");
        }
        catch (Exception e)
        {
            Debug.LogError($"SaveName failed: {e.Message}");
        }
    }
}

[System.Serializable]
public class ChatHistoryWrapper
{
    public List<DialogueLine> history;
    public ChatHistoryWrapper(List<PhraseCharacterPair> history)
    {
        this.history = history.Select(h => new DialogueLine(h.character.GetName(), h.phrase)).ToList();
    }
}

[Serializable]
public class DialogueLine
{
    public string speaker;
    public string text;
    public DialogueLine(string speaker, string text)
    {
        this.speaker = speaker;
        this.text = text;
    }
}