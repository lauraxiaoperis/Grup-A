using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(InputController))]


public class PlayerMovement : MonoBehaviour
{
    public float Speed = 10;
    public float JumpSpeed = 10;


    public Transform GroundChecker;
    public float groundSphereRadius = 0.1f;


    public LayerMask WhatIsGround;
    public LayerMask WhatIsIce;


    float _lastVelocity_Y;


    CharacterController _characterController;
    InputController _inputController;
    // Start is called before the first frame update
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _inputController = GetComponent<InputController>();                
    }


    // Update is called once per frame
    void Update()
    {
        Move();
    }


    private void Move()
    {
        Vector3 velocity = _characterController.velocity;


        float smoothy = 1;
        if (!IsGrounded())
            smoothy = 0.002f;


        velocity.x = Mathf.Lerp(velocity.x,_inputController.InputMove.x * Speed , smoothy);
        velocity.y = GetGravity();
        velocity.z = Mathf.Lerp(velocity.z,_inputController.InputMove.y * Speed , smoothy);


        if (ShouldSlice())
        {
            Speed = 30;
        }
        else
        {
            Speed = 10;
        }


        if (ShouldJump())
        {
            velocity.y = JumpSpeed;
        }
        _lastVelocity_Y = velocity.y;
        //_characterController.SimpleMove(velocity);


        _characterController.Move(velocity * Time.deltaTime);
        //if moving
        if (velocity.magnitude > 0)
        {
            var lookPoint = transform.position + new Vector3(velocity.x, 0, velocity.z);
            transform.LookAt(lookPoint);
        }


        //look at point in move dir
    }


    private bool ShouldSlice()
    {
        return Physics.CheckSphere(GroundChecker.position, groundSphereRadius, WhatIsIce);
    }


    private bool ShouldJump()
    {
        return _inputController.Jumped && IsGrounded();
    }


    private bool IsGrounded()
    {
        return Physics.CheckSphere(GroundChecker.position,groundSphereRadius, WhatIsGround);
    }


    private float GetGravity()
    {
        float currentVelocity = _lastVelocity_Y;
        currentVelocity += Physics.gravity.y * Time.deltaTime;
        return currentVelocity;
    }
}