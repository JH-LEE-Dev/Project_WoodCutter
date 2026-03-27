using Godot;
using System;

public partial class CameraManager : Node
{
    private SignalHub signalHub;
    private IBootstrapProvider bootstrapProvider;

    private SubpixelCamera camera;
    [Export] PackedScene cameraPrefab;

    public void Initialize(SignalHub _signalHub, IBootstrapProvider _bootstrapProvider)
    {
        signalHub = _signalHub;
        bootstrapProvider = _bootstrapProvider;

        SubscribeSignals();

        camera = cameraPrefab.Instantiate<SubpixelCamera>();
        bootstrapProvider.GetTargetSceneNode().AddChild(camera);
    }

    public override void _ExitTree()
    {
        UnSubscribeSignals();
    }


    private void SubscribeSignals()
    {
        signalHub.Subscribe<CharacterSpawnedSignal>(CharacterSpawned);
    }

    private void UnSubscribeSignals()
    {
        signalHub.UnSubscribe<CharacterSpawnedSignal>(CharacterSpawned);
    }

    private void CharacterSpawned(CharacterSpawnedSignal _characterSpawnedSignal)
    {
        if (camera != null)
            camera.Initialize(_characterSpawnedSignal.character);
    }
}
