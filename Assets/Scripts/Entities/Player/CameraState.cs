﻿public abstract class CameraState : IState
{
    protected CameraController _controller;

    protected CameraState(CameraController controller)
    {
        _controller = controller;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public abstract void Update();
}
