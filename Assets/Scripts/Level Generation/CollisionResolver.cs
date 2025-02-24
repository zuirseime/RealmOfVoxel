using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CollisionResolver
{
    [SerializeField, Min(1e-3f)] private float _pushStrength;

    [HideInInspector] public List<Room> rooms;

    public void Resolve()
    {
        Debug.Log("Resolving collisions...");
        bool hasCollisions;

        if (rooms == null)
        {
            Debug.LogWarning("No sections to resolve collisions");
            return;
        }

        do
        {
            hasCollisions = false;
            Dictionary<Room, Vector3> displacementMap = new();

            foreach (var room1 in rooms)
            {
                foreach (var room2 in rooms)
                {
                    if (room1 == room2)
                        continue;
                    if (!BoundsOverlap(room1, room2))
                        continue;

                    hasCollisions = true;
                    Vector3 pushDirection = (room1.transform.position - room2.transform.position).normalized;

                    pushDirection += new Vector3(Random.Range(-0.1f, 0.1f), 0, Random.Range(-0.1f, 0.1f)).normalized;

                    if (!displacementMap.ContainsKey(room1))
                        displacementMap[room1] = Vector3.zero;
                    if (!displacementMap.ContainsKey(room2))
                        displacementMap[room2] = Vector3.zero;

                    displacementMap[room1] += pushDirection * _pushStrength;
                    displacementMap[room2] -= pushDirection * _pushStrength;
                }
            }

            foreach (var kvp in displacementMap)
            {
                kvp.Key.transform.position += kvp.Value;
            }

            Physics.SyncTransforms();

        } while (hasCollisions);

        foreach (var room in rooms)
        {
            room.transform.position = Vector3Int.RoundToInt(room.transform.position);
        }
    }

    private bool BoundsOverlap(Room room1, Room room2)
    {
        var bounds1 = room1.GetComponentInChildren<RoomBounds>().GetComponent<BoxCollider>().bounds;
        var bounds2 = room2.GetComponentInChildren<RoomBounds>().GetComponent<BoxCollider>().bounds;
        return bounds1.Intersects(bounds2);
    }
}
