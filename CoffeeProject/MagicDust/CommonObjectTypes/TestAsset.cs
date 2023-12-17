using MagicDustLibrary.Content;
using MagicDustLibrary.Display;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text.Json;
using System.Dynamic;
using System.Diagnostics;

namespace MagicDustLibrary.CommonObjectTypes
{
    public class TestAsset
    {
        public TestAsset(
            [FromContent("Folder1", "*")]dynamic json
            )
        {
            string a = json.Key;
        }
    }
}
