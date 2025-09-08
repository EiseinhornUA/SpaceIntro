using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BoltMiniGame : MonoBehaviour
{
    [SerializeField] private Wall wall;
    private Hole[] holes;
    public Hole holeFrom;
    public Hole holeTo;
    private Plank[] planks;
    private UniTaskCompletionSource gameFinished;

    public UnityEvent onGameFinished;

    [SerializeField] private float plankHoleCheckThreshold = 0.25f;
    [SerializeField] public float speedToSwapBolts = 6f;
    [SerializeField] public Ease boltSwapEase = Ease.InOutQuint;
    [SerializeField] private float plankGravityScale = 5f;
    [SerializeField] private Camera currentCamera;
    [SerializeField] private Vector3 miniGameOffset;
    [SerializeField] private bool showMiniGame = false;
    [SerializeField] private Player player;
    [SerializeField] private bool isDebuging = false;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private GameObject resetGame;
    [SerializeField] private bool debugDontDestroyPlanks = false;
    [SerializeField] private GameObject objectThatStartsGame;

    private enum GameState
    {
        NotStarted,
        Started,
        Finished,
        Restarting
    }

    [SerializeField]
    private GameState gameState = GameState.NotStarted;

    [ContextMenu("DebugDontDestroyPlanksFalse")]
    public void DebugDontDestroyPlanksFalse()
    {
        debugDontDestroyPlanks = false;
    }

    private void Awake()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        holes = wall.holes;
        planks = wall.planks;
        Vector3 localScale = transform.localScale;
        float avarageScale = (localScale.x + localScale.y + localScale.z) / 3;
        foreach (Plank plank in wall.planks)
            plank.plankHoleCheckThreshold = plankHoleCheckThreshold * avarageScale;

        foreach (Rigidbody2D rigidbody in GetComponentsInChildren<Rigidbody2D>())
        {
            rigidbody.gravityScale = avarageScale * plankGravityScale;
        }

        foreach (Plank plank in planks)
        {
            plank.AttachAllBolts();
        }
    }

    private void Update()
    {
        if (gameState != GameState.Started)
            return;

        AttachToCamera();

        if (!showMiniGame)
        {
            transform.position += new Vector3(0f, 50f, 50f);
        } 

        if (isDebuging)
        {
            foreach (Hole hole in holes)
            {
                if (IsAbleToPlace(hole))
                {
                    hole.GetComponent<SpriteRenderer>().color = Color.green;
                }
                else
                {
                    hole.GetComponent<SpriteRenderer>().color = Color.red;
                }
            }
        }

        RemoveDetachedPlanks();

        CheckIfGameFinishes();
    }

    private void AttachToCamera()
    {
        transform.position = currentCamera.transform.position + currentCamera.transform.forward * miniGameOffset.z;
        transform.GetChild(0).position = transform.position;
    }

    private void RemoveDetachedPlanks()
    {
        const float DistanceToRemovePlanks = -3f;
        bool removedPlank = false;
        foreach (Plank plank in planks)
        {
            if (plank == null) return;
            if ((plank.transform.position.y - transform.position.y) < DistanceToRemovePlanks)
            {
                removedPlank = true;
                if (!debugDontDestroyPlanks) 
                    Destroy(plank.gameObject);
            }
        }
        if(removedPlank)
            planks = planks.Where(p => p.isActiveAndEnabled).ToArray();
    }

    public void OnHoleClick(Hole hole)
    {
         if (!holeFrom)
         {
             holeFrom = hole;
             if (!holeFrom.HasBolt())
             {
                 holeFrom = null;
                 holeTo = null;
             }
             return;
        }

        if (!holeTo)
        {
            holeTo = hole;
            if (holeTo.HasBolt() && holeTo != holeFrom && IsAbleToPlace(holeTo))
                holeTo = null;
        }

        if (holeFrom && holeTo)
         {
             SwapBolts(holeFrom, holeTo).Forget();
             this.holeFrom = null;
             this.holeTo = null;
         }
    }

    public async UniTask StartMiniGame()
    {
        gameState = GameState.Started;
        ShowMiniGame();
        await WaitUntilFinished();

    }

    [ContextMenu("ShowMiniGame")]
    public void ShowMiniGame()
    {
        resetButton.gameObject.SetActive(true);
        closeButton.gameObject.SetActive(true);
        if (gameFinished == null)
            gameFinished = new UniTaskCompletionSource();
        player.GetComponent<Collider2D>().enabled = false;
        Hud.Instance.HideHud();
        showMiniGame = true;
    }

    public void AddListenerToShowGame()
    {
        objectThatStartsGame.GetComponent<Interactable>().onInteract.AddListener(ShowMiniGame);
    }

    [ContextMenu("HideMiniGame")]
    public void HideMiniGame()
    {   
        player.GetComponent<Collider2D>().enabled = true;
        Hud.Instance.ShowHud();
        showMiniGame = false;
        transform.position += new Vector3(0f, 50f, 50f);
        resetButton.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);
    }

    [ContextMenu("SwapBolts")]
    private async UniTask SwapBoltsWithArguments()
    {
        await SwapBolts(holeFrom, holeTo);
        this.holeFrom = null;
        this.holeTo = null;
    }

    private async UniTask SwapBolts(Hole holeFrom, Hole holeTo)
    {
        if (!holeFrom.HasBolt()) return;
        if (holeTo.HasBolt()) return;

        if (!IsAbleToPlace(holeTo)) return;

        foreach (Plank plank in planks)
            plank.DetachBolt(holeFrom);

        holeTo.PlaceBolt(holeFrom.GetBolt());

        foreach (Plank plank in planks)
            plank.HolesPlankIsMoveable(holeTo, false);

        await holeTo.MoveBolt(holeFrom.GetBolt());

        foreach (Plank plank in planks)
            plank.HolesPlankIsMoveable(holeTo, true);

        foreach (Plank plank in planks)
            plank.AttachToBolt(holeTo);

        holeFrom.RemoveBolt();
    }

    private bool IsAbleToPlace(Hole holeTo)
    {
        return !planks.Any(p => p.IsBlocking(holeTo));
    }

    public void CheckIfGameFinishes()
    {
        if(IsGameFinished())
        {
            gameFinished.TrySetResult();
            onGameFinished.Invoke();
            HideMiniGame();
            gameState = GameState.Finished;
            objectThatStartsGame.GetComponent<Interactable>().isActive = false;
        }
    }

    public async UniTask WaitUntilFinished()
    {
        await gameFinished.Task;
    }

    private bool IsGameFinished()
    {
        return planks.Length == 0;
    }

    public void ResetMiniGame()
    {
        gameState = GameState.Restarting;
        GameObject reloadedPrefab = Instantiate(resetGame,
            transform.position,
            Quaternion.identity,
            transform);

        Vector3 reloadedPrefabScale = reloadedPrefab.transform.localScale;

        wall = reloadedPrefab.GetComponentInChildren<Wall>();

        InitializeGame();

        Debug.Log("Made reset");
        Destroy(transform.GetChild(0).gameObject, Time.deltaTime);
        gameState = GameState.Started;
    }
}
