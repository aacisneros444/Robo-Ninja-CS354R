using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

/// <summary>
/// A class to conduct pathfinding on the sparse voxel octree.
/// </summary>
public class OctreePathfinder : MonoBehaviour {

    [SerializeField] private Octree _octree;

    public List<Vector3> GetPath(Vector3 start, Vector3 end) {
        if (!_octree.Root.NodeBounds.Contains(start)) {
            start = _octree.Root.NodeBounds.ClosestPoint(start);
        }
        if (!_octree.Root.NodeBounds.Contains(end)) {
            end = _octree.Root.NodeBounds.ClosestPoint(end);
        }
        SimplePriorityQueue<OctreeNode> frontier = new SimplePriorityQueue<OctreeNode>();
        Dictionary<OctreeNode, OctreeNode> cameFrom = new Dictionary<OctreeNode, OctreeNode>();
        Dictionary<OctreeNode, int> costSoFar = new Dictionary<OctreeNode, int>();

        OctreeNode startNode = _octree.Root.GetClosestEmptyLeafNode(start);
        OctreeNode goal = _octree.Root.GetClosestEmptyLeafNode(end);

        int numSearched = 0;
        frontier.Enqueue(startNode, 0);
        while (frontier.Count != 0) {
            OctreeNode current = frontier.Dequeue();

            if (current == goal) {
                break;
            }

            foreach (OctreeNode next in current.EmptyNeighbors) {
                numSearched++;
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
        // Debug.Log("Num nodes searched " + numSearched);
        return path;
    }

    private float Heuristic(OctreeNode node1, OctreeNode node2) {
        return Vector3.Distance(node1.NodeBounds.center, node2.NodeBounds.center);
    }
}
