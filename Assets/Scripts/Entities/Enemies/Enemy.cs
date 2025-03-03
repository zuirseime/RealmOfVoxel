using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : Entity
{
    [SerializeField] protected float _detectionRange;
    [SerializeField] protected float _attackRange;
    [SerializeField] protected float _wanderRadius;
    [SerializeField] protected float _wanderCooldown;

    protected Transform _player;
    protected bool _active;

    [SerializeField, Header("Read Only")] protected EnemyState _currentState;

    public Transform Player
    {
        get => _player;
        protected set => _player = value;
    }
    public float WanderRadius => _wanderRadius;
    public float WanderCooldown => _wanderCooldown;

    public void Activate()
    {
        _active = true;
        ChangeState(new WanderState(this));
    }

    protected override void Awake()
    {
        base.Awake();
        Player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    protected virtual void Update()
    {
        if (!_active)
            return;

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
        return Vector3.Distance(transform.position, Player.position) < _detectionRange;
    }

    public bool HasPlayerInAttackRange()
    {
        return Vector3.Distance(transform.position, Player.position) < _attackRange;
    }

    public void SetDestination(Vector3 destination)
    {
        agent.SetDestination(destination);
    }
}
