using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerMovement), typeof(PlayerRotator), typeof(CharacterLoader))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Joystick joystick;

    private PlayerMovement movement;
    private PlayerRotator rotator;
    private Animator animator;

    private void Start()
    {
        movement = GetComponent<PlayerMovement>();
        rotator = GetComponent<PlayerRotator>();
    }

    private void Update()
    {
        animator = GetComponent<CharacterLoader>().GetAnimator();
        if (!animator) return;

        Vector2 input = new Vector2(joystick.Horizontal, joystick.Vertical);

        Debuger debuger = GameObject.FindAnyObjectByType<Debuger>();
        if (debuger) debuger.ShowDebugText(input.ToString());

        rotator.RotatePlayer(movement.GetCurrentVelocity().normalized);

        if (animator) animator.SetFloat("Speed", movement.GetCurrentSpeed());

        movement.Move(input);
    }
}
