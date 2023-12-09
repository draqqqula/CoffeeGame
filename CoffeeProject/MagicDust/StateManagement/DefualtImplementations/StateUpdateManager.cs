using MagicDustLibrary.Logic;
using MagicDustLibrary.Organization.Services;

namespace MagicDustLibrary.Organization.DefualtImplementations
{
    public class StateUpdateManager : ComponentHandler<IUpdateComponent>, IUpdateService
    {
        private readonly List<IUpdateComponent> Updateables = new List<IUpdateComponent>();

        public bool RunOnPause => false;

        public override void Hook(IUpdateComponent component)
        {
            Updateables.Add(component);
        }

        public override void Unhook(IUpdateComponent component)
        {
            Updateables.Remove(component);
        }

        public void Update(IControllerProvider state, TimeSpan deltaTime)
        {
            var collection = Updateables.ToArray();
            foreach (var updateable in collection)
            {
                updateable.Update(state, deltaTime);
            }
        }
    }
}