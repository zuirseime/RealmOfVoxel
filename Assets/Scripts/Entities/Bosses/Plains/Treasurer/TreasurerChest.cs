using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider), typeof(ParticleSystem))]
public class TreasurerChest : MonoBehaviour
{
    [SerializeField] private float _lifeTime;
    [SerializeField] private float _damage;
    [SerializeField] Transform _geometry;

    private ParticleSystem _paricleSystem;

    private void Start()
    {
        _paricleSystem = GetComponent<ParticleSystem>();
        Destroy(gameObject, _lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            StartCoroutine(ExplosionRoutine());
            player.TakeDamage(null, _damage);
        }
    }

    private IEnumerator ExplosionRoutine()
    {
        _paricleSystem.Play();
        GetComponent<Collider>().enabled = false;
        Destroy(_geometry.gameObject);

        yield return new WaitForSeconds(_paricleSystem.main.duration + 1);

        Destroy(gameObject);
    }
}
