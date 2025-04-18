using System;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder
{
    private List<RoomBounds> _bounds;
    private float _step;

    public bool StraightenCorridors { get; set; }

    public Pathfinder(List<RoomBounds> bounds, float step = 1f)
    {
        _bounds = bounds;
        _step = step;
    }

    public List<PathNode> FindPath(Door door)
    {
        if (door == null || door.ConnectedDoor == null)
            return new List<PathNode>();

        return AStarSearch(door, door.ConnectedDoor);
    }

    private List<PathNode> AStarSearch(Door start, Door end)
    {
        foreach (var bound in _bounds)
        {
            if (bound.type == RoomBounds.BoundsType.Inner && bound.GetComponent<BoxCollider>().bounds.Contains(start.transform.position))
            {
                return new List<PathNode>();
            }
        }

        PriorityQueue<PathNode> openSet = new();
        Dictionary<Vector3, PathNode> allNodes = new();

        PathNode startNode = new(start.transform.position, null, 0, Heuristic(start.transform.position, end.transform.position), start.transform.forward);
        openSet.Enqueue(startNode);
        allNodes[startNode.Position] = startNode;

        while (openSet.Count > 0)
        {
            PathNode current = openSet.Dequeue();
            if (current.Position == end.transform.position)
            {
                return ReconstructPath(current);
            }

            ProcessNeighbors(current, end.transform.position, openSet, allNodes);
        }

        return new List<PathNode>();
    }

    private void ProcessNeighbors(PathNode current, Vector3 end, PriorityQueue<PathNode> openSet, Dictionary<Vector3, PathNode> allNodes)
    {
        foreach (var neighbor in GetNeighbors(current.Position))
        {
            if (IsBlocked(neighbor)) continue;

            float newG = current.G + Vector3.Distance(current.Position, neighbor);
            float newH = Heuristic(neighbor, end);

            if (StraightenCorridors)
            {
                float movementCost = Vector3.Dot((neighbor - current.Position).normalized, current.Direction) > 0.9f ? 0.5f : 1.0f;
                newG *= movementCost;
            }


            if (!allNodes.TryGetValue(neighbor, out PathNode neighborNode))
            {
                PathNode newNode = new(neighbor, current, newG, newH, (neighbor - current.Position).normalized);
                allNodes[neighbor] = newNode;
                openSet.Enqueue(newNode);
            } else if (newG < neighborNode.G)
            {
                neighborNode.Parent = current;
                neighborNode.G = newG;
                openSet.Enqueue(neighborNode);
            }
        }
    }

    private List<Vector3> GetNeighbors(Vector3 position)
    {
        List<Vector3> neighbors = new()
        {
            position + Vector3.right * _step,
            position + Vector3.left * _step,
            position + Vector3.forward * _step,
            position + Vector3.back * _step
        };
        return neighbors;
    }

    private bool IsBlocked(Vector3 position)
    {
        foreach (var bound in _bounds)
        {
            if (bound.type == RoomBounds.BoundsType.Inner && bound.GetComponent<BoxCollider>().bounds.Contains(position))
            {
                return true;
            }
        }
        return false;
    }

    private float Heuristic(Vector3 a, Vector3 b)
    {
        return Vector3.Distance(a, b);
    }

    private List<PathNode> ReconstructPath(PathNode node)
    {
        List<PathNode> path = new();
        while (node != null)
        {
            path.Add(node);
            node = node.Parent;
        }
        path.Reverse();
        return path;
    }
}