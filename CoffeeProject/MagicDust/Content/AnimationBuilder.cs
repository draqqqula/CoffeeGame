using MagicDustLibrary.Animations;
using MagicDustLibrary.Display;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MagicDustLibrary.Content
{
    /// <summary>
    /// Воссоздаёт пакет анимаций из .png файла и .txt файла
    /// </summary>
    public interface IAnimationProvider
    {
        /// <summary>
        /// Строит анимации, используя "<paramref name="name"/>.png" как общий спрайт и "<paramref name="name"/>_properites.txt" как описание анимаций
        /// </summary>
        /// <param newPriority="name"></param>
        /// <returns></returns>
        public Dictionary<string, Animation> BuildFromFiles(string name);
    }

    public class AnimationBuilder : IAnimationProvider
    {
        private readonly DefaultContentStorage ContentStorage;

        private static AnimationFrame BuildFrame(string line)
        {
            var numbers = Regex.Split(line, ",");
            var positions = numbers.Take(6).Select(int.Parse).ToArray();

            TimeSpan duration = TimeSpan.FromSeconds(double.Parse(numbers[6], CultureInfo.InvariantCulture));
            Rectangle borders = new Rectangle(positions[0], positions[1], positions[2], positions[3]);
            Vector2 anchor = new Vector2(positions[4], positions[5]);

            return new AnimationFrame(borders, anchor, duration);
        }

        private static Dictionary<string, string> BuildAnimationProperties(string[] properties)
        {
            return properties.ToDictionary(e => Regex.Split(e, "=")[0], e => Regex.Split(e, "=")[1]);
        }

        private Animation BuildAnimation(Match match, string sheet)
        {
            var animationProperties = BuildAnimationProperties(match.Groups["Settings"].Captures.Select(v => v.Value).ToArray());

            return new Animation(match.Groups["Name"].Value, ContentStorage.GetAsset<Texture2D>(sheet),
                match.Groups["Frames"].Captures.Select(v => BuildFrame(v.Value)).ToArray(),
                animationProperties);
        }

        public Dictionary<string, Animation> BuildFromFiles(string name)
        {
            var animations = new Dictionary<string, Animation>();

            var path = Path.Combine(Environment.CurrentDirectory, string.Concat(name, "_properties.txt"));
            var rawProperties = string.Join(' ', File.ReadAllLines(path));
            var properties = Regex.Matches(rawProperties, "#(?'Name'[^ ]+) (?>(?'Settings'[^, ]+),?)+(?> (?'Frames'[^# ]+))*[^#]?");

            foreach (Match match in properties)
            {
                animations.Add(match.Groups["Name"].Value, BuildAnimation(match, name));
            }

            return animations;
        }

        public AnimationBuilder(DefaultContentStorage contentStorage)
        {
            ContentStorage = contentStorage;
        }
    }
}
