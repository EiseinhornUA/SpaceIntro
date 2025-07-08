using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(CircleCollider2D))]
public class Interactable : MonoBehaviour
{
    [SerializeField] private UnityEvent onInteract = new();
    private InteractionPrompt interactionPrompt;
    private InteractionView interactionView;
    private UniTaskCompletionSource interactionTCS;
    private bool isActive = true;

    private void Start()
    {
        interactionPrompt = FindObjectOfType<InteractionPrompt>(includeInactive: true);
        interactionView = FindObjectOfType<InteractionView>(includeInactive: true);
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        collider.isTrigger = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!isActive) return;
        if (!IsPlayer(collision)) return;
        interactionPrompt.SetPosition(transform.position);
    }

    public UniTask WaitForInteraction()
    {
        interactionTCS = new UniTaskCompletionSource();
        return interactionTCS.Task;
    }

    public void OnInteract()
    {
        onInteract.Invoke();
        interactionPrompt.Hide();
        interactionView.Hide();

        interactionTCS?.TrySetResult(); // Resume WaitForInteraction
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActive) return;
        if (!IsPlayer(collision)) return;
        interactionPrompt.SetPosition(transform.position);
        interactionPrompt.Show();
        interactionView.Show();
        interactionView.AddListener(OnInteract);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isActive) return;
        if (!IsPlayer(collision)) return;
        interactionPrompt.Hide();
        interactionView.Hide();
        interactionView.RemoveListener(OnInteract);
    }
    private static bool IsPlayer(Collider2D collision)
    {
        return collision.CompareTag("Player");
    }

    internal void Activate() => isActive = true;
    internal void Deactivate()
    {
        isActive = false;
        interactionPrompt.Hide();
        interactionView.Hide();
        interactionView.RemoveListener(OnInteract);
    }
}
