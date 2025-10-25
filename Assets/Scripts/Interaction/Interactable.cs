using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(CircleCollider2D))]
public class Interactable : MonoBehaviour
{
    [field: SerializeField] public UnityEvent onInteract { get; private set; } = new();
    private InteractionView interactionView;
    private CircleCollider2D circleCollider;
    private UniTaskCompletionSource interactionTCS;
    private Player player;

    [SerializeField] private bool isActive = false;

    private void Awake()
    {
        interactionView = FindObjectOfType<InteractionView>(includeInactive: true);
        circleCollider = GetComponent<CircleCollider2D>();
        circleCollider.isTrigger = true;
        player = FindObjectOfType<Player>();
    }

    public UniTask WaitForInteraction()
    {
        interactionTCS = new UniTaskCompletionSource();
        return interactionTCS.Task;
    }

    [ContextMenu("Interact")]
    public void OnInteract()
    {
        onInteract.Invoke();
        interactionView.Hide();
        player.StopMovement();

        interactionTCS?.TrySetResult(); // Resume WaitForInteraction
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!isActive) return;
        if (!IsPlayer(collision)) return;

        if(!interactionView.gameObject.activeSelf)
        {
            interactionView.SetPosition(GetViewPosition());
            interactionView.Show();
            interactionView.AddListener(OnInteract);
        }

    }

    private Vector3 GetViewPosition()
    {
        return transform.position + new Vector3(circleCollider.offset.x, circleCollider.offset.y, 0);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isActive) return;
        if (!IsPlayer(collision)) return;
        interactionView.Hide();
        interactionView.RemoveListener(OnInteract);
    }

    private static bool IsPlayer(Collider2D collision)
    {
        return collision.CompareTag("Player");
    }

    public void Activate() => isActive = true;
    public void Deactivate()
    {
        isActive = false;
        interactionView.Hide();
        interactionView.RemoveListener(OnInteract);
    }

}
