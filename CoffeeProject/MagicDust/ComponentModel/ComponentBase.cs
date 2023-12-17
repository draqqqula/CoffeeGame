namespace MagicDustLibrary.ComponentModel
{
    public abstract class ComponentBase : IComponent
    {
        private IActionContainer Greetings { get; init; } = new ActionContainer();

        public virtual ComponentBase CombineWith(ComponentBase obj)
        {
            Greet(obj);
            obj.Greet(this);
            return new CompositeComponent(this, obj);
        }

        public virtual IEnumerable<T> GetComponents<T>()
        {
            if (this is T t)
            {
                yield return t;
            }
        }

        public virtual ComponentBase? Without<T>()
        {
            if (this is T)
            {
                return null;
            }
            return this;
        }

        public IEnumerable<ComponentBase> Decomposed => GetComponents<ComponentBase>();

        protected void AddGreetingFor<T>(Action<T> greetingAction) where T : IComponent
        {
            Greetings.Register(greetingAction);
        }

        internal void Greet(ComponentBase obj)
        {
            foreach (var component in obj.Decomposed)
            {
                Greetings.InvokeFor(component);
            }
        }
    }
}
