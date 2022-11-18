using UnityEngine;

public class PlayerFSM : MonoBehaviour {
    [SerializeField] private Camera _cam;
    [SerializeField] private Transform _playerModel;

    [Header("Float/Hover")]
    [SerializeField]
    private float _groundCheckRayLength = 1f;
    [SerializeField]
    private float _floatHeight = 0.5f;
    [SerializeField]
    private float _floatSpringStrength = 4000f;
    [SerializeField]
    private float _floatSpringDamper = 250f;

    [Header("Grounded Locomotion")]
    [SerializeField]
    private float _maxSpeed = 8f;
    [SerializeField]
    private float _acceleration = 200f;
    [SerializeField]
    private float _maxAcceleration = 150f;
    [SerializeField]
    private float _jumpStrength = 5000f;

    [Header("Aerial Locomotion")]
    [SerializeField]
    private float _maxBurstSpeed = 16f;
    [SerializeField]
    private float _maxBurstAcceleration = 50f;

    private PlayerControllerData _playerControllerData;
    private Rigidbody _rb;
    private StateMachine _fsm;

    private void Awake() {
        _rb = GetComponent<Rigidbody>();
        _playerControllerData = new PlayerControllerData {
            cam = _cam,
            rootTransform = transform,
            playerModel = _playerModel,
            rb = _rb,
            groundCheckRayLength = _groundCheckRayLength,
            floatHeight = _floatHeight,
            floatSpringStrength = _floatSpringStrength,
            floatSpringDamper = _floatSpringDamper,
            maxSpeed = _maxSpeed,
            acceleration = _acceleration,
            maxAcceleration = _maxAcceleration,
            jumpStrength = _jumpStrength,
            maxBurstSpeed = _maxBurstSpeed,
            maxBurstAcceleration = _maxBurstAcceleration
        };
        _fsm = new StateMachine();
        _fsm.PushState(new PlayerGroundedState(_fsm, _playerControllerData));
    }

    private void Update() {
        _fsm.GetCurrentState().Update();
    }

    private void FixedUpdate() {
        _fsm.GetCurrentState().FixedUpdate();
    }
}

public class PlayerControllerData {
    public Camera cam;
    public Transform rootTransform;
    public Transform playerModel;
    public Rigidbody rb;
    public float groundCheckRayLength;
    public float floatHeight;
    public float floatSpringStrength;
    public float floatSpringDamper;
    public float maxSpeed;
    public float acceleration;
    public float maxAcceleration;
    public float jumpStrength;
    public float maxBurstSpeed;
    public float maxBurstAcceleration;
}