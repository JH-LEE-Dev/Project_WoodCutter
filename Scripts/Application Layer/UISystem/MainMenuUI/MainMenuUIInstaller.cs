using Godot;
using System;

public partial class MainMenuUIInstaller : Node
{
    private IBootstrapProvider bootStrapProvider;
    private InputManager inputManager;

    private MainMenuUIManager uiManager;

    public void Initialize(IBootstrapProvider _bootStrapProvider, InputManager _inputManager)
    {
        bootStrapProvider = _bootStrapProvider;
        inputManager = _inputManager;

        uiManager = NodeUtils.FindChildByType<MainMenuUIManager>(this);

        uiManager.Initialize(inputManager);
    }

    public void Release()
    {
        ReleaseEvent();
    }

    public void MainMenuLevelStarted()
    {
        uiManager.SceneChanged();

        OpenUIView();
        SetupCanvasChilds();
    }

    private void SetupCanvasChilds()
    {

    }

    private void OpenUIView()
    {
        UIView_MainMenu mainMenuUIView = uiManager.Open<UIView_MainMenu>();

        BindEvent();
    }

    private void BindEvent()
    {
        UIView_MainMenu mainMenuUIView = uiManager.GetView<UIView_MainMenu>();

        if (mainMenuUIView != null)
        {
            mainMenuUIView.PlayButtonClickedEvent -= bootStrapProvider.GoToHubScene;
            mainMenuUIView.PlayButtonClickedEvent += bootStrapProvider.GoToHubScene;
        }
    }

    public void ReleaseEvent()
    {
        UIView_MainMenu mainMenuUIView = uiManager.Open<UIView_MainMenu>();

        mainMenuUIView.PlayButtonClickedEvent -= bootStrapProvider.GoToHubScene;
    }
}
