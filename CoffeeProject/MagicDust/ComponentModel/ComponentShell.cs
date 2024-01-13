namespace MagicDustLibrary.ComponentModel
{
    public class ComponentShell : ComponentBase
    {
        private ComponentBase? _component;

        public override IEnumerable<T> GetComponents<T>()
        {
            if (this is T t)
            {
                yield return t;
            }
            if (_component is null) 
            { 
                yield break; 
            }
            foreach (var component in _component.GetComponents<T>())
            {
                yield return component;
            }
        }

        public override ComponentBase CombineWith(ComponentBase component)
        {
            component.Greet(this);
            this.Greet(component);
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
            if (this is T)
            {
                return _component?.Without<T>() ?? null;
            }
            if (_component is not null)
            {
                _component = _component.Without<T>();
            }
            return this;
        }
    }
}
