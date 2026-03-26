using Godot;
using System;

public partial class UnitSpawner : Node
{
    public event Action<Character> characterSpawnedEvent;

    // 외부 의존성
    private InputManager inputManager;

    // 내부 의존성
    [Export] private Node2D characterSpawnPoint;
    [Export] private PackedScene characterPrefab;

    private Character character;

    public void Initialize(InputManager _inputManager)
    {
        this.inputManager = _inputManager;
    }

    public void SpawnCharacter()
    {
        if (characterSpawnPoint == null || characterPrefab == null || character != null)
        {
            return;
        }

        // Godot 방식의 프리팹 인스턴스화
        character = characterPrefab.Instantiate<Character>();
        
        // 현재 노드의 자식으로 추가
        AddChild(character);

        // 위치 설정 및 의존성 주입
        character.GlobalPosition = characterSpawnPoint.GlobalPosition;
        character.Initialize(inputManager);

        characterSpawnedEvent?.Invoke(character);
    }
}
