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
    [SerializeField] private UnityEvent onInteract = new();
    private InteractionPrompt interactionPrompt;
    private InteractionView interactionView;
    private CircleCollider2D circleCollider;
    private UniTaskCompletionSource interactionTCS;
    
    [SerializeField] private bool isActive = false;

    private void Start()
    {
        interactionPrompt = FindObjectOfType<InteractionPrompt>(includeInactive: true);
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
        interactionPrompt.Hide();
        interactionView.Hide();

        interactionTCS?.TrySetResult(); // Resume WaitForInteraction
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActive) return;
        if (!IsPlayer(collision)) return;
        interactionPrompt.SetPosition(GetPromptPosition());
        interactionPrompt.Show();
        interactionView.Show();
        interactionView.AddListener(OnInteract);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!isActive) return;
        if (!IsPlayer(collision)) return;
        interactionPrompt.SetPosition(GetPromptPosition());
    }

    private Vector3 GetPromptPosition()
    {
        return transform.position + new Vector3(circleCollider.offset.x, circleCollider.offset.y, 0);
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
