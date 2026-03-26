using Godot;

/// <summary>
/// 프로젝트에서 사용되는 씬의 종류를 정의합니다.
/// </summary>
public enum SceneType
{
    None,
    MainMenu,
    Hub,
    Dungeon
}

/// <summary>
/// 게임 내 씬 전환을 관리하는 매니저 클래스입니다.
/// </summary>
public partial class SceneManager : Node
{
    // 외부 의존성 (씬 경로 정의 - StringName을 사용하여 성능 최적화)
    public readonly StringName mainMenuPath = "res://Scenes/MainMenuScene.tscn";
    public readonly StringName hubPath = "res://Scenes/HubScene.tscn";
    public readonly StringName dungeonPath = "res://Scenes/DungeonScene.tscn";

    public readonly StringName mainMenuSceneName = "MainMenuScene";
    public readonly StringName hubSceneName = "HubScene";
    public readonly StringName dungeonSceneName = "DungeonScene";

    /// <summary>
    /// 지정된 씬 타입으로 화면을 전환합니다.
    /// </summary>
    /// <param name="_sceneType">전환할 씬의 타입</param>
    public void ChangeScene(SceneType _sceneType)
    {
        StringName _targetPath = GetScenePath(_sceneType);

        if (_targetPath == null)
        {
            GD.PrintErr($"[SceneManager] 유효하지 않은 씬 타입입니다: {_sceneType}");
            return;
        }
        // 씬 전환 시도 후
        GD.Print($"현재 활성화된 씬: {GetTree().CurrentScene.Name}");

        // Godot SceneTree를 통해 씬 전환 실행
        Error _error = GetTree().ChangeSceneToFile(_targetPath);

        if (_error != Error.Ok)
        {
            GD.PrintErr($"[SceneManager] 씬 전환 실패: {_targetPath}, 에러코드: {_error}");
        }
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
            _ => null
        };
    }

    // Godot 가상 함수
    public override void _Ready()
    {
        // 초기화 시 필요한 로직이 있다면 여기에 작성합니다.
    }
}