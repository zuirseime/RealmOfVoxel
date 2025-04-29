using UnityEngine;
using UnityEngine.AI;

public class TreasurerWanderState : BossState
{
    private Vector3 _wanderTarget;

    public TreasurerWanderState(Treasurer boss) : base(boss) { }

    public override void Enter()
    {
        _timer = 0;
        SetNewWanderTarget();
    }

    public override void Update()
    {
        _timer += Time.deltaTime;

        if (_boss.Agent.remainingDistance < 0.5f || _timer >= _boss.WanderCooldown)
        {
            SetNewWanderTarget();
            _timer = 0;
        }

        if (!_boss.Agent.pathPending && _boss.Agent.remainingDistance > 0.1f)
        {
            _boss.SetDestination(_wanderTarget);
        }

        if (_boss.HasPlayerInDetectionRange())
        {
            _boss.ChangeState<TreasurerChaseState>();
        }
    }

    public override void Exit()
    {
        _boss.ClearDestination();
    }

    private void SetNewWanderTarget()
    {
        Vector3 randomDirection = Random.insideUnitSphere * ((Treasurer)_boss).WanderRadius;
        randomDirection.y = 0;
        _wanderTarget = _boss.transform.position + randomDirection;
        if (NavMesh.SamplePosition(_wanderTarget, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        {
            _wanderTarget = hit.position;
        }
    }
}
