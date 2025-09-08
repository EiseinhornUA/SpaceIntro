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
    [SerializeField] public UnityEvent onInteract = new();
    private InteractionView interactionView;
    private CircleCollider2D circleCollider;
    private UniTaskCompletionSource interactionTCS;

    [SerializeField] public bool isActive = false;

    private void Awake()
    {
        interactionView = FindObjectOfType<InteractionView>(includeInactive: true);
        circleCollider = GetComponent<CircleCollider2D>();
        circleCollider.isTrigger = true;
    }

    public UniTask WaitForInteraction()
    {
        interactionTCS = new UniTaskCompletionSource();
        return interactionTCS.Task;
    }

    public void OnInteract()
    {
        onInteract.Invoke();
        interactionView.Hide();

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
