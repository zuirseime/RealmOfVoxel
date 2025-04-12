using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using System.Linq;

public class LevelGenerator : MonoBehaviour
{
    public static NavMeshSurface[] Surfaces;

    [SerializeField] private RoomPlacer _roomPlacer;
    [SerializeField] private CollisionResolver _collisionResolver;
    [SerializeField] private RoomConnector _roomConnector;
    [SerializeField] private CorridorBuilder _corridorBuilder;

    [field: Header("Read Only"), SerializeField]
    public List<Room> Rooms { get; private set; } = new();

    public static void UpdateSurfaces()
    {
        foreach (var surface in Surfaces)
        {
            surface.UpdateNavMesh(surface.navMeshData);
        }
    }

    private void Awake()
    {
        _roomPlacer.level = transform;
        _corridorBuilder.Initialize(transform);
        Surfaces = GetComponents<NavMeshSurface>();
    }

    private void Start()
    {
        GenerateLevel();

        foreach (var room in Rooms)
        {
            room.Prepare();
        }

        foreach (var surface in Surfaces)
        {
            surface.BuildNavMesh();
        }
    }

    private void GenerateLevel()
    {
        while (!IsLevelFullyConnected())
        {
            ClearLevel();

            try
            {
                Rooms = _roomPlacer.PlaceSections();

                _collisionResolver.rooms = Rooms;
                _collisionResolver.Resolve();

                _roomConnector.rooms = Rooms;
                _roomConnector.Connect();

                _corridorBuilder.rooms = Rooms;
                _corridorBuilder.FindPathes();
                _corridorBuilder.BuildCorridors();

                _roomPlacer.RemoveBounds();
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning(ex.Message);
                ClearLevel();
            }
        }

        transform.localScale *= 8f;
    }

    private void ClearLevel()
    {
        Rooms.ForEach(section => Destroy(section.gameObject));
        Rooms.Clear();

        _roomPlacer?.RevertRules();
        _corridorBuilder?.Clear();
    }

    private bool IsLevelFullyConnected()
    {
        if (Rooms.Count == 0) return false;

        SpawnRoom spawnRoom = Rooms.OfType<SpawnRoom>().FirstOrDefault();
        if (spawnRoom == null) return false;

        HashSet<Room> visited = new();
        Queue<Room> queue = new();

        queue.Enqueue(spawnRoom);
        visited.Add(spawnRoom);

        while (queue.Count > 0)
        {
            Room current = queue.Dequeue();

            foreach (var door in current.doors)
            {
                if (door.ConnectedDoor == null) continue;

                Room neigbour = door.ConnectedDoor.Parent;
                if (!visited.Contains(neigbour))
                {
                    visited.Add(neigbour);
                    queue.Enqueue(neigbour);
                }
            }
        }

        return visited.Count == Rooms.Count;
    }
}
