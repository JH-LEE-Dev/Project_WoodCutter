using Godot;
using System;

public partial class Bootstrap : Node
{
    // 유니티의 [SerializeField]
    [Export]
    public float MoveSpeed { get; set; } = 300.0f;

    // 유니티의 Start()
    public override void _Ready()
    {
        GD.Print("연결 성공! 이제 게임을 만들어봅시다.");
    }

    // 유니티의 Update()
    public override void _Process(double delta)
    {
        // 여기에 로직 작성
    }
}
