using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _lifeTime;

    private Entity _target;
    private bool _launched;
    private float _damage;

    public void Initialize(Entity target, float damage)
    {
        _target = target;
        _damage = damage;
        _launched = true;
        Destroy(gameObject, _lifeTime);
    }

    private void Update()
    {
        if (_launched)
        {
            var target = _target.transform.position + Vector3.up;
            var direction = (target - transform.position).normalized;
            transform.position += _speed * Time.deltaTime * direction;
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent(out Entity entity))
        {
            entity.TakeDamage(_damage);
            GetComponent<Collider>().enabled = false;
            Destroy(gameObject);
        }
    }
}