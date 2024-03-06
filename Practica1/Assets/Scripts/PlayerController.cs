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
    public float speed;
    [Header("Dash")]
    [SerializeField] private Cooldown dashCooldown;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private int dashMaxCharges;
    [SerializeField] private float dashChargeCooldown;
    public bool isDashing;
    public bool canDash = true;
    [SerializeField] private int dashCharges; //nom�s per fer debug
    private bool dashIsCharging;
    [Header("Camera")]
    [SerializeField] private float smoothTime = 0.05f;

    private Vector3 _direction;
    private float _currentVelocity;

    private Rigidbody _rb;
    private Camera _mainCamera;


    private void Awake()
    {
        canDash = true;
        _rb = GetComponent<Rigidbody>();
        _mainCamera = Camera.main;
        dashCharges = dashMaxCharges;
        Cursor.lockState = CursorLockMode.Locked;
    }
    //Al GameObject: PlayerInput -> Events -> Gameplay
    public void Move(InputAction.CallbackContext context) //S'activa al PlayerInput, fora de l'script, defineix _direction
    {
        Vector2 input = context.ReadValue<Vector2>();
        //Canviar els eixos als de la Main Camera.
        Vector3 cameraForward = Vector3.Scale(_mainCamera.transform.forward, new Vector3(1, 0, 1)).normalized;
        _direction = input.x * _mainCamera.transform.right + input.y * cameraForward;
    }
    public void Dash(InputAction.CallbackContext context) //S'activa al PlayerInput, fora de l'script, fa dash.
    {
        if (dashCharges < 1 || dashCooldown.IsCoolingDown || !context.started || !canDash) return;
        dashCharges--;
        StartCoroutine(Dash());
        dashCooldown.StartCooldown();
        if (!dashIsCharging)
        {
            dashIsCharging = true;
            StartCoroutine(DashChargeCooldown());
        }
    }
    IEnumerator Dash()
    {
        isDashing = true;
        float startTime = Time.time;
        while (Time.time < startTime + dashTime)
        {
            _rb.MovePosition(transform.position + transform.forward * dashSpeed * Time.fixedDeltaTime);

            yield return null;
        }
        isDashing = false;
    }
    IEnumerator DashChargeCooldown()
    {
        while (dashCharges < dashMaxCharges)
        {
            yield return new WaitForSecondsRealtime(dashChargeCooldown); // Pause for a second
            dashCharges++;
        }
        dashIsCharging = false;
    }
    private void FixedUpdate()
    {
        if (_direction.sqrMagnitude == 0 || isDashing) return;
        MovePlayer();
    }
    private void Update()
    {
        if (_direction.sqrMagnitude == 0 || isDashing) return;
        RotatePlayer();
    }
    private void RotatePlayer()
    {
        //Si no hi ha cap input, no rotem.
        //Agafa l'angle de la direcci� on vol anar
        var targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg;
        //Modifica l'angle de forma smooth per anar d'angle on vol anar des de angle actual.
        var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _currentVelocity, smoothTime);
        transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
    }
    private void MovePlayer()
    {
        //Mou el personatge
   
        _rb.MovePosition(transform.position + _direction * speed * Time.fixedDeltaTime);
    }
}
