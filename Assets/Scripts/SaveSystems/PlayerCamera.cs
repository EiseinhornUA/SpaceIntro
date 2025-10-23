using Cinemachine;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private float resolutionProportion;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    private CinemachineFramingTransposer framingTransposer;

    private void Awake()
    {
        resolutionProportion = (Screen.width * 0.5625f) / Screen.height;

        virtualCamera = GetComponent<CinemachineVirtualCamera>();

        framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

        Vector3 offset = framingTransposer.m_TrackedObjectOffset;
        offset.z = CalculateCameraOffset(resolutionProportion);
        framingTransposer.m_TrackedObjectOffset = offset;
    }

    private void Update()
    {
        if (framingTransposer == null) return;

        resolutionProportion = (Screen.width * 0.5625f) / Screen.height;
        Vector3 offset = framingTransposer.m_TrackedObjectOffset;
        offset.z = CalculateCameraOffset(resolutionProportion);
        framingTransposer.m_TrackedObjectOffset = offset;
    }

    private float CalculateCameraOffset(float resolutionProportion)
    {
        // |10f - resolutionProportion * 10f| + (10f - resolutionProportion * 10f) + 1f;
        return Mathf.Abs(10f - resolutionProportion * 10f) - resolutionProportion * 10f + 11f;
    }
}
