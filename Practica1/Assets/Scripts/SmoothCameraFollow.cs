using UnityEngine;
using UnityEngine.InputSystem;

public class SmoothCameraFollow : MonoBehaviour
{
    [Header("Camera and Target")]
    [SerializeField] private Camera firstPersonCamera;
    [SerializeField] private Camera thirdPersonCamera;
    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime;
    [SerializeField] Vector3 startOffsetPos;
    [Header("3D Z")]
    [SerializeField] private float smoothTimeZ;
    [SerializeField] private float startOrtSize;
    [SerializeField] private float zoomOrtSize;
    [Header("First person")]
    [SerializeField] private float sensitivity;
    [SerializeField] private float startFOV;
    [SerializeField] private float zoomOffset;
    [SerializeField] private float smoothTimeZoom;
    [SerializeField] private float crouchHeight = 3/2;

    private Vector3 _currentVelocity;
    private float currentZoom;
    private float _Zoffset;
    private Vector2 desiredRotation;
    private Vector2 currentRotation;

    private void Awake()
    {
        currentZoom = startFOV;
        _Zoffset = startOrtSize;
        transform.position = target.position + startOffsetPos;
    }
    private void FixedUpdate()
    {
        ZoomUpdate();
        //Camera Position Follow
        transform.position = Vector3.SmoothDamp(transform.position, target.position + startOffsetPos, ref _currentVelocity, smoothTime * Time.deltaTime);
        float height = (target.gameObject.GetComponent<PlayerController>().isCrouching) ? crouchHeight : 1f;
        firstPersonCamera.transform.position = new Vector3(transform.position.x, transform.position.y / height, transform.position.z);
    }
    private void Update()
    {
        //Camera Rotation Follow
        Rotate();
    }

    private void ZoomUpdate()
    {
        firstPersonCamera.fieldOfView = Mathf.SmoothStep(firstPersonCamera.fieldOfView, currentZoom, smoothTimeZoom * Time.deltaTime);
        thirdPersonCamera.orthographicSize = Mathf.SmoothStep(thirdPersonCamera.orthographicSize, _Zoffset, smoothTimeZ * Time.deltaTime);
    }

    private void Rotate()
    {
        if (desiredRotation.sqrMagnitude == 0) return;
        currentRotation.x += desiredRotation.y * sensitivity;
        currentRotation.y += desiredRotation.x * sensitivity;
        firstPersonCamera.transform.rotation = Quaternion.Euler(-Mathf.Clamp(currentRotation.x, -45f, 45f), currentRotation.y, 0);
    }
    public void ChangeView(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Cursor.visible = !Cursor.visible;
            firstPersonCamera.transform.rotation = target.rotation;
            firstPersonCamera.enabled = !firstPersonCamera.enabled;
            thirdPersonCamera.enabled = !thirdPersonCamera.enabled;
        }
    }
    public void Zoom(InputAction.CallbackContext context)
    {
        currentZoom = (context.ReadValue<float>() == 1) ? zoomOffset : startFOV;
        _Zoffset = (context.ReadValue<float>() == 1) ? zoomOrtSize : startOrtSize;
    }
    public void CameraRotate(InputAction.CallbackContext context)
    {
        if (!firstPersonCamera.enabled) return;
        desiredRotation = context.ReadValue<Vector2>();
    }
}
