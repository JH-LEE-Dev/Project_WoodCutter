using Godot;

public class RunState : CharacterState
{
    // 내부 의존성 및 상태 필드
    private Vector2 moveInput;
    private Vector2 lastVisualInput;
    private Vector2 pendingDirection;
    private float directionUpdateTimer;
    private const float graceDuration = 0.05f;

    public override void Enter()
    {
        bActivated = true;
        // character.AnimationPlayer.Play("run"); 
    }

    public override void Exit()
    {
        moveInput = Vector2.Zero;
        directionUpdateTimer = 0f;
        pendingDirection = Vector2.Zero;

        bActivated = false;
    }

    public override void Process(double _delta)
    {
        OnMove();
        HandleDelayedDirectionUpdate((float)_delta);
    }

    public override void PhysicsProcess(double _delta)
    {
        ApplyMovement();
    }

    protected override void SubscribeEvents()
    {
    }

    protected override void UnSubscribeEvents()
    {
    }

    private void OnMove()
    {
        if (bActivated == false)
        {
            return;
        }

        moveInput = character.inputManager.GetInputVector();

        if (moveInput == Vector2.Zero)
        {
            stateMachine.ChangeState<IdleState>();
            return;
        }

        int lastAxisCount = this.GetActiveAxisCount(this.lastVisualInput);
        int currentAxisCount = this.GetActiveAxisCount(moveInput);

        if (currentAxisCount >= lastAxisCount)
        {
            UpdateFacingDirection(moveInput);
        }
        else
        {
            pendingDirection = moveInput;
            directionUpdateTimer = graceDuration;
        }
    }

    private int GetActiveAxisCount(Vector2 _v)
    {
        int count = 0;
        if (Mathf.Abs(_v.X) > 0.01f) count++;
        if (Mathf.Abs(_v.Y) > 0.01f) count++;
        return count;
    }

    private void UpdateFacingDirection(Vector2 _input)
    {
        character.SetFacingDirection(this.GetIsometricVector(_input));
        lastVisualInput = _input;
        directionUpdateTimer = 0f;
    }

    private void HandleDelayedDirectionUpdate(float _delta)
    {
        if (directionUpdateTimer > 0)
        {
            directionUpdateTimer -= _delta;
            if (directionUpdateTimer <= 0)
            {
                UpdateFacingDirection(pendingDirection);
            }
        }
    }

    private void ApplyMovement()
    {
        Vector2 isoDir = GetIsometricVector(moveInput);

        if (isoDir.LengthSquared() > 0.001f)
        {
            // 1. 방향부터 정규화 (순수한 방향 추출)
            Vector2 rawDir = moveInput.Normalized();

            // 2. 그 후에 이소메트릭 비율 적용
            isoDir = GetIsometricVector(rawDir);

            // 3. 속도 곱하기
            character.Velocity = isoDir * character.speed;
        }
        else
        {
            // 입력이 없을 때 속도 초기화
            character.Velocity = Vector2.Zero;
        }

        character.MoveAndSlide();
    }

    private Vector2 GetIsometricVector(Vector2 _input)
    {
        return new Vector2(_input.X, _input.Y * 0.5f);
    }
}
