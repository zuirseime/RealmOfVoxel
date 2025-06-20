using System.Collections.Generic;
using UnityEngine;

public class Treasurer : Boss
{
    [Header("Treasurer Specific")]
    [SerializeField] private BossProjectile _projectilePrefab;
    [SerializeField] private float _projectileShootingCooldown;
    [SerializeField] private TreasurerChest _chestPrefab;
    [SerializeField] private int _chestCup;
    [SerializeField] private float _chestSpawnRadius;

    private float _attackTimer;

    private List<TreasurerChest> _chests = new();

    public override void Activate()
    {
        base.Activate();
        ChangeState<TreasurerWanderState>();
    }

    public override void Attack()
    {
        if (Target != null && _projectilePrefab != null && CanShootProjectile())
        {
            PlaySound(_attackSound);
            BossProjectile projectile = Instantiate(_projectilePrefab, transform.position + Vector3.up, Quaternion.identity);
            projectile.Initialize(Target.transform.position + Vector3.up, _attackDamage);

            _attackTimer = Time.time + _projectileShootingCooldown;
        }
    }

    public override void AlternativeAttack()
    {
        if (_chestPrefab != null && CanPlaceChest())
        {
            PlaySound(_abilitySound);
            Vector3 spawnPosition = Vector3.zero;
            do
            {
                Vector3 randomDirection = Random.insideUnitSphere * _chestSpawnRadius;
                randomDirection.y = 0;
                spawnPosition = Vector3Int.RoundToInt(transform.position + randomDirection);
                Instantiate(_chestPrefab, spawnPosition, Quaternion.identity);
            } while (!Physics.Raycast(spawnPosition + Vector3.up, spawnPosition - Vector3.up, out RaycastHit hit) &&
                     !hit.collider.TryGetComponent(out Entity _));
        }
    }

    public bool CanShootProjectile()
    {
        return _attackTimer < Time.time;
    }

    protected override void Update()
    {
        base.Update();

        List<TreasurerChest> chestsToRemove = new();
        foreach (TreasurerChest chest in _chests)
        {
            if (chest == null || chest.gameObject == null)
            {
                chestsToRemove.Add(chest);
            }
        }

        foreach (TreasurerChest chest in chestsToRemove)
        {
            _chests.Remove(chest);
        }
    }

    protected override void Awake()
    {
        base.Awake();

        _states = new EntityState[]
        {
            new TreasurerWanderState(this),
            new TreasurerChaseState(this),
            new TreasurerAttackState(this)
        };
    }

    private bool CanPlaceChest()
    {
        return _chests.Count < _chestCup;
    }
}
