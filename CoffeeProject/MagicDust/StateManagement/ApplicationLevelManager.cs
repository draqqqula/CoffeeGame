using MagicDustLibrary.Extensions.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Organization
{
    public class ApplicationLevelManager
    {
        #region FIELDS
        private MagicGameApplication _app;
        private readonly Dictionary<string, ILevel> Loaded = new Dictionary<string, ILevel>();
        private readonly RelativeCollection<ILevel> ActiveLevels = new RelativeCollection<ILevel>();
        #endregion


        #region PLACEMENT

        public void PlaceTop(string name)
        {
            ActiveLevels.PlaceTop(Loaded[name]);
        }

        public void PlaceBottom(string name)
        {
            ActiveLevels.PlaceBottom(Loaded[name]);
        }

        public void PlaceAbove(string name, string source)
        {
            ActiveLevels.PlaceAbove(Loaded[name], Loaded[source]);
        }

        public void PlaceBelow(string name, string source)
        {
            ActiveLevels.PlaceBelow(Loaded[name], Loaded[source]);
        }

        #endregion


        #region EXCEPTIONS
        private void ThrowWhenNotActive(string name)
        {
            if (!ActiveLevels.Contains(Loaded[name]))
            {
                throw new Exception($"Level \"{name}\" not loaded");
            }
        }

        private void ThrowWhenNotLoaded(string name)
        {
            if (!Loaded.ContainsKey(name) || Loaded[name] is null)
            {
                throw new Exception($"Level tagged \"{name}\" not found");
            }
        }
        #endregion


        #region CONTROL
        public ILevel GetLevel(string name)
        {
            return Loaded[name];
        }

        /// <summary>
        /// Возобновляет уровень на паузе.
        /// </summary>
        /// <param name="name"></param>
        public void Resume(string name)
        {
            ThrowWhenNotLoaded(name);
            ThrowWhenNotActive(name);
            var level = Loaded[name];
            level.Resume();
        }

        /// <summary>
        /// Ставит уровень на паузу.
        /// </summary>
        /// <param name="name"></param>
        public void Pause(string name)
        {
            ThrowWhenNotLoaded(name);
            ThrowWhenNotActive(name);
            var level = Loaded[name];
            level.Pause();
        }

        /// <summary>
        /// Инициализирует неактивный уровень под именем <paramref name="name"/>, делая его активным.
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="Exception"></exception>
        public void Launch(string name, LevelArgs arguments, bool keepState)
        {
            ThrowWhenNotLoaded(name);
            var lvl = Loaded[name];
            if (ActiveLevels.Contains(lvl))
            {
                throw new Exception($"Level \"{name}\" already launched");
            }

            ActiveLevels.PlaceTop(lvl);

            if (!lvl.HasState() || !keepState)
            {
                lvl.Start(_app, arguments, name);
            }
        }

        /// <summary>
        /// Инициализирует неактивный уровень под именем <paramref name="name"/>, делая его активным.<br/>
        /// <paramref name="keepState"/> - если true, то сохраняет состояние уровня(если то было инициализировано ранее).
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="Exception"></exception>
        public void Launch(string name, bool keepState)
        {
            Launch(name, LevelArgs.Empty, keepState);
        }

        /// <summary>
        /// Делает уровень неактивным.<br/>
        /// <paramref name="keepState"/> если true то не сбрасывает состояние уровня,<br/>
        /// что позволяет затем загрузить уровень обратно с состоянием на момент отгрузки.
        /// </summary>
        /// <param name="name"></param>
        public void Shut(string name, bool keepState)
        {
            ActiveLevels.Remove(Loaded[name]);
            if (!keepState)
            {
                Loaded[name].Shut();
            }
        }

        /// <summary>
        /// Перезапускает уровень, проводя инициализацию заново.
        /// </summary>
        /// <param name="name"></param>
        public void Restart(string name, LevelArgs arguments)
        {
            Loaded[name].Shut();
            Loaded[name].Start(_app, arguments, name);
        }

        /// <summary>
        /// Перезапускает уровень, проводя инициализацию заново.
        /// </summary>
        /// <param name="name"></param>
        public void Restart(string name)
        {
            Loaded[name].Shut();
            Loaded[name].Start(_app, LevelArgs.Empty, name);
        }

        /// <summary>
        /// Создаёт запись об новом уровне с именем <paramref name="name"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        public void LoadAs<T>(string name) where T : ILevel
        {
            Loaded[name] = Activator.CreateInstance<T>();
        }
        #endregion


        #region ACCESS
        public ILevel[] GetAllActive()
        {
            return ActiveLevels.ToArray();
        }
        #endregion


        public ApplicationLevelManager(MagicGameApplication app)
        {
            _app = app;
        }
    }
}
