namespace MagicDustLibrary.ComponentModel;

public interface IActionContainer
{
    public void Register<T>(Action<T> action);
    public void InvokeFor<T>(T obj);
}

public class ActionContainer : IActionContainer
{
    private readonly List<IActionContainerNode> _actionNodes = new();

    public void Register<T>(Action<T> action)
    {
        _actionNodes.Add(new ActionContainerNode<T>(action));
    }

    public void InvokeFor<T>(T obj)
    {
        foreach (var actionNode in _actionNodes)
        {
            actionNode.TryInvoke(obj);
        }
    }
}

public class ActionContainerNode<T> : IActionContainerNode
{
    private readonly Action<T> _action;
    public ActionContainerNode(Action<T> action)
    {
        _action = action;
    }

    public void TryInvoke<U>(U a)
    {
        if (a is T t)
        {
            _action(t);
        }
    }
}

public interface IActionContainerNode
{
    public void TryInvoke<U>(U a);
}