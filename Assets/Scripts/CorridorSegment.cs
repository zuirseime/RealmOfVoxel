using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorridorSegment : MonoBehaviour
{
    public List<CorridorWall> walls;
    public CorridorSegment Parent;

    void OnValidate()
    {
        walls = transform.GetComponentsInChildren<CorridorWall>().ToList();
    }

    public void RemoveWalls(Vector3 direction)
    {
        walls.Find(w => Vector3.Angle(w.transform.forward, direction) == 0)?.gameObject.SetActive(false);
    }
}
