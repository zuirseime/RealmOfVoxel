using System;
using UnityEngine;

[Serializable]
public class Rule
{
    public SectionType sectionType;
    [Min(0)] public int minAmount;
    public int maxAmount;

    [HideInInspector] public int actualAmount;
}
