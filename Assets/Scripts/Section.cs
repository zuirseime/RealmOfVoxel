using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum SectionType
{
    Spawn,
    Casual,
    Corridor,
    Treasure,
    Merchant,
    Boss,
    DeadEnd,
    Exit
}

public class Section : MonoBehaviour
{
    public SectionType type;
    public List<SectionType> creates;

    [Range(-1, 100)] public int deadEndProbability = 10;

    //[HideInInspector]
    public List<Door> doors;

    void OnEnable()
    {
        doors = transform.GetComponentsInChildren<Door>().ToList();
    }

    void OnValidate()
    {
        doors = transform.GetComponentsInChildren<Door>().ToList();
    }

    void Update()
    {
        
    }
}
