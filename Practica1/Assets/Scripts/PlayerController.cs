using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Speed")]
    public float walkSpeed = 10f;
    public float sprintSpeed = 15f;
    [Header("Movemtn Multipliers")]
    public float jumpingSpeedMultiplier = 0.125f;
    public float crouchSpeedMultiplier = 0.5f;
    public float iceSpeedMultiplier = 2f;
    private float currentSpeed = 10f;
    private float currentMultiplier = 1f;
    public float smoothTimeMultiplier; //Damp multiplicador
    [HideInInspector] public bool isRunning = false; //Per l'animator
    [HideInInspector] public bool isCrouching = false; //Per l'animator
    [HideInInspector] public bool isJumping = false; //Per l'animator
    [HideInInspector] public bool isIceJumping = false; //Per l'animator
    [Header("Camera")]
    public float smoothTimeCam = 0.05f;
    [Header("Crouch")]
    public float crouchingHeight = 1f; // Altura del jugador cuando está agachado
    private float standingHeight; // Altura normal del jugador
    [Header("Jump")]
    public float heightJump = 2f;
    public float timeJump = 1f;
    public float smoothTimeJump;
    public Transform GroundChecker;
    public float groundSphereRadius = 0.1f;
    public LayerMask WhatIsGround;
    public LayerMask WhatIsIce;
    public float JumpCounter = 2;

    public Vector3 horizontalDirection;
    private Vector3 direction;
    private Vector3 viewForward;
    private Vector3 viewRight;
    private Vector2 moveInput;
    private float verticalValue;
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
    private void FixedUpdate()
    {
        GetMultipliers();
        MovePlayer();
        RotatePlayer();
    }
    private void GetMultipliers()
    {
        if (ShouldSlice() || isIceJumping)
        {
            // Ajusta la velocidad si está sobre hielo
            currentMultiplier = Mathf.SmoothDamp(currentMultiplier, iceSpeedMultiplier, ref _currentMultiplier, smoothTimeMultiplier);
            return;
        }
        //Si está en el aire, aplicar jumpingSpeedMultiplier
        currentMultiplier = (!IsGrounded() && !isIceJumping) ? jumpingSpeedMultiplier : 1f;
    }
    private void MovePlayer()
    {
        if (isJumping)
        {
            // Se puede ajustar la altura y el tiempo del salto
            float v = 2 * heightJump / timeJump;
            float g = -2 * heightJump / (timeJump * timeJump);

            v += g * Time.deltaTime;
            verticalValue = Mathf.SmoothDamp(0, v, ref _currentJumpVelocity, smoothTimeJump);
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
        viewForward = isFirstPerson ? firstCamera.transform.forward : thirdCamera.transform.forward;
        viewRight = isFirstPerson ? firstCamera.transform.right : thirdCamera.transform.right;
        viewForward = Vector3.Scale(viewForward, new Vector3(1, 0, 1)).normalized;
        horizontalDirection = moveInput.x * viewRight + moveInput.y * viewForward;
        direction = horizontalDirection * currentSpeed * currentMultiplier + Vector3.up * verticalValue;
        //Mou el personatge
        _characterController.Move(direction * Time.fixedDeltaTime);
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
    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        //Canviar els eixos depenent de la vista
    }
    public void Sprint(InputAction.CallbackContext context)
    {
        if (isCrouching || isJumping) return;
        isRunning = context.performed;
        currentSpeed = isRunning ? sprintSpeed : walkSpeed;
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
        isIceJumping = ShouldSlice();
        StartCoroutine(Jumping());
    }
    IEnumerator Jumping()
    {
        yield return new WaitForSeconds(0.5f);
        isJumping = false;
        isIceJumping = false;
    }
    public void Crouch(InputAction.CallbackContext context)
    {
        if (isRunning) return;
        isCrouching = context.performed;
        _characterController.height = isCrouching ? crouchingHeight : standingHeight;
        currentSpeed = isCrouching ? (walkSpeed * crouchSpeedMultiplier) : walkSpeed;
        if (context.started)
        {
            _characterController.center -= new Vector3(_characterController.center.x, (standingHeight - crouchingHeight) / 2, _characterController.center.z);
        }
        if (context.canceled)
        {
            _characterController.center = new Vector3(_characterController.center.x, standingHeight / 2, _characterController.center.z);
        }
    }
    public void ChangeCameraView(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isFirstPerson = !isFirstPerson;
        }
    }
    private bool ShouldSlice()
    {
        return Physics.CheckSphere(GroundChecker.position, groundSphereRadius, WhatIsIce);
    }
    private bool IsGrounded()
    {
        return Physics.CheckSphere(GroundChecker.position, groundSphereRadius, WhatIsGround);
    }
}
