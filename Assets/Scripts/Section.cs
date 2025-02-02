using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public enum SectionType
{
    Spawn,
    Casual,
    Treasure,
    Merchant,
    Boss,
}

public class Section : MonoBehaviour
{
    public SectionType type;
    public List<SectionType> connects;

    //[HideInInspector]
    public List<Door> doors;

    [Header("Read Only")]
    public SerializedDictionary<Door, Section> connections;

    void OnValidate()
    {
        doors = transform.GetComponentsInChildren<Door>().ToList();
    }

    void Update()
    {
        
    }
}
