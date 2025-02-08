using UnityEngine;

[System.Serializable]
public class Rule
{
    public SectionType sectionType;
    [Min(0)] public int minAmount;
    public int maxAmount;

    [HideInInspector] public int actualAmount;

    public int Max => Random.Range(minAmount, maxAmount + 1);

    public bool Available() => actualAmount < Max;
    public void IncreaseActualAmount() => actualAmount++;
    public void Revert() => actualAmount = 0;
}
