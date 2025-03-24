using UnityEngine;

public class DummyEnemyAttackState : EnemyState
{
    public DummyEnemyAttackState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        _enemy.ClearDestination();
        _enemy.target.Died += _enemy.OnTargetDied;
        //Debug.Log($"{_enemy.name} is currently attacking the player...");
    }

    public override void Update()
    {
        if (_enemy.target == null)
        {
            _enemy.ChangeState(new DummyEnemyWanderState(_enemy));
            return;
        }

        if (!_enemy.HasPlayerInAttackRange())
        {
            _enemy.ChangeState(new DummyEnemyChaseState(_enemy));
            return;
        }

        if (_enemy.target != null)
        {
            _enemy.transform.LookAt(_enemy.target.transform);

            if (Time.time < _timer)
                return;

            _enemy.Attack();

            _timer = _enemy.AttackCooldown + Time.time;
        }
    }

    public override void Exit()
    {
        if (_enemy.target != null)
            _enemy.target.Died -= _enemy.OnTargetDied;
    }
}