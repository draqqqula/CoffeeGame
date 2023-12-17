using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MagicDustLibrary.Content
{
    public class MagicJsonContentTypeReader<T> : ContentTypeReader<T>
    {
        protected override T Read(ContentReader input, T existingInstance)
        {
            string json = input.ReadString();
            return JsonDocument.Parse(json).Deserialize<T>();
        }
    }
}
