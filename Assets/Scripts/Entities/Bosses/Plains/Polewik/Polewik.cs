using System;
using System.Collections;
using UnityEngine;

public class Polewik : Boss
{
    [Header("Polowik Specific")]
    [SerializeField] private Debris _debrisPrefab;
    [SerializeField] private int _debrisPerWave = 5;
    [SerializeField] private float _debrisSpeed = 5f;
    [SerializeField] private float _waveDistance = 10;
    [SerializeField, Range(0, 360)] private float _waveShift = 45;
    [SerializeField] private float _wavesOnAttack = 4;
    [SerializeField] private float _waveSpeed = 0.1f;
    [SerializeField, ReadOnly] private int _waveShiftIndex = 0;

    [Header("Polowik Second Phase")]
    [SerializeField] private float _teleporationRadius = 6f;

    public override void Attack()
    {
        float angleFactor = 360f / _wavesOnAttack;
        float angleOffset = _waveShiftIndex * _waveShift;
        for (int i = 0; i < _wavesOnAttack; i++)
        {
            PlaySound(_attackSound);
            float angle = angleOffset + i * angleFactor;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;

            StartCoroutine(SpawnSinusoidalWave(direction));
        }
    }
    public override void AlternativeAttack()
    {
        if (!HasPlayerInDetectionRange())
            return;

        Vector3 origin = transform.position;

        if (NavMeshUtils.TryGetRandomPoint(origin, _teleporationRadius, out Vector3 randomPosition))
        {
            PlaySound(_abilitySound);
            Target.transform.position = randomPosition;

            Vector3 awayFromBoss = (randomPosition - transform.position).normalized;
            awayFromBoss.y = 0;
            if (awayFromBoss.sqrMagnitude > 0.01f)
                Target.transform.rotation = Quaternion.LookRotation(awayFromBoss);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _states = new EntityState[]
        {
            new EnemyWanderState<Polewik>(this),
            new EnemyChaseState<Polewik>(this),
            new PolewikAttackState(this)
        };
    }

    private void Start()
    {
        ChangeState<EnemyWanderState<Polewik>>();
    }

    protected override void EnterSecondPhase()
    {
        base.EnterSecondPhase();
    }

    private IEnumerator SpawnSinusoidalWave(Vector3 direction)
    {
        Vector3 start = transform.position;
        float step = _waveDistance / _debrisPerWave;

        for (int i = 0; i < _debrisPerWave; i++)
        {
            float distance = i * step;
            Vector3 spawnPosition = Vector3Int.RoundToInt(start + direction * distance + Vector3.Cross(direction, Vector3.up).normalized);

            Debris debris = Instantiate(_debrisPrefab, spawnPosition, Quaternion.identity);
            debris.Initialize(_attackDamage, this);
            if (debris.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.velocity = Vector3.up * _debrisSpeed;
            }

            yield return new WaitForSeconds(_waveSpeed);
        }
    }
}
