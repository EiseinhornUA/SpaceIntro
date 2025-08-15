using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsLimiter : MonoBehaviour
{
    [SerializeField] private int fps = 30;
    [SerializeField] private bool realtimeChange = false;

    private void Awake()
    {
        Application.targetFrameRate = fps;
    }


    private void Update()
    {
        if (realtimeChange)
            if (fps != Application.targetFrameRate)
                Application.targetFrameRate = fps;
    }
}
