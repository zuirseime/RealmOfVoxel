using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public abstract class BossState : EntityState
{
    protected Boss _boss;
    public BossState(Boss boss) => _boss = boss;
}
