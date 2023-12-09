using MagicDustLibrary.Organization;
using MagicDustLibrary.Organization.DefualtImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Logic.Controllers
{
    public interface ILevelController : IStateController
    {
        public void LaunchLevel(string name, bool keepState);
        public void LaunchLevel(string name, LevelArgs arguments, bool keepState);
        public string GetCurrentLevelName();
        public void ResumeLevel(string name);
        public void PauseLevel(string name);
        public void PauseCurrent();
        public void RestartCurrent();
        public void RestartCurrent(LevelArgs arguments);
        public void RestartLevel(string name);
        public void RestartLevel(string name, LevelArgs arguments);
        public void ShutLevel(string name, bool keepState);
        public void ShutCurrent(bool keepState);
    }
    internal class DefaultLevelController : ILevelController
    {
        private readonly StateLevelManager _levelManager;
        public DefaultLevelController(StateLevelManager levelManager)
        {
            _levelManager = levelManager;
        }
        public void LaunchLevel(string name, bool keepState)
        {
            _levelManager.ApplicationLevelManager.Launch(name, keepState);
        }

        public void LaunchLevel(string name, LevelArgs arguments, bool keepState)
        {
            _levelManager.ApplicationLevelManager.Launch(name, arguments, keepState);
        }

        public string GetCurrentLevelName()
        {
            return _levelManager.LevelName;
        }

        public void ResumeLevel(string name)
        {
            _levelManager.ApplicationLevelManager.Resume(name);
        }

        public void PauseLevel(string name)
        {
            _levelManager.ApplicationLevelManager.Pause(name);
        }

        public void PauseCurrent()
        {
            _levelManager.PauseCurrent();
        }

        public void RestartCurrent()
        {
            _levelManager.RestartCurrent();
        }

        public void RestartCurrent(LevelArgs arguments)
        {
            _levelManager.RestartCurrent(arguments);
        }

        public void RestartLevel(string name)
        {
            _levelManager.ApplicationLevelManager.Restart(name);
        }

        public void RestartLevel(string name, LevelArgs arguments)
        {
            _levelManager.ApplicationLevelManager.Restart(name, arguments);
        }

        public void ShutLevel(string name, bool keepState)
        {
            _levelManager.ApplicationLevelManager.Shut(name, keepState);
        }

        public void ShutCurrent(bool keepState)
        {
            _levelManager.ShutCurrent(keepState);
        }

    }
}
