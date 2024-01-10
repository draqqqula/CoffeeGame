using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace AsepriteContentPipelineExtension
{
    [ContentTypeWriter]
    internal class MagicAnimationContentTypeWriter : ContentTypeWriter<AnimationWrapper>
    {
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "AsepriteImporter.AsepriteContentReader, AsepriteImporter";
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
            output.Write(new byte[4] { xnaColor.R, xnaColor.G, xnaColor.B, xnaColor.A });
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
}
