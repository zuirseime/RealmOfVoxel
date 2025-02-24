using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Room : MonoBehaviour
{
    [SerializeField] private GameObject _deadEndPrefab;
    public List<string> connectsTo = new();

    [Range(0, 1)] public float chanceToCloseDoors;
    public int spawnIndex = 0;

    [Header("Read Only")]
    public List<Door> doors;
    public List<RoomBounds> bounds;
    public List<MeshFilter> geometries;

    void OnValidate()
    {
        Validate();
    }

    protected virtual void Validate()
    {
        doors = transform.GetComponentsInChildren<Door>().ToList();
        bounds = transform.GetComponentsInChildren<RoomBounds>().ToList();
        geometries = transform.GetComponentsInChildren<MeshFilter>().Where(c => c.GetComponent<Door>() == null).ToList();
    }

    public virtual void Prepare() { }

    public Door GetNearestDoor(Room section)
    {
        return GetNearestDoor(section.transform.position);
    }

    private Door GetNearestDoor(Vector3 position)
    {
        return doors.Where(d => d.ConnectedDoor == null).OrderBy(d => Vector3.Distance(d.transform.position, position)).FirstOrDefault();
    }

    public bool IsAlreadyConnected(Room room)
    {
        return doors.Any(d => room.doors.Any(o => o == d.ConnectedDoor));
    }

    public bool Connects(Room room)
    {
        var connections = GetConnections();
        var connects = connections.Contains(room.GetType().Name);
        return connects;
    }

    public List<string> GetConnections()
    {
        return connectsTo.Select(t => t + nameof(Room)).ToList();
    }

    public bool Available()
    {
        return doors.Any(d => d.ConnectedDoor == null);
    }

    public void SealUnusedDoors()
    {
        List<Door> toSeal = new();
        foreach (Door door in doors.Where(d => d.ConnectedDoor == null))
        {
            var deadEnd = Instantiate(_deadEndPrefab, transform);

            Vector3 deadEndPos = deadEnd.transform.position;

            float angleY = Quaternion.LookRotation(-door.transform.forward, Vector3.up).eulerAngles.y;
            deadEnd.transform.rotation = Quaternion.Euler(0, angleY, 0);

            Vector3 positionOffset = door.transform.position - deadEnd.transform.position;
            deadEnd.transform.position += positionOffset;

            toSeal.Add(door);
        }

        foreach (Door door in toSeal)
        {
            Destroy(door.gameObject);
            doors.Remove(door);
        }
    }
}
