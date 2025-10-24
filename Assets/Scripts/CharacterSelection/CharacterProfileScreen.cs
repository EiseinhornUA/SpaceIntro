using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CharacterProfileScreen : Popup
{
    [SerializeField] private Button backButton;
    [SerializeField] private List<SliderSkillSOPair> skillBars;
    [SerializeField] private CharacterContainer characterContainer;
    [SerializeField] private Transform characterParent;
    [SerializeField] private TextMeshProUGUI userIdTMP;
    [SerializeField] private TextMeshProUGUI reportTMP;
    [SerializeField] private ReportFetcher reportFetcher;
    [SerializeField] private ProfileDataFetcher profileDataFetcher;
    private GameObject characterInstance;

    public UnityEvent onBackButtonClicked { get; private set; } = new();

    private async void OnEnable()
    {
        var skills = await profileDataFetcher.GetSkillsAsync();

        foreach (var skillBar in skillBars)
        {
            var skill = skills.Find(s => s.skillName == skillBar.skillSO.GetName());

            if (skill != null)
            {
                skillBar.slider.value = NormalizeSkillLevel(skill.level, skillBar.skillSO.GetMinLevel(), skillBar.skillSO.GetMaxLevel());
            }
        }

        userIdTMP.text = "UID: " + await profileDataFetcher.GetAliasUserIDAsync();

        List<Skill> normalizedSkills = NormalizeSkills(await profileDataFetcher.GetSkillsAsync());

        reportTMP.text = await reportFetcher.FetchReportAsync(PlayerPrefs.GetString("CharacterName", "Player"), normalizedSkills);
    }

    private List<Skill> NormalizeSkills(object v)
    {
        throw new NotImplementedException();
    }

    private void Start()
    {
        backButton.onClick.AddListener(onBackButtonClicked.Invoke);
        LoadCharacter();
    }

    private void LoadCharacter()
    {
        GameObject selectedCharacter = characterContainer.GetCharacter(PlayerPrefs.GetInt("SelectedCharacter"));
        characterInstance = Instantiate(selectedCharacter, characterParent);
    }

    private float NormalizeSkillLevel(float level, float min, float max)
    {
        return (level - min) / (max - min);
    }
    private List<Skill> NormalizeSkills(List<Skill> skills)
    {
        List<Skill> skillsNormalized = new();
        foreach (var skill in skills)
        {
            SkillSO skillSO = skillBars.Find(sb => sb.skillSO.GetName() == skill.skillName)?.skillSO;
            
            if (!skillSO) continue;
            
            skill.level = NormalizeSkillLevel(skill.level, skillSO.GetMinLevel(), skillSO.GetMaxLevel());

            skillsNormalized.Add(skill);
        }
        return skillsNormalized;
    }

    [System.Serializable]
    public class SliderSkillSOPair
    {
        public Slider slider;
        public SkillSO skillSO;
    }
}