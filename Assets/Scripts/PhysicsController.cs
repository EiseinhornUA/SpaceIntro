using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Timeline;

[RequireComponent(typeof(BoxCollider2D))]
public class PhysicsController : MonoBehaviour
{
    [Header("Physics")]
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private float gravity = -80f;
    [SerializeField] private float friction = 0.01f;
    [SerializeField] private float airFriction = 0.01f;
    [SerializeField] private float maxVelocity = 16f;
    [SerializeField] private float maxClimbAngle = 40f;
    
    [Header("Raycast Settings")]
    [SerializeField] int horizontalRayCount = 3;
    [SerializeField] int verticalRayCount = 3;
    private const float skinWidth = 0.01f;

    private float horizontalRaySpacing;
    private float verticalRaySpacing;
    private BoxCollider2D boxCollider;
    public RaycastOrigins raycastOrigins;
    private List<Vector2> forces = new List<Vector2>();
    public Vector2 velocity;
    private Vector2 shift;
    public CollisionInfo collisions;

    private void Start()
    {
        //Time.timeScale = 0.5f;
        boxCollider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }

    private void Update()
    {
        UpdatePhysics();
    }

    private void UpdatePhysics()
    {
        UpdateRaycastOrigins();
        collisions.Reset();
        
        AddGravity();
        AddForces();

        LimitVelocity();

        shift = velocity * Time.deltaTime;

        CollideHorizontally();
        CollideVertically();

        transform.Translate(shift);

        AddGroundFriction();
        AddAirFriction();
    }

    private void AddGravity() => velocity += gravity * Time.deltaTime * Vector2.up;

    private void AddForces()
    {
        foreach (var force in forces)
        {
            velocity += force * Time.deltaTime;
        }
    }

    private void LimitVelocity()
    {
        if (Mathf.Abs(velocity.magnitude) > maxVelocity)
        {
            velocity = velocity.normalized * maxVelocity;
        }
    }

    private void AddAirFriction()
    {
        if (!collisions.below && !collisions.above)
        {
            velocity *= (1 - airFriction);
        }
    }

    private void AddGroundFriction()
    {
        if (collisions.below || collisions.above)
        {
            velocity *= (1 - friction);
        }
    }

    internal void SetGravity(float force) => gravity = force;
    internal float GetGravity() => gravity;

    internal void AddForce(Vector2 force) => forces.Add(force);
    internal void RemoveForce(Vector2 force) => forces.Remove(force);

    internal void AddImpulse(Vector2 impuse) => velocity += impuse;

    internal void SetVelocityX(float velocityX) => this.velocity.x = velocityX;
    internal void SetVelocityY(float velocityY) => this.velocity.y = velocityY;

    //private void CollideHorizontally(ref Vector2 shift)
    //{
    //    if (shift.x == 0) return;

    //    List<RaycastHit2D> hits = GetHorizontalRaysHits(shift).Where(h => (bool)h).ToList();

    //    if (hits.Count == 0) return;

    //    float minDistance = hits.Select(h => h.distance).Min();

    //    shift.x = Mathf.Sign(shift.x) * (minDistance - skinWidth);

    //    velocity.x = shift.x / Time.deltaTime;
    //}

    //private void CollideVertically(ref Vector2 shift)
    //{
    //    if (shift.y == 0) return;

    //    List<RaycastHit2D> hits = GetVerticalRaysHits(shift).ToList();

    //    if (hits.Count == 0) return;

    //    float minDistance = hits.Select(h => h.distance).Min();

    //    shift.y = Mathf.Sign(shift.y) * (minDistance - skinWidth);

    //    velocity.y = shift.y / Time.deltaTime;
    //}

    private void CollideVertically()
    {
        float directionY = Mathf.Sign(shift.y);
        float rayLength = Mathf.Abs(shift.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY < 0) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.down * rayLength, Color.green);

            if (hit)
            {
                shift.y = (hit.distance  - skinWidth) * directionY;
                print(hit.distance);
                velocity.y = shift.y / Time.deltaTime;

                //if (hit.distance < skinWidth)
                //{
                //    Debug.Break();
                //}

                rayLength = hit.distance;

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }
        }
    }

    private void CollideHorizontally()
    {
        float directionX = Mathf.Sign(shift.x);
        float rayLength = Mathf.Abs(shift.x) + skinWidth;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX < 0) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * rayLength * directionX, Color.red);
            
            if (hit)
            {
                if (i == 0)
                {
                    ClimbSlope(hit);
                    Debug.DrawRay(rayOrigin, shift, Color.magenta);
                }

                shift.x = (hit.distance - skinWidth) * directionX;
                velocity.x = shift.x / Time.deltaTime;

                //if (hit.distance < skinWidth)
                //{
                //    Debug.Break();
                //}

                rayLength = hit.distance;

                collisions.left = directionX == -1;
                collisions.right = directionX == 1;
            }
        }
    }

    private void ClimbSlope(RaycastHit2D hit)
    {
        float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

        if (slopeAngle > maxClimbAngle) return;

        float moveDistance = Mathf.Abs(shift.x);

        shift.y = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
        shift.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(shift.x);
        velocity.y = shift.y / Time.deltaTime;
    }

    private void UpdateRaycastOrigins()
    {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.topLeft =  new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight =  new Vector2(bounds.max.x, bounds.max.y);
        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
    }

    private void CalculateRaySpacing()
    {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    internal LayerMask GetCollisionMask() => collisionMask;

    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    public struct CollisionInfo
    {
        public bool above;
        public bool below;
        public bool left;
        public bool right;

        public void Reset()
        {
            above = false;
            below = false;
            left = false;
            right = false;
        }
    }
}
