public class LifeRing : Charm
{
    public override void ApplyEffect(Entity entity)
    {
        _savedValue = entity.Health.MaxValue;

        entity.Health.MaxValue *= GetIncreaser();
        entity.Health.Value *= GetIncreaser();
    }

    public override void RemoveEffect(Entity entity)
    {
        float ratio = entity.Health.Value / entity.Health.MaxValue;

        entity.Health.MaxValue = _savedValue;
        entity.Health.Value = entity.Health.MaxValue * ratio;
    }
}