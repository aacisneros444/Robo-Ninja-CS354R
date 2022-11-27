using UnityEngine;

public class PlayerFSM : MonoBehaviour {
    [SerializeField] private Camera _cam;
    [SerializeField] private Transform _playerModel;
    [SerializeField] private Animator _playerAnimator;

    [Header("Float/Hover")]
    [SerializeField] private float _groundCheckRayLength = 1f;
    [SerializeField] private float _floatHeight = 0.5f;
    [SerializeField] private float _floatSpringStrength = 4000f;
    [SerializeField] private float _floatSpringDamper = 250f;

    [Header("Grounded Locomotion")]
    [SerializeField] private float _maxSpeed = 7f;
    [SerializeField] private float _maxAcceleration = 150f;
    [SerializeField] private float _jumpStrength = 2700f;

    [Header("Aerial Locomotion")]
    [SerializeField] private float _burstStartDampFactor = 0.5f;
    [SerializeField] private float _maxBurstSpeed = 22f;
    [SerializeField] private float _maxBurstAcceleration = 75f;

    [Header("Tether")]
    [SerializeField] private Transform _rightTetherOrigin;
    [SerializeField] private Transform _leftTetherOrigin;
    [SerializeField] private float _maxTetherFireDistance = 200f;
    [SerializeField] private float _tetherSpringStrength = 250f;
    [SerializeField] private float _tetherSpringDamper = 200f;
    [SerializeField] private float _maxReelSpeed = 22f;
    [SerializeField] private float _maxReelAcceleration = 50f;
    [SerializeField] private float _horizontalSwingStrength = 20f;

    [Header("Attacking")]
    [SerializeField] private float _enemySphereCastLatchRadius = 1.5f;
    [SerializeField] private float _attackRayDistance = 2f;
    [SerializeField] private GameObject _attackTriggerCollider;

    private Rigidbody _rb;
    private StateMachine _fsm;

    private void Awake() {
        _rb = GetComponent<Rigidbody>();
        PlayerControllerData controllerData = new PlayerControllerData {
            cam = _cam,
            rootTransform = transform,
            playerModel = _playerModel,
            rb = _rb,
            animator = _playerAnimator,
            groundCheckRayLength = _groundCheckRayLength,
            floatHeight = _floatHeight,
            floatSpringStrength = _floatSpringStrength,
            floatSpringDamper = _floatSpringDamper,
            maxSpeed = _maxSpeed,
            maxAcceleration = _maxAcceleration,
            jumpStrength = _jumpStrength,
            burstStartDampFactor = _burstStartDampFactor,
            maxBurstSpeed = _maxBurstSpeed,
            maxBurstAcceleration = _maxBurstAcceleration,
            maxTetherFireDistance = _maxTetherFireDistance,
            rightTetherOrigin = _rightTetherOrigin,
            leftTetherOrigin = _leftTetherOrigin,
            tetherSpringStrength = _tetherSpringStrength,
            tetherSpringDamper = _tetherSpringDamper,
            maxReelSpeed = _maxReelSpeed,
            maxReelAcceleration = _maxReelAcceleration,
            horitontalSwingStrength = _horizontalSwingStrength,
            enemySphereCastLatchRadius = _enemySphereCastLatchRadius,
            attackRayDistance = _attackRayDistance,
            attackTriggerCollider = _attackTriggerCollider
        };

        StateMachine tetherFsm = new StateMachine();
        tetherFsm.PushState(new NotTetheredState(tetherFsm, controllerData, null));

        _fsm = new StateMachine();
        _fsm.PushState(new PlayerGroundedState(_fsm, tetherFsm, controllerData));
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
    public Animator animator;
    public float groundCheckRayLength;
    public float floatHeight;
    public float floatSpringStrength;
    public float floatSpringDamper;
    public float maxSpeed;
    public float acceleration;
    public float maxAcceleration;
    public float jumpStrength;
    public float burstStartDampFactor;
    public float maxBurstSpeed;
    public float maxBurstAcceleration;
    public float maxTetherFireDistance;
    public Transform rightTetherOrigin;
    public Transform leftTetherOrigin;
    public float tetherSpringStrength;
    public float tetherSpringDamper;
    public float maxReelSpeed;
    public float maxReelAcceleration;
    public float horitontalSwingStrength;
    public float enemySphereCastLatchRadius;
    public float attackRayDistance;
    public GameObject attackTriggerCollider;
}