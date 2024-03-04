using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    private Vector2 _inputMovement;
    public Vector2 InputMove { get { return _inputMovement; } }

    private bool _jumped;
    
    public bool Jumped { get { return _jumped; } }

    private bool _sprinting;
    public bool Sprinting { get { return _sprinting; } }

    private Keyboard keyboard; // Para detectar las entradas del teclado

    private void Awake()
    {
        keyboard = Keyboard.current;
    }

    private void LateUpdate()
    {
        _jumped = false;

        // Leer el estado de la tecla Shift directamente
        if (keyboard != null)
        {
            _sprinting = keyboard.shiftKey.isPressed;
        }
    }

    private void OnMove(InputValue input)
    {
        _inputMovement = input.Get<Vector2>();
    }

    void OnJump()
    {
        _jumped = true;
    }
}