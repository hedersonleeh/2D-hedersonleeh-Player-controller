using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class JumpUpgrade : MonoBehaviour
{
    [SerializeField]private PlayerMovement playerMovement;
    [SerializeField]private float fallMultiplier = 2.5f;
    [SerializeField]private float lowJumpMultiplier = 0.5f;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if (rb.velocity.y < 0)
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;



    }
}
