using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(InputController))]
public class PlayerMovement : MonoBehaviour
{
    public float Speed = 10;
    public float SprintSpeedMultiplier = 2f;
    public float JumpSpeed = 10;
    public float gravity = -9.81f;
    public float groundCheckDistance = 0.2f;

    CharacterController _characterController;
    InputController _inputController;

    private float verticalVelocity = 0;

    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _inputController = GetComponent<InputController>();
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector3 velocity = new Vector3();
        velocity.x = _inputController.InputMove.x * Speed;
        velocity.z = _inputController.InputMove.y * Speed;

        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);

        if (isGrounded)
        {
            verticalVelocity = 0;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        if (_inputController.Jumped && isGrounded)
        {
            verticalVelocity = JumpSpeed;
        }

        if (_inputController.Sprinting && isGrounded)
        {
            velocity *= SprintSpeedMultiplier;
        }

        velocity.y = verticalVelocity;

        _characterController.Move(velocity * Time.deltaTime);

        if (velocity.magnitude > 0)
        {
            var lookPoint = transform.position + new Vector3(velocity.x, 0, velocity.z);
            transform.LookAt(lookPoint);
        }
    }
}