using AsepriteDotNet.Image;
using AsepriteDotNet;
using Microsoft.Xna.Framework.Content.Pipeline;
using TInput = System.String;
using TOutput = System.String;
using System.Collections.Generic;
using System.Buffers.Binary;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace AsepriteContentPipelineExtension
{
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

        private SerializedAnimation[] ProcessAnimations(AsepriteSheet sheet)
        {
            var aseAnimations = sheet.Spritesheet.Animations;
            var animations = new List<SerializedAnimation>();

            if (!aseAnimations.Any())
            {
                animations.Add(AsSingularAnimation(sheet.Spritesheet));
            }

            int indent = 0;
            foreach (var aseAnimation in aseAnimations)
            {
                var magicAnimation = SerializeAnimation(aseAnimation, indent);
                animations.Add(magicAnimation);
                indent += magicAnimation.Frames.Length;
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

        private SerializedAnimation AsSingularAnimation(Spritesheet sheet)
        {
            var properties = new Dictionary<string, string>();
            return SerializeAnimation(
                DEFAULT_NAME,
                GetFrames(sheet.Frames.ToArray(), 0),
                properties
                );
        }

        private SerializedAnimation SerializeAnimation(SpritesheetAnimation aseAnimation, int indent)
        {
            var properties = ParseTags(aseAnimation.Name);
            return SerializeAnimation(
                ParseName(aseAnimation.Name),
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
                    result.Add(match.Groups[1].Value, match.Groups[2].Value);
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

        private byte[][] GetFrames(SpritesheetFrame[] aseFrames, int indent)
        {
            return Enumerable.Range(0, aseFrames.Length).Select(n => SerializeFrame(aseFrames[n], indent + n)).ToArray();
        }

        private SerializedAnimation SerializeAnimation(string name, byte[][] frames, Dictionary<string, string> properties)
        {
            return new SerializedAnimation()
            {
                Frames = frames,
                Properties = properties,
                Name = name
            };
        }

        private byte[] SerializeFrame(SpritesheetFrame frame, int number)
        {
            var slices = frame.GetSlices();

            if (!slices.Any())
            {
                return SerializeFrame(
                    frame.SourceRectangle,
                    new AsepriteDotNet.Common.Point(frame.SourceRectangle.Width / 2, frame.SourceRectangle.Height / 2),
                    frame.Duration);
            }

            var mainSlice = slices.Where(it => it.Name.StartsWith("#")).First();
            var frameLocation = frame.SourceRectangle.Location;
            var resultBounds = new AsepriteDotNet.Common.Rectangle(frameLocation + new AsepriteDotNet.Common.Point(mainSlice.Bounds.Width * number, 0), mainSlice.Bounds.Size);

            return SerializeFrame(
                resultBounds,
                mainSlice.Pivot ?? new AsepriteDotNet.Common.Point(frame.SourceRectangle.Width / 2, frame.SourceRectangle.Height / 2),
                frame.Duration);
        }

        private byte[] SerializeFrame(AsepriteDotNet.Common.Rectangle borders, AsepriteDotNet.Common.Point anchor, int duration)
        {
            Span<byte> buffer = stackalloc byte[28];
            BinaryPrimitives.WriteInt32LittleEndian(buffer, borders.X);
            BinaryPrimitives.WriteInt32LittleEndian(buffer[4..], borders.Y);
            BinaryPrimitives.WriteInt32LittleEndian(buffer[8..], borders.Width);
            BinaryPrimitives.WriteInt32LittleEndian(buffer[12..], borders.Height);
            BinaryPrimitives.WriteInt32LittleEndian(buffer[16..], anchor.X);
            BinaryPrimitives.WriteInt32LittleEndian(buffer[20..], anchor.Y);
            BinaryPrimitives.WriteInt32LittleEndian(buffer[24..], duration);
            return buffer.ToArray();
        }
    }
}
