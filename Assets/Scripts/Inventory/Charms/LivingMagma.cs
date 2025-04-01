
public class LivingMagma : Charm
{
    public override void ApplyEffect(Entity entity)
    {
        entity.GetComponent<EntityModifiers>().DefenceModifier.Value *= GetIncreaser();
    }

    public override void RemoveEffect(Entity entity)
    {
        entity.GetComponent<EntityModifiers>().DefenceModifier.Reset();
    }
}
