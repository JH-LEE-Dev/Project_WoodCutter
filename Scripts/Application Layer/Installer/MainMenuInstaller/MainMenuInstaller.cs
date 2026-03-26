using Godot;
using System;

public partial class MainMenuInstaller : Node
{
    // 외부 의존성
    [Export] private MainMenuUIInstaller uiInstaller;

    // 내부 의존성
    private InputManager inputManager;
    private IBootstrapProvider bootstrapProvider;

    public void Initialize(IBootstrapProvider _bootstrapProvider, InputManager _inputManager)
    {
        bootstrapProvider = _bootstrapProvider;
        inputManager = _inputManager;

        // 에디터에서 할당되지 않았다면 자식 노드에서 검색하여 캐싱합니다.
        if (uiInstaller == null)
        {
            uiInstaller = NodeUtils.FindChildByType<MainMenuUIInstaller>(this);
        }

        if (uiInstaller != null)
        {
            uiInstaller.Initialize(bootstrapProvider, inputManager);
        }
        else
        {
            GD.PrintErr("[MainMenuInstaller] MainMenuUIInstaller를 자식 노드에서 찾을 수 없습니다.");
        }
    }

    public void Release()
    {
        if (uiInstaller != null)
        {
            uiInstaller.Release();
        }
    }

    public void StartMainMenuScene()
    {
        if (uiInstaller != null)
        {
            uiInstaller.MainMenuLevelStarted();
        }
    }

    public override void _Ready()
    {
    }

    public override void _Process(double _delta)
    {
    }
}
