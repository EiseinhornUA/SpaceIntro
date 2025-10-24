using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistanceManager : MonoBehaviour
{
    private const string SaveDataKey = "SaveData";
    [SerializeField] private ElevatorMultiInteractable elevator;

    private Save save = new();

    private Player player;
    private PlayerCamera playerCamera;
    private RobotFollow robot;
    private LightSwitcher lightSwitcher;
    private DoorDeactivator doorDeactivator;
    private SpaceSuit spaceSuit;
    private CharacterLoader characterLoader;
    private ItemContainer itemContainer;
    private ReportContainerView reportContainer;
    private SkillContainer skillContainer;
    private TriggerSwitcher triggerSwitcher;

    private DatabaseManager databaseManager;

    private MainMenu mainMenu;

    [SerializeField, TextArea(20, 30)] private string saveJson;

    private string GameSceneKey = "3DSci-fiScene";

    private void Awake()
    {
        GameStateProvider.OnGameStateChanged += SaveGameState;
        if (GameStateProvider.IsMainMenu())
        {
            mainMenu = FindObjectOfType<MainMenu>();
            mainMenu.OnContinueButtonClicked.AddListener(LoadGameScene);

            HairColorChanger hairColorChanger = FindObjectOfType<HairColorChanger>(true);
            if(hairColorChanger) hairColorChanger.LoadColor();
            SkinChanger skinChanger = FindObjectOfType<SkinChanger>(true);
            if (skinChanger) skinChanger.LoadSkin();

            GameStateProvider.SetState(LoadState());
        }
        if (GameStateProvider.IsGameCompleted())
        {
            ResetSave();
            SaveCurrentGameState();
            return;
        }
        if (GameStateProvider.IsGameStarted())
        {
            ResetSave();
        }

        CacheSceneReferences();
        if (!skillContainer) return;
        skillContainer.OnSkillLevelChanged += SaveSkillsHistory;
    }

    private void SaveSkillsHistory()
    {
        save.skillsHistory.Add(skillContainer.GetDictionarySkills());
        saveJson = JsonConvert.SerializeObject(save, Formatting.Indented);
        PlayerPrefs.SetString(SaveDataKey, saveJson);
        PlayerPrefs.Save();
    }

    public void ResetSave()
    {
        PlayerPrefs.DeleteKey(SaveDataKey);
    }

    private void LoadGameScene()
    {
        GameStateProvider.SetContinued();
        SceneManager.LoadScene(GameSceneKey);
    }

    private GameStateProvider.GameState LoadState()
    {
        LoadSaveFromPrefs();
        return save.gameState;
    }

    private void CacheSceneReferences()
    {
        player = FindObjectOfType<Player>();
        playerCamera = FindObjectOfType<PlayerCamera>();
        robot = FindObjectOfType<RobotFollow>();
        lightSwitcher = FindObjectOfType<LightSwitcher>();
        doorDeactivator = FindObjectOfType<DoorDeactivator>();
        spaceSuit = FindObjectOfType<SpaceSuit>();
        characterLoader = FindObjectOfType<CharacterLoader>();
        itemContainer = FindObjectOfType<ItemContainer>();
        reportContainer = FindObjectOfType<ReportContainerView>(true);
        skillContainer = FindObjectOfType<SkillContainer>();
        triggerSwitcher = FindObjectOfType<TriggerSwitcher>();
    }

    #region Context Menu Actions

    [ContextMenu("Clear Save")]
    public void ClearSave()
    {
        PlayerPrefs.DeleteKey(SaveDataKey);
        PlayerPrefs.Save();
    }

    [ContextMenu("Reload Scene")]
    public void ReloadScene() =>
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    [ContextMenu("Save Current Data")]
    public void SaveCurrentData()
    {
        PlayerPrefs.SetString(SaveDataKey, saveJson);
        PlayerPrefs.Save();
    }

    #endregion

    public void Load() => LoadAsync().Forget();

    private async UniTask LoadAsync()
    {
        LoadSaveFromPrefs();

        SaveCurrentGameState();

        if (!save.checkpoints.TryGetValue(save.checkpointNumber, out var checkpoint))
            return;


        ApplyCheckpointData(checkpoint);

        await LoadSpaceSuit(checkpoint);
    }

    public void SaveGameState(GameStateProvider.GameState state)
    {
        save.gameState = state;
        saveJson = JsonConvert.SerializeObject(save, Formatting.Indented);
        PlayerPrefs.SetString(SaveDataKey, saveJson);
        PlayerPrefs.Save();
    }

    public void SaveCurrentGameState()
    {
        save.gameState = GameStateProvider.State;
        saveJson = JsonConvert.SerializeObject(save, Formatting.Indented);
        PlayerPrefs.SetString(SaveDataKey, saveJson);
        PlayerPrefs.Save();
    }

    private async UniTask LoadSpaceSuit(SaveCheckpoint checkpoint)
    {
        await characterLoader.OnCharacterLoadedTCS.Task;
        await UniTask.Yield();

        if (checkpoint.hasSpaceSuit)
            spaceSuit.PutSpaceSuitOn();
    }

    private void LoadSaveFromPrefs()
    {
        string json = PlayerPrefs.GetString(SaveDataKey, "");
        saveJson = json;
        save = string.IsNullOrEmpty(json)
            ? new Save()
            : JsonConvert.DeserializeObject<Save>(json);
    }

    private void ApplyCheckpointData(SaveCheckpoint cp)
    {
        player.transform.position = cp.playerPosition;
        playerCamera.transform.position = new Vector3(cp.playerPosition.x, cp.playerPosition.y, playerCamera.transform.position.z);

        if (cp.isRobotFixed)
        {
            robot.TurnOnRobot();
            robot.transform.position = cp.robotPosition;
        }

        if (cp.isLightFixed)
            lightSwitcher.SwitchToGlobalLight();

        if (cp.isEngineeringDoorOpened)
            doorDeactivator.OpenDoor();

        if (cp.collectedItems?.Count > 0)
            itemContainer.AddItemsByNames(cp.collectedItems);

        if (cp.hasRobotReports)
            reportContainer.AddRobotReports();

        if (cp.hasAccessCodeReports)
            reportContainer.AddAccessCodeReports();

        skillContainer.SetLoadedSkills(cp.skills);

        if (cp.elevatorTriggered)
            triggerSwitcher.DisableElevatorTriggers();

        if (cp.elevatorEnabled)
            elevator.Activate();

        triggerSwitcher.DisableInitial();
    }

    public void Save(int checkpointNumber)
    {
        var cp = new SaveCheckpoint
        {
            playerPosition = player.transform.position,
            robotPosition = robot.transform.position,
            isRobotFixed = robot.IsFixed(),
            isLightFixed = lightSwitcher.isGlobalLightFixed(),
            isEngineeringDoorOpened = doorDeactivator.IsDoorOpened(),
            hasSpaceSuit = spaceSuit.PlayerHasSuit(),
            collectedItems = itemContainer.GetStoredItemNames(),
            skills = skillContainer.GetDictionarySkills(),
            elevatorTriggered = triggerSwitcher.AreElevatorTriggersDisabled(),
            elevatorEnabled = elevator.IsActive()
        };

        SaveReports(cp);

        GameStateProvider.SetContinued();

        save.checkpointNumber = checkpointNumber;
        save.checkpoints[checkpointNumber] = cp;

        saveJson = JsonConvert.SerializeObject(save, Formatting.Indented);
        PlayerPrefs.SetString(SaveDataKey, saveJson);
        PlayerPrefs.Save();
    }

    private void SaveReports(SaveCheckpoint cp)
    {
        cp.hasAccessCodeReports = reportContainer.HasAccessCodeReports();
        cp.hasRobotReports = reportContainer.HasRobotReports();
    }

    internal int GetCurrentCheckpoint() => save.checkpointNumber;
}

