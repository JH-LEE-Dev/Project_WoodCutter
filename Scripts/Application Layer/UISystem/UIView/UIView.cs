using Godot;
using System;
using System.Diagnostics;

public partial class UIView : Control
{
    // 외부 의존성
    protected UIViewContext viewCtx;

    // 내부 의존성 (인스펙터 할당)
    [Export] protected CanvasLayer canvasLayer;

    // 내부 상태
    [Export] private bool startHidden = true;
    private bool bVisible;
    private bool bWorld = false;

    public virtual void Initialize(UIViewContext _ctx)
    {
        viewCtx = _ctx;
        SetupUI();
    }

    public virtual void SetupUI()
    {
    }

    public virtual void ShowUI()
    {
        if (bVisible)
        {
            return;
        }

        if (canvasLayer != null)
        {
            canvasLayer.Visible = true;
        }

        bVisible = true;
    }

    public virtual void HideUI()
    {
        if (!bVisible)
        {
            return;
        }

        if (canvasLayer != null)
        {
            canvasLayer.Visible = false;
        }

        bVisible = false;
    }

    public override void _Ready()
    {
        if (startHidden)
        {
            Visible = false;
            bVisible = false;

            if (canvasLayer != null)
            {
                canvasLayer.Visible = false;
            }
        }
        else
        {
            bVisible = Visible;
            
            if (canvasLayer != null)
            {
                canvasLayer.Visible = Visible;
            }
        }
    }

    public override void _ExitTree()
    {
    }

    public override void _Process(double _delta)
    {
    }
}
