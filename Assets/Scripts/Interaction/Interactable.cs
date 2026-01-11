using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private static Interactable currentInteractable;

    private void Awake()
    {
        interactionView = FindObjectOfType<InteractionView>(includeInactive: true);
        circleCollider = GetComponent<CircleCollider2D>();
        circleCollider.isTrigger = true;
        player = FindObjectOfType<Player>();
    }

    //private void Update()
    //{
    //    player.colliderToPlayerDistances.Clear();
    //}

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
        if (!isActive) {
            player.collidedInteractions.Remove(this);
            return;
        }
        if (!IsPlayer(collision)) return;

        if (!player.collidedInteractions.Contains(this) && gameObject.activeInHierarchy)
            player.collidedInteractions.Add(this);

        float currentDist = Vector2.Distance(transform.position, player.transform.position);

        float minDist = float.MaxValue;
        Interactable closest = null;

        foreach (var collidedInteraction in player.collidedInteractions)
        {
            if (collidedInteraction == null) continue;
            if (!collidedInteraction.isActive) continue;

            float dist = Vector2.Distance(collidedInteraction.transform.position, player.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = collidedInteraction;
            }
        }

        if (closest != this)
        {
            return;
        }

        if (currentInteractable != this || !interactionView.isActiveAndEnabled)
        {
            currentInteractable = this;
            
            interactionView.SetPosition(GetViewPosition());
            interactionView.Show();
            interactionView.RemoveAllListeners();
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

        if (currentInteractable == this)
        {
            currentInteractable = null;
            interactionView.Hide();
            interactionView.RemoveAllListeners();
        }
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
