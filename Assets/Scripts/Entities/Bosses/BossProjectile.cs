using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class BossProjectile : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _lifeTime;

    private float _damage;
    private Vector3 _direction;

    public void Initialize(Vector3 target, float damage)
    {
        _direction = (target - transform.position).normalized;
        _damage = damage;
        Destroy(gameObject, _lifeTime);
    }

    void Update()
    {
        transform.position += _speed * Time.deltaTime * _direction;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider is not Collider other || other.isTrigger)
            return;

        if (other.TryGetComponent(out Boss _))
            return;

        if (other.TryGetComponent(out Player player))
        {
            player.TakeDamage(null, _damage);
        }
        Destroy(gameObject);
    }
}