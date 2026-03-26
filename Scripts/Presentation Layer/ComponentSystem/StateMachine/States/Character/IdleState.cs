

public class IdleState : CharacterState
{
    public override void Enter()
    {
        bActivated = true;
        //character.anim.SetBool(character.isMovingHash, false);
    }

    public override void Exit()
    {
        bActivated = false;
    }

    public override void PhysicsProcess(double _delta)
    {
        CheckMove();
    }


    public override void Process(double _delta)
    {

    }

    protected override void SubscribeEvents()
    {

    }

    protected override void UnSubscribeEvents()
    {

    }

    private void CheckMove()
    {
        if (bActivated == false)
            return;

        Godot.Vector2 inputVector = character.inputManager.GetInputVector();

        if (inputVector.LengthSquared() != 0f)
            stateMachine.ChangeState<RunState>();
    }
}
