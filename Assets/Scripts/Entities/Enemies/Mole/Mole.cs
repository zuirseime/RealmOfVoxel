using UnityEngine;
using UnityEngine.AI;

public class Mole : Enemy
{
    [Header("Mole Specific")]
    [SerializeField] private Renderer _model;
    [SerializeField] private Collider _collider;
    [SerializeField] private Canvas _info;
    [SerializeField] private float _landDuration = 0.25f;
    [SerializeField] private float _retreatingCooldown = 2f;

    private bool _isAttacking = false;
    private float _retreatTimer;
    private bool _isRetreating = false;

    public float LandDuration => _landDuration;
    public bool IsAttacking => _isAttacking;
    public bool Retreating => _isRetreating && _retreatTimer > Time.time;
    public void Retreat()
    {
        _isRetreating = true;
        _retreatTimer = Time.time + _retreatingCooldown;
    }

    public override bool CanBeDamagedBy(Entity attacker) => _isAttacking;

    public void StartAttack()
    {
        _isAttacking = true;
        SetModelActive(true);
    }

    public void EndAttack()
    {
        _isAttacking = false;
        SetModelActive(false);
    }

    public override void Activate()
    {
        base.Activate();
        ChangeState<MoleWanderState>();
    }

    public void SetModelActive(bool isActive)
    {
        _model.enabled = isActive;
        _collider.enabled = isActive;
        _info.enabled = isActive;
    }

    protected override void Awake()
    {
        base.Awake();
        _states = new EntityState[]
        {
            new MoleWanderState(this),
            new MoleChaseState(this),
            new MoleAttackState(this)
        };
    }
}

public class MoleWanderState : EnemyState
{
    public MoleWanderState(Mole mole) : base(mole) { }

    public override void Enter()
    {
        if (_enemy is Mole mole)
        {
            mole.ClearDestination();
            Wander();
            mole.SetModelActive(false);
        }
    }

    public override void Update()
    {
        if (_enemy is not Mole mole)
            return;

        if (!mole.Retreating && mole.HasPlayerInDetectionRange())
        {
            _enemy.ChangeState<MoleChaseState>();
            return;
        }

        if (Time.time < _timer)
            return;

        Wander();
    }

    private void Wander()
    {
        Vector3 randomPoint = GetRandomNavMeshPoint(_enemy.transform.position, _enemy.WanderRadius);

        if (randomPoint != Vector3.zero)
        {
            _enemy.Agent.SetDestination(randomPoint);
        }

        RefreshTimer(_enemy.WanderCooldown);
    }

    private Vector3 GetRandomNavMeshPoint(Vector3 center, float radius)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * radius;
            randomDirection.y = 0;
            randomDirection += center;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        return Vector3.zero;
    }
}

public class MoleChaseState : EnemyState
{
    public MoleChaseState(Mole mole) : base(mole) { }

    public override void Enter()
    {
        (_enemy as Mole).SetModelActive(false);
    }

    public override void Update()
    {
        if (!_enemy.HasPlayerInDetectionRange())
        {
            _enemy.ChangeState<MoleWanderState>();
            return;
        }

        if (_enemy.HasPlayerInAttackRange())
        {
            _enemy.ChangeState<MoleAttackState>();
            return;
        }

        _enemy.SetDestination(_enemy.Target.transform.position);
    }
}

public class MoleAttackState : EnemyState
{
    private float _onLandTimer;

    public MoleAttackState(Mole mole) : base(mole) { }

    public override void Enter()
    {
        _onLandTimer = Time.time + (_enemy as Mole).LandDuration;
        _enemy.ClearDestination();
        _enemy.Target.Died += _enemy.OnTargetDied;

        (_enemy as Mole).StartAttack();
    }

    public override void Update()
    {
        if (_enemy.Target == null)
        {
            _enemy.ChangeState<MoleWanderState>();
            return;
        }

        if (!_enemy.HasPlayerInAttackRange())
        {
            _enemy.ChangeState<MoleChaseState>();
            return;
        }

        _enemy.transform.LookAt(_enemy.Target.transform);

        if (Time.time < _timer)
            return;

        _enemy.Attack();

        RefreshTimer(_enemy.AttackCooldown);

        if (Time.time >= _onLandTimer)
        {
            (_enemy as Mole).Retreat();
            _enemy.ChangeState<MoleWanderState>();
        }
    }

    public override void Exit()
    {
        _enemy.Target.Died -= _enemy.OnTargetDied;
        (_enemy as Mole).EndAttack();
    }
}