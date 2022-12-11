using UnityEngine;

/// <summary>
/// A class to manage line renderers to act as tether visuals for the player.
/// </summary>
public class TetherRenderer : MonoBehaviour {
    [SerializeField] private PlayerControllerData _controllerData;
    [SerializeField] private Transform _rightTetherOrigin, _leftTetherOrigin;
    [SerializeField] private LineRenderer _leftLineRenderer, _rightLineRenderer;
    [SerializeField] private AudioSource _tetherSoundSource;
    private Transform _tetherEnd;
    private bool _isTethered = false;

    private void Update() {
        if (_isTethered && _tetherEnd != null) {
            UpdateLineRenderer(_leftLineRenderer, _rightTetherOrigin.position,
                               _tetherEnd.position);
            UpdateLineRenderer(_rightLineRenderer, _leftTetherOrigin.position,
                               _tetherEnd.position);
        }
    }

    private void UpdateLineRenderer(LineRenderer lineRenderer, Vector3 p1, Vector3 p2) {
        Vector3[] tetherPoints = new Vector3[2];
        tetherPoints[0] = p1;
        tetherPoints[1] = p2;
        lineRenderer.SetPositions(tetherPoints);
    }

    public void SetTether(Transform tetherEnd) {
        _isTethered = true;
        _tetherEnd = tetherEnd;
        _tetherSoundSource.PlayOneShot(_controllerData.TetherSound);
    }

    public void DetachTether() {
        _isTethered = false;
        UpdateLineRenderer(_leftLineRenderer, Vector3.zero, Vector3.zero);
        UpdateLineRenderer(_rightLineRenderer, Vector3.zero, Vector3.zero);
    }
}