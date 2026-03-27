using Godot;
using System;

public partial class Character : CharacterBody2D
{
    [Export] public float speed = 100;

    //외부 의존성
    public InputManager inputManager { get; private set; }

    //내부 의존성
    public StateMachine stateMachine { get; private set; }

    public void Initialize(InputManager _inputManager)
    {
        inputManager = _inputManager;

        stateMachine = new StateMachine();
        ComponentCtx componentCtx = new ComponentCtx();
        componentCtx.Initialize(inputManager);

        SetupStateMachine();
    }

    public override void _Process(double delta)
    {
        stateMachine?.Process(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        stateMachine?.PhysicsProcess(delta);
    }


    private void SetupStateMachine()
    {
        AddState(new IdleState());
        AddState(new RunState());

        // 초기 상태 설정
        stateMachine.ChangeState<IdleState>();
    }

    private void AddState(CharacterState _state)
    {
        _state.Initialize(stateMachine, this);
        stateMachine.AddState(_state);
    }

    public void SetFacingDirection(Vector2 _input)
    {
        // if (_input.sqrMagnitude < 0.01f) return;

        // // 8방향 인덱스 계산 (0: 우, 1: 우상, 2: 상, 3: 좌상, 4: 좌, 5: 좌하, 6: 하, 7: 우하)
        // float angle = Mathf.Atan2(_input.y, _input.x) * Mathf.Rad2Deg;
        // if (angle < 0) angle += 360;

        // int dirIndex = Mathf.RoundToInt(angle / 45f) % 8;
        // anim.SetFloat(facingDirHash, dirIndex);
    }
}
