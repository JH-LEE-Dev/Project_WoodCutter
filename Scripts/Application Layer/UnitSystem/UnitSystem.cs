using Godot;
using System;

public class UnitSystem
{
    private SignalHub signalHub;
    private UnitSpawner unitSpawner;

    public void Initialize(SignalHub _signalHub, UnitSpawner _unitSpawner)
    {
        signalHub = _signalHub;
        unitSpawner = _unitSpawner;

        BindEvents();
    }

    public void Release()
    {
        ReleaseEvents();
    }

    public void SpawnCharacter()
    {
        unitSpawner.SpawnCharacter();
    }

    private void BindEvents()
    {
        unitSpawner.characterSpawnedEvent -= CharacterSpawned;
        unitSpawner.characterSpawnedEvent += CharacterSpawned;
    }

    private void ReleaseEvents()
    {
        unitSpawner.characterSpawnedEvent -= CharacterSpawned;
    }

    private void CharacterSpawned(Character _character)
    {
        signalHub.Publish(new CharacterSpawnedSignal(_character));
    }
}
