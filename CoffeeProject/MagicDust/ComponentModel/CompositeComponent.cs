namespace MagicDustLibrary.ComponentModel
{
    public class CompositeComponent : ComponentBase
    {
        private readonly ComponentBase _componentA;
        private readonly ComponentBase _componentB;

        internal CompositeComponent(ComponentBase componentA, ComponentBase componentB)
        {
            _componentA = componentA;
            _componentB = componentB;
        }

        public override IEnumerable<T> GetComponents<T>()
        {
            foreach (var component in _componentA.GetComponents<T>())
            {
                yield return component;
            }
            foreach (var component in _componentB.GetComponents<T>())
            {
                yield return component;
            }
        }

        public override ComponentBase? Without<T>()
        {
            if (_componentA.Without<T>() is null && _componentB.Without<T>() is null)
            {
                return null;
            }
            else if (_componentA.Without<T>() is null)
            {
                return _componentB;
            }
            else if (_componentB.Without<T>() is null)
            {
                return _componentA;
            }
            return this;
        }
    }
}
