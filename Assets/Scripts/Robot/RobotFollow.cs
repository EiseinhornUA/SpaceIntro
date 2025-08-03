using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class RobotFollow : MonoBehaviour
{
    private const float ApproachingThreshold = 0.1f;
    [SerializeField] private Transform player;
    [SerializeField] private float followDistance = 2.5f;
    [SerializeField] private float followSpeed = 10f;
    [SerializeField] private float offsetX = 0.0f;
    [SerializeField] private float offsetY = 1.5f;
    [SerializeField] private float offsetZ = -0.5f;
    [SerializeField] private float frequency = 0.5f; // Frequency of the idle movement
    [SerializeField] private float amplitude = 0.01f; // Speed of the idle movement
    private bool isRobotOn = false;
    private bool isFollowingOn = true;
    private float yzSpeedNormalizer = 0.2f;

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

    public void TurnOnRobot()
    {
        isRobotOn = true;
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

    private void RotateTowardsPlayer()
    {
        transform.LookAt(player.position + new Vector3(0f, offsetY, 1f));
    }

    private void ApplyIdleMovement()
    {
        transform.position += new Vector3(
            0f,
            Mathf.Cos(Time.fixedTime * 2f * (float)Math.PI * frequency) * amplitude,
            0f);
    }

    public void FollowPlayer()
    {
        Vector3 robotPosition = transform.position;

        float directionX = player.position.x - transform.position.x + offsetX;
        float directionY = player.position.y - transform.position.y + offsetY;
        float directionZ = player.position.z - transform.position.z + offsetZ;

        if (Mathf.Abs(directionX) > followDistance)
        {
            robotPosition.x += (Mathf.Sign(directionX) - ((followDistance - ApproachingThreshold) / directionX))
                * followSpeed * Time.deltaTime;
        }

        robotPosition.y += yzSpeedNormalizer * directionY * followSpeed * Time.deltaTime;
        robotPosition.z += yzSpeedNormalizer * directionZ * followSpeed * Time.deltaTime;
        transform.position = robotPosition;
    }
}
