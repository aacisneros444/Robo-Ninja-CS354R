using UnityEngine;

/// <summary>
/// Octree monobehavior adapter. Handles creating
/// root OctreeNode and finding registered colliders
/// for creating octree.
/// </summary>
public class Octree : MonoBehaviour {
    [SerializeField] private int _worldSize = 512;
    [SerializeField] private int _minNodeSize = 8;
    [SerializeField] private int _maxNodeSize = 32;
    [SerializeField] private bool _drawEntireTree = false;

    private OctreeNode _root;
    public OctreeNode Root { get { return _root; } }

    private OctreeNode _posDrawTarget;


    private void Awake() {
        if ((_worldSize & (_worldSize - 1)) != 0) {
            Debug.LogError("World size not a power of 2.");
            return;
        }

        float worldRadius = _worldSize / 2;
        Vector3 worldCenter = Vector3.one * worldRadius;
        Vector3 worldSize = Vector3.one * _worldSize;
        Bounds worldBounds = new Bounds(worldCenter, worldSize);

        _root = new OctreeNode(worldBounds, null, 0, _minNodeSize, _maxNodeSize);

        GameObject[] collisionObjects = GameObject.FindGameObjectsWithTag("Octree Collider");
        _root.DivideForObjects(collisionObjects);
    }

    private void OnDrawGizmos() {
        if (Application.isPlaying) {
            if (_drawEntireTree) {
                _root.Draw();
            }
            if (_posDrawTarget != null) {
                _posDrawTarget.DrawOnlySelf();
                _posDrawTarget.DrawEmptyNeighbors();
            }
        }
    }

    /// <summary>
    /// Given a position highlight the OctreeNode at that position.
    /// </summary>
    /// <param name="positon">The given position.</param>
    public void HighlightNodeAtPosition(Vector3 positon) {
        _posDrawTarget = _root.GetLeafNodeAtPosition(positon);
    }

    /// <summary>
    /// Given an OctreeNode, draw only it (no children).
    /// Expects to be called from OnDrawGizmos.
    /// </summary>
    /// <param name="node">The OctreeNode to draw.</param>
    public void DrawSingleNode(OctreeNode node) {
        node.DrawOnlySelf();
        node.DrawEmptyNeighbors();
    }
}
