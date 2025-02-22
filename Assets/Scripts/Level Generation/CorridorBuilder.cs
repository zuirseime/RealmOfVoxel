using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class CorridorBuilder
{
    [HideInInspector] public List<Room> rooms;

    [SerializeField] private CorridorSegment _prefab;
    [SerializeField] private bool _preferStraightCorridors;

    private GameObject _container;
    private HashSet<Vector3Int> _positions = new();
    private List<CorridorSegment> _corridors = new();

    private Pathfinder _pathfinder;
    private List<List<PathNode>> _pathes = new();

    public void Initialize(Transform parent)
    {
        _container = new("Corridors");
        _container.transform.parent = parent;
    }

    public void FindPathes()
    {
        Debug.Log("Searchin for pathes...");
        _pathfinder = new(rooms.SelectMany(r => r.bounds).ToList());
        _pathfinder.StraightenCorridors = _preferStraightCorridors;

        foreach (var door in rooms.SelectMany(r => r.doors))
        {
            if (!door.Available)
                continue;

            door.Available = false;
            door.ConnectedDoor.Available = false;

            var path = _pathfinder.FindPath(door);
            if (path != null)
            {
                _pathes.Add(path);
            }
        }
    }

    public void BuildCorridors()
    {
        Debug.Log("Building corridors...");
        if (_pathes.Count == 0)
        {
            throw new System.Exception("Find ways first!");
        }

        foreach (var path in _pathes)
        {
            foreach (var node in path)
            {
                if (_positions.Contains(node.Position))
                    continue;

                var corridor = Object.Instantiate(_prefab, node.Position, Quaternion.identity, _container.transform);
                corridor.name = $"Corridor {node.Position}";

                _corridors.Add(corridor);
                _positions.Add(node.Position);
            }
        }

        FixCorridors();
    }

    public void Clear()
    {
        for (int i = 0; i < _container.transform.childCount; i++)
        {
            Object.Destroy(_container.transform.GetChild(i).gameObject);
        }

        _pathes.Clear();

        _corridors.ForEach(corridor => Object.Destroy(corridor.gameObject));
        _corridors.Clear();
        _positions.Clear();
    }

    private void FixCorridors()
    {
        foreach (var corridorSection in _corridors)
        {
            List<Vector3> directions = new();
            Vector3Int position = Vector3Int.RoundToInt(corridorSection.transform.position);

            if (_positions.Contains(position + Vector3Int.left))
                directions.Add(Vector3.left);

            if (_positions.Contains(position + Vector3Int.right))
                directions.Add(Vector3.right);

            if (_positions.Contains(position + Vector3Int.forward))
                directions.Add(Vector3.forward);

            if (_positions.Contains(position + Vector3Int.back))
                directions.Add(Vector3.back);

            foreach (var door in rooms.SelectMany(s => s.doors))
            {
                if (position == door.transform.position)
                    directions.Add(-door.transform.forward);
            }

            foreach (var direction in directions)
                corridorSection.RemoveWalls(direction);
        }
    }
}
