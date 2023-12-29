using Microsoft.Xna.Framework.Content.Pipeline;
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
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Content;
using System.Collections.ObjectModel;

namespace AsepriteImporter
{
    [ContentImporter(".aseprite", DisplayName = "AsepriteImporter - MagicDust", DefaultProcessor = nameof(AsepriteContentProcessor))]
    public class AsepriteContentImporter : ContentImporter<AsepriteFile>
    {
        public override AsepriteFile Import(string filename, ContentImporterContext context)
        {
            var file = AsepriteFile.Load($"{filename}.aseprite");
            return file;
        }
    }


    [ContentProcessor(DisplayName = "AsepriteProcessor - MagicDust")]
    public class AsepriteContentProcessor : ContentProcessor<AsepriteFile, AnimationWrapper>
    {
        private const string DEFAULT_NAME = "Default";
        public override AnimationWrapper Process(AsepriteFile input, ContentProcessorContext context)
        {
            AnimationWrapper wrapper = new();
            var sheet = input.ToAsepriteSheet(GetSpritesheetOptions(), GetTilesheetOptions());

            wrapper.TextureWidth = sheet.Spritesheet.Size.Width;
            wrapper.TextureHeight = sheet.Spritesheet.Size.Height;
            wrapper.Pixels = sheet.Spritesheet.Pixels.ToArray();
            wrapper.Animations = ProcessAnimations(sheet);

            return wrapper;
        }

        private Animation[] ProcessAnimations(AsepriteSheet sheet)
        {
            var aseAnimations = sheet.Spritesheet.Animations;
            var animations = new List<Animation>();

            if (!aseAnimations.Any())
            {
                animations.Add(AsSingularAnimation(sheet.Spritesheet));
            }

            int indent = 0;
            foreach (var aseAnimation in aseAnimations)
            {
                var magicAnimation = ToMagicAnimation(aseAnimation, indent);
                animations.Add(magicAnimation);
                indent += magicAnimation.FrameCount;
            }
            return animations.ToArray();
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

        private Animation AsSingularAnimation(Spritesheet sheet)
        {
            var properties = new Dictionary<string, string>();
            return new Animation(
                DEFAULT_NAME,
                null,
                GetFrames(sheet.Frames.ToArray(), 0),
                properties
                );
        }

        private Animation ToMagicAnimation(SpritesheetAnimation aseAnimation, int indent)
        {
            var properties = ParseTags(aseAnimation.Name);
            return new Animation(
                ParseName(aseAnimation.Name),
                null,
                GetFrames(aseAnimation.Frames.ToArray(), indent),
                properties
                );
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

        private AnimationFrame[] GetFrames(SpritesheetFrame[] aseFrames, int indent)
        {
            return Enumerable.Range(0, aseFrames.Length).Select(n => ToMagicAnimationFrame(aseFrames[n], indent + n)).ToArray();
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
    }


    [ContentTypeWriter]
    internal class MagicJsonContentTypeWriter : ContentTypeWriter<AnimationWrapper>
    {
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return $"{typeof(AsepriteContentReader).FullName}, {typeof(AsepriteContentReader).Assembly}";
        }

        protected override void Write(ContentWriter output, AnimationWrapper value)
        {
            output.Write7BitEncodedInt(value.TextureWidth);
            output.Write7BitEncodedInt(value.TextureHeight);
            foreach (var color in value.Pixels)
            {
                WriteColor(color, output);
            }

            foreach (var animation in value.Animations)
            {
                WriteAnimation(animation, output);
            }
        }

        private void WriteColor(AsepriteDotNet.Common.Color aseColor, ContentWriter output)
        {
            output.Write([aseColor.R, aseColor.G, aseColor.B, aseColor.A]);
        }

        private void WriteAnimation(Animation animation, ContentWriter output)
        {
            output.Write(animation.Duration.TotalSeconds);
            output.Write(animation.NextAnimation);
            output.Write(animation.Looping);
            output.Write(animation.SpeedFactor);
        }
    }

    public class AsepriteContentReader : ContentTypeReader<AnimationCollection>
    {
        protected override AnimationCollection Read(ContentReader input, AnimationCollection existingInstance)
        {
            var result = new AnimationCollection();

            var device = input.GetGraphicsDevice();
            int textureWidth = input.Read7BitEncodedInt();
            int textureHeight = input.Read7BitEncodedInt();
            byte[] pixelBuffer = new byte[textureWidth * textureHeight * 4];
            input.Read(pixelBuffer);

            var texture = new Texture2D(device, textureWidth, textureHeight);
            texture.SetData(pixelBuffer);


            return result;
        }
    }

    public class AnimationCollection
    {
        public Dictionary<string, Animation> Animations { get; set; } = [];
    }

    public class AnimationWrapper
    {
        public int TextureWidth { get; set; }
        public int TextureHeight { get; set; }
        public AsepriteDotNet.Common.Color[] Pixels { get; set; }
        public Animation[] Animations { get; set; }
    }
}
