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

    private Vector2 moveDirection;
    private bool crouch;
    private bool jump;
    private bool climb;




    // Use this for initialization
    private void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Inputs();
    }

    private void Inputs()
    {

        moveDirection = new Vector2(Input.GetAxisRaw("Horizontal") * speed, Input.GetAxisRaw("Vertical")); ;
        crouch = Input.GetAxisRaw("Vertical") < 0;
        if (Input.GetButtonDown("Jump")) jump = true;
        climb = Input.GetButton("Fire3");

    }
    private void FixedUpdate()
    {
        controller2D.Move(moveDirection * Time.deltaTime, jump, crouch, climb);
        jump = false;

    }
    public Vector2 MoveDirection { get { return moveDirection; } }

    public bool Crouch { get { return crouch; } }

    public bool Jump { get { return jump; } }
}
