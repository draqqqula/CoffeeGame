using MagicDustLibrary.Organization;
using MagicDustLibrary.Organization.DefualtImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Logic.Controllers
{
    public interface ILayerController : IStateController
    {
        public void PlaceAbove(GameObject target, GameObject source);
        public void PlaceBelow(GameObject target, GameObject source);
        public void PlaceTop(GameObject target);
        public void PlaceBottom(GameObject target);
        public void PlaceTo<L>(GameObject target) where L : Layer;
    }

    internal class DefaultLayerController : ILayerController
    {
        private readonly StateLayerManager _stateLayerManager;
        public DefaultLayerController(StateLayerManager layerManager)
        {
            _stateLayerManager = layerManager;
        }
        public void PlaceAbove(GameObject target, GameObject source)
        {
            _stateLayerManager.GetLayer(target.Placement.GetLayerType()).Remove(target);
            _stateLayerManager.GetLayer(source.Placement.GetLayerType()).PlaceAbove(target, source);
        }

        public void PlaceBelow(GameObject target, GameObject source)
        {
            _stateLayerManager.GetLayer(target.Placement.GetLayerType()).Remove(target);
            _stateLayerManager.GetLayer(source.Placement.GetLayerType()).PlaceBelow(target, source);
        }

        public void PlaceBottom(GameObject target)
        {
            _stateLayerManager.GetLayer(target.Placement.GetLayerType()).PlaceBottom(target);
        }

        public void PlaceTop(GameObject target)
        {
            _stateLayerManager.GetLayer(target.Placement.GetLayerType()).PlaceTop(target);
        }
        public void PlaceTo<L>(GameObject target) where L : Layer
        {
            _stateLayerManager.GetLayer(target.Placement.GetLayerType()).Remove(target);
            _stateLayerManager.GetLayer<L>().PlaceTop(target);
        }
    }
}
