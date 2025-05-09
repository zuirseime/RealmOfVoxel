﻿using UnityEngine;

public class DummyEnemyChaseState : EnemyState<DummyEnemy>
{
    public DummyEnemyChaseState(DummyEnemy enemy) : base(enemy) { }

    public override void Update()
    {
        if (!_enemy.HasPlayerInDetectionRange())
        {
            _enemy.ChangeState<DummyEnemyWanderState>();
            return;
        }

        if (_enemy.HasPlayerInAttackRange())
        {
            _enemy.ChangeState<DummyEnemyAttackState>();
            return;
        }

        _enemy.SetDestination(_enemy.Target.transform.position);
    }
}
