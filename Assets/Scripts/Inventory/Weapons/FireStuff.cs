using System.Collections;
using UnityEngine;

public class FireStuff : Weapon
{
    [SerializeField] private Projectile _projectilePrefab;

    [SerializeField] private Vector3[] _projectilesOffsets;

    protected override void AttackLogic(Entity target)
    {
        if (_projectilePrefab != null && target != null)
        {
            StartCoroutine(FireRoutine(target));
        }
    }

    private IEnumerator FireRoutine(Entity target)
    {
        foreach (var offset in _projectilesOffsets)
        {
            Projectile projectile = Instantiate(_projectilePrefab, transform.position + offset, transform.rotation);
            projectile.Initialize(target, Damage, Owner);

            yield return null;
        }
    }
}
