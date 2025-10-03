using Cysharp.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    [SerializeField] private CharacterLoader characterLoader;
    
    private const string InitialAssessmentName = "A0";
    private DatabaseReference userReference;
    private string userID;
    private UniTask initializeTask;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(true);

        SubscribeToEvents();

        initializeTask = InitializeAsync();
    }

    private async UniTask InitializeAsync()
    {
        await Login();
        InitDatabase();
        await SaveAliasID();
        await SaveNameAsync();
    }

    private void SubscribeToEvents()
    {
        FindObjectOfType<SkillContainer>(true).OnSkillLevelChanged += SaveSkill;
        characterLoader.OnCharacterLoaded += SaveInitialSkills;
        FindObjectOfType<RobotChat>(true).OnChatHistoryUpdated += OnChatHistoryUpdated;
    }

    private void SaveInitialSkills(List<Skill> skills)
    {
        SaveInitialSkillsAsync(skills).Forget();
    }

    private async UniTask SaveInitialSkillsAsync(List<Skill> skills)
    {
        await initializeTask;
        SaveAssesment(skills, InitialAssessmentName);
        foreach (var skill in skills)
        {
            SaveSkillAsync(skill).Forget();
        }
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

    private async UniTask SaveAliasID()
    {
        try
        {
            string userIDAlias = UIDGenerator.Generate6CharHash(userID);
            await userReference.Child("UID").SetValueAsync(userIDAlias);
            Debug.Log($"Saved aliasID: {userIDAlias}");
        }
        catch (Exception e)
        {
            Debug.LogError($"SaveAliasID failed: {e.Message}");
        }
    }

    private void SaveSkill(Skill skill)
    {
        SaveSkillAsync(skill).Forget();
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


    private async UniTask SaveSkillAsync(Skill skill)
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

    private async UniTask SaveNameAsync()
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

    public void SaveAssesment(List<Skill> skills, string assessmentName)
    {
        SaveAssesmentAsync(skills, assessmentName).Forget();
    }

    private async UniTask SaveAssesmentAsync(List<Skill> skills, string assessmentName)
    {
        try
        {
            await userReference.Child("assessments")
                .Child(assessmentName)
                .SetRawJsonValueAsync(JsonConvert.SerializeObject(skills.ToDictionary(s => s.skillName, v => v.level)));
        }
        catch (Exception e)
        {
            Debug.LogError($"SaveAssesment failed: {e.Message}");
        }
    }

    public async UniTask<List<Skill>> GetSkillsAsync()
    {
        var skills = new List<Skill>();

        try
        {
            var snapshot = await userReference.Child("metrics").GetValueAsync();

            if (snapshot.Exists)
            {
                foreach (var child in snapshot.Children)
                {
                    string skillName = child.Key;
                    float level = float.Parse(child.Value.ToString());

                    skills.Add(new Skill(skillName, level));
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"GetSkills failed: {e.Message}");
        }

        return skills;
    }

    public async UniTask<string> GetAliasUserIDAsync()
    {
        try
        {
            var snapshot = await userReference.Child("UID").GetValueAsync();

            if (snapshot.Exists)
            {
                return snapshot.Value.ToString();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"GetAliasUserIDAsync failed: {e.Message}");
        }
        return default;
    }

    public void SaveMiniGameTime(int time, string gameName) => SaveMiniGameTimeAsync(time, gameName).Forget();

    private async UniTask SaveMiniGameTimeAsync(int time, string gameName)
    {
        try {
            await userReference.Child("miniGames").Child(gameName).Child("timeSeconds").SetValueAsync(time);
        }
        catch (Exception e)
        {
            Debug.LogError($"Saving Time in mini game failed: {e.Message}");
        }
    }

    public void SaveMiniGameAttempts(int attemptsCount, string gameName) => SaveMiniGameAttemptsAsync(attemptsCount, gameName).Forget();

    private async UniTask SaveMiniGameAttemptsAsync(int attemptsCount, string gameName)
    {
        try {
        await userReference.Child("miniGames").Child(gameName).Child("attemptsCount").SetValueAsync(attemptsCount);
        }
        catch (Exception e)
        {
            Debug.LogError($"Saving Attempts in mini game failed: {e.Message}");
        }
    }

    public void SaveMiniGameMoves(int movesCount, string gameName) => SaveMiniGameMovesAsync(movesCount, gameName).Forget();

    private async UniTask SaveMiniGameMovesAsync(int boltMovesCount, string gameName)
    {
        try
        {
            await userReference.Child("miniGames").Child(gameName).Child("movesCount").SetValueAsync(boltMovesCount);
        }
        catch (Exception e)
        {
            Debug.LogError($"Saving Bolt moves in mini game failed: {e.Message}");
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