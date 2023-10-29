using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Organization
{
    public class StateLevelManager
    {
        public readonly ApplicationLevelManager ApplicationLevelManager;
        private readonly string _levelName;

        public StateLevelManager(ApplicationLevelManager applicationLevelManager, string levelName)
        {
            ApplicationLevelManager = applicationLevelManager;
            _levelName = levelName;
        }

        public void PauseCurrent()
        {
            ApplicationLevelManager.Pause(_levelName);
        }

        public void ResumeCurrent()
        {
            ApplicationLevelManager.Resume(_levelName);
        }

        public void ShutCurrent(bool keepState)
        {
            ApplicationLevelManager.Shut(_levelName, keepState);
        }

        public void RestartCurrent()
        {
            ApplicationLevelManager.Restart(_levelName);
        }

        public void RestartCurrent(LevelArgs arguments)
        {
            ApplicationLevelManager.Restart(_levelName, arguments);
        }
    }
}
