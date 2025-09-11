using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using Cinemachine;

[System.Serializable]
public class BoltMiniGameSerializes
{
    public BoltMiniGame boltMiniGame;
    public Interactable startGameInteractable;
}

public class BoltGameStarter : MonoBehaviour
{
    [SerializeField] private BoltMiniGameSerializes[] boltMiniGamesSerializes;

    [SerializeField] private Player player;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    public float initialCameraDampingTime;
    [SerializeField] public float cameraDampingTime = 0.2f;

    private void Awake()
    {
        foreach (var boltMiniGameSerializes in boltMiniGamesSerializes)
        {
            boltMiniGameSerializes.boltMiniGame.startGameInteractable =
                boltMiniGameSerializes.startGameInteractable;
            boltMiniGameSerializes.boltMiniGame.virtualCamera = virtualCamera;
            boltMiniGameSerializes.boltMiniGame.player = player;
            boltMiniGameSerializes.boltMiniGame.closeButton = closeButton;
            boltMiniGameSerializes.boltMiniGame.resetButton = resetButton;
        }
    }

    private async UniTask StartGame(BoltMiniGameSerializes gameSerializes)
    {
        gameSerializes.boltMiniGame.gameObject.SetActive(true);
        gameSerializes.boltMiniGame.InitializeGame();
        //gameSerializes.boltMiniGame.AttachAllBoltsToAllPlanks();
        gameSerializes.boltMiniGame.closeButton.onClick.AddListener(
            gameSerializes.boltMiniGame.AddListenerToShowGame);
        gameSerializes.boltMiniGame.closeButton.onClick.AddListener(gameSerializes.boltMiniGame.HideMiniGame);
        gameSerializes.boltMiniGame.resetButton.onClick.AddListener(gameSerializes.boltMiniGame.ResetMiniGame);
        await gameSerializes.boltMiniGame.StartMiniGame();
        gameSerializes.boltMiniGame.gameObject.SetActive(false);
    }

    [ContextMenu("StartCryoTerminalGame")]
    public async UniTask StartCryoTerminalGame()
    {
        foreach (var boltMiniGameSerializes in boltMiniGamesSerializes)
        {
            if (boltMiniGameSerializes.boltMiniGame.gameObject.name == "BoltGameCryoTerminal")
            {
                await StartGame(boltMiniGameSerializes);
            }
        }
    }

    [ContextMenu("StartRobotGame")]
    public async UniTask StartRobotGame()
    {
        foreach (var boltMiniGameSerializes in boltMiniGamesSerializes)
        {
            if (boltMiniGameSerializes.boltMiniGame.gameObject.name == "BoltGameRobot")
            {
                await StartGame(boltMiniGameSerializes);
            }
        }
    }
}
