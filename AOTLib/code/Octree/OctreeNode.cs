using UnityEngine;
using System.Collections.Generic;

public enum Octant {
    LDF, // left, down, front
    RDF, // right, down, front
    LUF, // left, up, front
    RUF, // right, up, front
    LDB, // left, down, back
    RDB, // right, down, back
    LUB, // left, up, back
    RUB, // right, up, back
}

public enum Face {
    Right,
    Left,
    Up,
    Down,
    Front,
    Back
}

public static class FaceExtensions {
    public static readonly Face[] s_opposites = {
        Face.Left,
        Face.Right,
        Face.Down,
        Face.Up,
        Face.Back,
        Face.Front
    };

    public static Face Opposite(this Face dir) {
        return s_opposites[(int)dir];
    }
}

/// <summary>
/// A class to model a node for a sparse voxel octree.
/// </summary>
public class OctreeNode {
    private static readonly Vector3[] s_neighborVectorDirs = {
        Vector3.right,
        Vector3.left,
        Vector3.up,
        Vector3.down,
        Vector3.back,
        Vector3.forward
    };
    private static readonly int[,] s_subOctantsInDirection = {
        { (int)Octant.RDF, (int)Octant.RUF, (int)Octant.RDB, (int)Octant.RUB }, // sub octants in right dir
        { (int)Octant.LDF, (int)Octant.LUF, (int)Octant.LDB, (int)Octant.LUB }, // sub octants in left dir
        { (int)Octant.LUF, (int)Octant.RUF, (int)Octant.LUB, (int)Octant.RUB }, // sub octants in up dir
        { (int)Octant.LDF, (int)Octant.RDF, (int)Octant.LDB, (int)Octant.RDB }, // sub octants in down dir
        { (int)Octant.LDF, (int)Octant.RDF, (int)Octant.LUF, (int)Octant.RUF }, // sub octants in front dir
        { (int)Octant.LDB, (int)Octant.RDB, (int)Octant.LUB, (int)Octant.RUB }, // sub octants in back dir
    };
    private static OctreeNode s_root;
    private static int[] s_layerSizes;

    private Bounds _nodeBounds;
    public Bounds NodeBounds { get { return _nodeBounds; } }
    private OctreeNode _parent;
    private int _depth;
    private int _minSize;
    private int _maxSize;
    private int _nodeSize { get { return Mathf.RoundToInt(_nodeBounds.size.x); } }
    private int _numContainedObjects;

    private OctreeNode[] _children;
    private Bounds[] _childrenBounds;
    private OctreeNode[] _faceNeighbors;
    private List<OctreeNode> _emptyNeighbors;
    public List<OctreeNode> EmptyNeighbors { get { return _emptyNeighbors; } }

    public OctreeNode(Bounds bounds, OctreeNode parent, int depth, int minSize, int maxSize) {
        this._nodeBounds = bounds;
        this._parent = parent;
        this._depth = depth;
        this._minSize = minSize;
        this._maxSize = maxSize;
        this._numContainedObjects = 0;
        if (_nodeSize > minSize) {
            // Only store children bounds for non-leaf nodes.
            ComputeAndStoreChildrenBounds();
        }
        if (depth == 0) {
            // If root node, store self for children to use.
            s_root = this;
            // Store layer sizes for future use.
            int numLayers = Mathf.RoundToInt(Mathf.Log(_nodeSize, 2)) -
                                Mathf.RoundToInt(Mathf.Log(minSize, 2)) + 1;
            s_layerSizes = new int[numLayers];
            int layerSize = _nodeSize;
            for (int i = 0; i < numLayers; i++) {
                s_layerSizes[i] = layerSize;
                layerSize /= 2;
            }
        }


        // Enforce maximum octant size by force dividing octants which are too big.
        if (_nodeSize > _maxSize) {
            _children = new OctreeNode[8];
            for (int i = 0; i < 8; i++) {
                _children[i] = new OctreeNode(_childrenBounds[i], this, depth + 1, minSize, maxSize);
            }
        } else {
            // <= maximum octant size, will have face neighbors.
            _faceNeighbors = new OctreeNode[6];
        }
    }

