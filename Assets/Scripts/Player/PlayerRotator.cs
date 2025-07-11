using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotator : MonoBehaviour
{
    [SerializeField] private GameObject modelParent;

    public void RotatePlayer(float direction)
    {
        int angle = (direction == 0) ? 180 : 90;
        modelParent.transform.GetChild(0).rotation = Quaternion.Euler(0, Mathf.Sign(direction) * angle, 0);
    }
}
