using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

[RequireComponent (typeof (Controller2D), (typeof (AnimationHandler)))]
public class Player : MonoBehaviour {
    public float maxJumpHeight = 4;
	public float minJumpHeight = 1;
	public float timeToJumpApex = .4f;
	float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .1f;
	[SerializeField] private float moveSpeed = 6;

	public Vector2 wallJumpClimb;
	public Vector2 wallJumpOff;
	public Vector2 wallLeap;

	public float wallSlideSpeedMax = 3;
	public float wallStickTime = .25f;
	float timeToWallUnstick;

	float gravity;
	float maxJumpVelocity;
	float minJumpVelocity;
	Vector3 velocity;
	[SerializeField] private float velocityXSmoothing = 4;

	Controller2D controller;

	Vector2 directionalInput;
	bool wallSliding;
	int wallDirX;

	private AnimationHandler animationHandler;
    private PlayerRotator playerRotator;
    private float previousDirectionInput;
	private Joystick joystick;
    [SerializeField] private float jumpThreshold = .8f;

    void Start() {
		controller = GetComponent<Controller2D> ();
        animationHandler = GetComponent<AnimationHandler>();
        playerRotator = GetComponent<PlayerRotator>();
        joystick = FindObjectOfType<Joystick>();

        gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (gravity) * minJumpHeight);
	}

	void Update() {
		CalculateVelocity ();
		HandleWallSliding ();

		OnPlayerMove();
		OnPlayerJump();

        controller.Move(velocity * Time.deltaTime, directionalInput);
        animationHandler.SetHorizontalSpeed(directionalInput.x);

		if (playerRotator)
		{
			playerRotator.RotatePlayer(velocity.x);
			//if (controller.collisions.climbingSlope)
			//	playerRotator.RotateParent(controller.collisions.slopeAngle);
			//if (!controller.collisions.climbingSlope)
			//	playerRotator.RotateParent(0);
        }


        if (controller.collisions.above || controller.collisions.below) {
			if (controller.collisions.slidingDownMaxSlope) {
				velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
			} else {
				velocity.y = 0;
			}
		}
	}

	public void SetDirectionalInput (Vector2 input) {
		directionalInput = input;
	}

	public void OnJumpInputDown() {
		if (wallSliding) {
			if (wallDirX == directionalInput.x) {
				velocity.x = -wallDirX * wallJumpClimb.x;
				velocity.y = wallJumpClimb.y;
			}
			else if (directionalInput.x == 0) {
				velocity.x = -wallDirX * wallJumpOff.x;
				velocity.y = wallJumpOff.y;
			}
			else {
				velocity.x = -wallDirX * wallLeap.x;
				velocity.y = wallLeap.y;
			}
		}
		if (controller.collisions.below) {
			if (controller.collisions.slidingDownMaxSlope) {
				if (directionalInput.x != -Mathf.Sign (controller.collisions.slopeNormal.x)) { // not jumping against max slope
					velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
					velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
				}
			} else {
				velocity.y = maxJumpVelocity;
			}
		}
	}

	public void OnJumpInputUp() {
		if (velocity.y > minJumpVelocity) {
			velocity.y = minJumpVelocity;
		}
	}
		

	void HandleWallSliding() {
		wallDirX = (controller.collisions.left) ? -1 : 1;
		wallSliding = false;
		if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0) {
			wallSliding = true;

			if (velocity.y < -wallSlideSpeedMax) {
				velocity.y = -wallSlideSpeedMax;
			}

			if (timeToWallUnstick > 0) {
				velocityXSmoothing = 0;
				velocity.x = 0;

				if (directionalInput.x != wallDirX && directionalInput.x != 0) {
					timeToWallUnstick -= Time.deltaTime;
				}
				else {
					timeToWallUnstick = wallStickTime;
				}
			}
			else {
				timeToWallUnstick = wallStickTime;
			}

		}

	}

	void CalculateVelocity() {
		float targetVelocityX = directionalInput.x * moveSpeed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);
		velocity.y += gravity * Time.deltaTime;
	}

	//public void OnPlayerMove(InputAction.CallbackContext context)
	//{
	//	directionalInput.x = context.ReadValue<Vector2>().x * -1;
	//	//if ((directionalInput.x != previousDirectionInput) && (directionalInput.x != 0 || previousDirectionInput != 0))
	//	//{
	//	//	animationHandler.Turn();
	//	//          previousDirectionInput = directionalInput.x;
	//	//      }
	//}

	private void OnPlayerMove()
	{
		directionalInput.x = (joystick.Horizontal == 0) ? 0 : Mathf.Sign(joystick.Horizontal)  * -1;
    }

	//   public void OnPlayerJump(InputAction.CallbackContext context)
	//{
	//	if (context.performed)
	//	{
	//		OnJumpInputDown();
	//		animationHandler.Jump();
	//	}
	//	if (context.canceled)
	//	{
	//		OnJumpInputUp();
	//	}
	//}

	private void OnPlayerJump()
	{
		if (joystick.Vertical >= jumpThreshold)
			OnJumpInputDown();
		else
			OnJumpInputUp();
	}
}
