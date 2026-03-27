using Godot;

public class RunState : CharacterState
{
    // 내부 의존성 및 상태 필드
    private Vector2 moveInput;
    private Vector2 lastVisualInput;
    private Vector2 pendingDirection;
    private float directionUpdateTimer;
    private const float graceDuration = 0.05f;

    private Vector2 lastDir = Vector2.Zero;

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
        // 1. 현재 입력의 순수한 방향을 먼저 구합니다.
        Vector2 rawDir = moveInput.Normalized();

        // 🚨 포럼 작성자의 핵심 꼼수 적용: 방향이 바뀌었거나 새로 출발할 때
        if (rawDir != lastDir)
        {
            // 픽셀 격자와 어긋난 소수점 찌꺼기를 날려버리고 정수에 강제로 스냅!
            character.GlobalPosition = character.GlobalPosition.Round();

            // 방향 상태 갱신
            lastDir = rawDir;
        }

        // 2. 입력이 있는지 확인 (기존 로직 유지)
        if (moveInput.LengthSquared() > 0.001f) // isoDir 대신 moveInput으로 체크하는 것이 더 정확합니다.
        {
            // 3. 이소메트릭 비율 적용
            Vector2 isoDir = GetIsometricVector(rawDir);

            // 4. 속도 곱하기
            character.Velocity = isoDir * character.speed;
        }
        else
        {
            // 입력이 없을 때 속도 초기화
            character.Velocity = Vector2.Zero;
        }

        // 5. 물리 엔진 이동 실행
        character.MoveAndSlide();
    }

    private Vector2 GetIsometricVector(Vector2 _input)
    {
        return new Vector2(_input.X, _input.Y * 0.5f);
    }
}
