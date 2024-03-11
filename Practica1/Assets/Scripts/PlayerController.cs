using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

//Agafa el Input Sistem i l'aplica al componenet Character Controller.
[RequireComponent(typeof(Rigidbody))] 
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 10f;
    public float sprintSpeed = 15f; // Velocidad de sprint
    public float crouchSpeedMultiplier = 0.5f; // Multiplicador de velocidad cuando se agacha
    public float standingHeight = 2f; // Altura normal del jugador
    public float iceSpeedMultiplier = 2f; // Multiplicador de velocidad sobre hielo
    public float jumpingSpeedMultiplier = 0.125f; // Multiplicador de velocidad sobre hielo
    public float crouchingHeight = 1f; // Altura del jugador cuando está agachado
    private float currentSpeed = 10f;
    private float currentMultiplier = 1f;
    public bool isRunning = false;
    public bool isCrouching = false;
    public bool isJumping = false;
    [Header("Camera")]
    public float smoothTime = 0.05f;
    public float jumpForce = 10f;
    [Header("Jump")]
    public Transform GroundChecker;
    public float groundSphereRadius = 0.1f;
    public LayerMask WhatIsGround;
    public LayerMask WhatIsIce;
    private Vector3 _groundCheckerOffset;

    public Vector3 direction;
    public bool isFirstPerson = false;
    private float _currentRotVelocity;

    private Rigidbody _rb;
    private CapsuleCollider _collider;
    public Camera thirdCamera;
    public Camera firstCamera;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
        // Guarda la posición inicial del GroundChecker en relación con la altura del jugador
        _groundCheckerOffset = GroundChecker.localPosition;
    }
    public void Crouch(InputAction.CallbackContext context)
    {
        if (isRunning) return;
        if (context.started)
        {
            isCrouching = true;
            currentSpeed = walkSpeed * crouchSpeedMultiplier;
            _collider.height = crouchingHeight;
            GroundChecker.localPosition = new Vector3(_groundCheckerOffset.x, crouchingHeight / 2f, _groundCheckerOffset.z);
        }
        if (context.canceled)
        {
            isCrouching = false;
            _collider.height = standingHeight;
            GroundChecker.localPosition = _groundCheckerOffset;
            currentSpeed = walkSpeed;
        }
    }
    public void Sprint(InputAction.CallbackContext context)
    {
        if (isCrouching) return;
        if (context.started)
        {
            isRunning= true;
            currentSpeed = sprintSpeed;
        }
        if (context.canceled)
        {
            isRunning= false;
            currentSpeed = walkSpeed;
        }
    }
    public void Jump(InputAction.CallbackContext context)
    {
        if (!context.started || !IsGrounded()) return;
        isJumping = true;
        StartCoroutine(Jumping());
        Vector3 jumpVector = new Vector3(0, jumpForce, 0);
        _rb.AddForce(jumpVector, ForceMode.VelocityChange);
    }
    public void ChangeCameraView(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isFirstPerson = !isFirstPerson;
        }
    }
    private void FixedUpdate()
    {
        currentMultiplier = 1f;
        if (ShouldSlice())
        {
            // Ajusta la velocidad si está sobre hielo
            currentMultiplier = iceSpeedMultiplier; // Aumenta la velocidad cuando está sobre hielo
        }
        if (!IsGrounded())
        {
            // Ajusta la velocidad si está en el aire
            currentMultiplier = jumpingSpeedMultiplier;
        }
        if (direction.sqrMagnitude != 0)
        {
            MovePlayer();
        }
    }
    private void Update()
    {
        if (direction.sqrMagnitude == 0) return;
        RotatePlayer();
    }
    public void Move(InputAction.CallbackContext context) //S'activa al PlayerInput, fora de l'script, defineix direction
    {
        Vector2 input = context.ReadValue<Vector2>();
        //Canviar els eixos als de la Main Camera.
        if (isFirstPerson)
        {
            direction = input.x * firstCamera.transform.right + input.y * firstCamera.transform.forward;
        }
        else
        {
            Vector3 cameraForward = Vector3.Scale(thirdCamera.transform.forward, new Vector3(1, 0, 1)).normalized;
            direction = input.x * thirdCamera.transform.right + input.y * cameraForward;
        }
    }
    private void RotatePlayer()
    {
        //Si no hi ha cap input, no rotem.
        //Agafa l'angle de la direcci� on vol anar
        var targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        //Modifica l'angle de forma smooth per anar d'angle on vol anar des de angle actual.
        var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _currentRotVelocity, smoothTime);
        transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
    }
    private void MovePlayer()
    {
        //Mou el personatge
        _rb.MovePosition(transform.position + direction * currentSpeed * currentMultiplier * Time.fixedDeltaTime);
    }
    private bool ShouldSlice()
    {
        return Physics.CheckSphere(GroundChecker.position, groundSphereRadius, WhatIsIce);
    }
    private bool IsGrounded()
    {
        return Physics.CheckSphere(GroundChecker.position, groundSphereRadius, WhatIsGround);
    }
    IEnumerator Jumping()
    {
        yield return new WaitForSeconds(0.5f);
        isJumping = false;
    }
}
