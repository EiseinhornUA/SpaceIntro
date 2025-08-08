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
    private DatabaseReference userReference;

    private string userID;

    private void Start()
    {
        userID = SystemInfo.deviceUniqueIdentifier;
        database = FirebaseDatabase.DefaultInstance.RootReference;
        userReference = database.Child("users").Child(userID);

        SaveName(PlayerPrefs.GetString("CharacterName", "Name"));

        skillContainer = FindObjectOfType<SkillContainer>();
        skillContainer.OnSkillLevelAdded += OnSkillLevelAdded;
    }

    private void OnSkillLevelAdded(Skill skill)
    {
        SaveSkill(skill);
    }

    private void SaveSkill(Skill skill)
    {
        userReference.Child("metrics")
            .Child(skill.skillName)
            .SetValueAsync(skill.level);
    }

    private void SaveName(string CharacterName)
    {
        userReference.Child("name")
            .SetValueAsync(CharacterName);
    }
}
