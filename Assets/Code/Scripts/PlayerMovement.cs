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

    private float horizontalMove;
    private bool crouch;
    private bool jump;



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
        horizontalMove = Input.GetAxis("Horizontal") * speed;
        crouch = Input.GetAxisRaw("Vertical") < 0;
        if (Input.GetButtonDown("Jump")) jump = true;



    }
    private void FixedUpdate()
    {

        controller2D.Move(horizontalMove * Time.deltaTime, jump, crouch);

        jump = false;

    }
}
