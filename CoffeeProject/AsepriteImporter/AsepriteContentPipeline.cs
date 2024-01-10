using Microsoft.Xna.Framework.Content.Pipeline;
using AsepriteDotNet;
using AsepriteDotNet.Image;
using MagicDustLibrary.Animations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Content;
using System.Buffers.Binary;

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
                    new AsepriteDotNet.Common.Point(frame.SourceRectangle.Width/2, frame.SourceRectangle.Height / 2),
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


    [ContentTypeWriter]
    internal class MagicJsonContentTypeWriter : ContentTypeWriter<AnimationWrapper>
    {
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return $"{typeof(AsepriteContentReader).FullName}, {typeof(AsepriteContentReader).Assembly}";
        }

        protected override void Write(ContentWriter output, AnimationWrapper value)
        {
            output.Write(value.TextureWidth);
            output.Write(value.TextureHeight);
            foreach (var color in value.Pixels)
            {
                //4 bytes for one color
                WriteColor(color, output);
            }

            output.Write(value.Animations.Length);
            foreach (var animation in value.Animations)
            {
                WriteAnimation(animation, output);
            }
        }

        private Color ToXnaColor(AsepriteDotNet.Common.Color aseColor)
        {
            return Color.FromNonPremultiplied(new Vector4(aseColor.R / 256f, aseColor.G / 256f, aseColor.B / 256f, aseColor.A / 256f));
        }

        private void WriteColor(AsepriteDotNet.Common.Color aseColor, ContentWriter output)
        {
            var xnaColor = ToXnaColor(aseColor);
            output.Write([xnaColor.R, xnaColor.G, xnaColor.B, xnaColor.A]);
        }

        private void WriteAnimation(SerializedAnimation animation, ContentWriter output)
        {
            output.Write(animation.Frames.Length);
            foreach (var frame in animation.Frames)
            {
                output.Write(frame);
            }
            output.Write(animation.Name);
            output.Write(animation.Properties.Count);
            foreach (var property in animation.Properties)
            {
                output.Write(property.Key);
                output.Write(property.Value);
            }
        }
    }

    public class AsepriteContentReader : ContentTypeReader<AnimationCollection>
    {
        protected override AnimationCollection Read(ContentReader input, AnimationCollection existingInstance)
        {
            var result = new AnimationCollection();

            var device = input.GetGraphicsDevice();
            int textureWidth = input.ReadInt32();
            int textureHeight = input.ReadInt32();
            byte[] pixelBuffer = new byte[textureWidth * textureHeight * 4];
            input.Read(pixelBuffer);
            var texture = new Texture2D(device, textureWidth, textureHeight);
            texture.SetData(pixelBuffer);

            var animationCount = input.ReadInt32();

            for (int i = 0; i < animationCount; i++)
            {
                var animation = ReadAnimation(input, texture);
                result.Animations.Add(animation.Name, animation);
            }

            return result;
        }

        private Animation ReadAnimation(ContentReader input, Texture2D texture)
        {
            var frameCount = input.ReadInt32();
            var frames = new AnimationFrame[frameCount];
            for (int i = 0; i < frameCount; i++)
            {
                var boundsX = input.ReadInt32();
                var boundsY = input.ReadInt32();
                var boundsWidth = input.ReadInt32();
                var boundsHeight = input.ReadInt32();
                var pivotX = input.ReadInt32();
                var pivotY = input.ReadInt32();
                var duration = input.ReadInt32();
                frames[i] = new AnimationFrame(boundsX, boundsY, boundsWidth, boundsHeight, pivotX, pivotY, Convert.ToDouble(duration) / 1000);
            }
            var name = input.ReadString();
            var propertyCount = input.ReadInt32();
            var properties = new Dictionary<string, string>();
            for (int i = 0; i < propertyCount; i++)
            {
                var propertyName = input.ReadString();
                var propertyValue = input.ReadString();
                properties.Add(propertyName, propertyValue);
            }
            return new Animation(name, texture, frames, properties);
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
        public SerializedAnimation[] Animations { get; set; }
    }

    public class SerializedAnimation
    {
        public string Name { get; set; }

        //28 byte array each frame
        public byte[][] Frames { get; set; }
        public Dictionary<string, string> Properties { get; set; }
    }
}
