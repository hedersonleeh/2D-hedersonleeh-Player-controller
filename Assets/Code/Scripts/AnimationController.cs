using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationController : MonoBehaviour
{

    [SerializeField] private CharacterController2D controller;
    [SerializeField] private PlayerMovement playerScript;
    [SerializeField] private Animator animator;
    // Use this for initialization
  
    // Update is called once per frame
    void LateUpdate()
    {
        animator.SetFloat("MoveSpeed", Mathf.Abs(playerScript.MoveDirection.x));
        animator.SetBool("IsGrounded", controller.IsGrounded);
        if (playerScript.Jump)
            animator.SetTrigger("IsJumping");
        if (playerScript.Dash)
            animator.SetTrigger("Dash");
	    animator.SetBool("IsCrouching", playerScript.Crouch);

    }
}
