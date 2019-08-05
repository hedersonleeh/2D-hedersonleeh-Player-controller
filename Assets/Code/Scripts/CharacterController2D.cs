using System.Collections;
using UnityEngine;
using NaughtyAttributes;
using System;

[RequireComponent(typeof(Rigidbody2D))]

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
    [SerializeField, BoxGroup("Values")] private float dashForce;
    [SerializeField, BoxGroup("Values")] private float wallSlideSpeed;
    [SerializeField, BoxGroup("Values")] private float climpSpeed;
    [SerializeField, BoxGroup("Values")] private float coolDownDash;

    [Space]
    [SerializeField, BoxGroup("Booleans")] private bool canWallSlide = false;
    [SerializeField, BoxGroup("Booleans")] private bool canDash;
    [SerializeField, BoxGroup("Booleans")] private bool canclimb;
    [SerializeField, BoxGroup("Booleans")] private bool canWallJump;
    [SerializeField, BoxGroup("Booleans")] private bool CanControlAir;

    [Space]
    [SerializeField, ReadOnly, BoxGroup("States")] private bool isGrounded;
    [SerializeField, ReadOnly, BoxGroup("States")] private bool facingRight = true;
    [SerializeField, ReadOnly, BoxGroup("States")] private bool isWallJumping = false;
    [SerializeField, ReadOnly, BoxGroup("States")] private bool isDashing;
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

        if (wallRayCheck = Physics2D.Raycast((Vector2)transform.position, transform.right, wallCheck, whatIsGround))
            if (canWallJump) wallJumpDirection = wallRayCheck.normal;

    }
    public void Move(Vector2 dir, bool jump, bool crouch, bool climb, bool dash)
    {
        if (!isClimbing && (airControl || isGrounded))
        {
            if (!crouch)
            {
                targetVelocity = new Vector2(dir.x * 10f, rb.velocity.y);

                if (!isWallJumping)
                    rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing);
                else
                    rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, targetVelocity.x, 0.2f), rb.velocity.y);
                ChangeFacing(dir.x);
            }

        }

        wallCheckInAir = wallRayCheck && !isGrounded && dir.x * wallJumpDirection.x < 0;

        if (wallRayCheck && climb && canclimb)
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

        if (!isDashing && dash && canDash)
            Dash(dir);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay(transform.position, transform.right * wallCheck);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(bottomCheck.position, checkBoxSize);
    }

    private void Dash(Vector2 dir)
    {
        if (dir != Vector2.zero)
        {
            StartCoroutine(DisableMove(.1f));
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;
            rb.velocity += dir.normalized * dashForce;
            StartCoroutine(DashCooldown(.5f));
        }
        else
        {
            StartCoroutine(DisableMove(.1f));
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;
            rb.velocity += (Vector2)transform.forward * dashForce;
            StartCoroutine(DashCooldown(.5f));
        }
    }

    private bool Climb(Vector2 dir)
    {
        rb.gravityScale = 0;

        targetVelocity = new Vector2(0, dir.y * climpSpeed);

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

    private void ChangeFacing(float dir)
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
        airControl = CanControlAir;
    }
    IEnumerator DashCooldown(float time)
    {
        isDashing = true;
        yield return new WaitForSeconds(time);
        isDashing = false;
    }
    public bool IsGrounded { get { return isGrounded; } }
    public bool IsWallJumping { get { return isWallJumping; } }

    public bool IsDashing { get { return isDashing; } }
}