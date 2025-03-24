using System;

[Serializable]
public abstract class EnemyState : EntityState
{
    protected Enemy _enemy;

    protected EnemyState(Enemy enemy)
    {
        _enemy = enemy;
    }
}
