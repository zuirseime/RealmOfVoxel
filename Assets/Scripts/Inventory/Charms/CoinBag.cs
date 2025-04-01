public class CoinBag : Charm
{
    public override void ApplyEffect(Entity entity)
    {
        entity.GetComponent<EntityModifiers>().CoinModifier.Value *= GetIncreaser();
    }

    public override void RemoveEffect(Entity entity)
    {
        entity.GetComponent<EntityModifiers>().CoinModifier.Reset();
    }
}