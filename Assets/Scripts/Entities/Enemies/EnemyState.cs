using System;

[Serializable]
public abstract class EnemyState : IState
{
    protected Enemy _enemy;

    public EnemyState(Enemy enemy)
    {
        _enemy = enemy;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public abstract void Update();
}
