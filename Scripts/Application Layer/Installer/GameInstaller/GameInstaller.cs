using Godot;
using System;

public partial class GameInstaller : Node
{
    private InputManager inputManager;
    private IBootstrapProvider bootstrapProvider;
    private UnitSpawner unitSpawner;


    public void Initialize(IBootstrapProvider _bootstrapProvider, InputManager _inputManager)
    {
        bootstrapProvider = _bootstrapProvider;
        inputManager = _inputManager;

        unitSpawner = NodeUtils.FindChildByType<UnitSpawner>(this);
        unitSpawner.Initialize(inputManager);
    }
    
    public void StartGameplayScene()
    {
        unitSpawner.SpawnCharacter();
    }
}
