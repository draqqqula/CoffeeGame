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
        public readonly string LevelName;

        public StateLevelManager(ApplicationLevelManager applicationLevelManager, string levelName)
        {
            ApplicationLevelManager = applicationLevelManager;
            LevelName = levelName;
        }

        public void PauseCurrent()
        {
            ApplicationLevelManager.Pause(LevelName);
        }

        public void ResumeCurrent()
        {
            ApplicationLevelManager.Resume(LevelName);
        }

        public void ShutCurrent(bool keepState)
        {
            ApplicationLevelManager.Shut(LevelName, keepState);
        }

        public void RestartCurrent()
        {
            ApplicationLevelManager.Restart(LevelName);
        }

        public void RestartCurrent(LevelArgs arguments)
        {
            ApplicationLevelManager.Restart(LevelName, arguments);
        }
    }
}
