using System;
using UnityEngine;

public class PathNode : IComparable<PathNode>
{
    public Vector3Int Position { get; set; }
    public Vector3Int Direction { get; set; }
    public PathNode Parent { get; set; }

    public float G { get; set; }
    public float H { get; set; }
    public float F => G + H;

    public PathNode(Vector3 position, PathNode parent, float g, float h, Vector3 direction)
    {
        Position = Vector3Int.RoundToInt(position);
        Parent = parent;
        G = g;
        H = h;
        Direction = Vector3Int.RoundToInt(direction);
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
