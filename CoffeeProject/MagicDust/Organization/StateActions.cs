using MagicDustLibrary.Logic;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Organization
{
    public partial class GameState
    {
        class StateActions : IStateController
        {
            private readonly GameState _state;
            public T CreateObject<T, L>(Vector2 position) where T : GameObject where L : Layer
            {
                var obj =  _state._gameObjectFactory.CreateObject<T, L>(position);
                _state.Hook(obj);
                return obj;
            }

            public void GetFamily<F>() where F : class, IFamily
            {
                _state._stateFamilyManager.GetFamily<F>();
            }

            public void PlaceAbove(GameObject target, GameObject source)
            {
                _state._stateLayerManager.GetLayer(target.Placement.GetLayerType()).Remove(target);
                _state._stateLayerManager.GetLayer(source.Placement.GetLayerType()).PlaceAbove(target, source);
            }

            public void PlaceBelow(GameObject target, GameObject source)
            {
                _state._stateLayerManager.GetLayer(target.Placement.GetLayerType()).Remove(target);
                _state._stateLayerManager.GetLayer(source.Placement.GetLayerType()).PlaceBelow(target, source);
            }

            public void PlaceBottom(GameObject target)
            {
                _state._stateLayerManager.GetLayer(target.Placement.GetLayerType()).PlaceBottom(target);
            }

            public void PlaceTop(GameObject target)
            {
                _state._stateLayerManager.GetLayer(target.Placement.GetLayerType()).PlaceTop(target);
            }
            public void PlaceTo<L>(GameObject target) where L : Layer
            {
                _state._stateLayerManager.GetLayer(target.Placement.GetLayerType()).Remove(target);
                _state._stateLayerManager.GetLayer<L>().PlaceTop(target);
            }

            public void OpenServer(int port)
            {
                _state._stateConnectionManager.StartServer(port);
            }

            public StateActions(GameState state)
            {
                _state = state;
            }
        }
    }
}
