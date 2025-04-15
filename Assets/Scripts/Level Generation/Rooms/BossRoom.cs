using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossRoom : Room
{
    [SerializeField] private Boss[] _possibleBosses;

    private bool _clear = false;
    private Boss _boss;
    private BossBar _bossBar;

    private List<Enemy> _enemiesRemain = new();

    private void Start()
    {
        _bossBar = FindObjectOfType<BossBar>();
        _bossBar.gameObject.SetActive(false);
    }

    public override void Prepare()
    {
        StartCoroutine(CloseDoors());
    }

    public void AddEnemiesToTrack(IEnumerable<Enemy> enemies)
    {
        foreach (var enemy in enemies)
        {
            enemy.Died += OnEnemyDied;
        }

        _enemiesRemain.AddRange(enemies);
    }

    private void OnEnemyDied(object sender, System.EventArgs args)
    {
        if (sender is Enemy enemy)
        {
            enemy.Died -= OnEnemyDied;
            _enemiesRemain.Remove(sender as Enemy);
            if (_enemiesRemain.Count == 0)
            {
                StartCoroutine(OpenDoors());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_clear) return;

        if (other.TryGetComponent(out Player player))
        {
            player.SwitchToBattleMode();
            player.transform.parent = transform;

            var randomPrefab = _possibleBosses.OrderBy(p => Random.value).FirstOrDefault();
            _boss = Instantiate(randomPrefab, transform);
            _boss.Died += OnBossDied;
            _boss.Activate();

            _bossBar.Initialize(_boss);

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

        LevelGenerator.UpdateSurfaces();
    }

    private IEnumerator OpenDoors()
    {
        doors.ForEach(d => d.Open());

        float animationTime = doors.FirstOrDefault().GetAnimationState();

        yield return new WaitForSeconds(animationTime);

        LevelGenerator.UpdateSurfaces();
    }
}
