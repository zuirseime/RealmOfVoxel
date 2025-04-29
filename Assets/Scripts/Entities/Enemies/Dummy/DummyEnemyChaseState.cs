using UnityEngine;

public class DummyEnemyChaseState : EnemyState
{
    public DummyEnemyChaseState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        //Debug.Log($"{_enemy.name} is currently chasing the player...");
        //Player player = Object.FindObjectOfType<Player>();
        //if (player != null && player.IsAlive)
        //{
        //    _enemy.Target = player;
        //}
    }

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
