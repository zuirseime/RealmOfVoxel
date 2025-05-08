using System;
using UnityEngine;

[Serializable]
public abstract class EntityState : IState
{
    protected float _timer;

    public virtual void Enter() { }
    public virtual void Exit() { }
    public abstract void Update();

    protected void RefreshTimer(float duration)
    {
        _timer = duration + Time.time;
    }
    protected bool IsTimerFinished()
    {
        return Time.time >= _timer;
    }

    public override string ToString()
    {
        return $"{GetType().Name}";
    }
}
