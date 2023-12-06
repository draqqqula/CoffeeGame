using MagicDustLibrary.ComponentModel;
using MagicDustLibrary.Factorys;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Organization.StateManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MagicDustLibrary.Organization.StateManagement
{
    class StateActions : IStateController
    {
        private readonly GameState1 _state;

        #region COMMON
        public T CreateObject<T>() where T : ComponentBase
        {
            var obj = _state.GetProvider().GetService<IGameObjectFactory>().CreateObject<T>();
            return obj;
        }

        public void AddToState<T>(T obj) where T : ComponentBase
        {
            _state.Hook(obj);
        }

        public void GetFamily<F>() where F : class, IFamily
        {
            _state.GetProvider().GetService<StateFamilyManager>().GetFamily<F>();
        }
        #endregion


        #region LAYER PLACEMENT
        public void PlaceAbove(GameObject target, GameObject source)
        {
            _state.GetProvider().GetService<StateLayerManager>().GetLayer(target.Placement.GetLayerType()).Remove(target);
            _state.GetProvider().GetService<StateLayerManager>().GetLayer(source.Placement.GetLayerType()).PlaceAbove(target, source);
        }

        public void PlaceBelow(GameObject target, GameObject source)
        {
            _state.GetProvider().GetService<StateLayerManager>().GetLayer(target.Placement.GetLayerType()).Remove(target);
            _state.GetProvider().GetService<StateLayerManager>().GetLayer(source.Placement.GetLayerType()).PlaceBelow(target, source);
        }

        public void PlaceBottom(GameObject target)
        {
            _state.GetProvider().GetService<StateLayerManager>().GetLayer(target.Placement.GetLayerType()).PlaceBottom(target);
        }

        public void PlaceTop(GameObject target)
        {
            _state.GetProvider().GetService<StateLayerManager>().GetLayer(target.Placement.GetLayerType()).PlaceTop(target);
        }
        public void PlaceTo<L>(GameObject target) where L : Layer
        {
            _state.GetProvider().GetService<StateLayerManager>().GetLayer(target.Placement.GetLayerType()).Remove(target);
            _state.GetProvider().GetService<StateLayerManager>().GetLayer<L>().PlaceTop(target);
        }
        #endregion


        #region NETWORK
        public void OpenServer(int port)
        {
            _state.GetProvider().GetService<StateConnectionRecieveManager>().StartServer(port);
        }
        #endregion


        #region LEVEL MANAGEMENT
        public void LaunchLevel(string name, bool keepState)
        {
            _state.GetProvider().GetService<StateLevelManager>().ApplicationLevelManager.Launch(name, keepState);
        }

        public void LaunchLevel(string name, LevelArgs arguments, bool keepState)
        {
            _state.GetProvider().GetService<StateLevelManager>().ApplicationLevelManager.Launch(name, arguments, keepState);
        }

        public string GetCurrentLevelName()
        {
            return _state.GetProvider().GetService<StateLevelManager>().LevelName;
        }

        public void ResumeLevel(string name)
        {
            _state.GetProvider().GetService<StateLevelManager>().ApplicationLevelManager.Resume(name);
        }

        public void PauseLevel(string name)
        {
            _state.GetProvider().GetService<StateLevelManager>().ApplicationLevelManager.Pause(name);
        }

        public void PauseCurrent()
        {
            _state.GetProvider().GetService<StateLevelManager>().PauseCurrent();
        }

        public void RestartCurrent()
        {
            _state.GetProvider().GetService<StateLevelManager>().RestartCurrent();
        }

        public void RestartCurrent(LevelArgs arguments)
        {
            _state.GetProvider().GetService<StateLevelManager>().RestartCurrent(arguments);
        }

        public void RestartLevel(string name)
        {
            _state.GetProvider().GetService<StateLevelManager>().ApplicationLevelManager.Restart(name);
        }

        public void RestartLevel(string name, LevelArgs arguments)
        {
            _state.GetProvider().GetService<StateLevelManager>().ApplicationLevelManager.Restart(name, arguments);
        }

        public void ShutLevel(string name, bool keepState)
        {
            _state.GetProvider().GetService<StateLevelManager>().ApplicationLevelManager.Shut(name, keepState);
        }

        public void ShutCurrent(bool keepState)
        {
            _state.GetProvider().GetService<StateLevelManager>().ShutCurrent(keepState);
        }

        public void TransferClient(GameClient client, string targetLevel)
        {
            var stateLevelManager = _state.GetProvider().GetService<StateLevelManager>();
            var targetLevelState = stateLevelManager.ApplicationLevelManager.GetLevel(targetLevel).GameState;
            targetLevelState.GetProvider().GetService<StateClientManager>().Connect(client);
        }

        public void Disconnect(GameClient client)
        {
            _state.GetProvider().GetService<StateClientManager>().Disconnect(client);
        }
        #endregion


        #region SOUND PLAYER
        public SoundEffectInstance? CreateSoundInstance(string fileName, string tag)
        {
            return _state.GetProvider().GetService<StateSoundManager>().CreateInstance(fileName, tag);
        }

        public SoundEffectInstance? GetSoundInstance(string tag)
        {
            return _state.GetProvider().GetService<StateSoundManager>().GetInstance(tag);
        }
        #endregion

        public StateActions(GameState1 state)
        {
            _state = state;
        }
    }
}