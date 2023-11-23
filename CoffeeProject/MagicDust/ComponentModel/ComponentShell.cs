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

        public ComponentShell CombineWith<T>(T component) where T : ComponentBase
        {
            if (_component is null)
            {
                _component = component;
                return this;
            }
            _component = _component.CombineWith(component);
            return this;
        }
    }
}
