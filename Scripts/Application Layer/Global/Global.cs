using Godot;

public static class NodeUtils
{
    // static을 붙여야 어디서든 NodeUtils.FindChildByType<T>(...)로 호출 가능합니다.
    public static T FindChildByType<T>(Node parentNode) where T : Node
    {
        if (parentNode == null) return null;

        foreach (Node child in parentNode.GetChildren())
        {
            if (child is T typedChild)
            {
                return typedChild;
            }

            T result = FindChildByType<T>(child);
            if (result != null)
            {
                return result;
            }
        }
        return null;
    }
}
