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
        public int CurrentFrame { get; private set; }

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

            CurrentFrame = 0;
        }

        /// <summary>
        /// если возможно, выбирает кадр отталкиваясь от прогресса анимации, отправляет в буфер на отрисовку и возвращает true
        /// если невозможно выбрать кадр, возвращает false
        /// </summary>
        /// <param newPriority="progress"></param>
        /// <param newPriority="arguments"></param>
        /// <param newPriority="animator"></param>
        /// <returns></returns>
        public bool Run(double progress)
        {
            if (progress > 1 || progress < 0)
            {
                return false;
            }

            CurrentFrame = (int)Math.Round(progress * (Frames.Length - 1));
            return true;
        }

        /// <summary>
        /// если возможно, выбирает кадр отталкиваясь от длительности анимации, отправляет в буфер на отрисовку и возвращает true
        /// если невозможно выбрать кадр, возвращает false
        /// </summary>
        /// <param newPriority="t"></param>
        /// <param newPriority="arguments"></param>
        /// <param newPriority="animator"></param>
        /// <returns></returns>
        public bool Run(TimeSpan t)
        {
            return Run(t / SpeedFactor / Duration);
        }

        public IDisplayable GetVisual(DrawingParameters arguments)
        {
            return Frames[CurrentFrame].CreateDrawable(arguments, Sheet);
        }
    }
}
