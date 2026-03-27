using Godot;
using System;

public partial class GameInstaller : Node
{
    private SignalHub signalHub;
    private InputManager inputManager;
    private IBootstrapProvider bootstrapProvider;
    private UnitSpawner unitSpawner;
    private CameraManager cameraManager;
    private UnitSystem unitSystem;

    public void Initialize(IBootstrapProvider _bootstrapProvider, InputManager _inputManager)
    {
        signalHub = new SignalHub();

        bootstrapProvider = _bootstrapProvider;
        inputManager = _inputManager;

        unitSpawner = NodeUtils.FindChildByType<UnitSpawner>(this);
        unitSpawner.Initialize(inputManager,bootstrapProvider);

        cameraManager = NodeUtils.FindChildByType<CameraManager>(this);
        cameraManager.Initialize(signalHub,bootstrapProvider);


        unitSystem = new UnitSystem();
        unitSystem.Initialize(signalHub,unitSpawner);
    }

    public void Release()
    {
        unitSystem.Release();
    }

    public void StartGameplayScene()
    {
        unitSpawner.SpawnCharacter();
    }
}
