using AsepriteDotNet;
using AsepriteDotNet.Document;
using AsepriteDotNet.Image;
using MagicDustLibrary.Animations;
using MagicDustLibrary.Content;
using MagicDustLibrary.Display;
using MagicDustLibrary.Organization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AsepriteImporter
{
    public class AsepriteAnimationBuilder : IAnimationProvider
    {
        private const string DEFAULT_NAME = "Default";
        private readonly GraphicsDevice _device;
        private readonly IContentStorage _storage;

        public Dictionary<string, Animation> BuildFromFiles(string name)
        {
            var file = AsepriteFile.Load(Path.Combine("Sprites", $"{name}.aseprite"));
            var sheet = file.ToAsepriteSheet(GetSpritesheetOptions(), GetTilesheetOptions());
            var aseAnimations = sheet.Spritesheet.Animations;
            Texture2D texture;
            if (_storage.GetAsset<Texture2D>(name) is not null)
            {
                texture = _storage.GetAsset<Texture2D>(name);
            }
            else
            {
                texture = ToXnaTexture(
                sheet.Spritesheet.Pixels,
                sheet.Spritesheet.Size.Width,
                sheet.Spritesheet.Size.Height);
                _storage.AddAsset(texture, name);
            }

            var dictionary = new Dictionary<string, Animation>();

            if (!aseAnimations.Any())
            {
                dictionary.Add(DEFAULT_NAME, AsSingularAnimation(sheet.Spritesheet, texture));
            }

            int indent = 0;
            foreach (var aseAnimation in aseAnimations)
            {
                var magicAnimation = ToMagicAnimation(aseAnimation, indent, texture);
                dictionary.Add(ParseName(aseAnimation.Name), magicAnimation);
                indent += magicAnimation.FrameCount;
            }
            return dictionary;
        }

        private SpritesheetOptions GetSpritesheetOptions()
        {
            return new SpritesheetOptions()
            {
                PackingMethod = PackingMethod.HorizontalStrip,
                MergeDuplicates = false
            };
        }

        private TilesheetOptions GetTilesheetOptions()
        {
            return new TilesheetOptions();
        }

        private Animation AsSingularAnimation(Spritesheet sheet, Texture2D texture)
        {
            var properties = new Dictionary<string, string>();
            return new Animation(
                DEFAULT_NAME,
                texture,
                GetFrames(sheet.Frames.ToArray(), 0),
                properties
                );
        }

        private Animation ToMagicAnimation(SpritesheetAnimation aseAnimation, int indent, Texture2D texture)
        {
            var properties = ParseTags(aseAnimation.Name);
            return new Animation(
                ParseName(aseAnimation.Name),
                texture,
                GetFrames(aseAnimation.Frames.ToArray(), indent),
                properties
                );
        }

        private AnimationFrame[] GetFrames(SpritesheetFrame[] aseFrames, int indent)
        {
            return Enumerable.Range(0, aseFrames.Length).Select(n => ToMagicAnimationFrame(aseFrames[n], indent+n)).ToArray();
        }

        private AnimationFrame ToMagicAnimationFrame(SpritesheetFrame frame, int number)
        {
            var slices = frame.GetSlices();

            if (!slices.Any())
            {
                return new AnimationFrame(
                    ToXnaRectangle(frame.SourceRectangle),
                    ToXnaRectangle(frame.SourceRectangle).Center.ToVector2(),
                    TimeSpan.FromMilliseconds(frame.Duration));
            }

            var mainSlice = slices.Where(it => it.Name.StartsWith("#")).First();
            var frameLocation = ToXnaRectangle(frame.SourceRectangle).Location;
            var sliceBounds = ToXnaRectangle(mainSlice.Bounds);
            var resultBounds = new Rectangle(frameLocation + new Point(sliceBounds.Width * number, 0), sliceBounds.Size);

            return new AnimationFrame(
                resultBounds,
                ToXnaPoint(mainSlice.Pivot.Value),
                TimeSpan.FromMilliseconds(frame.Duration));
        }

        private Rectangle ToXnaRectangle(AsepriteDotNet.Common.Rectangle aseRectangle)
        {
            return new Rectangle(aseRectangle.X, aseRectangle.Y, aseRectangle.Width, aseRectangle.Height);
        }

        private Vector2 ToXnaPoint(AsepriteDotNet.Common.Point asePoint)
        {
            return new Vector2(asePoint.X, asePoint.Y);
        }

        private Texture2D ToXnaTexture(IEnumerable<AsepriteDotNet.Common.Color> pixels, int width, int height)
        {
            var xnaPixels = pixels.Select(it => ToXnaColor(it)).ToArray();
            var texture = new Texture2D(_device, width, height);
            texture.SetData(xnaPixels);
            return texture;
        }

        private Color ToXnaColor(AsepriteDotNet.Common.Color aseColor)
        {
            return Color.FromNonPremultiplied(new Vector4(aseColor.R/256f, aseColor.G/256f, aseColor.B / 256f, aseColor.A/256f));
        }

        private Dictionary<string, string> ParseTags(string line)
        {
            var result = new Dictionary<string, string>();
            var tagsPattern = new Regex("(?<=:).+");
            var tagLine = tagsPattern.Match(line);
            if (!tagLine.Success)
            {
                return result;
            }
            var tags = tagLine.Value.Split(',');
            var equalPattern = new Regex("(.+)=(.+)");

            foreach (var tag in tags)
            {
                var match = equalPattern.Match(tag);
                if (match.Success)
                {
                    result.Add(match.Groups[0].Value, match.Groups[1].Value);
                    continue;
                }
                result.Add(tag, "true");
            }
            return result;
        }

        private string ParseName(string line)
        {
            var namePattern = new Regex(".+(?=:)");
            var name = namePattern.Match(line);
            if (!name.Success)
            {
                return line;
            }
            return name.Value;
        }

        public AsepriteAnimationBuilder(GraphicsDevice device, IContentStorage storage)
        {
            _device = device;
            _storage = storage;
        }
    }
}
