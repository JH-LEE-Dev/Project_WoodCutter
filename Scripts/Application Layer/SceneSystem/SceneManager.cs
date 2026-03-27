using System.Threading.Tasks;
using Godot;

/// <summary>
/// 프로젝트에서 사용되는 씬의 종류를 정의합니다.
/// </summary>
public enum SceneType
{
    None,
    Main,
    MainMenu,
    Hub,
    Dungeon
}

/// <summary>
/// 게임 내 씬 전환을 관리하는 매니저 클래스입니다.
/// </summary>
public partial class SceneManager : Node
{
    private Node currentScene;
    private MainScene mainScene;

    // 외부 의존성 (씬 경로 정의 - StringName을 사용하여 성능 최적화)
    public readonly StringName mainScenePath = "res://Scenes/MainScene.tscn";
    public readonly StringName mainMenuPath = "res://Scenes/MainMenuScene.tscn";
    public readonly StringName hubPath = "res://Scenes/HubScene.tscn";
    public readonly StringName dungeonPath = "res://Scenes/DungeonScene.tscn";

    public readonly StringName mainMenuSceneName = "MainMenuScene";
    public readonly StringName hubSceneName = "HubScene";
    public readonly StringName dungeonSceneName = "DungeonScene";
    public readonly StringName mainSceneName = "MainScene";

    public async Task ChangeScene(SceneType sceneType)
    {
        StringName targetScenePath = GetScenePath(sceneType);

        if (string.IsNullOrEmpty(targetScenePath))
        {
            GD.PrintErr($"[SceneManager] 유효하지 않은 씬 타입입니다: {sceneType}");
            return;
        }

        Node currentScene = GetTree().CurrentScene;
        GD.Print($"[SceneManager] 현재 씬 : {currentScene.Name}");

        // 1. 씬 전환 요청을 보냅니다.
        Error errorResult = GetTree().ChangeSceneToFile(targetScenePath);

        if (errorResult != Error.Ok)
        {
            GD.PrintErr($"[SceneManager] 씬 전환 실패: {targetScenePath}, 에러코드: {errorResult}");
            return;
        }

        // 2. [해결책] 엔진의 신호에 의존하지 않고, C# 자체적인 비동기 대기를 사용합니다.
        // 100ms 정도 아주 짧게 쉬면서 고도가 씬을 갈아치울 시간을 줍니다.
        await Task.Delay(100);

        // 3. 이제 GetTree().CurrentScene은 새롭게 로드된 씬을 가리킵니다.
        Node freshlyLoadedScene = GetTree().CurrentScene;
        GD.Print($"[SceneManager] 씬 전환 완료: {freshlyLoadedScene.Name}");
    }

    /// <summary>
    /// 씬 타입에 해당하는 리소스 경로를 반환합니다.
    /// </summary>
    private StringName GetScenePath(SceneType _sceneType)
    {
        return _sceneType switch
        {
            SceneType.MainMenu => mainMenuPath,
            SceneType.Hub => hubPath,
            SceneType.Dungeon => dungeonPath,
            SceneType.Main => mainScenePath,
            _ => null
        };
    }

    // Godot 가상 함수
    public override void _Ready()
    {
        // 초기화 시 필요한 로직이 있다면 여기에 작성합니다.
    }

    public void SetCurrentScene()
    {
        currentScene = GetTree().CurrentScene;

        if (currentScene is MainScene _mainScene)
        {
            mainScene = _mainScene;
        }
    }

    public Node GetTargetScene()
    {
        if (mainScene == null)
            return null;

        return mainScene.GetTargetScene();
    }
}