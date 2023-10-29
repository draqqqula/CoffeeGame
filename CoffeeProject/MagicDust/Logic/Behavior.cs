using MagicDustLibrary.CommonObjectTypes;
using MagicDustLibrary.Display;

namespace MagicDustLibrary.Logic
{
    public abstract class Behavior<T> : IBehavior where T : GameObject
    {
        public bool Enabled { get; set; } = true;
        public DrawingParameters ChangeAppearanceUnhandled(GameObject parent, DrawingParameters parameters)
        {
            if (parent is T)
            {
                return ChangeAppearance((T)parent, parameters);
            }
            return parameters;
        }
        protected virtual DrawingParameters ChangeAppearance(T parent, DrawingParameters parameters)
        {
            return parameters;
        }
        public void UpdateUnhandled(IStateController state, TimeSpan deltaTime, GameObject parent)
        {
            if (parent is T)
            {
                Update(state, deltaTime, (T)parent);
            }
        }
        protected virtual void Update(IStateController state, TimeSpan deltaTime, T parent)
        {
        }
    }

    public interface IBehavior
    {
        public bool Enabled { get; set; }

        public DrawingParameters ChangeAppearanceUnhandled(GameObject parent, DrawingParameters parameters);

        public void UpdateUnhandled(IStateController state, TimeSpan deltaTime, GameObject parent);
    }
}
