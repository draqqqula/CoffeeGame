using MagicDustLibrary.ComponentModel;
using MagicDustLibrary.Display;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Logic
{
    public class LevelSettings
    {
        private readonly Dictionary<string, object> _entries = new();

        public T? ReadEntry<T>(string key)
        {
            if (_entries[key] is T t)
            {
                return t;
            }
            return default;
        }

        public void AddEntry(string key, object entry)
        {
            _entries[key] = entry;
        }

        public CameraSettings CameraSettings { get; init; } = new CameraSettings();
        public object UpdateLock { get; init; } = new();
    }
}
