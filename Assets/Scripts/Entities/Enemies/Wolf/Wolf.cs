using UnityEngine;
using UnityEngine.AI;

public class Wolf : Enemy
{
    [Header("Wolf Specific")]
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _stunDuration;
    [SerializeField] private float _retreatDuration;

    public float StunDuration => _stunDuration;
    public float RetreatDuration => _retreatDuration;

    public override void Activate()
    {
        base.Activate();
        ChangeState<WolfWanderState>();
    }

    public void JumpTowardsTarget()
    {
        if (Target == null)
            return;
        Vector3 direction = (Target.transform.position - transform.position).normalized;
        Agent.velocity = direction * _jumpForce;
    }

    public void RetreatFromTarget()
    {
        if (Target == null)
            return;
        Vector3 direction = (transform.position - Target.transform.position).normalized;
        Agent.velocity = direction * _jumpForce;
    }

    protected override void Awake()
    {
        base.Awake();
        _states = new EntityState[] {
            new WolfWanderState(this),
            new WolfFlankState(this),
            new WolfAttackState(this),
            new WolfRetreatState(this)
        };
    }
}

public class WolfWanderState : EnemyState
{
    public WolfWanderState(Wolf wolf) : base(wolf) { }

    public override void Enter()
    {
        if (_enemy is Wolf wolf)
        {
            wolf.ClearDestination();
            Wander();
        }
    }

    public override void Update()
    {
        if (_enemy.HasPlayerInDetectionRange())
        {
            _enemy.ChangeState<WolfFlankState>();
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

public class WolfFlankState : EnemyState
{
    public WolfFlankState(Wolf wolf) : base(wolf) { }

    public override void Update()
    {
        if (!_enemy.HasPlayerInDetectionRange())
        {
            _enemy.ChangeState<WolfWanderState>();
            return;
        }

        if (_enemy.HasPlayerInAttackRange())
        {
            _enemy.ChangeState<WolfAttackState>();
            return;
        }

        (_enemy as Wolf).JumpTowardsTarget();
    }
}

public class WolfAttackState : EnemyState
{
    public WolfAttackState(Wolf wolf) : base(wolf) { }

    public override void Enter()
    {
        _enemy.ClearDestination();
        _enemy.Target.Died += _enemy.OnTargetDied;
    }

    public override void Update()
    {
        if (Time.time < _timer)
            return;

        if (_enemy.Target != null)
            _enemy.transform.LookAt(_enemy.Target.transform);

        _enemy.Attack();
        _enemy.ChangeState<WolfRetreatState>();
    }

    public override void Exit()
    {
        if (_enemy.Target != null)
            _enemy.Target.Died -= _enemy.OnTargetDied;
        RefreshTimer(_enemy.AttackCooldown);
    }
}

public class WolfRetreatState : EnemyState
{
    private bool _retreated = false;

    public WolfRetreatState(Wolf wolf) : base(wolf) { }

    public override void Enter()
    {
        if (_enemy is not Wolf wolf)
            return;
        RefreshTimer(wolf.RetreatDuration);
        wolf.RetreatFromTarget();
    }

    public override void Update()
    {
        if (!_retreated && Time.time > _timer)
        {
            _retreated = true;
            RefreshTimer((_enemy as Wolf).StunDuration);
        }

        if (_retreated && Time.time > _timer)
        {
            _enemy.ChangeState<WolfWanderState>();
        }
    }
}