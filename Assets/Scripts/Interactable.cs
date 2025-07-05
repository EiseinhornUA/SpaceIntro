using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(CircleCollider2D))]
public class Interactable : MonoBehaviour
{
    [SerializeField] private UnityEvent onInteract;
    private InteractionPrompt interactionPrompt;
    private InteractionView interactionView;

    private void Start()
    {
        interactionPrompt = FindObjectOfType<InteractionPrompt>(includeInactive: true);
        interactionView = FindObjectOfType<InteractionView>(includeInactive: true);
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        collider.isTrigger = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!IsPlayer(collision)) return;
        interactionPrompt.SetPosition(transform.position);
    }

    public void OnInteract()
    {
        onInteract.Invoke();
        interactionPrompt.Hide();
        interactionView.Hide();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsPlayer(collision)) return;
        interactionPrompt.SetPosition(transform.position);
        interactionPrompt.Show();
        interactionView.Show();
        interactionView.AddListener(OnInteract);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!IsPlayer(collision)) return;
        interactionPrompt.Hide();
        interactionView.Hide();
        interactionView.RemoveListener(OnInteract);
    }
    private static bool IsPlayer(Collider2D collision)
    {
        return collision.CompareTag("Player");
    }
}
