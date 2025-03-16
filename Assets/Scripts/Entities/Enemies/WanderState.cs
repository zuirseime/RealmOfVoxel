using UnityEngine;
using UnityEngine.AI;

public class WanderState : EnemyState
{
    private float _wanderTimer;

    public WanderState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        RefreshTimer();
        //Debug.Log($"{_enemy.name} is currently wandering...");
    }

    public override void Update()
    {
        _wanderTimer -= Time.deltaTime;

        if (_wanderTimer <= 0f)
        {
            Vector3 randomPoint = GetRandomNavMeshPoint(_enemy.transform.position, _enemy.WanderRadius);

            if (randomPoint != Vector3.zero)
            {
                _enemy.agent.SetDestination(randomPoint);
            }
            RefreshTimer();
        }

        if (_enemy.HasPlayerInDetectionRange())
        {
            _enemy.ChangeState(new ChaseState(_enemy));
        }
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
        _wanderTimer = _enemy.WanderCooldown;
    }
}
