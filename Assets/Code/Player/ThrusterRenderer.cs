using UnityEngine;

public class ThrusterRenderer : MonoBehaviour {
    [SerializeField] private TrailRenderer _rightTrail;
    [SerializeField] private TrailRenderer _leftTrail;
    // refactor out
    [SerializeField] private AudioSource _tetherReelingSound;
    [SerializeField] private Rigidbody _playerRb;
    [SerializeField] private float _maxSpeed;
    private int _numThrusterStates;

    private void Awake() {
        BurstForwardState.OnBurst += OnThrustRequired;
        BurstBackwardState.OnBurst += OnThrustRequired;
        BurstLeftState.OnBurst += OnThrustRequired;
        BurstRightState.OnBurst += OnThrustRequired;
        BurstUpState.OnBurst += OnThrustRequired;
        BurstDownState.OnBurst += OnThrustRequired;
        IsReelingState.OnReeling += OnThrustRequired;
        BurstForwardState.OnExitBurst += OnThurstStop;
        BurstBackwardState.OnExitBurst += OnThurstStop;
        BurstLeftState.OnExitBurst += OnThurstStop;
        BurstRightState.OnExitBurst += OnThurstStop;
        BurstUpState.OnExitBurst += OnThurstStop;
        BurstDownState.OnExitBurst += OnThurstStop;
        IsReelingState.OnStopReeling += OnThurstStop;
    }

    private void Start() {
        _rightTrail.emitting = false;
        _leftTrail.emitting = false;
    }

    private void Update() {
        if (_numThrusterStates > 0) {
            // gradually increase pitch based on speed
            _tetherReelingSound.pitch = 0.05f + (_playerRb.velocity.magnitude / _maxSpeed * 0.35f);
        }
    }

    private void OnThrustRequired() {
        _rightTrail.Clear();
        _leftTrail.Clear();
        _rightTrail.emitting = true;
        _leftTrail.emitting = true;
        _numThrusterStates++;
        // refactor sound out
        if (_numThrusterStates == 1) {
            // just started thrusting
            _tetherReelingSound.Play();
            _tetherReelingSound.pitch = 0.05f;
        }
    }

    private void OnThurstStop() {
        _numThrusterStates--;
        if (_numThrusterStates == 0) {
            _rightTrail.emitting = false;
            _leftTrail.emitting = false;
            _tetherReelingSound.Stop();
        } else if (_numThrusterStates == -1) {
            _numThrusterStates = 0;
        }
    }
}