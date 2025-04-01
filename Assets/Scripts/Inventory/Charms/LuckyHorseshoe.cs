public class LuckyHorseshoe : Charm
{
    public override void ApplyEffect(Entity entity)
    {
        entity.GetComponent<EntityModifiers>().CritChanceModifier.Value *= GetIncreaser();
    }

    public override void RemoveEffect(Entity entity)
    {
        entity.GetComponent<EntityModifiers>().CritChanceModifier.Reset();
    }
}
