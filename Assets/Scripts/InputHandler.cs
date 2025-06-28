using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private Joystick joystick;

    private void Start()
    {
        joystick = GetComponent<Joystick>();
        print(joystick.Horizontal);
        print(joystick.Vertical);
    }
}
