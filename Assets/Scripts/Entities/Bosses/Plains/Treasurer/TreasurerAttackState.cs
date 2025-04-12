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
        if (_boss.target == null || !_boss.target.IsAlive)
        {
            _boss.ChangeState(new TreasurerWanderState((Treasurer)_boss));
            return;
        }

        _boss.transform.LookAt(_boss.target.transform);

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
            _boss.ChangeState(new TreasurerChaseState((Treasurer)_boss));
        }
    }
}