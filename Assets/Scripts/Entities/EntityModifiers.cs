using UnityEngine;

public class EntityModifiers : MonoBehaviour
{
    [field: SerializeField] public EntityModifier MoveSpeedModifier { get; set; }
    [field: SerializeField] public EntityModifier CritChanceModifier { get; set; }
    [field: SerializeField] public EntityModifier CritMultiplicationModifier { get; set; }
    [field: SerializeField] public EntityModifier DamageModifier { get; set; }
    [field: SerializeField] public EntityModifier CooldownModifier { get; set; }
    [field: SerializeField] public EntityModifier DefenceModifier { get; set; }
    [field: SerializeField] public EntityModifier CoinModifier { get; set; }

    private void Awake()
    {
        ResetModifiers();
    }

    private void ResetModifiers()
    {
        DamageModifier.Reset();
        MoveSpeedModifier.Reset();
        CooldownModifier.Reset();
        CritChanceModifier.Reset();
        CritMultiplicationModifier.Reset();
        DefenceModifier.Reset();
        CoinModifier.Reset();
    }
}
