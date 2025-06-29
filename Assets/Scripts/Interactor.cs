using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public interface IInteractable
{
    public void OnInteract();
}

public class Interactor : MonoBehaviour
{
    [SerializeField] private float interactionRange = 2f;
    [SerializeField] private InteractionPrompt interactionPrompt;
    [SerializeField] private Button interactionButton;
    private bool interacting = false;

    private void Update()
    {
        if (!TryGetNearestInteractable(out Interactable interactable)) return;
        if (IsInRange(interactable) && !interacting)
        {
            interactionPrompt.Show();
            interactionPrompt.SetPosition(interactable.transform.position);
        }
        else
            interactionPrompt.Hide();
    }

    internal void Interact()
    {
        if (!TryGetNearestInteractable(out Interactable interactable)) return;
        if (IsInRange(interactable))
        {
            interactable.OnInteract();
            Interacting();
        }
    }

    private void Interacting()
    {
        interacting = true;
        interactionButton.gameObject.SetActive(false);
    }

    private void NotInteracting() => interacting = false;

    private bool IsInRange(Interactable interactable)
    {
        return Vector3.Distance(transform.position, interactable.transform.position) <= interactionRange;
    }

    private bool TryGetNearestInteractable(out Interactable interactable)
    {
        Vector3 origin = transform.position;
        interactable = FindObjectsByType<Interactable>(FindObjectsSortMode.None)
            .OrderBy(obj => Vector3.Distance(origin, obj.transform.position))
            .FirstOrDefault();
        return interactable != null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
