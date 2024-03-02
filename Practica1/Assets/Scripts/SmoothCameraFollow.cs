using UnityEngine;
using UnityEngine.InputSystem;

public class SmoothCameraFollow : MonoBehaviour
{
    public Vector2 _XYoffset;
    public float _Zoffset;
    public float smoothTimeZ;
    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime;
    [SerializeField] private float dashOffsetZ;
    [SerializeField] private float moveOffsetXY;
    private Vector3 _currentVelocity = Vector3.zero;
    private Vector3 _offsetVector;
    private Vector3 targetPosition;

    private void Awake()
    {
        transform.position = target.position;
    }
    public void Move(InputAction.CallbackContext context) //S'activa al PlayerInput, fora de l'script, defineix _direction
    {
        _XYoffset = context.ReadValue<Vector2>() * moveOffsetXY;
    }
    private void FixedUpdate()
    {
        //Si fa dash, canvia la posició final fins que acabi de dashear.
        if (GameManager.Instance.IsDashing() == false)
        {
            //Setejar valor Zoffset
            Camera.main.orthographicSize = Mathf.SmoothStep(Camera.main.orthographicSize, _Zoffset, smoothTimeZ * Time.deltaTime);
            //Projectar vector offset a la orientació de la càmera
            _offsetVector = transform.rotation * new Vector3(_XYoffset.x, _XYoffset.y, 0);
            targetPosition = target.position + _offsetVector;
        }
        else
        {
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, _Zoffset + dashOffsetZ, smoothTimeZ * Time.deltaTime);
        }
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _currentVelocity, smoothTime * Time.deltaTime);
    }
}
