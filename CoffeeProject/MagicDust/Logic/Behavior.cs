using MagicDustLibrary.CommonObjectTypes;
using MagicDustLibrary.Display;

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
    public abstract class Behavior<T> : IBehavior where T : IMultiBehaviorComponent
    {
        public bool Enabled { get; set; } = true;
        public DrawingParameters ChangeAppearanceUnhandled(IMultiBehaviorComponent parent, DrawingParameters parameters)
        {
            if (parent is T)
            {
                return ChangeAppearance((T)parent, parameters);
            }
            return parameters;
        }
        /// <summary>
        /// Метод, который будет вызываться в get <see cref="GameObject.DisplayInfo"/>.<br/>
        /// Изменяет <b>настройки отрисовки</b>, которые будут применяться к объекту.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected virtual DrawingParameters ChangeAppearance(T parent, DrawingParameters parameters)
        {
            return parameters;
        }
        public void UpdateUnhandled(IStateController state, TimeSpan deltaTime, IMultiBehaviorComponent parent)
        {
            if (parent is T)
            {
                Update(state, deltaTime, (T)parent);
            }
        }
        /// <summary>
        /// Метод, который будет вызываться <b>перед</b> <see cref="GameObject.Update(IStateController, TimeSpan)"/>.<br/>
        /// Должен реализовывать <b>добавочную логику</b> обновления.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected virtual void Update(IStateController state, TimeSpan deltaTime, T parent)
        {
        }
    }

    public interface IBehavior
    {
        public bool Enabled { get; set; }

        public DrawingParameters ChangeAppearanceUnhandled(IMultiBehaviorComponent parent, DrawingParameters parameters);

        public void UpdateUnhandled(IStateController state, TimeSpan deltaTime, IMultiBehaviorComponent parent);
    }
}
