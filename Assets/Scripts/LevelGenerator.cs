using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private int _maxRoomAmount;
    [SerializeField] private int _radius;

    [SerializeField] private List<Section> _sectionPrefabs = new();

    [SerializeField] private List<Rule> _rules = new();

    [Header("Read Only")]
    [SerializeField] private List<Section> _sections = new();

    private float[] _possibleRotations = new float[] { 0, 90, 180, 270 };

    private void Start()
    {
        GenerateLevel();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ClearLevel();
            GenerateLevel();
        }

        foreach (var section in _sections)
        {
            foreach (var door in section.doors)
            {
                Debug.DrawLine(door.transform.position, door.ConnectedDoor.transform.position);
            }
        }
    }

    private void ClearLevel()
    {
        _sections.ForEach(section => Destroy(section.gameObject));
        _sections.Clear();
        _rules.ForEach(rule => rule.Revert());
    }

    private void GenerateLevel()
    {
        while (_sections.Count < _maxRoomAmount)
        {
            SectionType type = GetRandomSectionType();
            Vector3 position = GetRandomPosition();

            CreateSection(type, position);
        }

        ResolveCollisions();

        ConnectSections();

        _sections.ForEach(s => s.SealUnusedDoors());
        _sections.ForEach(s => s.FixRotation());
    }

    private SectionType GetRandomSectionType()
    {
        var validTypes = _rules.Where(r => r.actualAmount < r.maxAmount).Select(r => r.sectionType).ToList();
        return validTypes.Count > 0 ? validTypes[Random.Range(0, validTypes.Count)] : SectionType.Casual;
    }

    private Vector3 GetRandomPosition()
    {
        return new Vector3(Random.Range(-_radius, _radius), 0, (Random.Range(-_radius, _radius)));
    }

    private void CreateSection(SectionType type, Vector3 position)
    {
        var prefab = _sectionPrefabs.OrderBy(_ => Random.value).FirstOrDefault(s => s.type == type);
        if (prefab == null)
            return;

        var rotation = _possibleRotations.OrderBy(_ => Random.value).First();

        var section = Instantiate(prefab, position, Quaternion.Euler(0, rotation, 0), transform);
        _sections.Add(section);
        _rules.FirstOrDefault(r => r.sectionType == type)?.IncreaseActualAmount();
    }

    private void ResolveCollisions()
    {
        float pushStrength = 1f;
        bool hasCollisions;

        do
        {
            hasCollisions = false;
            Dictionary<Section, Vector3> displacementMap = new();

            foreach (var section1 in _sections)
            {
                foreach (var section2 in _sections)
                {
                    if (section1 == section2)
                        continue;
                    if (!BoundsOverlap(section1, section2))
                        continue;

                    hasCollisions = true;
                    Vector3 pushDirection = (section1.transform.position - section2.transform.position).normalized;

                    if (!displacementMap.ContainsKey(section1))
                        displacementMap[section1] = Vector3.zero;
                    if (!displacementMap.ContainsKey(section2))
                        displacementMap[section2] = Vector3.zero;

                    displacementMap[section1] += pushDirection * pushStrength;
                    displacementMap[section2] -= pushDirection * pushStrength;
                }
            }

            foreach (var kvp in displacementMap)
            {
                kvp.Key.transform.position += Vector3Int.RoundToInt(kvp.Value);
            }

            Physics.SyncTransforms();

        } while (hasCollisions);
    }

    private bool BoundsOverlap(Section section1, Section section2)
    {
        var bounds1 = section1.GetComponentInChildren<SectionBounds>().GetComponent<BoxCollider>().bounds;
        var bounds2 = section2.GetComponentInChildren<SectionBounds>().GetComponent<BoxCollider>().bounds;
        return bounds1.Intersects(bounds2);
    }

    //private void ConnectSections(Door door, Section newSection)

    private void ConnectSections()
    {
        List<Section> possibleConnections = new();

        var givers = _sections.OrderBy(s => s.type);
        foreach (var giver in givers)
        {
            if (!giver.Available())
                continue;

            possibleConnections.Clear();

            var receivers = _sections.OrderBy(s => Vector3.Distance(giver.transform.position, s.transform.position)).Take(_maxRoomAmount / 2);
            foreach (var receiver in receivers)
            {
                if (giver == receiver)
                    continue;

                if (!(giver.Connects(receiver) && receiver.Connects(giver)))
                    continue;

                if (!receiver.Available())
                    continue;

                if (giver.IsAlreadyConnected(receiver))
                    continue;

                possibleConnections.Add(receiver);
            }

            foreach (var nearest in possibleConnections.OrderBy(r => Vector3.Distance(giver.transform.position, r.transform.position)))
            {
                var giverDoor = giver.GetNearestDoor(nearest);
                var receiverDoor = nearest.GetNearestDoor(giver);

                if (giverDoor.Connects(receiverDoor))
                {
                    giverDoor.Connect(receiverDoor);
                    receiverDoor.Connect(giverDoor);

                    break;
                }
            }
        }
    }
}
