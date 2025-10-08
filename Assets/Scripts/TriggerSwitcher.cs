using System.Collections;
using UnityEngine;

public class TriggerSwitcher : MonoBehaviour
{
    [SerializeField] private OnTriggerEnter elevatorTriggerAbove;
    [SerializeField] private OnTriggerEnter elevatorTriggerBelow;
    [SerializeField] private OnTriggerEnter initialTrigger;

    private bool areElevatorTriggersDisabled = false;

    private void Awake()
    {
        elevatorTriggerAbove.AddListener(() =>
        {
            areElevatorTriggersDisabled = true;
        });
    }

    public void DisableInitial() => initialTrigger.gameObject.SetActive(false);

    public void DisableElevatorTriggers()
    {
        elevatorTriggerAbove.gameObject.SetActive(false);
        elevatorTriggerBelow.gameObject.SetActive(false);
        areElevatorTriggersDisabled = true;
    }

    public bool AreElevatorTriggersDisabled() => areElevatorTriggersDisabled;
}
