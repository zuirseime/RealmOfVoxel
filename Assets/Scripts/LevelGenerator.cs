using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private int _maxRoomAmount;
    [SerializeField] private int _radius;

    [SerializeField] private List<Section> _sectionPrefabs = new();
    [SerializeField] private CorridorSegment _corridorPrefab;

    [SerializeField] private List<Rule> _rules = new();

    [Header("Read Only")]
    [SerializeField] private List<Section> _sections = new();

    private float[] _possibleRotations = new float[] { 0, 90, 180, 270 };

    private Pathfinder _pathfinder;
    private List<List<Vector3>> _pathes = new();
    private HashSet<Vector3> _corridorPositions = new();
    private List<CorridorSegment> _corridors = new();

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
    }

    private void ClearLevel()
    {
        _sections.ForEach(section => Destroy(section.gameObject));
        _sections.Clear();
        _rules.ForEach(rule => rule.Revert());
        _pathes.Clear();
        _corridors.ForEach(corridor => Destroy(corridor.gameObject));
        _corridors.Clear();
        _corridorPositions.Clear();
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

        BuildCorridors();
    }

    private void BuildCorridors()
    {
        _pathfinder = new Pathfinder(_sections.SelectMany(s => s.bounds).ToList());

        List<Door> used = new();
        foreach (var door in _sections.SelectMany(s => s.doors))
        {
            if (used.Contains(door))
                continue;

            BuildCorridor(door);

            used.Add(door.ConnectedDoor);
        }

        FixCorridors();
    }

    private void BuildCorridor(Door door)
    {
        foreach (var segment in _pathfinder.FindPath(door))
        {
            if (_corridorPositions.Contains(segment.Position))
                continue;

            var corridor = Instantiate(_corridorPrefab, segment.Position, Quaternion.identity, transform);

            _corridors.Add(corridor);
            _corridorPositions.Add(segment.Position);
        }
    }

    private void FixCorridors()
    {
        foreach (var corridorSection in _corridors)
        {
            if (_corridorPositions.Contains(corridorSection.transform.position + Vector3.left))
            {
                corridorSection.RemoveWalls(Vector3.left);
            }
            if (_corridorPositions.Contains(corridorSection.transform.position + Vector3.right))
            {
                corridorSection.RemoveWalls(Vector3.right);
            }
            if (_corridorPositions.Contains(corridorSection.transform.position + Vector3.forward))
            {
                corridorSection.RemoveWalls(Vector3.forward);
            }
            if (_corridorPositions.Contains(corridorSection.transform.position + Vector3.back))
            {
                corridorSection.RemoveWalls(Vector3.back);
            }

            foreach (var door in _sections.SelectMany(s => s.doors))
            {
                if (corridorSection.transform.position == door.transform.position)
                {
                    corridorSection.RemoveWalls(-door.transform.forward);
                }
            }
        }
    }

    private SectionType GetRandomSectionType()
    {
        var validTypes = _rules.Where(r => r.Available()).Select(r => r.sectionType).ToList();
        return validTypes.Count > 0 ? validTypes[Random.Range(0, validTypes.Count)] : SectionType.Room;
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

    private void ConnectSections()
    {
        foreach (var giver in _sections.Where(s => s.Available()).OrderBy(s => s.type))
        {
            var receivers = _sections.Where(r => r != giver).Where(r => r.Available())
                                     .Where(r => !giver.IsAlreadyConnected(r))
                                     .Where(r => r.Connects(giver) && giver.Connects(r)).ToArray();

            foreach (var receiver in receivers.OrderBy(r => Vector3.Distance(giver.transform.position, r.transform.position)))
            {
                var giverDoor = giver.GetNearestDoor(receiver);
                var receiverDoor = receiver.GetNearestDoor(giver);

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
