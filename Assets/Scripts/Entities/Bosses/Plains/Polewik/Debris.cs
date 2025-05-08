using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
public class Debris : MonoBehaviour
{
    [SerializeField] private float _lifetime;
    [SerializeField] private float _knockbackForce;

    private float _damage;
    private Boss _owner;

    public void Initialize(float damage, Boss owner)
    {
        _damage = damage;
        _owner = owner;

        Destroy(gameObject, _lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out Entity entity)) return;
        if (entity == _owner || !entity.IsAlive) return;
        if (!entity.CanBeDamagedBy(_owner)) return;

        entity.TakeDamage(_owner, _damage);

        if (other.TryGetComponent<Rigidbody>(out var rb))
        {
            Vector3 direction = (other.transform.position - transform.position).normalized;
            rb.AddForce(direction * _knockbackForce, ForceMode.Impulse);
        }

        Destroy(gameObject);
    }
}