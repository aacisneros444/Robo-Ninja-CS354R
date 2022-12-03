using UnityEngine;
using System.Collections.Generic;

public class DebugPathRenderer : MonoBehaviour {
    public static DebugPathRenderer Instance { get; private set; }

    [SerializeField] private GameObject _pathMarker;
    private List<Vector3> _currentRenderingPath;
    private List<GameObject> _currentPathMarkers;

    private void Awake() {
        Instance = this;
        _currentPathMarkers = new List<GameObject>();
    }

    public void RenderPath(List<Vector3> path) {
        for (int i = 0; i < _currentPathMarkers.Count; i++) {
            GameObject.Destroy(_currentPathMarkers[i]);
        }
        _currentPathMarkers.Clear();
        foreach (Vector3 waypoint in path) {
            GameObject marker = Instantiate(_pathMarker, waypoint, Quaternion.identity);
            _currentPathMarkers.Add(marker);
        }
        _currentRenderingPath = path;
    }

    public void OnDrawGizmos() {
        if (Application.isPlaying && _currentRenderingPath != null) {
            for (int i = 0; i < _currentRenderingPath.Count - 1; i++) {
                Gizmos.DrawLine(_currentRenderingPath[i], _currentRenderingPath[i + 1]);
            }
        }
    }
}