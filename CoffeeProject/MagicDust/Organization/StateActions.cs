﻿using MagicDustLibrary.Factorys;
using MagicDustLibrary.Logic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MagicDustLibrary.Organization
{
    public partial class GameState
    {
        class StateActions : IStateController
        {
            private readonly GameState _state;

            #region COMMON
            public T CreateObject<T, L>(Vector2 position) where T : GameObject where L : Layer
            {
                var obj =  _state.StateServices.GetService<IGameObjectFactory>().CreateObject<T, L>(position);
                _state.Hook(obj);
                return obj;
            }

            public void GetFamily<F>() where F : class, IFamily
            {
                _state.StateServices.GetService<StateFamilyManager>().GetFamily<F>();
            }
            #endregion


            #region LAYER PLACEMENT
            public void PlaceAbove(GameObject target, GameObject source)
            {
                _state.StateServices.GetService<StateLayerManager>().GetLayer(target.Placement.GetLayerType()).Remove(target);
                _state.StateServices.GetService<StateLayerManager>().GetLayer(source.Placement.GetLayerType()).PlaceAbove(target, source);
            }

            public void PlaceBelow(GameObject target, GameObject source)
            {
                _state.StateServices.GetService<StateLayerManager>().GetLayer(target.Placement.GetLayerType()).Remove(target);
                _state.StateServices.GetService<StateLayerManager>().GetLayer(source.Placement.GetLayerType()).PlaceBelow(target, source);
            }

            public void PlaceBottom(GameObject target)
            {
                _state.StateServices.GetService<StateLayerManager>().GetLayer(target.Placement.GetLayerType()).PlaceBottom(target);
            }

            public void PlaceTop(GameObject target)
            {
                _state.StateServices.GetService<StateLayerManager>().GetLayer(target.Placement.GetLayerType()).PlaceTop(target);
            }
            public void PlaceTo<L>(GameObject target) where L : Layer
            {
                _state.StateServices.GetService<StateLayerManager>().GetLayer(target.Placement.GetLayerType()).Remove(target);
                _state.StateServices.GetService<StateLayerManager>().GetLayer<L>().PlaceTop(target);
            }
            #endregion


            #region NETWORK
            public void OpenServer(int port)
            {
                _state.StateServices.GetService<StateConnectionRecieveManager>().StartServer(port);
            }
            #endregion


            #region LEVEL MANAGEMENT
            public void LaunchLevel(string name, bool keepState)
            {
                _state.StateServices.GetService<StateLevelManager>().ApplicationLevelManager.Launch(name, keepState);
            }

            public void LaunchLevel(string name, LevelArgs arguments, bool keepState)
            {
                _state.StateServices.GetService<StateLevelManager>().ApplicationLevelManager.Launch(name, arguments, keepState);
            }

            public string GetCurrentLevelName()
            {
                return _state.StateServices.GetService<StateLevelManager>().LevelName;
            }

            public void ResumeLevel(string name)
            {
                _state.StateServices.GetService<StateLevelManager>().ApplicationLevelManager.Resume(name);
            }

            public void PauseLevel(string name)
            {
                _state.StateServices.GetService<StateLevelManager>().ApplicationLevelManager.Pause(name);
            }

            public void PauseCurrent()
            {
                _state.StateServices.GetService<StateLevelManager>().PauseCurrent();
            }

            public void RestartCurrent()
            {
                _state.StateServices.GetService<StateLevelManager>().RestartCurrent();
            }

            public void RestartCurrent(LevelArgs arguments)
            {
                _state.StateServices.GetService<StateLevelManager>().RestartCurrent(arguments);
            }

            public void RestartLevel(string name)
            {
                _state.StateServices.GetService<StateLevelManager>().ApplicationLevelManager.Restart(name);
            }

            public void RestartLevel(string name, LevelArgs arguments)
            {
                _state.StateServices.GetService<StateLevelManager>().ApplicationLevelManager.Restart(name, arguments);
            }

            public void ShutLevel(string name, bool keepState)
            {
                _state.StateServices.GetService<StateLevelManager>().ApplicationLevelManager.Shut(name, keepState);
            }

            public void ShutCurrent(bool keepState)
            {
                _state.StateServices.GetService<StateLevelManager>().ShutCurrent(keepState);
            }
            #endregion


            #region SOUND PLAYER
            public SoundEffectInstance? CreateSoundInstance(string fileName, string tag)
            {
                return _state.StateServices.GetService<StateSoundManager>().CreateInstance(fileName, tag);
            }

            public SoundEffectInstance? GetSoundInstance(string tag)
            {
                return _state.StateServices.GetService<StateSoundManager>().GetInstance(tag);
            }
            #endregion

            public StateActions(GameState state)
            {
                _state = state;
            }
        }
    }
}
