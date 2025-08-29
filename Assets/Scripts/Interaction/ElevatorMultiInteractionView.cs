using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ElevatorMultiInteractionView : MonoBehaviour
{
    [SerializeField] private List<Button> interactionButtons;
    private Vector3 worldPosition;

    // Store references to the exact delegates
    private List<UnityAction> cachedListeners = new List<UnityAction>();
    private int currentButtonAmount;

    private void Start()
    {
        Hide();
    }

    private void Update()
    {
        UpdatePosition();
    }

    internal void AddListener(UnityAction<int> onInteract)
    {
        cachedListeners.Clear();

        for (int i = 0; i < currentButtonAmount; i++)
        {
            int capturedIndex = i;
            interactionButtons[i].onClick.AddListener(() => onInteract(capturedIndex));
        }
    }

    internal void RemoveListeners()
    {
        interactionButtons.ForEach(button => button.onClick.RemoveAllListeners());
        cachedListeners.Clear();
    }

    public void SetPosition(Vector3 position) => worldPosition = position;

    public void UpdatePosition()
    {
        GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(worldPosition);
    }

    public void Show(int buttonAmount)
    {
        currentButtonAmount = buttonAmount;
        gameObject.SetActive(true);
        for (int i = 0; i < buttonAmount; i++)
        {
            interactionButtons[i].gameObject.SetActive(true);
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        interactionButtons.ForEach(button => button.gameObject.SetActive(false));
    }

    public bool IsActive() => gameObject.activeSelf;
}
