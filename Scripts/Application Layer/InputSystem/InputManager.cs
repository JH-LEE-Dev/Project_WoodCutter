using Godot;

/// <summary>
/// 플레이어의 입력을 관리하는 매니저 클래스입니다.
/// </summary>
public partial class InputManager : Node
{
    // 외부 의존성 (Input Action 이름 캐싱 - 성능 최적화)
    private static readonly StringName moveLeft = "move_left";
    private static readonly StringName moveRight = "move_right";
    private static readonly StringName moveUp = "move_up";
    private static readonly StringName moveDown = "move_down";

    // 내부 의존성
    private Vector2 inputVector = Vector2.Zero;
    private bool isJumpJustPressed = false;

    /// <summary>
    /// 현재 입력된 이동 벡터를 반환합니다.
    /// </summary>
    public Vector2 GetInputVector()
    {
        return inputVector;
    }

    /// <summary>
    /// 점프 키가 방금 눌렸는지 여부를 반환합니다.
    /// </summary>
    public bool GetIsJumpJustPressed()
    {
        return isJumpJustPressed;
    }

    // Godot 가상 함수
    public override void _Ready()
    {
        // 초기화 로직
    }

    public override void _Process(double _delta)
    {
        // 이동 벡터 계산 (상하좌우 입력 조합)
        inputVector = Input.GetVector(moveLeft, moveRight, moveUp, moveDown);
    }
}