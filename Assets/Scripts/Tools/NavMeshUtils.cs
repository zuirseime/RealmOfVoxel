using UnityEngine;
using UnityEngine.AI;

public static class NavMeshUtils
{
    public static bool TryGetRandomPoint(Vector3 center, float range, out Vector3 result, int areaMask = NavMesh.AllAreas, int maxAttempts = 30)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * range;
            center.y = 0;
            randomDirection.y = 0;
            Vector3 randomPoint = center + randomDirection;

            float sampleRadius = 1f;

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, sampleRadius, areaMask))
            {
                result = hit.position;
                return true;
            }
        }

        result = Vector3.zero;
        return false;
    }

    public static bool CheckAndAdjustPointOnNavMesh(Vector3 origin, Vector3 direction, float distance, out Vector3 result, float maxSearchRadius = 2f, int areaMask = NavMesh.AllAreas)
    {
        NavMeshHit hit;
        if (direction == Vector3.zero && distance == 0)
        {
            if (NavMesh.SamplePosition(origin, out hit, maxSearchRadius, areaMask))
            {
                result = hit.position;
                return true;
            } else
            {
                result = origin;
                return false;
            }
        }

        Vector3 targetPoint = origin + direction.normalized * distance;

        const float directCheckRadius = 0.1f;
        if (NavMesh.SamplePosition(targetPoint, out hit, directCheckRadius, areaMask))
        {
            result = hit.position;
            return true;
        }

        if (NavMesh.SamplePosition(targetPoint, out hit, maxSearchRadius, areaMask))
        {
            result = hit.position;
            return true;
        }

        result = origin;
        return false;
    }
}
