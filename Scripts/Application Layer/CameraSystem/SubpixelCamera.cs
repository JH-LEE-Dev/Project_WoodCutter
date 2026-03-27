using Godot;
using System;
using System.Diagnostics;

public partial class SubpixelCamera : Camera2D
{
    private Character character;

    private Vector2 camPos;
    private Vector2 subpixelPos;

    [Export]
    public float CameraSmoothingSpeed = 2.0f;

    public void Initialize(Character _character)
    {
        character = _character;

        if (character != null)
        {
            GlobalPosition = character.GlobalPosition;
            camPos = GlobalPosition;
        }
    }

    public override void _Ready()
    {

    }

    public override void _PhysicsProcess(double delta)
    {
        if (character == null)
            return;

        Vector2 targetPosition = character.GlobalPosition;
        float weight = (float)(CameraSmoothingSpeed * delta);

        camPos = camPos.Lerp(targetPosition, weight);

        // 렌더링될 정수 위치와 실제 수학적 위치의 차이를 구합니다.
        subpixelPos = camPos.Round() - camPos;

        // 3단계 위의 부모 노드(SubViewportContainer 예상)를 가져옵니다.
        Node parentLevel3 = GetParent().GetParent().GetParent();

        if (parentLevel3 is CanvasItem parentCanvasItem)
        {
            if (parentCanvasItem.Material is ShaderMaterial shaderMaterial)
            {
                // 셰이더로 서브픽셀 오프셋을 전달하여 화면을 미세하게 보정합니다.
                shaderMaterial.SetShaderParameter("cam_offset", subpixelPos);
            }
        }

        // 카메라 노드 자체는 픽셀 퍼펙트를 위해 완벽한 정수(Round) 위치에 고정합니다.
        GlobalPosition = camPos.Round();
    }
}