    /// <summary>
    /// Compute 8 child octant bounds from this node's bounds.
    /// </summary>
    private void ComputeAndStoreChildrenBounds() {
        float quarter = _nodeSize / 4.0f;
        float childLength = _nodeSize / 2.0f;
        Vector3 childSize = new Vector3(childLength, childLength, childLength);
        _childrenBounds = new Bounds[8];
        _childrenBounds[(int)Octant.LDF] =
            new Bounds(_nodeBounds.center + new Vector3(-quarter, -quarter, -quarter), childSize);
        _childrenBounds[(int)Octant.RDF] =
            new Bounds(_nodeBounds.center + new Vector3(quarter, -quarter, -quarter), childSize);
        _childrenBounds[(int)Octant.LUF] =
            new Bounds(_nodeBounds.center + new Vector3(-quarter, quarter, -quarter), childSize);
        _childrenBounds[(int)Octant.RUF] =
            new Bounds(_nodeBounds.center + new Vector3(quarter, quarter, -quarter), childSize);
        _childrenBounds[(int)Octant.LDB] =
            new Bounds(_nodeBounds.center + new Vector3(-quarter, -quarter, quarter), childSize);
        _childrenBounds[(int)Octant.RDB] =
            new Bounds(_nodeBounds.center + new Vector3(quarter, -quarter, quarter), childSize);
        _childrenBounds[(int)Octant.LUB] =
            new Bounds(_nodeBounds.center + new Vector3(-quarter, quarter, quarter), childSize);
        _childrenBounds[(int)Octant.RUB] =
            new Bounds(_nodeBounds.center + new Vector3(quarter, quarter, quarter), childSize);
    }

    /// <summary>
    /// Divide the tree for an array of GameObjects. Will subdivide
    /// tree as if to place objects in smallest possible octants.
    /// </summary>
    /// <param name="gameObjects">The array of GameObjects to divide for.</param>
    public void DivideForObjects(GameObject[] gameObjects) {
        if (_depth > 0) {
            Debug.LogError("Can only add objects to root");
            return;
        }
        foreach (GameObject go in gameObjects) {
            DivideForObject(go.GetComponent<Collider>().bounds);
        }
        SetFaceNeighbors();
        SetEmptyNeighbors();
    }

    /// <summary>
    /// Divide the tree for a single object. Will subdivide tree as if
    /// to place object in smallest possible octant.
    /// </summary>
    /// <param name="bounds">The bounds of the object to add.</param>
    private void DivideForObject(Bounds bounds) {
        if (_nodeSize == _minSize) {
            // Increase the contained object count for a leaf node.
            // Used to determine if leaf node is empty.
            _numContainedObjects++;
            return;
        }

        for (int i = 0; i < 8; i++) {
            if (_childrenBounds[i].Intersects(bounds)) {
                if (_children == null) {
                    _children = new OctreeNode[8];
                }

                if (_children[i] == null) {
                    _children[i] = new OctreeNode(_childrenBounds[i], this, _depth + 1, _minSize, _maxSize);
                }
                _children[i].DivideForObject(bounds);
            }
        }

        if (_children != null) {
            // An object was inserted into a child node. Make sure we split this
            // node evenly by creating any children we are missing (must divide)
            // octant evenly).
            for (int i = 0; i < 8; i++) {
                if (_children[i] == null) {
                    _children[i] = new OctreeNode(_childrenBounds[i], this, _depth + 1, _minSize, _maxSize);
                }
            }
        }
    }

    /// <summary>
    /// Find and set face neighbors in all face directions.
    /// </summary>
    private void SetFaceNeighbors() {
        if (_nodeSize > _maxSize) {
            if (_children != null) {
                for (int i = 0; i < 8; i++) {
                    if (_children[i] != null) {
                        _children[i].SetFaceNeighbors();
                    }
                }
            }
            return;
        }

        for (int dir = 0; dir < 6; dir++) {
            OctreeNode neighbor = GetFaceNeighborInDirection(this, (Face)dir);
            if (neighbor != null) {
                // Has neighbor on same level
                _faceNeighbors[dir] = neighbor;
            } else {
                // No neighbor on same level, try to get parent's neighbor.
                OctreeNode parent = _parent;
                neighbor = GetFaceNeighborInDirection(parent, (Face)dir);
                while (neighbor == null && parent._nodeSize <= _maxSize) {
                    // Parent had no neighbor in direction either.
                    // Find an ancestor's neighbor in the same direction.
                    parent = parent._parent;
                    neighbor = GetFaceNeighborInDirection(parent, (Face)dir);
                }
                _faceNeighbors[dir] = neighbor;
            }
        }

        if (_children != null) {
            for (int i = 0; i < 8; i++) {
                if (_children[i] != null) {
                    _children[i].SetFaceNeighbors();
                }
            }
        }
    }

