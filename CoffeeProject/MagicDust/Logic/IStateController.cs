using MagicDustLibrary.Organization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Logic
{
    /// <summary>
    /// Содержит операции, доступные для выполнения внутри методов <see cref="IStateUpdateable.Update(IStateController, TimeSpan)"/>.
    /// </summary>
    public interface IStateController
    {
        public T CreateObject<T, L>(Vector2 position) where T : GameObject where L : Layer;
        public void PlaceAbove(GameObject target, GameObject source);
        public void PlaceBelow(GameObject target, GameObject source);
        public void PlaceTop(GameObject target);
        public void PlaceBottom(GameObject target);
        public void PlaceTo<L>(GameObject target) where L : Layer;
        public void GetFamily<F>() where F : class, IFamily;
        public void OpenServer(int port);
        public void LaunchLevel(string name, bool keepState);
        public void LaunchLevel(string name, LevelArgs arguments, bool keepState);
        public void PauseLevel(string name);
        public void PauseCurrent();
        public void RestartCurrent();
        public void RestartCurrent(LevelArgs arguments);
        public void RestartLevel(string name);
        public void RestartLevel(string name, LevelArgs arguments);
        public void ShutLevel(string name, bool keepState);
        public void ShutCurrent(bool keepState);
        public SoundEffectInstance? CreateSoundInstance(string fileName, string tag);
        public SoundEffectInstance? GetSoundInstance(string tag);
    }
}
