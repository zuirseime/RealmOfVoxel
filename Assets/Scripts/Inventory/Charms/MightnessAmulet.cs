public class MightnessAmulet : Charm
{
    public override void ApplyEffect(Entity entity)
    {
        entity.GetComponent<EntityModifiers>().DamageModifier.Value *= GetIncreaser();
    }

    public override void RemoveEffect(Entity entity)
    {
        entity.GetComponent<EntityModifiers>().DamageModifier.Reset();
    }
}
