using UnityEngine;
using UnityEngine.AI;

public class TreasurerWanderState : EnemyWanderState<Treasurer>
{
    private Vector3 _wanderTarget;

    public TreasurerWanderState(Treasurer treasurer) : base(treasurer) { }

    public override void Enter()
    {
        RefreshTimer(_enemy.WanderCooldown);
        _enemy.ClearDestination();
    }

    public override void Update()
    {
        if (_enemy.Agent.remainingDistance < 0.5f || IsTimerFinished())
        {
            Wander();
        }

        if (!_enemy.Agent.pathPending && _enemy.Agent.remainingDistance > 0.1f)
        {
            _enemy.SetDestination(_wanderTarget);
        }

        if (_enemy.HasPlayerInDetectionRange())
        {
            _enemy.ChangeState<TreasurerChaseState>();
        }
    }

    public override void Exit()
    {
        _enemy.ClearDestination();
    }
}
