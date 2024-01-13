using MagicDustLibrary.Organization;
using MagicDustLibrary.Organization.DefualtImplementations;
using MagicDustLibrary.Organization.StateManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Logic.Controllers
{
    public interface ITimeController : IStateController
    {
        public void SetStateTimeScale(double timeScale);

        public void SetObjectTimeScale(IUpdateComponent obj, double timeScale);
    }

    public class DefaultTimeController : ITimeController
    {
        private readonly GameState1 _state;
        private readonly StateUpdateManager _stateUpdateManager;
        public DefaultTimeController(GameState1 state, StateUpdateManager updateManager)
        {
            _state = state;
            _stateUpdateManager = updateManager;
        }

        public void SetObjectTimeScale(IUpdateComponent obj, double timeScale)
        {
            _stateUpdateManager.TimeScales[obj] = timeScale;
        }

        public void SetStateTimeScale(double timeScale)
        {
            _state.TimeScale = timeScale;
        }
    }
}
