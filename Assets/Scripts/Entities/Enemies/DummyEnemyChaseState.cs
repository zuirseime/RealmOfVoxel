using UnityEngine;

public class DummyEnemyChaseState : EnemyState
{
    public DummyEnemyChaseState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        //Debug.Log($"{_enemy.name} is currently chasing the player...");
        Player player = Object.FindObjectOfType<Player>();
        if (player != null && player.IsAlive)
        {
            _enemy.target = player;
        }
    }

    public override void Update()
    {
        if (!_enemy.HasPlayerInDetectionRange())
        {
            _enemy.ChangeState(new DummyEnemyWanderState(_enemy));
            return;
        }

        if (_enemy.HasPlayerInAttackRange())
        {
            _enemy.ChangeState(new DummyEnemyAttackState(_enemy));
            return;
        }

        _enemy.SetDestination(_enemy.target.transform.position);
    }
}
