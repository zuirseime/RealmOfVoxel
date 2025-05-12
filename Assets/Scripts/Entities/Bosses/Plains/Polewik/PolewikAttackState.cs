using UnityEngine;

public class PolewikAttackState : EnemyAttackState<Polewik>
{
    private float _abilityTimer;

    public PolewikAttackState(Polewik polewik) : base(polewik) { }

    public override void Enter()
    {
        _abilityTimer = 0;
        _enemy.ClearDestination();
        RefreshTimer(_enemy.AttackCooldown);
    }

    public override void Update()
    {
        if (!_enemy.HasPlayerInDetectionRange())
        {
            _enemy.ChangeState<EnemyWanderState<Polewik>>();
            return;
        }

        if (!_enemy.HasPlayerInAttackRange())
        {
            _enemy.ChangeState<EnemyChaseState<Polewik>>();
            return;
        }

        _enemy.transform.LookAt(_enemy.Target.transform);

        if (IsTimerFinished())
        {
            _enemy.Attack();
            RefreshTimer(_enemy.AttackCooldown);
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
