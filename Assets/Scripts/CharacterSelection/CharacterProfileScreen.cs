using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CharacterProfileScreen : Popup
{
    [SerializeField] private Button backButton;
    [SerializeField] private List<SliderSkillSOPair> skillBars;
    [SerializeField] private CharacterContainer characterContainer;
    [SerializeField] private Transform characterParent;
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
    }

    private void Start()
    {
        backButton.onClick.AddListener(onBackButtonClicked.Invoke);
        GameObject selectedCharacter = characterContainer.GetCharacter(PlayerPrefs.GetInt("SelectedCharacter"));
        characterInstance = Instantiate(selectedCharacter, characterParent);
    }

    public override void Hide()
    {
        if(characterInstance) characterInstance.SetActive(false);
        base.Hide();
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