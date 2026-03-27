using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;

public partial class UIManager : Node
{
    // 외부 의존성
    protected UIViewContext viewContext;

    // 내부 상태
    [Export] private Array<PackedScene> viewPrefabs = new Godot.Collections.Array<PackedScene>();
    private System.Collections.Generic.Dictionary<Type, PackedScene> prefabByType = new System.Collections.Generic.Dictionary<Type, PackedScene>();
    private System.Collections.Generic.Dictionary<Type, UIView> instanceByType = new System.Collections.Generic.Dictionary<Type, UIView>();

    public void Initialize(InputManager _inputManager)
    {
        viewContext = new UIViewContext();
        viewContext.Initialize(_inputManager);
    }

    public void SceneChanged()
    {
        CloseAll();
    }

    public T Open<T>() where T : UIView
    {
        Type type = typeof(T);

        if (!instanceByType.TryGetValue(type, out UIView instance) || instance == null)
        {
            instance = CreateViewInstance<T>();
            if (instance != null)
            {
                instanceByType[type] = instance;
            }
        }

        if (instance != null)
        {
            instance.ShowUI();
        }

        return instance as T;
    }

    public void Close<T>() where T : UIView
    {
        Type type = typeof(T);

        if (instanceByType.TryGetValue(type, out UIView instance) && instance != null)
        {
            instance.Hide();
        }
    }

    public void CloseAll()
    {
        foreach (var kv in instanceByType)
        {
            UIView view = kv.Value;
            if (view != null)
            {
                view.Hide();
            }
        }
    }

    public T GetView<T>() where T : UIView
    {
        Type type = typeof(T);
        if (instanceByType.TryGetValue(type, out UIView instance))
        {
            return instance as T;
        }
        return null;
    }

    public void ReleaseDependency()
    {
        if (viewContext != null)
        {
            viewContext.ReleaseDependency();
        }
    }

    protected virtual void DataInjection(UIView _view)
    {
    }

    private T CreateViewInstance<T>() where T : UIView
    {
        Type type = typeof(T);

        if (!prefabByType.TryGetValue(type, out PackedScene prefabScene) || prefabScene == null)
        {
            GD.PrintErr($"[UIManager] Scene prefab not found for type: {type}");
            return null;
        }

        // PackedScene에서 인스턴스를 생성합니다. (유니티의 Instantiate(prefab)와 동일)
        T instance = prefabScene.Instantiate<T>();
        AddChild(instance);

        instance.Initialize(viewContext);
        DataInjection(instance);

        return instance;
    }

    public override void _Ready()
    {
        foreach (PackedScene scene in viewPrefabs)
        {
            if (scene == null)
            {
                continue;
            }

            // 타입을 확인하기 위해 임시로 인스턴스를 생성합니다.
            Node tempInstance = scene.Instantiate();
            if (tempInstance is UIView view)
            {
                Type type = view.GetType();
                if (!prefabByType.ContainsKey(type))
                {
                    prefabByType.Add(type, scene);
                }
            }

            // 타입 확인 후 임시 인스턴스는 즉시 해제합니다.
            tempInstance.Free();
        }
    }
}
