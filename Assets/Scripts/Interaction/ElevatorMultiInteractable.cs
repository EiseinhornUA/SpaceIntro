using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(ElevatorControlPanel))]
public class ElevatorMultiInteractable : MonoBehaviour
{
    [Range(2, 3)]
    [SerializeField] private int buttonAmount;
    private ElevatorMultiInteractionView multiInteractionView;
    private ElevatorControlPanel elevatorControlPanel;
    private PlayerInOutElevator elevator;
    private CircleCollider2D circleCollider;

    [SerializeField] private bool isActive = false;

    private void Awake()
    {
        multiInteractionView = FindObjectOfType<ElevatorMultiInteractionView>(true);
        elevatorControlPanel = GetComponent<ElevatorControlPanel>();
        elevator = elevatorControlPanel.GetElevator();
        circleCollider = GetComponent<CircleCollider2D>();
        circleCollider.isTrigger = true;
    }

    public void OnInteract(int interactionIndex)
    {
        elevator.GoToFloor(elevatorControlPanel.GetCurrentFloorIndex(), interactionIndex);
        multiInteractionView.Hide();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!isActive) return;
        if (!IsPlayer(collision)) return;


        if (!multiInteractionView.IsActive())
        {
            multiInteractionView.SetPosition(GetViewPosition());
            multiInteractionView.Show(buttonAmount);
            multiInteractionView.AddListener(OnInteract);
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
        multiInteractionView.Hide();
        multiInteractionView.RemoveListeners();
    }

    private static bool IsPlayer(Collider2D collision)
    {
        return collision.CompareTag("Player");
    }

    public void Activate() => isActive = true;
    public void Deactivate()
    {
        isActive = false;
        multiInteractionView.Hide();
        multiInteractionView.RemoveListeners();
    }
}
