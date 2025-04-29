using Unity.AI.Navigation;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(NavMeshModifier))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _lifeTime;

    private Entity _target;
    private bool _launched;
    private float _damage;

    public Entity Owner { get; private set; }

    public void Initialize(Entity target, float damage, Entity owner)
    {
        GetComponent<Collider>().isTrigger = true;

        _launched = true;
        _damage = damage;
        Owner = owner;
        _target = target;
        _target.Died += OnTargetDied;
        
        if (_launched && _target.IsAlive)
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Entity entity)
            && entity.GetType() != Owner.GetType() && !other.isTrigger)
        {
            entity.TakeDamage(Owner, _damage);
            GetComponent<Collider>().enabled = false;
            if (gameObject != null)
                Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (_target != null)
            _target.Died -= OnTargetDied;
    }

    private void OnTargetDied(object sender, System.EventArgs args)
    {
        if (gameObject != null)
            Destroy(gameObject);
        _launched = false;
    }
}