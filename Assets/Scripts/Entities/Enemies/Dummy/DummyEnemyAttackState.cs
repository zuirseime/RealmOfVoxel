﻿using UnityEngine;

public class DummyEnemyAttackState : EnemyState<DummyEnemy>
{
    public DummyEnemyAttackState(DummyEnemy enemy) : base(enemy) { }

    public override void Enter()
    {
        RefreshTimer(_enemy.AttackCooldown / 2f);
        _enemy.ClearDestination();
        _enemy.Target.Died += _enemy.OnTargetDied;
    }

    public override void Update()
    {
        if (_enemy.Target == null)
        {
            _enemy.ChangeState<DummyEnemyWanderState>();
            return;
        }

        if (!_enemy.HasPlayerInAttackRange())
        {
            _enemy.ChangeState<DummyEnemyChaseState>();
            return;
        }

        if (_enemy.Target != null)
        {
            _enemy.transform.LookAt(_enemy.Target.transform);

            if (Time.time < _timer)
                return;

            _enemy.Attack();

            _timer = _enemy.AttackCooldown + Time.time;
        }
    }

    public override void Exit()
    {
        if (_enemy.Target != null)
            _enemy.Target.Died -= _enemy.OnTargetDied;
    }
}