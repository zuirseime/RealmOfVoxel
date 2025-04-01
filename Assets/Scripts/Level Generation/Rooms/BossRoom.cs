using System.Collections;
using System.Linq;
using UnityEngine;

public class BossRoom : Room
{
    [SerializeField] private Boss[] _prefabs;

    private bool _clear = false;
    private Boss _boss;

    private void OnTriggerEnter(Collider other)
    {
        if (_clear) return;

        if (other.TryGetComponent(out Player player))
        {
            player.SwitchToBattleMode();
            player.transform.parent = transform;

            var randomPrefab = _prefabs.OrderBy(p => Random.value).FirstOrDefault();
            _boss = Instantiate(randomPrefab, transform);
            _boss.Died += OnBossDied;

            StartCoroutine(CloseDoors());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Player player) && !_boss.IsAlive)
        {
            player.SwitchToSprintMode();
            player.transform.parent = null;
        }
    }

    private void OnBossDied(object sender, System.EventArgs args)
    {
        _boss.Died -= OnBossDied;
        _clear = true;
        StartCoroutine(OpenDoors());
        FindObjectOfType<Player>().SwitchToSprintMode();
    }

    private IEnumerator CloseDoors()
    {
        doors.ForEach(d => d.Close());

        float animationTime = doors.FirstOrDefault().GetAnimationState();

        yield return new WaitForSeconds(animationTime / 2f);

        LevelGenerator.Surface.UpdateNavMesh(LevelGenerator.Surface.navMeshData);
    }

    private IEnumerator OpenDoors()
    {
        doors.ForEach(d => d.Open());

        float animationTime = doors.FirstOrDefault().GetAnimationState();

        yield return new WaitForSeconds(animationTime);

        LevelGenerator.Surface.UpdateNavMesh(LevelGenerator.Surface.navMeshData);
    }
}
