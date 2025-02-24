using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class TrialRoom : Room
{
    [SerializeField] protected int _enemiesNumber;
    [SerializeField] protected List<Enemy> _enemyPrefabs;

    [SerializeField] private float _colorThreshold = 0.1f;

    [Header("Read Only")]
    [SerializeField] protected List<Enemy> _enemies;

    private HashSet<Vector3> _usedSpawnablePositions = new();
    private List<Vector3> _spawnablePositions = new();

    private void OnDrawGizmosSelected()
    {
        foreach(var position in _spawnablePositions)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(position, Vector3.one);
        }
    }

    public override void Prepare()
    {
        _spawnablePositions = GetSpawnablePositions();

        while (_enemies.Count <= _enemiesNumber)
        {
            var position = _spawnablePositions.OrderBy(p => Random.value).FirstOrDefault();

            if (_usedSpawnablePositions.Contains(position))
                continue;

            var prefab = _enemyPrefabs.OrderBy(p => Random.value).FirstOrDefault();
            var enemy = Instantiate(prefab, position, Random.rotationUniform, transform);
            _enemies.Add(enemy);

            _usedSpawnablePositions.Add(position);
        }
    }

    protected override void Validate()
    {
        base.Validate();
    }

    private List<Vector3> GetSpawnablePositions()
    {
        List<Vector3> matchingVertices = new();

        foreach (var meshFilter in geometries)
        {
            Mesh mesh = meshFilter.sharedMesh;
            Vector3[] vertices = mesh.vertices;
            Color[] colors = mesh.colors;

            if (colors == null || colors.Length == 0)
            {
                Debug.LogWarning("No vertex colors found in mesh.");
                continue;
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                if (ColorDistance(colors[i], Color.red) < _colorThreshold)
                {
                    Vector3 worldPosition = meshFilter.transform.TransformPoint(vertices[i]);
                    matchingVertices.Add(worldPosition);
                }
            }
        }

        return matchingVertices;
    }

    float ColorDistance(Color a, Color b)
    {
        return Mathf.Abs(a.r - b.r) + Mathf.Abs(a.g - b.g) + Mathf.Abs(a.b - b.b);
    }
}
