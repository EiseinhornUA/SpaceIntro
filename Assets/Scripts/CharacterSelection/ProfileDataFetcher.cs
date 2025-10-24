using Cysharp.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ProfileDataFetcher : MonoBehaviour
{
    private DatabaseReference userReference;
    private string userID;
    private UniTaskCompletionSource initializeTCS = new();

    private async void Awake()
    {
        await Login();
        InitDatabase();
        initializeTCS?.TrySetResult();
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

    public async UniTask<string> GetAliasUserIDAsync()
    {
        await initializeTCS.Task;
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

    public async UniTask<List<Skill>> GetSkillsAsync()
    {
        await initializeTCS.Task;

        var skills = new List<Skill>();

        try
        {
            var snapshot = await userReference.Child("metricsSum").GetValueAsync();

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
        await UniTask.Yield();
        return skills;
    }
}