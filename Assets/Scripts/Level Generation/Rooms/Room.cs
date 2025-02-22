using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//public enum SectionType
//{
//    Spawn = 0,
//    Boss = 1,
//    Treasure = 2,
//    Merchant = 3,
//    Room = 4,
//    BigRoom = 5
//}

public class Room : MonoBehaviour
{
    [SerializeField] private GameObject _deadEndPrefab;
    [SerializeField] protected List<string> _connectsTo = new();

    [Range(0, 1)] public float chanceToCloseDoors;
    public int spawnIndex = 0;

    [Header("Read Only")]
    public List<Door> doors;
    public List<SectionBounds> bounds;

    void OnValidate()
    {
        Validate();
    }

    protected virtual void Validate()
    {
        doors = transform.GetComponentsInChildren<Door>().ToList();
        bounds = transform.GetComponentsInChildren<SectionBounds>().ToList();
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
        return _connectsTo.Select(t => t + nameof(Room)).ToList();
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
