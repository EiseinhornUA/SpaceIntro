using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private Transform characterParent;

    private void Update()
    {
        animator = characterParent.GetChild(0).GetComponent<Animator>();
    }

    internal void SetHorizontalSpeed(float speed)
    {
        if (!animator) return;

        animator.SetFloat("HorizontalSpeed", Mathf.Abs(speed));
    }

    internal void SetVerticalSpeed(float speed)
    {
        if (!animator) return;
        animator.SetFloat("VerticalSpeed", speed);
    }

    internal void Jump()
    {
        if (!animator) return;
        animator.SetBool("Jump", true);
    }

    internal void Turn()
    {
        if (!animator) return;
        animator.SetTrigger("Turn");
    }

    internal void StartWalking()
    {
        animator.SetBool("IsWalking", true);
    }

    internal void StopWalking()
    {
        animator.SetBool("IsWalking", false);
    }

    internal void StartRunning()
    {
        animator.SetBool("IsRunning", true);
    }

    internal void StopRunning()
    {
        animator.SetBool("IsRunning", false);
    }

    internal void StartFlying()
    {
        animator.SetBool("IsFlying", true);
    }

    internal void StopFlying()
    {
        animator.SetBool("IsFlying", false);
    }
}
