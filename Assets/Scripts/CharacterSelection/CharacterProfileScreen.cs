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
    private GameObject characterInstance;

    public UnityEvent onBackButtonClicked { get; private set; } = new();

    private async void OnEnable()
    {
        var databaseManager = FindObjectOfType<DatabaseManager>(true);
        
        var skills = await databaseManager.GetSkillsAsync();

        foreach (var skillBar in skillBars)
        {
            var skill = skills.Find(s => s.skillName == skillBar.skillSO.GetName());

            if (skill != null)
            {
                skillBar.slider.value = NormalizeSkillLevel(skill.level, skillBar.skillSO.GetMinLevel(), skillBar.skillSO.GetMaxLevel());
            }
        }

        userIdTMP.text = "UID: " + await databaseManager.GetAliasUserIDAsync();

        reportTMP.text = await reportFetcher.FetchReportAsync(PlayerPrefs.GetString("CharacterName"), await databaseManager.GetSkillsAsync());
    }

    private void Start()
    {
        backButton.onClick.AddListener(onBackButtonClicked.Invoke);
        GameObject selectedCharacter = characterContainer.GetCharacter(PlayerPrefs.GetInt("SelectedCharacter"));
        characterInstance = Instantiate(selectedCharacter, characterParent);
    }

    private float NormalizeSkillLevel(float level, float min, float max)
    {
        return (level - min) / (max - min);
    }


    [System.Serializable]
    public class SliderSkillSOPair
    {
        public Slider slider;
        public SkillSO skillSO;
    }
}