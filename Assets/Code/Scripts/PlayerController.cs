using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
public class PlayerController : MonoBehaviour
{

    [SerializeField, Range(0f, 100f), BoxGroup("Player")] private float health = 100;
    [SerializeField, Range(0, 4f), BoxGroup("Player")] private float moveSpeed;

    [SerializeField, BoxGroup("Player")] private Rigidbody2D rb;
    [SerializeField, BoxGroup("Player")] private float jumpFonce;


    void Start()
    {

    }


    void FixedUpdate()
    {
        rb.velocity = new Vector2(Input.GetAxis("Horizontal")*(rb.velocity.x+moveSpeed*100)*Time.fixedDeltaTime, rb.velocity.y);
    }
    public Rigidbody2D Rb
    {
        get { return rb; }
    }

}