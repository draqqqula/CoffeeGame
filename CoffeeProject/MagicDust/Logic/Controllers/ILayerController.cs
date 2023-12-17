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
        public void PlaceAbove(IDisplayComponent target, IDisplayComponent source);
        public void PlaceBelow(IDisplayComponent target, IDisplayComponent source);
        public void PlaceTop(IDisplayComponent target);
        public void PlaceBottom(IDisplayComponent target);
        public void PlaceTo<L>(IDisplayComponent target) where L : Layer;
    }

    internal class DefaultLayerController : ILayerController
    {
        private readonly StateLayerManager _stateLayerManager;
        public DefaultLayerController(StateLayerManager layerManager)
        {
            _stateLayerManager = layerManager;
        }
        public void PlaceAbove(IDisplayComponent target, IDisplayComponent source)
        {
            _stateLayerManager.GetLayer(target).Remove(target);
            _stateLayerManager.GetLayer(target).PlaceAbove(target, source);
        }

        public void PlaceBelow(IDisplayComponent target, IDisplayComponent source)
        {
            _stateLayerManager.GetLayer(target).Remove(target);
            _stateLayerManager.GetLayer(source).PlaceBelow(target, source);
        }

        public void PlaceBottom(IDisplayComponent target)
        {
            _stateLayerManager.GetLayer(target).PlaceBottom(target);
        }

        public void PlaceTop(IDisplayComponent target)
        {
            _stateLayerManager.GetLayer(target).PlaceTop(target);
        }
        public void PlaceTo<L>(IDisplayComponent target) where L : Layer
        {
            _stateLayerManager.GetLayer(target).Remove(target);
            _stateLayerManager.GetLayer<L>().PlaceTop(target);
        }
    }
}
