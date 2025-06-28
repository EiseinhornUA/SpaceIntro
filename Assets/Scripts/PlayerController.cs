using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerController : MonoBehaviour
{
    private Vector2 movementDirection;
    private PlayerMovement playerMovement;
    [SerializeField] private Joystick joystick;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        OnPlayerMove();
    }

    private void OnPlayerMove()
    {
        movementDirection = new Vector2(joystick.Horizontal, joystick.Vertical);
        playerMovement.Move(movementDirection);
    }
}
