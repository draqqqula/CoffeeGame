using MagicDustLibrary.ComponentModel;
using MagicDustLibrary.Factorys;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Organization.BaseServices;
using Microsoft.Xna.Framework.Audio;

namespace MagicDustLibrary.Organization
{
    public partial class GameState
    {
        class StateActions : IStateController
        {
            private readonly GameState _state;

            #region COMMON
            public ComponentShell CreateObject<T>() where T : GameObjectComponentBase
            {
                var shell = new ComponentShell();
                var obj =  _state.Services.GetService<IGameObjectFactory>().CreateObject<T>();
                shell.CombineWith(obj);
                return shell;
            }

            public void AddToState<T>(T obj) where T : ComponentBase
            {
                _state.Services.GetService<ComponentHandlerCollection>().Hook(obj);
            }

            public void GetFamily<F>() where F : class, IFamily
            {
                _state.Services.GetService<StateFamilyManager>().GetFamily<F>();
            }
            #endregion


            #region LAYER PLACEMENT
            public void PlaceAbove(GameObject target, GameObject source)
            {
                _state.Services.GetService<StateLayerManager>().GetLayer(target.Placement.GetLayerType()).Remove(target);
                _state.Services.GetService<StateLayerManager>().GetLayer(source.Placement.GetLayerType()).PlaceAbove(target, source);
            }

            public void PlaceBelow(GameObject target, GameObject source)
            {
                _state.Services.GetService<StateLayerManager>().GetLayer(target.Placement.GetLayerType()).Remove(target);
                _state.Services.GetService<StateLayerManager>().GetLayer(source.Placement.GetLayerType()).PlaceBelow(target, source);
            }

            public void PlaceBottom(GameObject target)
            {
                _state.Services.GetService<StateLayerManager>().GetLayer(target.Placement.GetLayerType()).PlaceBottom(target);
            }

            public void PlaceTop(GameObject target)
            {
                _state.Services.GetService<StateLayerManager>().GetLayer(target.Placement.GetLayerType()).PlaceTop(target);
            }
            public void PlaceTo<L>(GameObject target) where L : Layer
            {
                _state.Services.GetService<StateLayerManager>().GetLayer(target.Placement.GetLayerType()).Remove(target);
                _state.Services.GetService<StateLayerManager>().GetLayer<L>().PlaceTop(target);
            }
            #endregion


            #region NETWORK
            public void OpenServer(int port)
            {
                _state.Services.GetService<StateConnectionRecieveManager>().StartServer(port);
            }
            #endregion


            #region LEVEL MANAGEMENT
            public void LaunchLevel(string name, bool keepState)
            {
                _state.Services.GetService<StateLevelManager>().ApplicationLevelManager.Launch(name, keepState);
            }

            public void LaunchLevel(string name, LevelArgs arguments, bool keepState)
            {
                _state.Services.GetService<StateLevelManager>().ApplicationLevelManager.Launch(name, arguments, keepState);
            }

            public string GetCurrentLevelName()
            {
                return _state.Services.GetService<StateLevelManager>().LevelName;
            }

            public void ResumeLevel(string name)
            {
                _state.Services.GetService<StateLevelManager>().ApplicationLevelManager.Resume(name);
            }

            public void PauseLevel(string name)
            {
                _state.Services.GetService<StateLevelManager>().ApplicationLevelManager.Pause(name);
            }

            public void PauseCurrent()
            {
                _state.Services.GetService<StateLevelManager>().PauseCurrent();
            }

            public void RestartCurrent()
            {
                _state.Services.GetService<StateLevelManager>().RestartCurrent();
            }

            public void RestartCurrent(LevelArgs arguments)
            {
                _state.Services.GetService<StateLevelManager>().RestartCurrent(arguments);
            }

            public void RestartLevel(string name)
            {
                _state.Services.GetService<StateLevelManager>().ApplicationLevelManager.Restart(name);
            }

            public void RestartLevel(string name, LevelArgs arguments)
            {
                _state.Services.GetService<StateLevelManager>().ApplicationLevelManager.Restart(name, arguments);
            }

            public void ShutLevel(string name, bool keepState)
            {
                _state.Services.GetService<StateLevelManager>().ApplicationLevelManager.Shut(name, keepState);
            }

            public void ShutCurrent(bool keepState)
            {
                _state.Services.GetService<StateLevelManager>().ShutCurrent(keepState);
            }

            public void TransferClient(GameClient client, string targetLevel)
            {
                var stateLevelManager = _state.Services.GetService<StateLevelManager>();
                var targetLevelState = stateLevelManager.ApplicationLevelManager.GetLevel(targetLevel).GameState;
                targetLevelState.Services.GetService<StateClientManager>().Connect(client);
            }

            public void Disconnect(GameClient client)
            {
                _state.Services.GetService<StateClientManager>().Disconnect(client);
            }
            #endregion


            #region SOUND PLAYER
            public SoundEffectInstance? CreateSoundInstance(string fileName, string tag)
            {
                return _state.Services.GetService<StateSoundManager>().CreateInstance(fileName, tag);
            }

            public SoundEffectInstance? GetSoundInstance(string tag)
            {
                return _state.Services.GetService<StateSoundManager>().GetInstance(tag);
            }
            #endregion

            public StateActions(GameState state)
            {
                _state = state;
            }
        }
    }
}
