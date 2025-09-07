using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
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
    [SerializeField] private GameObject buttons;
    [SerializeField] private BoltMiniGame boltMiniGame;

    private void Awake()
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
        AttachToCamera();

        if (!showMiniGame) transform.position += new Vector3(0f, 0f, 50f);

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
    }

    private void RemoveDetachedPlanks()
    {
        const float DistanceToRemovePlanks = -3f;
        foreach (Plank plank in planks)
            if ((plank.transform.position.y - transform.position.y) < DistanceToRemovePlanks)
                plank.gameObject.SetActive(false);
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
        ShowMiniGame();
        await WaitUntilFinished();
    }

    [ContextMenu("ShowMiniGame")]
    public void ShowMiniGame()
    {
        buttons.SetActive(true);
        gameFinished = new UniTaskCompletionSource();
        player.GetComponent<Collider2D>().enabled = false;
        Hud.Instance.HideHud();
        showMiniGame = true;
    }

    [ContextMenu("HideMiniGame")]
    public void HideMiniGame()
    {
        player.GetComponent<Collider2D>().enabled = true;
        Hud.Instance.ShowHud();
        showMiniGame = false;
        buttons.SetActive(false);
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
        }
    }

    public async UniTask WaitUntilFinished()
    {
        await gameFinished.Task;
    }

    private bool IsGameFinished()
    {
        foreach (Plank plank in planks)
            if (plank.isActiveAndEnabled) return false;
        return true;
    }

    public void ResetMiniGame()
    {
        BoltMiniGame reloadedPrefab = Instantiate(boltMiniGame);
        reloadedPrefab.boltMiniGame = boltMiniGame;
        reloadedPrefab.gameFinished = new UniTaskCompletionSource();
        reloadedPrefab.currentCamera = currentCamera;
        reloadedPrefab.player = player;
        reloadedPrefab.buttons = buttons;
        reloadedPrefab.showMiniGame = true;
        for (int i = 0; i < reloadedPrefab.buttons.transform.childCount; i++)
        {
            Transform buttonObject = reloadedPrefab.buttons.transform.GetChild(i);
            if (buttonObject.name == "ResetButton")
            {
                Button button = buttonObject.GetComponent<Button>();
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => reloadedPrefab.ResetMiniGame());
            }
        }
        Debug.Log("Made reset");
        Destroy(gameObject, Time.deltaTime);
    }
}
