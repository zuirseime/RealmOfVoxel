using UnityEngine;
using UnityEngine.AI;

public class SpawnRoom : Room
{
    public override void Prepare()
    {
        base.Prepare();

        var player = FindObjectOfType<Player>();
        if (player == null || Camera.main == null)
            return;

        var agent = player.GetComponent<NavMeshAgent>();
        agent.enabled = false;

        player.transform.position = transform.position;
        agent.enabled = true;

        Camera camera = Camera.main;
        camera.transform.position = player.transform.position + Vector3.up * 3f;
    }
}
