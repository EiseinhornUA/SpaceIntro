using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PhysicsController))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] public float maxMoveSpeed = 16;
    [SerializeField] private float jumpHeight = 2.5f;
    [SerializeField] private float jumpImpulse;
    [SerializeField] private float accelerationTime = 0.125f;

    [Header("Physics")]
    private float horizontalInput;
    private float velocityXSmoothing;

    private PhysicsController physicsController;

    private void Start()
    {
        physicsController = GetComponent<PhysicsController>();
    }

    private void Update()
    {
        jumpImpulse = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(physicsController.GetGravity()));
        HandleMovement();
    }

    private void HandleMovement()
    {
        float targetVelocity = horizontalInput * maxMoveSpeed;
        if (physicsController.collisions.below || physicsController.collisions.left || physicsController.collisions.right)
        {
            float smoothTime = accelerationTime;

            float newVelocityX = Mathf.SmoothDamp(
            physicsController.velocity.x,
            targetVelocity,
            ref velocityXSmoothing,
            smoothTime
            );
            physicsController.SetVelocityX(newVelocityX);
        }
    }

    public void OnPlayerMove(InputAction.CallbackContext context)
    {
        horizontalInput = context.ReadValue<Vector2>().x;
    }

    public void OnPlayerJump(InputAction.CallbackContext context)
    {
        if (context.performed && physicsController.collisions.below)
        {
            physicsController.AddImpulse(Vector2.up * jumpImpulse);
        }
    }

    //public void OnPlayerInteract(InputAction.CallbackContext context)
    //{
    //    if (context.performed)
    //    {
    //        interactor.Interact();
    //    }
    //}
}
