using System.Collections.Generic;
using UnityEngine;

public class AbyssalShroudEffect : AuraEffect
{
    protected float _nextTickTime;

    protected readonly List<Entity> _entitiesToAffect = new();

    public float RotationSpeed { get; set; }
    public float Damage { get; set; }
    public float TickRate { get; set; }
    public float EffectRadius { get; set; }

    protected override void Start()
    {
        base.Start();

        transform.localScale = new Vector3(EffectRadius, transform.localScale.y, EffectRadius);
    }

    protected override void Update()
    {
        base.Update();
        transform.position = Owner.transform.position;
        transform.Rotate(RotationSpeed * Time.deltaTime * Vector3.up);

        if (Time.time >= _nextTickTime)
        {
            foreach (Entity entity in _entitiesToAffect)
            {
                AffectEntity(entity);
            }

            _nextTickTime = Time.time + TickRate;
        }
    }

    private void AffectEntity(Entity entity)
    {
        if (entity != null && entity.GetType() != Owner.GetType())
            entity.TakeDamage(Damage);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsColliderAcceptable(other, out Entity entity))
            return;

        if (!_entitiesToAffect.Contains(entity))
            _entitiesToAffect.Add(entity);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsColliderAcceptable(other, out Entity entity))
            return;

        if (_entitiesToAffect.Contains(entity))
            _entitiesToAffect.Remove(entity);
    }

    private bool IsColliderAcceptable(Collider other, out Entity entity)
    {
        entity = null;
        return !other.isTrigger && other.TryGetComponent(out entity);
    }
}