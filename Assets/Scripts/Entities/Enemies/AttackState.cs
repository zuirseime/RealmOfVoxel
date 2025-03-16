using UnityEngine;

public class AttackState : EnemyState
{
    private float _attackTimer;

    public AttackState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        _enemy.agent.ResetPath();
        //Debug.Log($"{_enemy.name} is currently attacking the player...");
    }

    public override void Update()
    {
        if (!_enemy.HasPlayerInAttackRange())
        {
            _enemy.ChangeState(new ChaseState(_enemy));
            return;
        }

        _attackTimer -= Time.deltaTime;
        if (_attackTimer <= 0)
        {
            _enemy.Attack();
            _attackTimer = _enemy.AttackCooldown;
        }

        _enemy.transform.LookAt(_enemy.target.transform);
    }
}