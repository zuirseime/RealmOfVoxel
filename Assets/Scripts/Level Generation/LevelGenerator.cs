using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.AI.Navigation;

public class LevelGenerator : MonoBehaviour
{
    public static NavMeshSurface Surface;

    [SerializeField] private RoomPlacer _roomPlacer;
    [SerializeField] private CollisionResolver _collisionResolver;
    [SerializeField] private RoomConnector _roomConnector;
    [SerializeField] private CorridorBuilder _corridorBuilder;

    [field: Header("Read Only"), SerializeField]
    public List<Room> Rooms { get; private set; } = new();

    private void Start()
    {
        Surface = GetComponent<NavMeshSurface>();

        _roomPlacer.level = transform;
        _corridorBuilder.Initialize(transform);

        GenerateLevel();
    }

    private void GenerateLevel()
    {
        while (true)
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
                continue;
            }

            break;
        }

        transform.localScale *= 8f;

        foreach (var room in Rooms)
        {
            room.Prepare();
        }

        Surface.BuildNavMesh();
        Surface.BuildNavMesh();
    }

    private void ClearLevel()
    {
        Rooms.ForEach(section => Destroy(section.gameObject));
        Rooms.Clear();

        _roomPlacer?.RevertRules();
        _corridorBuilder?.Clear();
    }
}
