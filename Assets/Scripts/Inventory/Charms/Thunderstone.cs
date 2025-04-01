public class Thunderstone : Charm
{
    public override void ApplyEffect(Entity entity)
    {
        entity.GetComponent<EntityModifiers>().MoveSpeedModifier.Value *= GetIncreaser();
    }
    public override void RemoveEffect(Entity entity)
    {
        entity.GetComponent<EntityModifiers>().MoveSpeedModifier.Reset();
    }
}
