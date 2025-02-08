using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum SectionType
{
    Spawn = 0,
    Boss = 1,
    Treasure = 2,
    Merchant = 3,
    Room = 4,
    BigRoom = 5
}

public class Section : MonoBehaviour
{
    [SerializeField] private Section _deadEndPrefab;

    public SectionType type;
    public List<SectionType> connectsTo;

    [Header("Read Only")]
    public List<Door> doors;
    public List<SectionBounds> bounds;

    void OnValidate()
    {
        doors = transform.GetComponentsInChildren<Door>().ToList();
        bounds = transform.GetComponentsInChildren<SectionBounds>().ToList();
    }

    public Door GetNearestDoor(Section section)
    {
        return GetNearestDoor(section.transform.position);
    }

    private Door GetNearestDoor(Vector3 position)
    {
        return doors.Where(d => d.Available).OrderBy(d => Vector3.Distance(d.transform.position, position)).FirstOrDefault();
    }

    public bool IsAlreadyConnected(Section section)
    {
        return doors.Any(d => section.doors.Any(o => o == d.ConnectedDoor));
    }

    public bool Connects(Section section)
    {
        return connectsTo.Contains(section.type);
    }

    public bool Available()
    {
        return doors.Any(d => d.Available);
    }

    public void SealUnusedDoors()
    {
        List<Door> toSeal = new();
        foreach (Door door in doors.Where(d => d.ConnectedDoor == null))
        {
            var deadEnd = Instantiate(_deadEndPrefab, transform);

            Vector3 deadEndPos = deadEnd.doors[0].transform.position;
            Vector3 deadEndForward = deadEnd.doors[0].transform.forward;

            Quaternion rotationOffset = Quaternion.FromToRotation(deadEndForward, -door.transform.forward);

            deadEnd.transform.rotation = rotationOffset * deadEnd.transform.rotation;

            Vector3 adjustedNewDoorPos = deadEnd.transform.position;

            Vector3 positionOffset = door.transform.position - adjustedNewDoorPos;
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
