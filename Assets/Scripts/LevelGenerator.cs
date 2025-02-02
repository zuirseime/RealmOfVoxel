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

    private List<(Vector3 start, Vector3 end)> _connections = new();

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

        _connections.ForEach(c => Debug.DrawLine(c.start, c.end));
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

    private void ConnectSections()
    {
        foreach (var giver in _sections)
        {
            foreach (var receiver in _sections)
            {
                if (giver == receiver) continue;

                if (giver.connects.All(t => t != receiver.type) || 
                    receiver.connects.All(t => t != giver.type))
                    continue;

                _connections.Add((giver.transform.position, receiver.transform.position));
            }
        }
    }
}
