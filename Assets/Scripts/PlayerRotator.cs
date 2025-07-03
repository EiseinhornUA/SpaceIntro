using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotator : MonoBehaviour
{
    [SerializeField] private GameObject modelParent;

    public void RotatePlayer(Vector2 direction)
    {
        if (direction == Vector2.zero) return;

        float angle = Vector2.SignedAngle(Vector2.up, direction);
        modelParent.transform.GetChild(0).rotation = Quaternion.Euler(0, -angle, 0);
    }
}
