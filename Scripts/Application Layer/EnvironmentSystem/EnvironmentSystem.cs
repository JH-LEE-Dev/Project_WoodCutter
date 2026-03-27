using Godot;
using System;

public partial class EnvironmentSystem : Node
{
    private SignalHub signalHub;
    private IBootstrapProvider bootstrapProvider;

    private TileMapGenerator tileMapGenerator;


    public void Initialize(SignalHub _signalHub, IBootstrapProvider _bootstrapProvider)
    {
        signalHub = _signalHub;
        bootstrapProvider = _bootstrapProvider;

        tileMapGenerator = NodeUtils.FindChildByType<TileMapGenerator>(this);
        tileMapGenerator.InitializeMapData();

        SubscribeSignals();
    }

    public void GenerateMap()
    {
        tileMapGenerator.GenerateMap(bootstrapProvider);
    }

    public void Release()
    {
        UnSubscribeSignals();
    }
    
    private void SubscribeSignals()
    {
        
    }

    private void UnSubscribeSignals()
    {
        
    }
}
