using Cinemachine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class RobotFollow : MonoBehaviour
{
    private const float ApproachingThreshold = 0.1f;
    [SerializeField] private Transform player;
    [SerializeField] private float followDistance = 2.5f;
    [SerializeField] private float followSpeed = 10f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 1.5f, -0.5f);
    [SerializeField] private float frequency = 0.5f;
    [SerializeField] private float amplitude = 0.01f;
    [SerializeField]
    private bool isRobotOn = false;
    [SerializeField]
    private bool isFollowingOn = true;
    private float yzSpeedNormalizer = 0.2f;
    private float idleOffsetY = 0f;

    [SerializeField] private float accelerationDistance = 5f;
    [SerializeField] private float breakSpeed = 2f;
    [SerializeField] private Transform doorTransform;
    [SerializeField] private float hitPointOffsetX = 0f;
    [SerializeField] private float hitPointOffsetY = 0f;
    [SerializeField] private float hitPointOffsetZ = 0f;
    [SerializeField] private float breakingForce = 15f;
    [SerializeField] private float disableTimeAfterHit = 1f;
    [SerializeField] private float rotateToDoorTime = 0.3f;
    private float robotColliderRadius;

    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] private float cameraOffsetY = -2f;
    [SerializeField] private Transform playerParent;

    [SerializeField] private ReportContainerView reportContainerView;

    private void Awake()
    {
        robotColliderRadius = GetComponent<CircleCollider2D>().radius;
    }

    private void Update()
    {
        if (isRobotOn)
        {
            ApplyIdleMovement();

            if (isFollowingOn)
            {
                FollowPlayer();
                RotateTowardsPlayer();
            }
        }
    }

    public void TurnOnRobot() => TurnOnRobotAsync().Forget();

    public async UniTask TurnOnRobotAsync()
    {
        isRobotOn = true;
        gameObject.GetComponent<SoundPlayer>().Play();
        await UniTask.WaitForSeconds(gameObject.GetComponent<SoundPlayer>().clipToPlay.length);
        gameObject.GetComponent<AudioSource>().Play();
    }

    public void TurnOffRobot()
    {
        isRobotOn = false;
    }

    public void TurnOnRobotFollowing()
    {
        isFollowingOn = true;
    }

    public void TurnOffRobotFollowing()
    {
        isFollowingOn = false;
    }

    public bool IsRobotOn() => isRobotOn;


    public void FollowPlayer()
    {
        Vector3 robotPosition = transform.position;

        Vector3 direction = player.position - transform.position + offset;

        if (Mathf.Abs(direction.x) > followDistance)
        {
            robotPosition.x += (Mathf.Sign(direction.x) - ((followDistance - ApproachingThreshold) / direction.x))
                * followSpeed * Time.deltaTime;
        }

        robotPosition.y += yzSpeedNormalizer * direction.y * followSpeed * Time.deltaTime;
        robotPosition.z += yzSpeedNormalizer * direction.z * followSpeed * Time.deltaTime;
        transform.position = robotPosition;    
    }

    [ContextMenu("Destroy Door")]
    public async UniTask DestroyDoor()
    {
        playerParent.GetComponent<Player>().EnableControls(false);
        TurnOffRobot();
        await RotateRobotTowardsDoor();
        await MoveBackToAccelerate();
        await Accelerate();
        await EnableRobotsPhysic();
        KnockDownDoor();
        playerParent.GetComponent<Player>().EnableControls(true);
        await UniTask.Delay(System.TimeSpan.FromSeconds(disableTimeAfterHit));

        await DisableRobotPhysic();

        doorTransform.gameObject.layer = 13;
        doorTransform.GetChild(0).gameObject.layer = 13;
    }

    private async UniTask RotateRobotTowardsDoor()
    {
        await transform.DOLookAt(new Vector3(doorTransform.position.x + hitPointOffsetX, 
            doorTransform.position.y + hitPointOffsetY 
            - (doorTransform.GetComponentInParent<BrokenSlidingDoor>().openLength * 
            (doorTransform.GetComponentInParent<BrokenSlidingDoor>().openPercent / 100f)), 
            doorTransform.position.z + hitPointOffsetZ), rotateToDoorTime);
    }

    private async UniTask DisableRobotPhysic()
    {
        Destroy(gameObject.GetComponent<Rigidbody>());
        Destroy(gameObject.GetComponent<MeshCollider>());
        await UniTask.WaitForFixedUpdate();
        TurnOnRobot();
        gameObject.AddComponent<CircleCollider2D>().radius = robotColliderRadius;
        gameObject.AddComponent<Interactable>();
    }

    private async UniTask Accelerate()
    {
        await transform.DOMove(
                    new Vector3(doorTransform.position.x + hitPointOffsetX,
                    doorTransform.position.y + hitPointOffsetY,
                    doorTransform.position.z + hitPointOffsetZ),
                    breakSpeed).SetEase(Ease.InQuart);
    }

    private async UniTask MoveBackToAccelerate()
    {
        await transform.DOMove(new
            Vector3(transform.position.x + accelerationDistance,
            transform.position.y,
            transform.position.z), breakSpeed)
            .SetEase(Ease.InOutQuad);
    }

    private void KnockDownDoor()
    {
        var doorRigidBody = doorTransform.AddComponent<Rigidbody>();
        const float RobotDampingFactor = 2f;
        gameObject.GetComponent<Rigidbody>().AddForce(
        new Vector3(-breakingForce / RobotDampingFactor, 0f, 0f), ForceMode.Impulse);
        doorRigidBody.AddForce(
        new Vector3(-breakingForce, 0f, 0f), ForceMode.Impulse);
    }

    private async UniTask EnableRobotsPhysic()
    {
        Destroy(gameObject.GetComponent<Interactable>());
        Destroy(gameObject.GetComponent<CircleCollider2D>());
        await UniTask.WaitForFixedUpdate();
        var meshCollider = gameObject.AddComponent<MeshCollider>();
        gameObject.AddComponent<Rigidbody>();
        meshCollider.convex = true;
        meshCollider.providesContacts = true;
    }
    private void RotateTowardsPlayer()
    {
        transform.LookAt(player.position + new Vector3(0f, offset.y, 1f));
    }

    private void ApplyIdleMovement()
    {
        transform.position += new Vector3(
            0f,
            Mathf.Cos(Time.timeSinceLevelLoad * 2f * (float)Math.PI * frequency) * amplitude * Time.deltaTime,
            0f);
    }
}
