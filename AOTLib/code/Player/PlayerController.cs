using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

    [SerializeField] private Camera _cam;
    [SerializeField] private Transform _playerModel;

    [Header("Float/Hover")]
    [SerializeField]
    private float _groundCheckRayLength = 1f;
    [SerializeField]
    private float _floatHeight = 0.5f;
    [SerializeField]
    private float _floatSpringStrength = 2000f;
    [SerializeField]
    private float _floatSpringDamper = 100f;

    [Header("Locomotion")]
    [SerializeField]
    private float _maxSpeed = 8f;
    [SerializeField]
    private float _acceleration = 200f;
    [SerializeField]
    private float _maxAcceleration = 150f;

    private Rigidbody _rb;

    private void Awake() {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"),
            0f, Input.GetAxisRaw("Vertical"));
        Vector3 moveDir = (Quaternion.AngleAxis(_cam.transform.eulerAngles.y, Vector3.up) * input).normalized;

        ApplyFloatForceGrounded();
        ApplyGroundedMovement(moveDir);
        if (input.magnitude > 0f) {
            _playerModel.rotation = Quaternion.LookRotation(moveDir);
        }
    }

    private void ApplyFloatForceGrounded() {
        RaycastHit rayHit;
        if (Physics.Raycast(transform.position, Vector3.down,
            out rayHit, _groundCheckRayLength)) {
            Vector3 hitRbVelocity = Vector3.zero;
            if (rayHit.rigidbody != null) {
                hitRbVelocity = rayHit.rigidbody.velocity;
            }

            // Find the amount of velocity in the direction of the raycast
            // for our own rigidbody and a rigidbody we may have hit.
            float downVelocity = Vector3.Dot(Vector3.down, _rb.velocity);
            float otherRbVelocity = Vector3.Dot(Vector3.down, hitRbVelocity);

            float relativeVelocity = downVelocity - otherRbVelocity;

            float x = rayHit.distance - _floatHeight;
            // spring formula: f = -kx-bv
            // where f is force to apply, k is spring stiffness, x is displacement,
            // b is dampening value, and v is velocity
            float springForce = -1 * ((_floatSpringStrength * x) - (_floatSpringDamper * downVelocity));
            _rb.AddForce(Vector3.up * springForce);
        }
    }

    private void ApplyGroundedMovement(Vector3 direction) {
        // Determine if character is on a rigidbody to
        // save that body's velocity. It will need to
        // be applied to the character (think 
        // moving platform).
        RaycastHit rayHit;
        Vector3 groundVelocity = Vector3.zero;
        if (Physics.Raycast(transform.position, Vector3.down,
            out rayHit, _groundCheckRayLength)) {
            if (rayHit.rigidbody != null) {
                groundVelocity = rayHit.rigidbody.velocity;
            }
        } else {
            // Can't move while not grounded.
            return;
        }

        Vector3 goalVelocity = (direction.normalized * _maxSpeed) + groundVelocity;

        Vector3 oldRbVelocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        Vector3 goalAccel = (goalVelocity - oldRbVelocity) / Time.fixedDeltaTime;
        goalAccel = Vector3.ClampMagnitude(goalAccel, _maxAcceleration);

        // F=ma
        _rb.AddForce(_rb.mass * goalAccel);
    }
}