using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D ridgidbody;
    private Vector2 tileSize;

    [Header("Movement Settings")]
    [Range(0, 10)]
    [SerializeField] private float movementSpeed = 1;

    private void Start()
    {
        ridgidbody = GetComponent<Rigidbody2D>();
        tileSize = FindObjectOfType<Grid>().cellSize;
    }

    internal void Move(Vector2 movementDirection)
    {
        ridgidbody.velocity = IsometrizeMovement(movementDirection) * movementSpeed;
    }

    private Vector2 IsometrizeMovement(Vector2 movementDirection) => movementDirection * tileSize;
    private Vector2 Desometrize(Vector2 movementDirection) => movementDirection / tileSize;

    public float GetCurrentSpeed() => Desometrize(ridgidbody.velocity).magnitude / movementSpeed;
    public Vector2 GetCurrentVelocity() => Desometrize(ridgidbody.velocity);
}
