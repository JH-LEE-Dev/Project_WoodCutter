using System.Threading.Tasks;
using Godot;

public partial class Bootstrap : Node, IBootstrapProvider
{
    // 외부 의존성 (프리팹/씬 리소스)
    [Export] private PackedScene gameInstallerPrefab;
    [Export] private PackedScene mainMenuInstallerPrefab;

    // 내부 의존성 (매니저 및 인스턴스)
    private SceneManager sceneManager;
    private InputManager inputManager;
    private GameInstaller gameInstaller;
    private MainMenuInstaller mainMenuInstaller;

    // TODO: SceneType enum의 위치에 따라 네임스페이스 확인 필요
    private SceneType currentSceneType = SceneType.MainMenu; // 기본값 예시
    private SceneType prevSceneType = SceneType.MainMenu;

    public async Task GoToMainMenuScene()
    {
        if (sceneManager != null)
            await sceneManager.ChangeScene(SceneType.MainMenu);
        if (mainMenuInstaller != null)
            mainMenuInstaller.StartMainMenuScene();
    }

    public async Task GoToHubScene()
    {
        if (sceneManager != null)
            await sceneManager.ChangeScene(SceneType.Main);

        if (sceneManager != null)
            sceneManager.SetCurrentScene();
        if (gameInstaller != null)
        {
            gameInstaller.Initialize(this, inputManager);
            gameInstaller.StartGameplayScene();
        }
    }

    public void GoToOtherScene(string _sceneName)
    {

    }


    public override void _Ready()
    {
        if (sceneManager == null)
        {
            sceneManager = NodeUtils.FindChildByType<SceneManager>(this);
        }

        if (inputManager == null)
        {
            inputManager = NodeUtils.FindChildByType<InputManager>(this);
        }

        currentSceneType = SceneType.MainMenu;
        SetupGamplayScene();
        SetupMainMenuScene();

        if (mainMenuInstaller != null)
            mainMenuInstaller.StartMainMenuScene();
    }

    public override void _ExitTree()
    {
        ReleaseEvent();
    }

    // 내부 로직
    private void BindEvent()
    {
    }

    private void ReleaseEvent()
    {
    }

    private void SetupMainMenuScene()
    {
        if (mainMenuInstaller == null && mainMenuInstallerPrefab != null)
        {
            mainMenuInstaller = mainMenuInstallerPrefab.Instantiate<MainMenuInstaller>();

            AddChild(mainMenuInstaller);

            mainMenuInstaller.Initialize(this, inputManager);
        }
    }

    private void SetupGamplayScene()
    {
        if (gameInstaller == null && gameInstallerPrefab != null)
        {
            gameInstaller = gameInstallerPrefab.Instantiate<GameInstaller>();

            AddChild(gameInstaller);
        }
    }

    public Node GetTargetSceneNode()
    {
        return sceneManager.GetTargetScene();
    }

}
