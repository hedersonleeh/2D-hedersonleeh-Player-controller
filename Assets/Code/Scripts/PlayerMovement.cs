using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController2D))]
public class PlayerMovement : MonoBehaviour
{

    [Header("Controller")]
    [SerializeField] private CharacterController2D controller2D;
    [SerializeField, Range(0, 50f)] private float speed = 50f;
    [SerializeField, Range(0, 100f)] private float runSpeed = 100f;

    private Vector2 moveDirection;
    private bool crouch;
    private bool jump;
    private bool climb;
    private bool dash;
    float moveSpeed;
    // Use this for initialization
    private void Awake()
    {
        moveSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        Inputs();
    }

    private void Inputs()
    {

        moveDirection = new Vector2(Input.GetAxisRaw("Horizontal") * speed, Input.GetAxisRaw("Vertical")*speed); ;
        crouch = Input.GetAxisRaw("Vertical") < 0;
        jump = Input.GetButtonDown("Jump");
        climb = Input.GetButton("Fire1");
        speed = Input.GetButton("Fire3") ? runSpeed : moveSpeed;
        dash = Input.GetButtonDown("Fire2");

    }
    private void FixedUpdate()
    {
        controller2D.Move(moveDirection * Time.deltaTime, jump, crouch, climb,dash);
        jump = false;
    }
    public Vector2 MoveDirection { get { return moveDirection; } }

    public bool Crouch { get { return crouch; } }

    public bool Jump { get { return jump; } }
}
