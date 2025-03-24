using System;

[Serializable]
public abstract class EntityState : IState
{
    protected float _timer;

    public virtual void Enter() { }
    public virtual void Exit() { }
    public abstract void Update();

    public override string ToString()
    {
        return $"{GetType().Name}";
    }
}
