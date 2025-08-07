using Cysharp.Threading.Tasks;
using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DatabaseManager : MonoBehaviour
{
    private SkillContainer skillContainer;
    private DatabaseReference database;

    private string userID;

    private void Start()
    {
        userID = SystemInfo.deviceUniqueIdentifier;
        database = FirebaseDatabase.DefaultInstance.RootReference;
        skillContainer = FindObjectOfType<SkillContainer>();
        skillContainer.OnSkillLevelAdded += OnSkillLevelAdded;
    }

    private void OnSkillLevelAdded(SkillSO sO, int arg2)
    {
        StoreSkillsToFirebase();
    }

    public async void StoreSkillsToFirebase()
    {
        if (skillContainer == null) return;

        List<Skill> skills = skillContainer.GetSkills();
        Dictionary<string, object> skillsData = new Dictionary<string, object>();

        foreach (Skill skill in skills)
        {
            skillsData[skill.skillName] = new Dictionary<string, object>
            {
                { "level", skill.level }
            };
        }

        await SaveSkillsAsync(skillsData);
    }

    private async UniTask SaveSkillsAsync(Dictionary<string, object> skillsData)
    {
        await database.Child("users").Child(userID).Child("skills").SetValueAsync(skillsData);
        Debug.Log("Skills saved to Firebase.");
    }
}
