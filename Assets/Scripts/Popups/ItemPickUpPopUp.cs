using Cysharp.Threading.Tasks;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemPickUpPopUp : Popup
{
    [SerializeField] float moveToInventoryDuration = 0.75f;
    [SerializeField] RectTransform invetoryIconPosition;

    private Transform itemTransform;
    private Image itemIcon;

    //public void SaveItemTransform(Transform objectTransform)
    //{
    //    itemTransform = objectTransform;
    //}

    public void SetIcon(Sprite icon)
    {
        itemIcon = GetComponent<Image>();
        itemIcon.sprite = icon;
    }

    public async UniTask MoveIconToPosition(Transform objectTransform)
    {
        Vector2 startScreenPosition = Camera.main.WorldToScreenPoint(objectTransform.position);
        var inventoryIconRectTransform = invetoryIconPosition.GetComponent<RectTransform>();
        RectTransform itemIconTransform = itemIcon.GetComponent<RectTransform>();
        var targetScreenPosition = new Vector2(Screen.width + inventoryIconRectTransform.anchoredPosition.x, Screen.height + inventoryIconRectTransform.anchoredPosition.y);
        itemIconTransform.anchoredPosition = startScreenPosition;
        await itemIconTransform.DOAnchorPos(targetScreenPosition, moveToInventoryDuration);
        Hide();
    }

public static Vector2 WorldToAnchoredPosition(Camera camera, RectTransform rectTransform, Vector3 worldPosition)
{
    // 1. Convert world position to screen position
    Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(camera, worldPosition);

    // 2. Convert screen position to local position on the RectTransform
    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, camera, out Vector2 localPoint))
    {
        return localPoint;
    }
    else
    {
        Debug.LogError("World position is not within the RectTransform.");
        return Vector2.zero;
    }
}
}
