using System;
using UnityEngine;

[Serializable]
public abstract class EnemyState<T> : EntityState where T : Enemy
{
    protected T _enemy;

    protected EnemyState(T enemy)
    {
        _enemy = enemy;
    }
}

[Serializable]
public class EnemyWanderState<T> : EnemyState<T> where T : Enemy
{
    public EnemyWanderState(T enemy) : base(enemy) { }

    public override void Enter()
    {
        RefreshTimer(_enemy.WanderCooldown);
        _enemy.ClearDestination();
    }

    public override void Update()
    {
        if (_enemy.HasPlayerInDetectionRange())
        {
            _enemy.ChangeState<EnemyChaseState<T>>();
            return;
        }

        if (!IsTimerFinished())
            return;

        Wander();
    }

    protected void Wander()
    {
        if (TryFindRandomNavMeshPoint(out Vector3 randomPoint))
        {
            _enemy.SetDestination(randomPoint);
            RefreshTimer(_enemy.WanderCooldown);
        }
    }

    private bool TryFindRandomNavMeshPoint(out Vector3 foundPoint)
    {
        if (_enemy == null)
        {
            foundPoint = Vector3.zero;
            return false;
        }

        return NavMeshUtils.TryGetRandomPoint(_enemy.transform.position, _enemy.WanderRadius, out foundPoint);
    }
}

[Serializable]
public class EnemyChaseState<T> : EnemyState<T> where T : Enemy
{
    public EnemyChaseState(T enemy) : base(enemy) { }

    public override void Enter()
    {
        _enemy.SetDestination(_enemy.Target.transform.position);
    }

    public override void Update()
    {
        if (_enemy.HasPlayerInAttackRange())
        {
            _enemy.ChangeState<EnemyAttackState<T>>();
            return;
        }

        if (!_enemy.HasPlayerInDetectionRange())
        {
            _enemy.ChangeState<EnemyWanderState<T>>();
            return;
        }

        _enemy.SetDestination(_enemy.Target.transform.position);
    }

    public override void Exit()
    {
        _enemy.ClearDestination();
    }
}

[Serializable]
public class EnemyAttackState<T> : EnemyState<T> where T : Enemy
{
    public EnemyAttackState(T enemy) : base(enemy) { }

    public override void Update() { }
}