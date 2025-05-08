using UnityEngine;
using UnityEngine.AI;

public class DummyEnemyWanderState : EnemyWanderState<DummyEnemy>
{
    public DummyEnemyWanderState(DummyEnemy enemy) : base(enemy) { }

    public override void Update()
    {
        if (_enemy.HasPlayerInDetectionRange())
        {
            _enemy.ChangeState<DummyEnemyChaseState>();
            return;
        }

        if (Time.time < _timer)
            return;

        Wander();
    }
}
