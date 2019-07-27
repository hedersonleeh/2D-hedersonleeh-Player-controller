using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D),typeof(CharacterController2D))]
public class JumpUpgrade : MonoBehaviour
{
    [SerializeField] private CharacterController2D controller;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 0.5f;
    [SerializeField] private float gravity = 1.5f;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if (!controller.IsGrounded && rb.velocity.y < 0)
            rb.gravityScale = fallMultiplier;
        else if (!controller.IsGrounded && rb.velocity.y > 0 && !Input.GetButton("Jump"))
            rb.gravityScale = lowJumpMultiplier;
        else
            rb.gravityScale = gravity;
    }
}
