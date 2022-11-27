using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyFSM : MonoBehaviour {
    [Header("Pathfinding")]
    [SerializeField] private OctreePathfinder _pathfinder;
    [SerializeField] private float _repathRate;
    [Header("Targeting")]
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private float _destinationOffsetFromPlayer;
    [Header("Locomotion")]
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _maxAcceleration;

    private StateMachine _fsm;
    private Rigidbody _rb;

    private void Awake() {
        _rb = GetComponent<Rigidbody>();
        _fsm = new StateMachine();
    }

    private void Start() {
        EnemyControllerData controllerData = new EnemyControllerData {
            pathFinder = _pathfinder,
            repathRate = _repathRate,
            rb = _rb,
            rootTransform = transform,
            playerTransform = _playerTransform,
            destinationOffsetFromPlayer = _destinationOffsetFromPlayer,
            maxSpeed = _maxSpeed,
            maxAcceleration = _maxAcceleration
        };
        _fsm.PushState(new NoDestinationState(_fsm, controllerData, _repathRate));
    }

    private void Update() {
        if (_fsm.GetCurrentState() != null) {
            _fsm.GetCurrentState().Update();
        }
    }

    private void FixedUpdate() {
        if (_fsm.GetCurrentState() != null) {
            _fsm.GetCurrentState().FixedUpdate();
        }
    }
}

public class EnemyControllerData {
    public OctreePathfinder pathFinder;
    public float repathRate;
    public Rigidbody rb;
    public Transform rootTransform;
    public Transform playerTransform;
    public float destinationOffsetFromPlayer;
    public float maxSpeed;
    public float maxAcceleration;
}