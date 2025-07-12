using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotator : MonoBehaviour
{
    [SerializeField] private GameObject modelParent;

    internal void RotatePlayer(float direction)
    {
        int angle = (direction == 0) ? 180 : 90;
        Vector3 currentRotation = modelParent.transform.GetChild(0).transform.localRotation.eulerAngles;
        modelParent.transform.GetChild(0).transform.localRotation = Quaternion.Euler(currentRotation.x, Mathf.Sign(direction) * angle, currentRotation.z);
    }

    internal void RotateParent(float angle)
    {
        Vector3 currentRotation = modelParent.transform.rotation.eulerAngles;
        modelParent.transform.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, -angle);
    }
}
