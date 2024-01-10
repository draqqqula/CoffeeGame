using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsepriteContentPipelineExtension
{
    public class SerializedAnimation
    {
        public string Name { get; set; }

        //28 byte array each frame
        public byte[][] Frames { get; set; }
        public Dictionary<string, string> Properties { get; set; }
    }

    public class AnimationWrapper
    {
        public int TextureWidth { get; set; }
        public int TextureHeight { get; set; }
        public AsepriteDotNet.Common.Color[] Pixels { get; set; }
        public SerializedAnimation[] Animations { get; set; }
    }
}
