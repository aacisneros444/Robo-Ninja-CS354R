using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyFSM : MonoBehaviour {
    [Header("Pathfinding")]
    [SerializeField] private OctreePathfinder _pathfinder;
    [SerializeField] private float _repathRate;
    [Header("Targeting")]
    [SerializeField] private Octree _worldOctree;
    [SerializeField] private Rigidbody _playerRb;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private float _destinationOffsetFromPlayer;
    [Header("Locomotion")]
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _maxAcceleration;
    [Header("Attacking")]
    [SerializeField] private float _attackRange;
    [SerializeField] private float _attackReadyingTime;
    [SerializeField] private float _explosionRadius;
    [SerializeField] private Transform _projectileOrigin;
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private GameObject _attackIndicatorPrefab;

    private StateMachine _fsm;
    private Rigidbody _rb;

    private void Awake() {
        _rb = GetComponent<Rigidbody>();
        _fsm = new StateMachine();
    }

    private void Start() {
        EnemyControllerData controllerData = new EnemyControllerData {
            worldOctree = _worldOctree,
            pathFinder = _pathfinder,
            repathRate = _repathRate,
            rb = _rb,
            rootTransform = transform,
            playerRb = _playerRb,
            playerTransform = _playerTransform,
            destinationOffsetFromPlayer = _destinationOffsetFromPlayer,
            maxSpeed = _maxSpeed,
            maxAcceleration = _maxAcceleration,
            attackRange = _attackRange,
            attackReadyingTime = _attackReadyingTime,
            explosionRadius = _explosionRadius,
            projectileOrigin = _projectileOrigin,
            projectilePrefab = _projectilePrefab,
            attackIndicatorPrefab = _attackIndicatorPrefab
        };
        _fsm.PushState(new NoDestinationState(_fsm, controllerData, _repathRate));
    }

    private void Update() {
        if (_fsm.GetCurrentState() != null) {
            _fsm.GetCurrentState().Update();
            //_fsm.PrintCurrentState();
        }
    }

    private void FixedUpdate() {
        if (_fsm.GetCurrentState() != null) {
            _fsm.GetCurrentState().FixedUpdate();
        }
    }
}

public class EnemyControllerData {
    public Octree worldOctree;
    public OctreePathfinder pathFinder;
    public float repathRate;
    public Rigidbody rb;
    public Transform rootTransform;
    public Rigidbody playerRb;
    public Transform playerTransform;
    public float destinationOffsetFromPlayer;
    public float maxSpeed;
    public float maxAcceleration;
    public float attackRange;
    public float attackReadyingTime;
    public float explosionRadius;
    public Transform projectileOrigin;
    public GameObject projectilePrefab;
    public GameObject attackIndicatorPrefab;
}