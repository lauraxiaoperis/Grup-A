using System;
using UnityEngine;

[RequireComponent(typeof(InputController))]

public class PlayerMovement : MonoBehaviour
{
    public float WalkSpeed = 10f;
    public float SprintSpeed = 15f; // Velocidad de sprint
    public float CrouchSpeedMultiplier = 0.5f; // Multiplicador de velocidad cuando se agacha
    public float StandingHeight = 2f; // Altura normal del jugador
    public float CrouchingHeight = 1f; // Altura del jugador cuando está agachado
    public float IceSpeedMultiplier = 2f; // Multiplicador de velocidad sobre hielo

    public float JumpSpeed = 10f;

    public Transform GroundChecker;
    public float groundSphereRadius = 0.1f;
    public LayerMask WhatIsGround;
    public LayerMask WhatIsIce;

    float _lastVelocity_Y;
    CharacterController _characterController;
    InputController _inputController;

    Vector3 _groundCheckerOffset;

    // Start is called before the first frame update
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _inputController = GetComponent<InputController>();

        // Guarda la posición inicial del GroundChecker en relación con la altura del jugador
        _groundCheckerOffset = GroundChecker.localPosition;
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

        float speed = _inputController.Sprinting && !_inputController.Crouching ? SprintSpeed : WalkSpeed; // Ajusta la velocidad de acuerdo a si está sprintando y no está agachado

        if (_inputController.Crouching)
        {
            // Si el jugador está agachado, ajusta la velocidad de movimiento y altura del jugador
            speed *= CrouchSpeedMultiplier; // Reducir la velocidad cuando está agachado
            _characterController.height = CrouchingHeight; // Altura del jugador cuando está agachado
            // Ajusta la posición del GroundChecker para mantenerlo por encima del suelo
            GroundChecker.localPosition = new Vector3(_groundCheckerOffset.x, CrouchingHeight / 2f, _groundCheckerOffset.z);
        }
        else
        {
            _characterController.height = StandingHeight; // Altura normal del jugador
            // Restaura la posición original del GroundChecker
            GroundChecker.localPosition = _groundCheckerOffset;
        }

        if (ShouldSlice())
        {
            // Ajusta la velocidad si está sobre hielo
            speed *= IceSpeedMultiplier; // Reduce la velocidad cuando está sobre hielo
        }

        velocity.x = Mathf.Lerp(velocity.x, _inputController.InputMove.x * speed, smoothy);
        velocity.y = GetGravity();
        velocity.z = Mathf.Lerp(velocity.z, _inputController.InputMove.y * speed, smoothy);

        if (ShouldJump())
        {
            velocity.y = JumpSpeed;
        }

        _lastVelocity_Y = velocity.y;

        _characterController.Move(velocity * Time.deltaTime);

        // Rotar el jugador hacia la dirección del movimiento
        if (velocity.magnitude > 0)
        {
            var lookPoint = transform.position + new Vector3(velocity.x, 0, velocity.z);
            transform.LookAt(lookPoint);
        }
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
        return Physics.CheckSphere(GroundChecker.position, groundSphereRadius, WhatIsGround);
    }

    private float GetGravity()
    {
        float currentVelocity = _lastVelocity_Y;
        currentVelocity += Physics.gravity.y * Time.deltaTime;
        return currentVelocity;
    }
}