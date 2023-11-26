namespace MagicDustLibrary.ComponentModel
{
    public class CompositeComponent<A, B> : ComponentBase where A : ComponentBase where B : ComponentBase
    {
        private readonly A _componentA;
        private readonly B _componentB;

        internal CompositeComponent(A componentA, B componentB)
        {
            componentA.Greet(componentB);
            componentB.Greet(componentA);
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

        protected internal override void Greet<T>(T obj)
        {
            _componentA.Greet(obj);
            _componentB.Greet(obj);
        }
    }

    public static class CompositeExtensions
    {
        internal static CompositeComponent<A, B> CombineWith<A, B>(this A a, B b) where A : ComponentBase where B : ComponentBase
        {
            return new CompositeComponent<A, B>(a, b);
        }
    }
}
