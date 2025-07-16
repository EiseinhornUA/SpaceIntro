using UnityEngine;

public class PlayerRotator : MonoBehaviour
{
    [SerializeField] private GameObject modelParent;
    [SerializeField] private float rotationSpeed = 10f;

    private Quaternion targetRotation = Quaternion.Euler(0, 90, 0);

    private void Update()
    {
        Transform model = modelParent.transform.GetChild(0);
        model.localRotation = Quaternion.Lerp(model.localRotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    internal void RotatePlayer(float direction)
    {
        if (direction == 0) return;

        float targetY = (direction > 0) ? 90f : -90f;
        Vector3 currentEuler = modelParent.transform.GetChild(0).localRotation.eulerAngles;
        targetRotation = Quaternion.Euler(currentEuler.x, targetY, currentEuler.z);
    }
}
