using MagicDustLibrary.Organization.DefualtImplementations;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Logic.Controllers
{
    public interface ISoundController : IStateController
    {
        public SoundEffectInstance? CreateSoundInstance(string fileName, string tag);
        public SoundEffectInstance? GetSoundInstance(string tag);
    }

    internal class DefaultSoundController : ISoundController
    {
        private readonly StateSoundManager _soundManager;
        public DefaultSoundController(StateSoundManager soundManager)
        {
            _soundManager = soundManager;
        }
        public SoundEffectInstance? CreateSoundInstance(string fileName, string tag)
        {
            return _soundManager.CreateInstance(fileName, tag);
        }

        public SoundEffectInstance? GetSoundInstance(string tag)
        {
            return _soundManager.GetInstance(tag);
        }
    }
}
