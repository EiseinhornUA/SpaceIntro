using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class TetrominoView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private RectTransform rectTransform;
    public UnityEvent onBeginDrag { get; set; } = new();
    public UnityEvent onDrag { get; set; } = new();
    public UnityEvent onEndDrag { get; set; } = new();
    public UnityEvent onClick { get; set; } = new();

    [SerializeField] private RectTransform outline;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        onBeginDrag.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        onDrag.Invoke();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        onEndDrag.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick.Invoke();
    }

    public void RotateClockwise(float rotationDurationSeconds)
    {
        rectTransform.DORotate(rectTransform.rotation.eulerAngles + new Vector3(0, 0, -90), rotationDurationSeconds);
        outline.DORotate(outline.rotation.eulerAngles + new Vector3(0, 0, -90), rotationDurationSeconds);
    }

    public void ShowOutline() => outline.gameObject.SetActive(true);
    public void HideOutline() => outline.gameObject.SetActive(false);

    public void SetOutlinePosition(Vector2 position, Vector2 parentPosition)
    {
        outline.localPosition = position + (Vector2)rectTransform.localPosition - parentPosition;
    }

    public void RenderOutlineAbove() => outline.SetAsLastSibling();
    public void RenderOutlineBelow() => transform.SetAsLastSibling();
    public bool IsOutlineVisible() => outline.gameObject.activeSelf;
    public void ResetOutlinePosition() => outline.localPosition = Vector3.zero;
    public void SetColor(Color tetrominoColor) => GetComponent<Image>().color = tetrominoColor;
}