using MagicDustLibrary.Display;
using Microsoft.Xna.Framework.Graphics;
using System.Globalization;

namespace MagicDustLibrary.Animations
{
    public class Animation
    {
        private readonly AnimationFrame[] Frames;
        private readonly Texture2D Sheet;

        public readonly string Name;
        /// <summary>
        /// если true и не задан NextAnimation то по окончании текущей анимации начинает её проигрывание с начала
        /// </summary>
        public readonly bool Looping;
        /// <summary>
        /// общая длительность анимации
        /// </summary>
        public readonly TimeSpan Duration;
        /// <summary>
        /// если задано то по окончании текущей анимации начнёт проигрывать эту с первого кадра
        /// </summary>
        public string NextAnimation;
        /// <summary>
        /// коефициент скорости проигрывания
        /// </summary>
        public double SpeedFactor;

        public readonly int FrameCount;

        public Animation(string name, Texture2D sheet, AnimationFrame[] frames, Dictionary<string, string> properties)
        {
            var property =
            (string key, string _default) =>
            { if (properties.ContainsKey(key)) return properties[key]; else return _default; };

            Looping = bool.Parse(property("Looping", "false"));
            NextAnimation = property("NextAnimation", null);
            SpeedFactor = double.Parse(property("SpeedFactor", "1"), CultureInfo.InvariantCulture);

            Frames = frames;
            Name = name;
            FrameCount = frames.Length;
            Sheet = sheet;
            Duration = TimeSpan.FromSeconds(frames.Sum(frame => frame.Duration.TotalSeconds));
        }

        public IDisplayable GetVisual(DrawingParameters arguments, int frame)
        {
            if (frame < 0 || frame >= Frames.Length)
            {
                throw new ArgumentOutOfRangeException($"Couldn't display frame {frame} since Animation {Name} has only {FrameCount}");
            }
            return Frames[frame].CreateDrawable(arguments, Sheet);
        }
    }
}
