using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

//Agafa el Input Sistem i l'aplica al componenet Character Controller.
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 10f;
    public float sprintSpeed = 15f; // Velocidad de sprint
    public float crouchSpeedMultiplier = 0.5f; // Multiplicador de velocidad cuando se agacha
    private float standingHeight; // Altura normal del jugador
    public float iceSpeedMultiplier = 2f; // Multiplicador de velocidad sobre hielo
    public float jumpingSpeedMultiplier = 0.125f; // Multiplicador de velocidad sobre hielo
    public float crouchingHeight = 1f; // Altura del jugador cuando está agachado
    public float smoothTimeJump;
    public float smoothTimeMultiplier;
    private float currentSpeed = 10f;
    private float currentMultiplier = 1f;
    public bool isRunning = false;
    public bool isCrouching = false;
    public bool isJumping = false;
    public bool isIceJumping = false;
    [Header("Camera")]
    public float smoothTimeCam = 0.05f;
    public float jumpForce = 10f;
    [Header("Jump")]
    public Transform GroundChecker;
    public float groundSphereRadius = 0.1f;
    public LayerMask WhatIsGround;
    public LayerMask WhatIsIce;

    [Header("Double Jump")]
    public float JumpCounter = 2;

    public Vector3 horizontalDirection;
    private float verticalValue;
    public Vector3 direction;
    public bool isFirstPerson = false;
    private float _currentRotVelocity;
    private float _currentJumpVelocity;
    private float _currentMultiplier;

    private CharacterController _characterController;
    public Camera thirdCamera;
    public Camera firstCamera;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        standingHeight = _characterController.height;
    }
    public void Crouch(InputAction.CallbackContext context)
    {
        if (isRunning) return;
        if (context.started)
        {
            isCrouching = true;
            firstCamera.transform.position = new Vector3(firstCamera.transform.position.x, firstCamera.transform.position.y / 1.5f, firstCamera.transform.position.z);
            currentSpeed = walkSpeed * crouchSpeedMultiplier;
            _characterController.height = crouchingHeight;
            _characterController.center -= new Vector3(_characterController.center.x, (standingHeight - crouchingHeight)/2, _characterController.center.z);
        }
        if (context.canceled)
        {
            isCrouching = false;
            firstCamera.transform.position = new Vector3(firstCamera.transform.position.x, firstCamera.transform.position.y * 1.5f, firstCamera.transform.position.z);
            _characterController.height = standingHeight;
            _characterController.center = new Vector3(_characterController.center.x, standingHeight / 2, _characterController.center.z);
            currentSpeed = walkSpeed;
        }
    }
    public void Sprint(InputAction.CallbackContext context)
    {
        if (isCrouching || isJumping) return;
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
        if (!context.started || (!IsGrounded() && JumpCounter<1)) return;
        //Double Jump
        if (JumpCounter > 0)
        {
            isJumping = true;
            JumpCounter--;
        }
        if (ShouldSlice()) isIceJumping = true;
        StartCoroutine(Jumping());
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
        GetMultipliers();
        MovePlayer();
    }
    private void Update()
    {
        if (isFirstPerson == true) return;
        RotatePlayer();
    }
    private void GetMultipliers()
    {
        currentMultiplier = 1f;
        if (ShouldSlice() || isIceJumping)
        {
            // Ajusta la velocidad si está sobre hielo
            currentMultiplier = Mathf.SmoothDamp(currentMultiplier, iceSpeedMultiplier, ref _currentMultiplier, smoothTimeMultiplier);
        }
        if (!IsGrounded() && !isIceJumping)
        {
            // Ajusta la velocidad si está en el aire
            currentMultiplier = jumpingSpeedMultiplier;
        }
    }
    public void Move(InputAction.CallbackContext context) //S'activa al PlayerInput, fora de l'script, defineix horizontalDirection
    {
        Vector2 input = context.ReadValue<Vector2>();
        //Canviar els eixos als de la Main Camera.
        if (isFirstPerson)
        {
            Vector3 cameraForward = Vector3.Scale(firstCamera.transform.forward, new Vector3(1, 0, 1)).normalized;
            horizontalDirection = input.x * firstCamera.transform.right + input.y * cameraForward;
        }
        else
        {
            Vector3 cameraForward = Vector3.Scale(thirdCamera.transform.forward, new Vector3(1, 0, 1)).normalized;
            horizontalDirection = input.x * thirdCamera.transform.right + input.y * cameraForward;
        }
    }
    private void RotatePlayer()
    {
        //Si no hi ha cap input, no rotem.
        if (horizontalDirection.sqrMagnitude == 0) return;
        //Agafa l'angle de la direcci� on vol anar
        var targetAngle = Mathf.Atan2(horizontalDirection.x, horizontalDirection.z) * Mathf.Rad2Deg;
        //Modifica l'angle de forma smooth per anar d'angle on vol anar des de angle actual.
        var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _currentRotVelocity, smoothTimeCam);
        transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
    }
    private void MovePlayer()
    {
        if (isJumping)
        {
            verticalValue = Mathf.SmoothDamp(0, jumpForce, ref _currentJumpVelocity, smoothTimeJump);
        }
        else if (!IsGrounded())
        {
            verticalValue -= 19.8f * Time.deltaTime;
        }
        else
        {
            verticalValue = 0;
            //Reinici contador doble salt
            JumpCounter = 2;
        }
        direction = horizontalDirection * currentSpeed * currentMultiplier + Vector3.up * verticalValue;
        //Mou el personatge
        _characterController.Move(direction * Time.fixedDeltaTime);
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
        isIceJumping = false;
    }
}
