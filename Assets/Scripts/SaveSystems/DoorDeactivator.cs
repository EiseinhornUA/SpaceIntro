
using UnityEngine;

public class DoorDeactivator : MonoBehaviour
{
    [SerializeField] private EngineeringDoor door;

    private bool isDoorOpened = false;

    public bool IsDoorOpened() => isDoorOpened;

    private void Awake()
    {
        door.OnDoorDestroyed += SetDoorOpened;
    }

    public void SetDoorOpened()
    {
        isDoorOpened = true;
    }

    public void OpenDoor()
    {
        door.Open();
    }
}
