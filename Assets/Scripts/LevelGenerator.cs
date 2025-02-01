using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using Unity.VisualScripting;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private int _sectionAmount;

    [SerializeField] private List<Section> _sectionPrefabs = new();
    [SerializeField] private Section _deadEnd;

    [SerializeField] private List<Rule> _rules = new();

    [Header("Read Only")]
    [SerializeField] private List<Section> _sections = new();

    private void Start()
    {
        GenerateLevel();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RegenerateLevel();
        }
    }

    private void RegenerateLevel()
    {
        ClearLevel();
        GenerateLevel();
    }

    private void ClearLevel()
    {
        _sections.Clear();

        foreach (var child in transform.GetComponentsInChildren<Section>())
        {
            Destroy(child.gameObject);
        }

        foreach (var rule in _rules)
        {
            rule.actualAmount = 0;
        }
    }

    private void GenerateLevel()
    {
        CreateSection(SectionType.Spawn, Vector3.zero, Vector3.forward);

        for (int i = 0; i < _sectionAmount; i++)
        {
            var currentSection = _sections[i];
            var sectionCreates = currentSection.creates;

            if (sectionCreates.Count == 0)
            {
                continue;
            }

            for (int door = 0; door < currentSection.doors.Count; door++)
            {
                var doorPosition = currentSection.doors[door].transform.position;
                var doorDirection = currentSection.doors[door].transform.forward;

                if (!currentSection.doors[door].Available)
                {
                    continue;
                }

                if (Random.Range(0, 100) < currentSection.deadEndProbability |
                    !CanPlaceSection(doorPosition, doorDirection))
                {
                    CreateDeadEnd(doorPosition, doorDirection);
                    continue;
                }

                SectionType nextSectionType = SectionType.Casual;
                bool typePoolIsFull = false;
                while (!typePoolIsFull)
                {
                    nextSectionType = sectionCreates[Random.Range(0, sectionCreates.Count)];

                    if (!_rules.Any(r => r.sectionType == nextSectionType && r.maxAmount <= r.actualAmount))
                    {
                        typePoolIsFull = true;
                    }
                }

                CreateSection(nextSectionType, doorPosition, doorDirection);
            }
        }
    }

    private bool CanPlaceSection(Vector3 origin, Vector3 direction, float checkDistance = 32f)
    {
        if (Physics.Raycast(origin, direction, out RaycastHit hit, checkDistance))
        {
            return hit.collider.GetComponent<Bounds>() is null;
        }

        return true;
    }

    private void CreateSection(SectionType type, Vector3 position, Vector3 forward)
    {
        Section[] sections = _sectionPrefabs.Where(s => s.type == type).ToArray();
        //Debug.Log(sections[0].doors.Count);

        var newSection = Instantiate(sections[Random.Range(0, sections.Length)], transform);
        _sections.Add(newSection);
        
        if (_sections.Count > 1)
        {
            ConnectSections(newSection, position, forward);
        }

        var rule = _rules.FirstOrDefault(r => r.sectionType == newSection.type);
        if (rule is not null)
        {
            rule.actualAmount++;
        }
    }

    private void ConnectSections(Section newSection, Vector3 existingPosition, Vector3 existingDirection)
    {
        Door newDoor = newSection.doors[Random.Range(0, newSection.doors.Count)];

        Vector3 newDoorPos = newDoor.transform.position;
        Vector3 newDoorForward = newDoor.transform.forward;

        Quaternion rotationOffset = Quaternion.FromToRotation(newDoorForward, -existingDirection);

        newSection.transform.rotation = rotationOffset * newSection.transform.rotation;

        Vector3 adjustedNewDoorPos = newDoor.transform.position;

        Vector3 positionOffset = existingPosition - adjustedNewDoorPos;
        newSection.transform.position += positionOffset;

        newDoor.Available = false;
    }

    private void CreateDeadEnd(Vector3 position, Vector3 forward)
    {
        var deadEnd = Instantiate(_deadEnd, position, Quaternion.LookRotation(-forward), transform);
        _sections.Add(deadEnd);
    }
}
