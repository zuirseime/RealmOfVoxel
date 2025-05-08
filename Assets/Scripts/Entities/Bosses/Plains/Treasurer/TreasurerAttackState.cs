using UnityEngine;

public class TreasurerAttackState : EnemyState<Treasurer>
{
    private float _abilityTimer;

    public TreasurerAttackState(Treasurer treasurer) : base(treasurer) { }

    public override void Enter()
    {
        _timer = 0;
        _abilityTimer = 0;
        _enemy.ClearDestination();
    }

    public override void Update()
    {
        if (!_enemy.HasPlayerInDetectionRange())
        {
            _enemy.ChangeState<TreasurerWanderState>();
            return;
        }

        if (!_enemy.HasPlayerInAttackRange())
        {
            _enemy.ChangeState<TreasurerChaseState>();
        }

        _enemy.transform.LookAt(_enemy.Target.transform);

        _timer += Time.deltaTime;
        if (_timer >= _enemy.AttackCooldown)
        {
            _enemy.Attack();
            _timer = 0;
        }

        if (_enemy.IsSecondPhase)
        {
            _abilityTimer += Time.deltaTime;
            if (_abilityTimer >= _enemy.AbilityCooldown)
            {
                _enemy.AlternativeAttack();
                _abilityTimer = 0;
            }
        }
    }
}