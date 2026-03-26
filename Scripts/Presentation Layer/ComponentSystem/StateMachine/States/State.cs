
public abstract class State
{
    protected StateMachine stateMachine;
    protected bool bActivated = false;

    public abstract void Enter();
    public abstract void Exit();
    public abstract void Process(double _delta);
    public abstract void PhysicsProcess(double _delta);

    public void Release() { UnSubscribeEvents(); }

    protected abstract void SubscribeEvents();
    protected abstract void UnSubscribeEvents();
}