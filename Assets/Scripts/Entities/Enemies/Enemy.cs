using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Enemy : Entity
{
    [SerializeField] protected float _detectionRange;

    [SerializeField] protected float _attackRange;
    [SerializeField] protected float _attackCooldown;

    [SerializeField] protected float _wanderRadius;
    [SerializeField] protected float _wanderCooldown;

    protected List<Player> _players;

    [SerializeField, Header("Read Only")] protected EnemyState _currentState;

    public float WanderRadius => _wanderRadius;
    public float WanderCooldown => _wanderCooldown;
    public bool Active { get; protected set; }
    public float DetectionRange => _detectionRange;
    public float AttackRange => _attackRange;
    public float AttackCooldown => _attackCooldown;

    public void Activate()
    {
        enabled = true;
        Active = true;
        ChangeState(new WanderState(this));
    }

    protected override void Awake()
    {
        base.Awake();
        _players = FindObjectsOfType<Player>().ToList();
    }

    protected virtual void Update()
    {
        if (!Active)
            return;

        target = _players.OrderBy(p => DistanceToTarget(p)).FirstOrDefault();

        _currentState?.Update();
    }

    internal void ChangeState(EnemyState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter();
    }

    public bool HasPlayerInDetectionRange()
    {
        return DistanceToTarget(target) < _detectionRange;
    }

    public bool HasPlayerInAttackRange()
    {
        return DistanceToTarget(target) < _attackRange;
    }

    private float DistanceToTarget(Entity target)
    {
        return Vector3.Distance(transform.position, target.transform.position);
    }

    protected override void Die()
    {
        base.Die();
        transform.Rotate(Vector3.right * 90, Space.Self);
        agent.enabled = false;
        this.enabled = false;
        GetComponent<Collider>().enabled = false;
    }
}
