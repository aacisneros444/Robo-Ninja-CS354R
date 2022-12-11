using UnityEngine;

public class PlayerCameraController : MonoBehaviour {

    [SerializeField] private Camera _cam;

    [Tooltip("The transform to follow.")]
    [SerializeField] private Transform _follow;

    [Tooltip("Transform that the camera will be offset from. Camera should be a child of this transform.")]
    [SerializeField] private Transform _cameraCenter;

    [Tooltip("A transform rotated to compute where the camera center should be.")]
    [SerializeField] private Transform _orbiter;

    [Tooltip("How far away should the camera be from the follow target.")]
    [SerializeField] private float _zOffset = -3.5f;

    [Tooltip("A y offset applied to the camera following the follow target.")]
    [SerializeField] private float _yOffset = 1.15f;

    [Tooltip("How far should the camera orbit around the follow target.")]
    [SerializeField] private float _orbitRadius = 0.8f;

    [Tooltip("Camera sensitivity")]
    [SerializeField] private float _sensitivity = 3f;

    /// <summary>
    /// Defines whether or not the camera controller is currently responding to input.
    /// </summary>
    public bool TakingInput = true;

    private void Start() {
        transform.parent = null;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update() {
        Vector3 mouseInput = new Vector3(Input.GetAxis("Mouse X"),
            Input.GetAxis("Mouse Y"), 0f) * _sensitivity;

        if (!TakingInput) {
            mouseInput = Vector3.zero;
        }

        // Calculate camera center's position which is orbit radius away from follow.
        _orbiter.position = _follow.position + new Vector3(0f, _yOffset, 0f);
        _orbiter.rotation = Quaternion.Euler(_orbiter.eulerAngles.x,
                                             _orbiter.eulerAngles.y + mouseInput.x,
                                             0f);
        // Set camera center position
        _cameraCenter.position = _orbiter.position + _orbiter.right * _orbitRadius;

        // Set camera rotation.
        Vector3 currEulerAngles = _cameraCenter.transform.eulerAngles;
        float newRotX = currEulerAngles.x - mouseInput.y;
        float newRotY = currEulerAngles.y + mouseInput.x;
        if (newRotX > 90f && newRotX < 180f) {
            // clamp to quadrant 1
            newRotX = 90f;
        } else if (newRotX > 180f && newRotX < 270f) {
            // clamp to quadrant 4
            newRotX = 270f;
        }
        Quaternion newRotation = Quaternion.Euler(newRotX, newRotY, currEulerAngles.z);
        _cameraCenter.rotation = newRotation;

        // // Set camera z offset.
        _cam.transform.localPosition = new Vector3(0f, 0f, _zOffset);

        // Check for collisions
        Vector3 centerToCamDir = (_cam.transform.position - _cameraCenter.position).normalized;
        if (Physics.Raycast(_cameraCenter.position, centerToCamDir,
                            out RaycastHit rayHit, Mathf.Abs(_zOffset),
                            ~LayerMask.GetMask("Player"))) {
            _cam.transform.position = rayHit.point;
        }
    }
}