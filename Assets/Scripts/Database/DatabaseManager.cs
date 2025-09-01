using Cysharp.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    private SkillContainer skillContainer;
    private DatabaseReference userReference;
    private string userID;

    private async void Awake()
    {
        // Enable Firebase offline persistence (data is cached and syncs when online)
        FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(true);
    }

    private async void Start()
    {
        await Login();
        InitDatabase();
        await SaveName();

        skillContainer = FindObjectOfType<SkillContainer>();
        skillContainer.OnSkillLevelChanged += OnSkillLevelChanged;
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

    private async void OnSkillLevelChanged(Skill skill)
    {
        await SaveSkill(skill);
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
        catch (System.Exception e)
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
        catch (System.Exception e)
        {
            Debug.LogError($"SaveName failed: {e.Message}");
        }
    }
}
