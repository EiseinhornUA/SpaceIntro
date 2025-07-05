using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerRotator))]
[RequireComponent(typeof(CharacterLoader))]

public class PlayerController : MonoBehaviour
{
    private Vector2 movementDirection;
    private PlayerMovement playerMovement;
    private PlayerRotator playerRotator;
    private CharacterLoader characterLoader;
    private Animator animator;
    [SerializeField] private Joystick joystick;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerRotator = GetComponent<PlayerRotator>();
        characterLoader = GetComponent<CharacterLoader>();
        animator = characterLoader.GetAnimator();
    }

    private void Update()
    {
        OnPlayerMove();
    }

    private void OnPlayerMove()
    {
        movementDirection = new Vector2(joystick.Horizontal, joystick.Vertical);
        Debuger debuger = GameObject.FindAnyObjectByType<Debuger>();
        if(debuger) debuger.ShowDebugText(movementDirection.ToString());
        playerRotator.RotatePlayer(playerMovement.GetCurrentVelocity().normalized);
        animator.SetFloat("Speed", playerMovement.GetCurrentSpeed());
        playerMovement.Move(movementDirection);
    }
}
