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
    [SerializeField, BoxGroup("checks")] private Vector2 checkBoxSize;
    [SerializeField, BoxGroup("checks")] private bool airControl = true;
    [BoxGroup("States"), ReadOnly, SerializeField] private bool isGrounded;
    [BoxGroup("States"), ReadOnly, SerializeField] private bool facingRight = true;
    [BoxGroup("Movement"), Range(0, .1f), SerializeField] private float movementSmoothing = .05f;
    [BoxGroup("Movement"), SerializeField] private float jumpforce;
    [BoxGroup("Movement"), SerializeField] private bool wallJump;
    [BoxGroup("Movement"), SerializeField] private Vector2 wallJumpForce;
    [BoxGroup("Movement"), SerializeField] private float distanceToWallJump;
    [BoxGroup("Movement"), SerializeField, ReadOnly] private bool ReadyToWallJump = false;
    [BoxGroup("Movement"), SerializeField] private Vector2 wallJumpDirection;
    [BoxGroup("Movement"), SerializeField] private RaycastHit2D wallRayCheck;
    [BoxGroup("Movement"), SerializeField, ReadOnly] private bool isWallJumping = false;

    private Vector3 targetVelocity;
    private Rigidbody2D rb;
    private Vector3 velocity = Vector3.zero;

    public bool IsGrounded { get { return isGrounded; } }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {

        if (isGrounded = Physics2D.OverlapBox((Vector2)bottomCheck.position, checkBoxSize, 0f, whatIsGround))
        {
            isGrounded = true;
            isWallJumping = false;
        }
        else
        {
            //isWallJumping = false;
            isGrounded = false;
        }

        if (wallJump)
        {
            if (wallRayCheck = Physics2D.Raycast((Vector2)transform.position, transform.right, distanceToWallJump, whatIsGround))
            {
                wallJumpDirection = wallRayCheck.normal;
                ReadyToWallJump = true;
            }
            else
                ReadyToWallJump = false;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay(transform.position, transform.right * distanceToWallJump);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(bottomCheck.position, checkBoxSize);
    }
    public void Move(float move, bool jump, bool crouch)
    {
        if (crouch && isGrounded)
            move = 0;

        if (airControl || isGrounded)
        {
            targetVelocity = new Vector2(move * 10f, rb.velocity.y);

            if (!isWallJumping)
                rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing);
            else
                rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, targetVelocity.x, 0.1f), rb.velocity.y);
            //rb.velocity = Vector3.SmoothDamp(rb.velocity, new Vector2(targetVelocity.x * 0.2f + targetVelocity.x, targetVelocity.y), ref velocity, movementSmoothing);


            if (move < 0 && !facingRight)
                Flip();
            else if (move > 0 && facingRight)
                Flip();
        }

        if (isGrounded && jump)
        {
            rb.AddForce(Vector2.up * jumpforce * Time.deltaTime, ForceMode2D.Impulse);
            isGrounded = false;
        }
        else if (ReadyToWallJump && jump)
            if (!isGrounded && move * wallJumpDirection.x < 0) WallJump();


        //if (isWallJumping && rb.velocity.y < 0)


    }

    public void Flip()
    {
        facingRight = !facingRight;
        if (facingRight) transform.localEulerAngles = Vector3.up * 180;
        else transform.localEulerAngles = Vector3.up * 0;
    }


    private void WallJump()
    {
        StopCoroutine(DisableMove(0));
        Flip();
        StartCoroutine(DisableMove(0.1f));
        isWallJumping = true;
        rb.velocity = Vector2.zero;
        Vector2 wallJumpTargetVelocity = wallJumpForce * wallJumpDirection;
        rb.velocity = new Vector2(wallJumpTargetVelocity.x * Time.deltaTime, wallJumpForce.y * Time.deltaTime);

    }
    IEnumerator DisableMove(float timeDisable)
    {
        
        airControl = false;
        yield return new WaitForSeconds(timeDisable);
        airControl = true;
    }
}