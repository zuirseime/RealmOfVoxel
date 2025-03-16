using System.Linq;
using UnityEngine;

public class DetectionRangeManager : MonoBehaviour
{
    public Material material;
    private Enemy[] _enemies;
    private Vector4[] _enemyPositions;
    private float[] _detectionRanges;
    private float[] _attackRanges;

    private Player _player;

    private static readonly int playerPositionID = Shader.PropertyToID("_PlayerPosition");
    private static readonly int playerAttackRangeID = Shader.PropertyToID("_PlayerAttackRange");

    private static readonly int enemyPositionID = Shader.PropertyToID("_EnemyPositions");
    private static readonly int detectionRangesID = Shader.PropertyToID("_DetectionRanges");
    private static readonly int attackRangesID = Shader.PropertyToID("_AttackRanges");
    private static readonly int enemyCountID = Shader.PropertyToID("_EnemyCount");

    private void Update()
    {
        _player = FindObjectOfType<Player>();
        float attackRange = _player.CurrentWeapon.range;

        if (_player.IsAlive)
        {
            material.SetVector(playerPositionID, _player.transform.position);
            material.SetFloat(playerAttackRangeID, attackRange);
        }

        _enemies = FindObjectsOfType<Enemy>().Where(e => e.IsAlive).ToArray();
        int enemyCount = Mathf.Min(_enemies.Length, 100);

        _enemyPositions = new Vector4[enemyCount];
        _detectionRanges = new float[enemyCount];
        _attackRanges = new float[enemyCount];

        for (int i = 0; i < enemyCount; i++)
        {
            _enemyPositions[i] = _enemies[i].transform.position;
            _detectionRanges[i] = _enemies[i].DetectionRange;
            _attackRanges[i] = _enemies[i].AttackRange;
        }

        material.SetVectorArray(enemyPositionID, _enemyPositions);
        material.SetFloatArray(detectionRangesID, _detectionRanges);
        material.SetFloatArray(attackRangesID, _attackRanges);
        material.SetInt(enemyCountID, enemyCount);
    }
}
