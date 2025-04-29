using UnityEngine;

public class TreasurerAttackState : BossState
{
    private float _abilityTimer;

    public TreasurerAttackState(Treasurer boss) : base(boss) { }

    public override void Enter()
    {
        _timer = 0;
        _abilityTimer = 0;
        _boss.ClearDestination();
    }

    public override void Update()
    {
        if (_boss.Target == null || !_boss.Target.IsAlive)
        {
            _boss.ChangeState<TreasurerWanderState>();
            return;
        }

        _boss.transform.LookAt(_boss.Target.transform);

        _timer += Time.deltaTime;
        if (_timer >= _boss.AttackCooldown)
        {
            _boss.Attack();
            _timer = 0;
        }

        if (_boss.IsSecondPhase)
        {
            _abilityTimer += Time.deltaTime;
            if (_abilityTimer >= _boss.AbilityCooldown)
            {
                _boss.AlternativeAttack();
                _abilityTimer = 0;
            }
        }

        if (!_boss.HasPlayerInAttackRange())
        {
            _boss.ChangeState<TreasurerChaseState>();
        }
    }
}