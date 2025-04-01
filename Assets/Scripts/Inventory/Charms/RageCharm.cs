public class RageCharm : Charm
{
    public override void ApplyEffect(Entity entity)
    {
        entity.GetComponent<EntityModifiers>().CooldownModifier.Value *= GetDecreaser();
    }
    public override void RemoveEffect(Entity entity)
    {
        entity.GetComponent<EntityModifiers>().CooldownModifier.Reset();
    }
}
