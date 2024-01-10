using AsepriteDotNet;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace AsepriteContentPipelineExtension
{
    [ContentImporter(".aseprite", DisplayName = "AsepriteImporter - MagicDust", DefaultProcessor = nameof(AsepriteContentProcessor))]
    public class AsepriteContentImporter : ContentImporter<AsepriteFile>
    {
        public override AsepriteFile Import(string filename, ContentImporterContext context)
        {
            var file = AsepriteFile.Load(filename);
            return file;
        }
    }
}
