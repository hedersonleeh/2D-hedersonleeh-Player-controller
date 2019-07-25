using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using System;

[RequireComponent(typeof(SpriteRenderer))]

public class CharacterController2D : MonoBehaviour
{



    [BoxGroup("checks"), SerializeField] private Transform bottomCheck;
    [BoxGroup("checks"), SerializeField] private LayerMask whatIsGround;
    [SerializeField, Range(0f, 0.5f), BoxGroup("checks")] private float bottomCheckRadius = 0.2f;
    [SerializeField, BoxGroup("checks")] private bool airControl = true;




    [BoxGroup("States"), ReadOnly, SerializeField] private bool isGrounded;
    [BoxGroup("States"), ReadOnly, SerializeField] private bool facingRight = true;
    [BoxGroup("Movement"), Range(0, .1f), SerializeField] private float movementSmoothing = .05f;
    [BoxGroup("Movement"),Range(0,30f), SerializeField] private float jumpforce ;
    //[BoxGroup("Movement"), SerializeField] private float dashForce = 100f;

    private Vector3 targetVelocity;
    private Rigidbody2D rb;
    private Vector3 velocity = Vector3.zero;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        isGrounded = false;
        isGrounded = Physics2D.OverlapCircle((Vector2)bottomCheck.position, bottomCheckRadius
                                                                      , whatIsGround);

   
               

    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(bottomCheck.position, bottomCheckRadius);
    }
    public void Move(float move, bool jump, bool crouch)
    {

        if (crouch && isGrounded)
            move = 0;

        if (airControl || isGrounded)
        {
            targetVelocity = new Vector2(move * 10f, rb.velocity.y);

            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing);

            if (move < 0 && !facingRight)
                Flip();
            else if (move > 0 && facingRight)
                Flip();
        }



        if (isGrounded && jump)
        {
            rb.velocity = new Vector2(rb.velocity.x, 1* jumpforce);          
            isGrounded = false;
        }
    }

    public void Flip()
    {
        facingRight = !facingRight;
        GetComponent<SpriteRenderer>().flipX = facingRight; ;
    }

}