using UnityEngine;

/// <summary>
/// A class to control trail renderers for the player for the appearance of thrusters.
/// </summary>
public class ThrusterRenderer : MonoBehaviour {
    [SerializeField] private PlayerControllerData _controllerData;
    [SerializeField] private TrailRenderer _rightTrail, _leftTrail;
    [SerializeField] private AudioSource _thrusterSoundSource;
    [SerializeField] private Rigidbody _playerRb;
    [Tooltip("Player rigidbody speed at which thruster sound is at max pitch.")]
    [SerializeField] private float _maxSoundSpeed;
    private int _numThrusterStates;

    private void Start() {
        _thrusterSoundSource.clip = _controllerData.ThrustersSound;
        _rightTrail.emitting = false;
        _leftTrail.emitting = false;
    }

    private void Update() {
        if (_numThrusterStates > 0) {
            ChangeThrusterAudioPitch();
        }
    }

    // Gradually increase pitch based on speed.
    private void ChangeThrusterAudioPitch() {
        float pitchFactor = (_playerRb.velocity.magnitude / _maxSoundSpeed);
        const float BasePitch = 0.05f;
        const float MaxPitchAdd = 0.35f;
        _thrusterSoundSource.pitch = BasePitch + pitchFactor * MaxPitchAdd;
    }

    public void AddThrust() {
        _rightTrail.Clear();
        _leftTrail.Clear();
        _rightTrail.emitting = true;
        _leftTrail.emitting = true;
        _numThrusterStates++;
        // refactor sound out
        if (_numThrusterStates == 1) {
            // just started thrusting
            _thrusterSoundSource.Play();
        }
    }

    public void RemoveThrust() {
        _numThrusterStates--;
        if (_numThrusterStates == 0) {
            _rightTrail.emitting = false;
            _leftTrail.emitting = false;
            _thrusterSoundSource.Stop();
        } else if (_numThrusterStates == -1) {
            _numThrusterStates = 0;
        }
    }

    private void StopThrustImmediate() {
        _numThrusterStates = 0;
        _rightTrail.emitting = false;
        _leftTrail.emitting = false;
        _thrusterSoundSource.Stop();
    }
}