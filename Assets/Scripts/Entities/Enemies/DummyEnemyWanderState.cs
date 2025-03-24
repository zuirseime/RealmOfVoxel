using UnityEngine;
using UnityEngine.AI;

public class DummyEnemyWanderState : EnemyState
{
    public DummyEnemyWanderState(Enemy enemy) : base(enemy) { }

    public override void Update()
    {
        if (_enemy.HasPlayerInDetectionRange())
        {
            _enemy.ChangeState(new DummyEnemyChaseState(_enemy));
            return;
        }

        if (Time.time < _timer)
            return;

        Vector3 randomPoint = GetRandomNavMeshPoint(_enemy.transform.position, _enemy.WanderRadius);

        if (randomPoint != Vector3.zero)
        {
            _enemy.Agent.SetDestination(randomPoint);
        }
        RefreshTimer();
    }

    private Vector3 GetRandomNavMeshPoint(Vector3 center, float radius)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * radius;
            randomDirection.y = 0;
            randomDirection += center;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        return Vector3.zero;
    }

    private void RefreshTimer()
    {
        _timer = _enemy.WanderCooldown + Time.time;
    }
}
