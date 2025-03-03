using UnityEngine;

public class ChaseState : EnemyState
{
    public ChaseState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        Debug.Log($"{_enemy.name} is currently chasing the player...");
    }

    public override void Update()
    {
        if (!_enemy.HasPlayerInDetectionRange())
        {
            _enemy.ChangeState(new WanderState(_enemy));
            return;
        }

        _enemy.SetDestination(_enemy.Player.position);

        if (_enemy.HasPlayerInAttackRange())
        {
            _enemy.ChangeState(new AttackState(_enemy));
        }
    }
}
