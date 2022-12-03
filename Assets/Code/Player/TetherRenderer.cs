using UnityEngine;

public class TetherRenderer : MonoBehaviour {
    [SerializeField] private LineRenderer _leftLineRenderer;
    [SerializeField] private LineRenderer _rightLineRenderer;
    // refactor out
    [SerializeField] private AudioSource _onTetherSound;
    [SerializeField] private PlayerController _controller;
    private Transform _tetherEnd;
    private bool _isTethered = false;

    private void Awake() {
        NotTetheredState.ExitedTetherState += OnDetachTether;
        IsTetheredState.EnteredTetherState += OnTether;
    }

    private void OnDestroy() {
        IsTetheredState.EnteredTetherState -= OnTether;
    }

    private void Update() {
        if (_isTethered && _tetherEnd != null) {
            UpdateLineRenderer(_leftLineRenderer, _controller.RightTetherOrigin.position,
                               _tetherEnd.position);
            UpdateLineRenderer(_rightLineRenderer, _controller.LeftTetherOrigin.position,
                               _tetherEnd.position);
        }
    }

    private void UpdateLineRenderer(LineRenderer lineRenderer, Vector3 p1, Vector3 p2) {
        Vector3[] tetherPoints = new Vector3[2];
        tetherPoints[0] = p1;
        tetherPoints[1] = p2;
        lineRenderer.SetPositions(tetherPoints);
    }

    private void OnTether(Transform tetherEnd) {
        _isTethered = true;
        _tetherEnd = tetherEnd;
        _onTetherSound.Play();
    }

    private void OnDetachTether() {
        _isTethered = false;
        UpdateLineRenderer(_leftLineRenderer, Vector3.zero, Vector3.zero);
        UpdateLineRenderer(_rightLineRenderer, Vector3.zero, Vector3.zero);
    }
}