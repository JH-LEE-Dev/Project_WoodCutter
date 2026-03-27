using Godot;
using System;

public partial class UIView_MainMenu : UIView
{
    public event Action PlayButtonClickedEvent;

    public override void Initialize(UIViewContext _ctx)
    {
        base.Initialize(_ctx);

    }

    public override void _ExitTree()
    {
        PlayButtonClickedEvent = null;
    }

    public override void ShowUI()
    {
        base.ShowUI();
    }

    public override void HideUI()
    {
        base.HideUI();
    }

    public void OnGameStartButton()
    {
        HideUI();
        PlayButtonClickedEvent?.Invoke();
    }
}
