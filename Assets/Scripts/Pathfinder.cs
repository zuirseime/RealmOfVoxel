using System;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder
{
    private List<SectionBounds> _bounds;

    public Pathfinder(List<SectionBounds> bounds)
    {
        _bounds = bounds;
    }

    public List<PathNode> FindPath(Door door)
    {
        Door start = door;
        Door end = door.ConnectedDoor;

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

            foreach (var neighbor in GetNeighbors(current.Position))
            {
                if (IsBlocked(neighbor))
                    continue;

                float movementCost = Vector3.Dot((neighbor - current.Position).normalized, current.Direction) > 0.9f ? 0.5f : 1.0f;
                float newG = current.G + Vector3.Distance(current.Position, neighbor) * movementCost;
                float newH = Heuristic(neighbor, end.transform.position);

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

        return new List<PathNode>();
    }

    private List<Vector3> GetNeighbors(Vector3 position)
    {
        List<Vector3> neighbors = new();
        float step = 1f;
        neighbors.Add(position + Vector3.right * step);
        neighbors.Add(position + Vector3.left * step);
        neighbors.Add(position + Vector3.forward * step);
        neighbors.Add(position + Vector3.back * step);
        return neighbors;
    }

    private bool IsBlocked(Vector3 position)
    {
        foreach (var bound in _bounds)
        {
            if (bound.type == SectionBounds.BoundsType.Inner && bound.GetComponent<BoxCollider>().bounds.Contains(position))
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

public class PathNode : IComparable<PathNode>
{
    public Vector3 Position { get; set; }
    public Vector3 Direction { get; set; }
    public PathNode Parent { get; set; }

    public float G { get; set; }
    public float H { get; set; }
    public float F => G + H;

    public PathNode(Vector3 position, PathNode parent, float g, float h, Vector3 direction)
    {
        Position = position;
        Parent = parent;
        G = g;
        H = h;
        Direction = direction;
    }

    public int CompareTo(PathNode other)
    {
        return F.CompareTo(other.F);
    }

    public override string ToString()
    {
        return $"{Position}, F = {F}";
    }
}

public class PriorityQueue<T> where T : IComparable<T>
{
    private List<T> _elements = new();

    public int Count => _elements.Count;

    public void Enqueue(T element)
    {
        _elements.Add(element);
        _elements.Sort();
    }

    public T Dequeue()
    {
        T item = _elements[0];
        _elements.RemoveAt(0);
        return item;
    }
}