#region Data Classes

[Serializable]
public class SaveCheckpoint
{
    [JsonIgnore]
    public Vector3 playerPosition
    {
        get => new(playerPositionX, playerPositionY, playerPositionZ);
        set { playerPositionX = value.x; playerPositionY = value.y; playerPositionZ = value.z; }
    }

    [JsonIgnore]
    public Vector3 robotPosition
    {
        get => new(robotPositionX, robotPositionY, robotPositionZ);
        set { robotPositionX = value.x; robotPositionY = value.y; robotPositionZ = value.z; }
    }

    public float playerPositionX, playerPositionY, playerPositionZ;
    public float robotPositionX, robotPositionY, robotPositionZ;

    public bool isRobotFixed;
    public bool isLightFixed;
    public bool isEngineeringDoorOpened;
    public bool hasRobotReports;
    public bool hasAccessCodeReports;
    public bool hasSpaceSuit;
    public bool elevatorTriggered;
    public bool elevatorEnabled;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<string> collectedItems = new();

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public Dictionary<string, float> skills = new();
}

[Serializable]
public class Save
{
    public int checkpointNumber;
    public GameStateProvider.GameState gameState;
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<Dictionary<string, float>> skillsHistory = new();
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public Dictionary<int, SaveCheckpoint> checkpoints = new();
}

#endregion
