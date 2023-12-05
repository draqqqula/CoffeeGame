using MagicDustLibrary.Logic;
using MagicDustLibrary.Organization.BaseServices;
using MagicDustLibrary.Organization.StateManagement;

namespace MagicDustLibrary.Organization
{
    public class ComponentUpdateManager : ComponentHandler<IUpdateComponent>
    {
        private readonly List<IUpdateComponent> Updateables = new List<IUpdateComponent>();


        public override void Hook(IUpdateComponent obj)
        {
            Updateables.Add(obj);
        }

        public override void Unhook(IUpdateComponent obj)
        {
            Updateables.Remove(obj);
        }

        public void Update(IStateController state, TimeSpan deltaTime)
        {
            var collection = Updateables.ToArray();
            foreach (var updateable in collection)
            {
                updateable.Update(state, deltaTime);
            }
        }
    }
}