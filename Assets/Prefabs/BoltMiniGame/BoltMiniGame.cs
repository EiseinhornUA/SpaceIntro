using Cinemachine;
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
    public CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Vector3 miniGameOffset;
    [SerializeField] private bool showMiniGame = false;
    public Player player;
    [SerializeField] private bool isDebuging = false;
    public Button closeButton;
    public Button resetButton;
    [SerializeField] private GameObject resetGame;
    [SerializeField] private bool debugDontDestroyPlanks = false;
    public Interactable startGameInteractable;
    private float initialBoltSize;
    private float scaledBoltSize;
    [SerializeField] private float scalePercent = 20f;
    [SerializeField] private float screwingTime = 0.25f;
    [SerializeField] private int screwingRotation = 360;

    private enum GameState
    {
        NotStarted,
        Started,
        Finished,
        Restarting
    }

    [SerializeField] private GameState gameState = GameState.NotStarted;

    [ContextMenu("DebugDontDestroyPlanksFalse")]
    public void DebugDontDestroyPlanksFalse()
    {
        debugDontDestroyPlanks = false;
    }

    private void Awake()
    {
        initialBoltSize = FindAnyObjectByType<Bolt>().transform.localScale.x;
        scaledBoltSize = initialBoltSize * (scalePercent / 100f + 1f);
    }

    public void InitializeGame()
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

        AttachAllBoltsToAllPlanks();
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
        transform.position = virtualCamera.transform.position + virtualCamera.transform.forward * miniGameOffset.z;
        transform.GetChild(0).position = transform.position;
    }

    private void RemoveDetachedPlanks()
    {
        const float DistanceToRemovePlanks = -5f;
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
            else
            {
                UnscrewBolt(holeFrom);
            }
            return;
         }

        if (!holeTo)
        {
            holeTo = hole;
            if (holeFrom == holeTo)
            {
                ScrewBolt(holeFrom);
                holeFrom = null;
                holeTo = null;
                return;
            }

            if (holeTo.HasBolt())
            {
                ScrewBolt(holeFrom);
                holeFrom = holeTo;
                UnscrewBolt(holeFrom);
                holeTo = null;
            }
        }

        if (holeFrom && holeTo)
        {
             SwapBolts(holeFrom, holeTo).Forget();
             this.holeFrom = null;
             this.holeTo = null;
        }
    }

    private void UnscrewBolt(Hole hole)
    {
        if (isScrewed(hole))
        {
            hole.GetBolt().transform.DOScale(hole.GetBolt().transform.localScale *
                (scalePercent / 100f + 1f), screwingTime).SetEase(Ease.InOutQuad);
            hole.GetBolt().transform
                .DOLocalRotate(new Vector3(0f, 0f, -screwingRotation), screwingTime, RotateMode.FastBeyond360)
                .SetEase(Ease.InOutQuad);
        }
    }

    private bool isScrewed(Hole hole)
    {
        return Mathf.Abs(hole.GetBolt().transform.localScale.x - initialBoltSize) < 0.001;
    }

    private void ScrewBolt(Hole hole)
    {
        if (isUnScrewed(hole))
        {
            hole.GetBolt().transform.DOScale(hole.GetBolt().transform.localScale /
                 (scalePercent / 100f + 1f), screwingTime).SetEase(Ease.InOutQuad);
            hole.GetBolt().transform
                .DOLocalRotate(new Vector3(0f, 0f, screwingRotation), screwingTime, RotateMode.FastBeyond360)
                .SetEase(Ease.InOutQuad);
        }
    }

    private bool isUnScrewed(Hole hole)
    {
        return Mathf.Abs(hole.GetBolt().transform.localScale.x - scaledBoltSize) < 0.001;
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
        transform.parent.GetComponent<BoltGameStarter>().initialCameraDampingTime = 
            virtualCamera.GetCinemachineComponent<CinemachineComposer>().m_HorizontalDamping;
        virtualCamera.GetCinemachineComponent<CinemachineComposer>().m_HorizontalDamping = 
            transform.parent.GetComponent<BoltGameStarter>().cameraDampingTime;
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
        startGameInteractable.onInteract.RemoveAllListeners();
        startGameInteractable.onInteract.AddListener(ShowMiniGame);
    }

    [ContextMenu("HideMiniGame")]
    public void HideMiniGame()
    {
        virtualCamera.GetCinemachineComponent<CinemachineComposer>().m_HorizontalDamping =
            transform.parent.GetComponent<BoltGameStarter>().initialCameraDampingTime;
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

        ScrewBolt(holeTo);

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
            startGameInteractable.Deactivate();
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

    public void AttachAllBoltsToAllPlanks()
    {
        foreach (Plank plank in planks)
        {
            plank.AttachAllBolts();
        }
    }
    
    [ContextMenu("FinishGame")]
    private void FinishGame()
    {
        gameFinished.TrySetResult();
        onGameFinished.Invoke();
        HideMiniGame();
        gameState = GameState.Finished;
        startGameInteractable.Deactivate();
    }
}
