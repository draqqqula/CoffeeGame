using MagicDustLibrary.Content;
using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MagicDustLibrary.CommonObjectTypes
{
    public class TestDto
    {
        public string Key { get; set; }
    }

    public class TestDtoReader1 : MagicJsonContentTypeReader<TestDto>
    {
    }

    public class TestDtoReader : ContentTypeReader<List<ExpandoObject>>
    {
        protected override List<ExpandoObject> Read(ContentReader input, List<ExpandoObject> existingInstance)
        {
            string json = input.ReadString();
            return JsonConvert.DeserializeObject<List<ExpandoObject>>(json);
        }
    }

    public class TestDtoSingleReader : ContentTypeReader<ExpandoObject>
    {
        protected override ExpandoObject Read(ContentReader input, ExpandoObject existingInstance)
        {
            string json = input.ReadString();
            return JsonConvert.DeserializeObject<ExpandoObject>(json);
        }
    }
}
