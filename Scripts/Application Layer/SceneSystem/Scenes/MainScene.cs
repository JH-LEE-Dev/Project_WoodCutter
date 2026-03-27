using Godot;
using System;

public partial class MainScene : Node
{
    [Export] Node targetScene;

    public Node GetTargetScene()
    {
        return targetScene;
    }    
}
