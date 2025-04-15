using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class AreaEffect : SpellEffect
{
    protected float _nextTickTime;

    protected readonly List<Entity> _entitiesToAffect = new();

    public float MaxExpansion { get; set; }
    public float TickRate { get; set; }

    protected virtual void Start()
    {
        Destroy();
    }

    protected virtual void Update()
    {
        if (Time.time >= _nextTickTime)
        {
            foreach (Entity entity in _entitiesToAffect)
            {
                AffectEntity(entity);
            }

            _nextTickTime = Time.time + TickRate;
        }
    }

    protected abstract void AffectEntity(Entity entity);

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!IsColliderAcceptable(other, out Entity entity)) return;

        if (!_entitiesToAffect.Contains(entity))
            _entitiesToAffect.Add(entity);
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (!IsColliderAcceptable(other, out Entity entity)) return;

        if (_entitiesToAffect.Contains(entity))
            _entitiesToAffect.Remove(entity);
    }

    protected bool IsColliderAcceptable(Collider other, out Entity entity)
    {
        entity = null;
        return !other.isTrigger && other.TryGetComponent(out entity);
    }
}
