using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider2D))]
public class PhysicsController : MonoBehaviour
{
    [Header("Physics")]
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private float gravity = -80;
    [SerializeField] private float friction = 0.01f;
    [SerializeField] private float airFriction = 0.01f;
    [SerializeField] private float maxVelocity = 16f;
    [SerializeField] private float minVelocity = 0.01f;

    [Header("Raycast Settings")]
    [SerializeField] int horizontalRayCount = 3;
    [SerializeField] int verticalRayCount = 3;
    private const float skinWidth = 0.02f;
    private float horizontalRaySpacing;
    private float verticalRaySpacing;
    private BoxCollider2D boxCollider;
    public RaycastOrigins raycastOrigins;
    private List<Vector2> forces = new List<Vector2>();
    public Vector2 velocity;
    public CollisionInfo collisions;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }

    private void Update()
    {
        UpdatePhysics();
    }

    public void UpdatePhysics()
    {
        UpdateRaycastOrigins();
        collisions.Reset();

        velocity += Vector2.up * gravity * Time.deltaTime;

        foreach (var force in forces)
        {
            velocity += force * Time.deltaTime;
        }

        // Max speed limit
        if(Mathf.Abs(velocity.magnitude) > maxVelocity)
        {
            velocity = velocity.normalized * maxVelocity;
        }
        // Min speed limit
        if(Mathf.Abs(velocity.x) < minVelocity)
        {
            velocity.x = 0;
        }
        if (Mathf.Abs(velocity.y) < minVelocity)
        {
            velocity.y = 0;
        }

        Vector2 shift = velocity * Time.deltaTime;

        CollisionClampHorizontalShift(ref shift);
        CollisionClampVerticalShift(ref shift);

        transform.Translate(shift);
        
        // Ground friction
        if(collisions.below || collisions.above)
        {
            velocity *= (1 - friction);
        }
        // Air friction
        if(!collisions.below && !collisions.above)
        {
            velocity *= (1 - airFriction);
        }
    }

    internal void SetGravity(float force)
    {
        gravity = force;
    }
    internal float GetGravity()
    {
        return gravity;
    }

    public void AddForce(Vector2 force)
    {
        forces.Add(force);
    }

    public void RemoveForce(Vector2 force)
    {
        forces.Remove(force);
    }

    public void AddImpulse(Vector2 impuse)
    {
        velocity += impuse;
    }

    public void SetVelocityX(float velocityX) => this.velocity.x = velocityX;
    public void SetVelocityY(float velocityY) => this.velocity.y = velocityY;

    public void ClampVelocityX(float limit)
    {
        velocity.x = Mathf.Clamp(velocity.x, -limit, limit);
    }

    private void CollisionClampVerticalShift(ref Vector2 shift)
    {
        List<RaycastHit2D> hits = GetVerticalRaysHits(shift).ToList();
        if (hits.Count == 0) return;
        var minDistance = hits.Select(h => h.distance).Min();
        shift.y = Mathf.Sign(shift.y) * (minDistance - skinWidth);
        velocity.y = 0;
    }

    private void CollisionClampHorizontalShift(ref Vector2 shift)
    {
        List<RaycastHit2D> hits = GetHorizontalRaysHits(shift).ToList();
        if (hits.Count == 0) return;
        var minDistance = hits.Select(h => h.distance).Min();
        shift.x = Mathf.Sign(shift.x) * (minDistance - skinWidth);
        velocity.x = 0;
    }

    private IEnumerable<RaycastHit2D> GetVerticalRaysHits(Vector2 shift)
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
                yield return hit;
                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }
        }
    }

    private IEnumerable<RaycastHit2D> GetHorizontalRaysHits(Vector2 shift)
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
                yield return hit;
                collisions.left = directionX == -1;
                collisions.right = directionX == 1;
            }
        }
    }

    private void UpdateRaycastOrigins()
    {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
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

    internal LayerMask GetCollisionMask()
    {
        return collisionMask;
    }

    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    // public List<> GetRaycastOrigins()
    // {
    //     return RaycastOrigins.top
    // }

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
