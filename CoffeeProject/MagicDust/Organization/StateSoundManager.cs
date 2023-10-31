using MagicDustLibrary.Display;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Organization
{
    public class StateSoundManager : IDisposable
    {
        private readonly IContentStorage _content;
        private readonly Dictionary<string, SoundEffectInstance> _soundEffects = new();

        public SoundEffectInstance? CreateInstance(string fileName, string tag)
        {
            try
            {
                var file = _content.GetAsset<SoundEffect>(fileName);
                var sound = file.CreateInstance();
                _soundEffects.Add(tag, sound);
                return sound;
            }
            finally
            {
            }
        }

        public SoundEffectInstance? GetInstance(string tag)
        {
            if (_soundEffects.ContainsKey(tag))
            {
                return _soundEffects[tag];
            }
            return null;
        }

        public void Dispose()
        {
            foreach (var soundEffect in _soundEffects.Values)
            {
                soundEffect.Stop(true);
                soundEffect.Dispose();
            }
        }

        public StateSoundManager(IContentStorage content)
        {
            _content = content;
        }
    }
}
