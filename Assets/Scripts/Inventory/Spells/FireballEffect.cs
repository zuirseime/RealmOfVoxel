using System.Collections;
using UnityEngine;

public class FireballEffect : ProjectileEffect
{
    private bool _expoding = false;
    public float MaxExpansion { get; set; }

    protected override void Update()
    {
        if (!_expoding && (Vector3.Distance(_startPosition, transform.position) >= Radius
            || (Vector3.Distance(_targetPosition, transform.position) <= 1f)))
        {
            Explode();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;
        if (other.TryGetComponent(out Entity entity) && entity.GetType() == Owner.GetType())
            return;

        if (entity != null)
            entity.TakeDamage(Owner, Damage);

        if (!_expoding)
            Explode();
    }

    private void Explode()
    {
        _expoding = true;
        _rigidbody.velocity = Vector3.zero;

        Destroy();
        StartCoroutine(ExplosionRoutine());
    }

    private IEnumerator ExplosionRoutine()
    {
        float startSize = 1;
        float timer = 0;

        while (timer < Duration)
        {
            float scaleFactor = Mathf.Lerp(startSize, MaxExpansion, timer / Duration);
            transform.localScale = Vector3.one * scaleFactor;

            timer += Time.deltaTime;
            yield return null;
        }
    }
}
