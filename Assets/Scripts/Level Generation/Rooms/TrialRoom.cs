using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public abstract class TrialRoom : Room
{
    [SerializeField] protected int _enemiesNumber;
    [SerializeField] protected List<Enemy> _enemyPrefabs;

    [SerializeField] private float _colorThreshold = 0.1f;

    [Header("Read Only")]
    [SerializeField] protected List<Enemy> _enemies;

    private List<Vector3> _spawnablePositions = new();
    private List<BoxCollider> _triggers;
    private bool _isClean = false;

    private void OnDrawGizmosSelected()
    {
        foreach(var position in _spawnablePositions)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(position, Vector3.one);
        }
    }

    private void Start()
    {
        _triggers = GetComponents<BoxCollider>().ToList();
    }

    private void Update()
    {
        if (!_enemies.Any(e => e.IsAlive) && !_isClean)
        {
            StartCoroutine(OpenDoors());
        }
    }

    public override void Prepare()
    {
        _spawnablePositions = GetSpawnablePositions();

        while (_enemies.Count < _enemiesNumber)
        {
            var position = _spawnablePositions.OrderBy(p => Random.value).FirstOrDefault();

            var prefab = _enemyPrefabs.OrderBy(p => Random.value).FirstOrDefault();
            var enemy = Instantiate(prefab, position, Random.rotationUniform, transform);
            _enemies.Add(enemy);

            _spawnablePositions.Remove(position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Player>() is Player player)
        {
            Debug.Log($"Player entered the {name}");

            //_triggers.ForEach(t => t.enabled = false);
            player.transform.parent = transform;

            if (!_isClean && _enemies.Any(e => !e.Active))
            {
                _enemies.ForEach(e => e.Activate());
                StartCoroutine(CloseDoors());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Player>() is Player player && _enemies.All(e => !e.IsAlive))
        {
            player.transform.parent = null;
        }
    }

    private IEnumerator CloseDoors()
    {
        doors.ForEach(d => d.Close());

        float animationTime = doors.FirstOrDefault()?.GetAnimationState() ?? 1f;

        yield return new WaitForSeconds(animationTime / 2f);

        LevelGenerator.Surface.BuildNavMesh();
    }

    private IEnumerator OpenDoors()
    {
        _isClean = true;

        doors.ForEach(d => d.Open());

        float animationTime = doors.FirstOrDefault()?.GetAnimationState() ?? 1f;

        yield return new WaitForSeconds(animationTime);

        LevelGenerator.Surface.BuildNavMesh();
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

    private float ColorDistance(Color a, Color b)
    {
        return Mathf.Abs(a.r - b.r) + Mathf.Abs(a.g - b.g) + Mathf.Abs(a.b - b.b);
    }
}
