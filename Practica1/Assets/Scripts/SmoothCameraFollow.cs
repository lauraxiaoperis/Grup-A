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
    [SerializeField] private Vector2 firstOffsetXY;
    [SerializeField] private float moveOffsetXY;
    [SerializeField] private float zoomOffset;
    [SerializeField] private float smoothTimeZoom;
    [SerializeField] private float startFOV;
    private Vector3 _currentVelocity = Vector3.zero;
    private Vector3 _offsetVector;
    private Vector3 targetPosition;
    private float currentZoom;
    [SerializeField] Camera firstPersonCamera;
    [SerializeField] Camera thirdPersonCamera;

    private void Awake()
    {
        currentZoom = startFOV;
        transform.position = target.position;
    }
    public void Move(InputAction.CallbackContext context) //S'activa al PlayerInput, fora de l'script, defineix _direction
    {
        _XYoffset = firstOffsetXY + context.ReadValue<Vector2>() * moveOffsetXY;
    }
    public void ChangeView(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            firstPersonCamera.enabled = !firstPersonCamera.enabled;
            thirdPersonCamera.enabled = !thirdPersonCamera.enabled;
            firstPersonCamera.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
    public void Zoom(InputAction.CallbackContext context)
    {
        Debug.Log(context.ReadValue<float>());
        if (context.ReadValue<float>() == 1)
        {
            currentZoom = zoomOffset;
        }
        else
        {
            currentZoom = startFOV;
        }
    }
    public void CameraRotate(InputAction.CallbackContext context)
    {
        firstPersonCamera.transform.rotation = Quaternion.Euler(context.ReadValue<Vector2>().y, context.ReadValue<Vector2>().x, 0);
    }
    private void FixedUpdate()
    {
        firstPersonCamera.fieldOfView = Mathf.SmoothStep(firstPersonCamera.fieldOfView, currentZoom, smoothTimeZoom * Time.deltaTime);
        thirdPersonCamera.orthographicSize = Mathf.SmoothStep(thirdPersonCamera.orthographicSize, _Zoffset, smoothTimeZ * Time.deltaTime);
        //Projectar vector offset a la orientaci� de la c�mera
        _offsetVector = transform.rotation * new Vector3(_XYoffset.x, _XYoffset.y, 0);
        targetPosition = target.position + _offsetVector;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _currentVelocity, smoothTime * Time.deltaTime);
    }
}
