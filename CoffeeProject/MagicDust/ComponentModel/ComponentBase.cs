namespace MagicDustLibrary.ComponentModel
{
    public abstract class ComponentBase : IComponent
    {
        private IActionContainer Greetings { get; init; } = new ActionContainer();

        public virtual IEnumerable<T> GetComponents<T>() where T : IComponent
        {
            if (this is T t)
            {
                yield return t;
            }
        }

        public IEnumerable<ComponentBase> Decomposed => GetComponents<ComponentBase>();

        protected void AddGreetingFor<T>(Action<T> greetingAction) where T : IComponent
        {
            Greetings.Register(greetingAction);
        }

        protected internal virtual void Greet<T>(T obj) where T : ComponentBase
        {
            foreach (var component in obj.Decomposed)
            {
                Greetings.InvokeFor(component);
            }
        }
    }
}
