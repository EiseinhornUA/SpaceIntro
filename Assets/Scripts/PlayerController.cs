using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerRotator))]
[RequireComponent(typeof(Interactor))]

public class PlayerController : MonoBehaviour
{
    private Vector2 movementDirection;
    private PlayerMovement playerMovement;
    private PlayerRotator playerRotator;
    private Interactor interactor;
    [SerializeField] private Joystick joystick;
    [SerializeField] private Animator animator;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerRotator = GetComponent<PlayerRotator>();
        interactor = GetComponent<Interactor>();
    }

    private void Update()
    {
        OnPlayerMove();
    }

    private void OnPlayerMove()
    {
        movementDirection = new Vector2(joystick.Horizontal, joystick.Vertical);
        playerRotator.RotatePlayer(movementDirection);
        playerMovement.Move(movementDirection);
        animator.SetFloat("Speed", movementDirection.magnitude);
    }
}
