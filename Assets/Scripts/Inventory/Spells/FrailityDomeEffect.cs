using UnityEngine;

public class FrailityDomeEffect : AreaEffect
{
    public float ProtectionReduction { get; set; }

    protected override void Start()
    {
        base.Start();

        transform.localScale = Vector3.one * MaxExpansion;
    }

    protected override void AffectEntity(Entity entity)
    {
        if (entity != null && entity.GetType() != Owner.GetType())
        {
            entity.GetComponent<EntityModifiers>().DefenceModifier.Value = 1 + ProtectionReduction;
        }
    }

    private void OnDestroy()
    {
        foreach (Entity entity in _entitiesToAffect)
        {
            if (entity != null)
            {
                entity.GetComponent<EntityModifiers>().DefenceModifier.Reset();
            }
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        if (!IsColliderAcceptable(other, out Entity entity))
            return;

        if (_entitiesToAffect.Contains(entity))
        {
            entity.GetComponent<EntityModifiers>().DefenceModifier.Reset();
            _entitiesToAffect.Remove(entity);
        }
    }
}
