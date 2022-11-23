using UnityEngine;

public class ThursterRenderer : MonoBehaviour {
    [SerializeField] private TrailRenderer _rightR;
    [SerializeField] private TrailRenderer _leftR;
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
        _rightR.emitting = false;
        _leftR.emitting = false;
    }

    private void OnThrustRequired() {
        _rightR.Clear();
        _leftR.Clear();
        _rightR.emitting = true;
        _leftR.emitting = true;
        _numThrusterStates++;
    }

    private void OnThurstStop() {
        _numThrusterStates--;
        if (_numThrusterStates == 0) {
            _rightR.emitting = false;
            _leftR.emitting = false;
        } else if (_numThrusterStates == -1) {
            _numThrusterStates = 0;
        }
    }
}