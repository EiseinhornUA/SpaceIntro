using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(Interactor))]
public class PlayerController : MonoBehaviour
{
    private Vector2 movementDirection;
    private PlayerMovement playerMovement;
    private Interactor interactor;
    [SerializeField] private Joystick joystick;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        interactor = GetComponent<Interactor>();
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
