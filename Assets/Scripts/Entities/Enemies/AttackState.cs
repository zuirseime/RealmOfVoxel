using UnityEngine;

public class AttackState : EnemyState
{
    private float _attackCooldown = 1.5f;
    private float _nextAttackTime;

    public AttackState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        Debug.Log($"{_enemy.name} is currently attacking the player...");
    }

    public override void Update()
    {
        if (!_enemy.HasPlayerInAttackRange())
        {
            _enemy.ChangeState(new ChaseState(_enemy));
            return;
        }

        if (Time.time > _nextAttackTime)
        {
            _enemy.Attack(_enemy.Player.GetComponent<Entity>());
            _nextAttackTime = Time.time + _attackCooldown;
        }
    }
}