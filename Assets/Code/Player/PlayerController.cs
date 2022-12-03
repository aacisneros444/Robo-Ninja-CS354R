using UnityEngine;

public class PlayerController : MonoBehaviour {
    [Header("Player Controller Settings Data")]
    [SerializeField] private PlayerControllerData _controllerData;

    [Header("General Purpose Components")]
    [SerializeField] private Camera _cam;
    [SerializeField] private Transform _playerModel;
    [SerializeField] private Animator _playerAnimator;
    private Rigidbody _rb;

    [Header("Tether Components")]
    [SerializeField] private Transform _rightTetherOrigin;
    [SerializeField] private Transform _leftTetherOrigin;

    [Header("Attacking Components")]
    [SerializeField] private GameObject _attackTriggerCollider;

    // Main fsm which handles top level states.
    private StateMachine _mainFsm;
    // Fsm which handles tether states.
    private StateMachine _tetherFsm;

    // Read-only properties for above fields.

    // State Machines
    public StateMachine MainFsm => _mainFsm;
    public StateMachine TetherFsm => _tetherFsm;

    // Controller Data
    public PlayerControllerData ControllerData => _controllerData;

    // General Purpose Components
    public Camera Cam => _cam;
    public Transform PlayerModel => _playerModel;
    public Animator PlayerAnimator => _playerAnimator;
    public Rigidbody Rb => _rb;

    // Tether Components
    public Transform RightTetherOrigin => _rightTetherOrigin;
    public Transform LeftTetherOrigin => _leftTetherOrigin;

    // Attacking Components
    public GameObject AttackTriggerCollider => _attackTriggerCollider;

    private void Awake() {
        _rb = GetComponent<Rigidbody>();

        _mainFsm = new StateMachine();
        _mainFsm.PushState(new PlayerGroundedState(_mainFsm, this));

        _tetherFsm = new StateMachine();
        _tetherFsm.PushState(new NotTetheredState(_tetherFsm, this, null));
    }

    private void Update() {
        _mainFsm.Update();
        _tetherFsm.Update();
    }

    private void FixedUpdate() {
        _mainFsm.FixedUpdate();
        _tetherFsm.FixedUpdate();
    }
}