using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using System;

[RequireComponent(typeof(SpriteRenderer))]

public class CharacterController2D : MonoBehaviour
{

    [SerializeField, BoxGroup("Checks")] private Transform bottomCheck;
    [SerializeField, BoxGroup("Checks")] private LayerMask whatIsGround;
    [SerializeField, BoxGroup("Checks")] private Vector2 checkBoxSize;
    [SerializeField, BoxGroup("Checks")] private float wallCheck;

    [Space]
    [SerializeField, Range(0, .1f), BoxGroup("Values")] private float movementSmoothing = .05f;
    [SerializeField, BoxGroup("Values"),] private Vector2 wallJumpForce;
    [SerializeField, BoxGroup("Values")] private float jumpforce;
    [SerializeField, BoxGroup("Values")] private float wallSlideSpeed;
    [SerializeField, BoxGroup("Values")] private float climpSpeed;

    [Space]
    [SerializeField, BoxGroup("Booleans")] private bool canWallSlide = false;
    [SerializeField, BoxGroup("Booleans")] private bool wallJump;
    [SerializeField, BoxGroup("Booleans")] private bool ControlAir;

    [Space]
    [SerializeField, ReadOnly, BoxGroup("States")] private bool isGrounded;
    [SerializeField, ReadOnly, BoxGroup("States")] private bool facingRight = true;
    [SerializeField, ReadOnly, BoxGroup("States")] private bool isWallJumping = false;
    [SerializeField, ReadOnly, BoxGroup("States")] private bool wallCheckInAir = false;


    private Vector3 targetVelocity;
    private Rigidbody2D rb;
    private RaycastHit2D wallRayCheck;
    private Vector3 velocity = Vector3.zero;

    private bool isClimbing = false;
    private bool airControl = true;
    private Vector2 wallJumpDirection;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {

        isGrounded = Physics2D.OverlapBox((Vector2)bottomCheck.position, checkBoxSize, 0f, whatIsGround);

        isWallJumping = !isGrounded;

        if (wallJump)
            if (wallRayCheck = Physics2D.Raycast((Vector2)transform.position, transform.right, wallCheck, whatIsGround))
                wallJumpDirection = wallRayCheck.normal;



    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay(transform.position, transform.right * wallCheck);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(bottomCheck.position, checkBoxSize);
    }
    public void Move(Vector2 dir, bool jump, bool crouch, bool climb)
    {

        if (!isClimbing && (airControl || isGrounded))
        {
            targetVelocity = new Vector2(dir.x * 10f, rb.velocity.y);

            if (!isWallJumping)
                rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing);
            else
                rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, targetVelocity.x, 0.2f), rb.velocity.y);
            ChangeFace(dir.x);
        }

        if (crouch && isGrounded)
            dir.x = 0;
        
        DynamicMoves(dir, jump, climb);
       
        


    }

    private void DynamicMoves(Vector2 dir, bool jump, bool climb)
    {
        wallCheckInAir = wallRayCheck && !isGrounded && dir.x * wallJumpDirection.x < 0;

        if (wallRayCheck && climb)
        {
            isClimbing = Climb(dir);
            if (jump && !isGrounded)
                WallJump();
        }
        else
            isClimbing = false;

        if (canWallSlide && rb.velocity.y < 0 && wallCheckInAir)
            if (!climb) WallSlide(wallSlideSpeed);

        if (isGrounded && jump)
            Jump(jumpforce);
        else if (wallCheckInAir && jump)
            WallJump();
    }

    private bool Climb(Vector2 dir)
    {
        rb.gravityScale = 0;

        targetVelocity = new Vector2(0, dir.y * climpSpeed * 10f);

        rb.velocity = targetVelocity;
        return isClimbing = true;
    }

    private void WallSlide(float speed)
    {
        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / speed);
    }

    private void Jump(float force)
    {
        rb.AddForce(Vector2.up * force * Time.deltaTime, ForceMode2D.Impulse);
    }

    private void ChangeFace(float dir)
    {
        if (isWallJumping)
        {
            if (rb.velocity.x < 0 && !facingRight)
                Flip();
            else if (rb.velocity.x > 0 && facingRight)
                Flip();
        }
        else if (!isWallJumping)
        {
            if (dir < 0 && !facingRight && !isWallJumping)
                Flip();
            else if (dir > 0 && facingRight && !isWallJumping)
                Flip();
        }
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
        airControl = ControlAir;
    }
    public bool IsGrounded { get { return isGrounded; } }
}