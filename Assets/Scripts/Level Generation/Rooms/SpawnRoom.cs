using UnityEngine;

public class SpawnRoom : Room
{

    public override void Prepare()
    {
        base.Prepare();

        var player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Entity>();
        player.transform.position = transform.position;

        Camera camera = Camera.main;
        camera.transform.position = player.transform.position + Vector3.up * 3f;
    }
}
