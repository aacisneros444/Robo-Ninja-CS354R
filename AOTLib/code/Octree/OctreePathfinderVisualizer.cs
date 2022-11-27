using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

/// <summary>
/// A class to conduct pathfinding on the sparse voxel octree
/// with visualization.
/// </summary>
public class OctreePathfinderVisualizer : MonoBehaviour {

    public Transform startT;
    public Transform endT;

    [SerializeField] private Octree _octree;
    private OctreeNode _currentFromNode;
    private OctreeNode _currentToNode;
    public GameObject pathMarkerPrefab;
    private List<Vector3> pathToDraw;
    [Header("Visualization Settings")]
    [SerializeField] private float _yieldTime = 0.05f;
    [SerializeField] private bool _visualizePathfinding = false;

    private void Update() {
        if (_visualizePathfinding) {
            StartCoroutine(GetPath(startT.position, endT.position));
            _visualizePathfinding = false;
        }
    }

    private void OnDrawGizmos() {
        if (_currentFromNode != null) {
            _octree.DrawSingleNode(_currentFromNode);
        }
        if (_currentToNode != null) {
            _octree.DrawSingleNode(_currentToNode);
        }
        if (pathToDraw != null) {
            Gizmos.color = Color.red;
            for (int i = 0; i < pathToDraw.Count - 1; i++) {
                Debug.DrawLine(pathToDraw[i], pathToDraw[i + 1]);
            }
        }
    }

    IEnumerator GetPath(Vector3 start, Vector3 end) {
        pathToDraw = null;
        SimplePriorityQueue<OctreeNode> frontier = new SimplePriorityQueue<OctreeNode>();
        Dictionary<OctreeNode, OctreeNode> cameFrom = new Dictionary<OctreeNode, OctreeNode>();
        Dictionary<OctreeNode, int> costSoFar = new Dictionary<OctreeNode, int>();

        OctreeNode startNode = _octree.Root.GetLeafNodeAtPosition(start);
        OctreeNode goal = _octree.Root.GetLeafNodeAtPosition(end);

        int numSearched = 0;
        frontier.Enqueue(startNode, 0);
        while (frontier.Count != 0) {
            OctreeNode current = frontier.Dequeue();
            _currentFromNode = current;

            if (current == goal) {
                break;
            }

            foreach (OctreeNode next in current.EmptyNeighbors) {
                numSearched++;
                _currentToNode = next;
                yield return new WaitForSeconds(_yieldTime);

                int newCost = 0;
                costSoFar.TryGetValue(next, out newCost);
                newCost += 1;
                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next]) {
                    costSoFar[next] = newCost;
                    float priority = newCost + Heuristic(next, goal);
                    frontier.Enqueue(next, priority);
                    cameFrom[next] = current;
                }
            }
        }

        // Reconstruct path
        OctreeNode temp = goal;
        List<Vector3> path = new List<Vector3>();
        path.Add(end);
        while (temp != startNode) {
            path.Add(temp.NodeBounds.center);
            temp = cameFrom[temp];
        }
        path.Add(start);
        path.Reverse();
        foreach (Vector3 waypoint in path) {
            Instantiate(pathMarkerPrefab, waypoint, Quaternion.identity);
        }
        Debug.Log("Num nodes searched " + numSearched);

        _currentToNode = null;
        _currentFromNode = null;
        pathToDraw = path;
    }

    private float Heuristic(OctreeNode node1, OctreeNode node2) {
        return Vector3.Distance(node1.NodeBounds.center, node2.NodeBounds.center);
    }
}
