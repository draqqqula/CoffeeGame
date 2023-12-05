using System.ComponentModel;

namespace MagicDustLibrary.ComponentModel
{
    public class ComponentShell : ComponentBase
    {
        private ComponentBase? _component;

        public override IEnumerable<T> GetComponents<T>()
        {
            if (_component is null)
            {
                return Enumerable.Empty<T>();
            }
            return _component.GetComponents<T>();
        }

        public override ComponentBase CombineWith(ComponentBase component)
        {
            if (_component is null)
            {
                _component = component;
                return this;
            }
            _component = _component.CombineWith(component);
            return this;
        }

        public override ComponentBase? Without<T>()
        {
            if (_component is not null)
            {
                _component = _component.Without<T>();
            }
            return this;
        }
    }
}
