using MagicDustLibrary.Animations;
using MagicDustLibrary.Content;
using MagicDustLibrary.Display;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsepriteImporter
{
    public class AsepritePipelineBuilder : IAnimationProvider
    {
        private readonly IContentStorage _storage;
        public AsepritePipelineBuilder(IContentStorage storage)
        {
            _storage = storage;
        }

        public Dictionary<string, Animation> BuildFromFiles(string name)
        {
            return _storage.GetAsset<AnimationCollection>(Path.Combine("Sprites", name)).Animations;
        }
    }
}
