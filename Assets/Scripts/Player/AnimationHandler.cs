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

        animator.SetFloat("HorizontalSpeed", speed);
    }

    internal void SetVerticalSpeed(float speed)
    {
        if (!animator) return;
        animator.SetFloat("VerticalSpeed", speed);
    }

    internal void Jump(bool isJumping)
    {
        if (!animator) return;
        animator.SetBool("IsJumping", isJumping);
    }
}