    // Get neighbor on same layer in direction

    /// <summary>
    /// Get a face neighbor for a node in a direction.
    /// </summary>
    /// <param name="node">The node to get a face neighbor for.</param>
    /// <param name="direction">The direction to get a face neighbor in.</param>
    /// <returns>A face neighbor in the direction specified or null if one didn't exist.</returns>
    private OctreeNode GetFaceNeighborInDirection(OctreeNode node, Face direction) {
        Vector3 targetPosition = _nodeBounds.center +
            s_neighborVectorDirs[(int)direction] * node._nodeSize;
        // Check if neighbor position exists in world bounds.
        if (s_root._nodeBounds.Contains(targetPosition)) {
            return GetNodeAtPositionOnLayer(targetPosition, node._depth);
        }
        return null;
    }

    /// <summary>
    /// Get node at a given position in a given tree layer/depth.
    /// </summary>
    /// <param name="position">The position to get the node at.</param>
    /// <param name="layer">The layer to get the node from.</param>
    /// <returns>A node at the given position on the given layer if it exists. Null otherwise.</returns>
    private OctreeNode GetNodeAtPositionOnLayer(Vector3 position, int layer) {
        return s_root.GetNodeAtPositionOnLayerHelper(position, s_layerSizes[layer]);
    }

    private OctreeNode GetNodeAtPositionOnLayerHelper(Vector3 position, int layerNodeSize) {
        if (_nodeSize == layerNodeSize) {
            return this;
        }
        if (_children != null) {
            for (int i = 0; i < 8; i++) {
                if (_children[i] != null && _childrenBounds[i].Contains(position)) {
                    return _children[i].GetNodeAtPositionOnLayerHelper(position, layerNodeSize);
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Get and store neighboring empty leaf nodes in for all octant face directions.
    /// </summary>
    private void SetEmptyNeighbors() {
        if (_nodeSize > _maxSize) {
            if (_children != null) {
                for (int i = 0; i < 8; i++) {
                    if (_children[i] != null) {
                        _children[i].SetEmptyNeighbors();
                    }
                }
            }
            return;
        }

        if (IsEmpty()) {
            _emptyNeighbors = new List<OctreeNode>();
            for (int faceDir = 0; faceDir < 6; faceDir++) {
                // Check if node has face neighbor in direction.
                if (_faceNeighbors[faceDir] != null) {
                    OctreeNode faceNeighbor = _faceNeighbors[faceDir];
                    if (faceNeighbor.IsLeafNode() && faceNeighbor.IsEmpty()) {
                        // Neighbor is an empty leaf node.
                        _emptyNeighbors.Add(faceNeighbor);
                    } else if (!faceNeighbor.IsLeafNode()) {
                        // Neighbor not a leaf node, get neighbor's higher resolution children.
                        Face neighborChildrenFace = ((Face)faceDir).Opposite();
                        List<OctreeNode> neighboringLeafs =
                            faceNeighbor.GetFaceLeafNodes(neighborChildrenFace);
                        foreach (OctreeNode leaf in neighboringLeafs) {
                            if (leaf.IsEmpty()) {
                                _emptyNeighbors.Add(leaf);
                            }
                        }
                    }
                }
            }
        }

        if (_children != null) {
            for (int i = 0; i < 8; i++) {
                if (_children[i] != null) {
                    _children[i].SetEmptyNeighbors();
                }
            }
        }
    }

    /// <summary>
    /// Get sub octant leaf nodes in octant face direction.
    /// </summary>
    /// <param name="face">The octant face to get leaf nodes for.</param>
    /// <returns>A list of leaf nodes in the face direction given.</returns>
    private List<OctreeNode> GetFaceLeafNodes(Face face) {
        List<OctreeNode> result = new List<OctreeNode>();
        Stack<OctreeNode> stack = new Stack<OctreeNode>();
        stack.Push(this);
        while (stack.Count > 0) {
            OctreeNode node = stack.Pop();
            if (!node.IsLeafNode()) {
                for (int i = 0; i < 4; i++) {
                    int subOctantIndex = s_subOctantsInDirection[(int)face, i];
                    OctreeNode subOctant = node._children[subOctantIndex];
                    stack.Push(subOctant);
                }
            } else {
                result.Add(node);
            }
        }
        return result;
    }

    /// <summary>
    /// Determine if this node is a leaf node.
    /// </summary>
    /// <returns>True if leaf node, false otherwise.</returns>
    private bool IsLeafNode() {
        return _children == null;
    }

    /// <summary>
    /// Determine if this node is empty.
    /// </summary>
    /// <returns>True if empty, false otherwise.</returns>
    private bool IsEmpty() {
        return _numContainedObjects == 0;
    }

    /// <summary>
    /// Get the leaf node at the given position.
    /// </summary>
    /// <param name="position">The position to get the leaf node.</param>
    /// <returns>A leaf node at the given position. Null if outside tree bounds.</returns>
    public OctreeNode GetLeafNodeAtPosition(Vector3 position) {

        if (_depth != 0) {
            Debug.LogError("Must be called on root.");
        }
        if (!_nodeBounds.Contains(position)) {
            return null;
        }
        Stack<OctreeNode> stack = new Stack<OctreeNode>();
        stack.Push(this);
        int it = 0;
        while (stack.Count > 0 && it < s_layerSizes.Length) {
            OctreeNode node = stack.Pop();
            if (!node.IsLeafNode()) {
                for (int i = 0; i < 8; i++) {
                    if (node._children[i] != null && node._children[i]._nodeBounds.Contains(position)) {
                        stack.Push(node._children[i]);
                    }
                }
            } else {
                return node;
            }
            it++;
        }
        return null;
    }

    /// <summary>
    /// Get the closest empty leaf node for a given position/
    /// </summary>
    /// <param name="position">The given position.</param>
    /// <returns>The leaf node at the given position if it is empty. Otherwise,
    /// a nearby node that is empty and leaf node. Null if not found.</returns>
    public OctreeNode GetClosestEmptyLeafNode(Vector3 position) {
        OctreeNode leafNodeAtPosition = GetLeafNodeAtPosition(position);
        if (leafNodeAtPosition.IsEmpty()) {
            return leafNodeAtPosition;
        }
        return GetClosestEmptyLeafNode(leafNodeAtPosition);
    }

    private OctreeNode GetClosestEmptyLeafNode(OctreeNode node) {
        Queue<OctreeNode> queue = new Queue<OctreeNode>();
        HashSet<OctreeNode> discovered = new HashSet<OctreeNode>();
        for (int i = 0; i < 6; i++) {
            OctreeNode neighbor = node._faceNeighbors[i];
            if (neighbor != null) {
                queue.Enqueue(neighbor);
                discovered.Add(neighbor);
            }
        }
        int it = 0;
        const int maxIterations = 100;
        while (queue.Count > 0 && it < maxIterations) {
            OctreeNode curr = queue.Dequeue();
            if (curr.IsLeafNode() && curr.IsEmpty()) {
                return curr;
            }
            for (int i = 0; i < 6; i++) {
                OctreeNode neighbor = curr._faceNeighbors[i];
                if (neighbor != null && !discovered.Contains(neighbor)) {
                    queue.Enqueue(neighbor);
                    discovered.Add(neighbor);
                }
            }
            it++;
        }
        return null;
    }

    public void Draw() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_nodeBounds.center, _nodeBounds.size);
        if (_children != null) {
            for (int i = 0; i < 8; i++) {
                if (_children[i] != null) {
                    _children[i].Draw();
                }
            }
        } else if (_numContainedObjects > 0) {
            Gizmos.color = new Color(0, 0, 1, 0.25f);
            Gizmos.DrawCube(_nodeBounds.center, _nodeBounds.size);
        }
    }

    public void DrawOnlySelf() {
        Gizmos.color = new Color(0, 1, 0, 0.25f);
        Gizmos.DrawCube(_nodeBounds.center, _nodeBounds.size);
    }

    public void DrawFaceNeighbors() {
        Gizmos.color = Color.red;
        if (_faceNeighbors != null) {
            for (int i = 0; i < 6; i++) {
                if (_faceNeighbors[i] != null) {
                    Gizmos.DrawLine(_nodeBounds.center, _faceNeighbors[i]._nodeBounds.center);
                }
            }
        }
    }

    public void DrawEmptyNeighbors() {
        Gizmos.color = Color.red;
        if (_emptyNeighbors != null) {
            for (int i = 0; i < _emptyNeighbors.Count; i++) {
                Gizmos.DrawLine(_nodeBounds.center, _emptyNeighbors[i]._nodeBounds.center);
            }
        }
    }
}
