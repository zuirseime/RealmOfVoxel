using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class RoomPlacer
{
    [HideInInspector] public Transform level;

    [SerializeField] private List<Room> roomPrefabs;
    [SerializeField] private List<Rule> rules;
    [SerializeField] private int maxRoomAmount;
    [SerializeField] private int radius;
    [SerializeField] private List<Room> rooms;

    private float[] _possibleRotations = new float[] { 0, 90, 180, 270 };

    public void RevertRules()
    {
        rules.ForEach(r => r.Revert());
    }

    public List<Room> PlaceSections()
    {
        Debug.Log("Placing rooms...");
        rooms = new();

        while (rooms.Count < maxRoomAmount)
        {
            System.Type type = GetRandomSectionType();
            Vector3 position = GetRandomPosition();

            Room room = CreateSection(type, position);
            if (room != null)
                rooms.Add(room);
        }

        return rooms;
    }

    public void RemoveBounds()
    {
        Debug.Log("Removing boundaries...");
        foreach (Room room in rooms)
        {
            room.bounds.ForEach(b => Object.Destroy(b.gameObject));
        }
        rooms.ForEach(r => r.bounds.Clear());
    }

    private System.Type GetRandomSectionType()
    {
        var validTypes = rules.Where(r => r.Available()).Select(r => r.GetRoomType()).ToList();
        return validTypes.Count > 0 ? validTypes[Random.Range(0, validTypes.Count)] : typeof(SmallTrialRoom);
    }

    private Vector3 GetRandomPosition()
    {
        return new Vector3(Random.Range(-radius, radius), 0, (Random.Range(-radius, radius)));
    }

    private Room CreateSection(System.Type type, Vector3 position)
    {
        var prefab = roomPrefabs.OrderBy(_ => Random.value).FirstOrDefault(r => r.GetType() == type);
        if (prefab == null)
            return null;

        var rotation = _possibleRotations.OrderBy(_ => Random.value).First();
        var room = Object.Instantiate(prefab, position, Quaternion.Euler(0, rotation, 0), level);
        room.name = $"[{rooms.Count:00}] {type}";

        rules.FirstOrDefault(r => r.GetRoomType() == type)?.IncreaseActualAmount();

        return room;
    }
}