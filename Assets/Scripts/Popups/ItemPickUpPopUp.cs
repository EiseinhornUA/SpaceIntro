using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemPickUpPopUp : Popup
{
    [SerializeField] float moveToInventoryDuration = 0.75f;
    [SerializeField] float inventoryIconScaleDuration = 0.25f;
    [SerializeField] float inventoryIconScale = 1.5f;
    [SerializeField] GameObject invetoryIcon;


    [SerializeField] Transform debugObjectToMove;

    private Image itemIcon;

    private void Awake()
    {
        itemIcon = gameObject.GetComponent<Image>();
    }

    private UniTaskCompletionSource taskCompletionSource;
    [SerializeField] private float offsetX = 0f;
    [SerializeField] private float offsetY = 100f;

    public async UniTask MoveIconToPosition(Transform objectTransform)
    {
        Hud hud = Hud.Instance;

        hud.HideHud();
        Vector2 startScreenPosition = Camera.main.WorldToScreenPoint(objectTransform.position);
        var inventoryIconRectTransform = invetoryIcon.GetComponent<RectTransform>();
        RectTransform itemIconTransform = itemIcon.GetComponent<RectTransform>();
        CanvasScaler canvasScaler = this.GetComponentInParent<CanvasScaler>();
        Canvas canvas = this.GetComponentInParent<Canvas>();

        startScreenPosition.x /= Screen.width / canvas.GetComponent<RectTransform>().sizeDelta.x;
        startScreenPosition.y /= Screen.width / canvas.GetComponent<RectTransform>().sizeDelta.y;

        Vector2 pos = itemIconTransform.anchoredPosition;
        pos.x = startScreenPosition.x + offsetX;
        pos.y = startScreenPosition.y + offsetY;
        itemIconTransform.anchoredPosition = pos;
        Debug.Log($"Canvas scale: {canvasScaler.scaleFactor}, Canvas size: {canvas.GetComponent<RectTransform>().sizeDelta}");
        var targetScreenPosition = new Vector2(canvas.GetComponent<RectTransform>().sizeDelta.x + inventoryIconRectTransform.anchoredPosition.x,
            canvas.GetComponent<RectTransform>().sizeDelta.y + inventoryIconRectTransform.anchoredPosition.y);
        await itemIconTransform.DOAnchorPos(targetScreenPosition, moveToInventoryDuration).SetEase(Ease.InCubic);
        FadeOut();
        var initialInventoryIconScale = inventoryIconRectTransform.localScale;
        await inventoryIconRectTransform.DOScale(new Vector3(inventoryIconScale, inventoryIconScale, inventoryIconScale),
            inventoryIconScaleDuration * 0.5f);
        await inventoryIconRectTransform.DOScale(initialInventoryIconScale, inventoryIconScaleDuration * 0.5f);
        taskCompletionSource?.TrySetResult();

        hud.ShowHud();
        Hide();
    }

    public async UniTask WaitForMove()
    {
        taskCompletionSource = new UniTaskCompletionSource();
        await taskCompletionSource.Task;
    }

    public void ShowPickedUpItem(Flow flow, ValueInput itemInput, ValueInput gameObjectInput)
    {
        Sprite icon = flow.GetValue<ItemSO>(itemInput).icon;
        itemIcon.sprite = icon;
        MoveIconToPosition(flow.GetValue<GameObject>(gameObjectInput).transform).Forget();
    }

    public override void Show()
    {
        base.Show();
        Color alphaColor = itemIcon.color;
        alphaColor.a = 1f;
        itemIcon.color = alphaColor;
    }

    private void FadeOut()
    {
        Color alphaColor = itemIcon.color;
        alphaColor.a = 0f;
        itemIcon.color = alphaColor;
    }

    [ContextMenu("Debug Fly")]
    private void DebugFly()
    {
        Show();
        MoveIconToPosition(debugObjectToMove);
    }
}
