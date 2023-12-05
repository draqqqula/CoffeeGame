using MagicDustLibrary.CommonObjectTypes;
using MagicDustLibrary.ComponentModel;
using MagicDustLibrary.Display;
using MagicDustLibrary.Organization.BaseServices;

namespace MagicDustLibrary.Logic
{
    /// <summary>
    /// Реализует добавочный функционал для объектов,<br/> который можно добавить или убрать прямо во время работы программы.
    /// <list type="bullet">
    /// <item><typeparamref name="T"/> обозначает тип объекта. <see cref="Behavior{T}"/> сможет применяться<br/>
    /// только к объектам типа <typeparamref name="T"/> или унаследованным от него</item>
    /// <item>Например, если <typeparamref name="T"/> будет <see cref="GameObject"/>, то такое поведение<br/>
    /// сможет применяться к любым объектам.</item>
    /// <item>Если <typeparamref name="T"/> будет <see cref="Sprite"/> то поведение сможет применяться к <see cref="Sprite"/>,<br/> но не сможет применяться к <see cref="TileMap"/></item>
    /// </list>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Behavior<T> : GameObjectComponentBase where T : class, IMultiBehaviorComponent
    {
        protected abstract void Act(IStateController state, TimeSpan deltaTime, T parent);

        [ContactComponent]
        private void GreetMultiBehavior(T parent)
        {
            parent.OnAct += Update;
        }

        private void Update(IStateController state, TimeSpan deltaTime, IMultiBehaviorComponent parent)
        {
            Act(state, deltaTime, parent as T);
        }
    }

    public interface IDisplayFilter : IComponent
    {
        public DrawingParameters ApplyFilter(DrawingParameters info);
    }
}
