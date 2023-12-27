using MagicDustLibrary.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.CommonObjectTypes.TextDisplays
{
    public class DynamicLabel : Label, IUpdateComponent
    {
        private Func<string> TextGenerator {  get; set; } = () => "Text";
        public void Update(IControllerProvider state, TimeSpan deltaTime)
        {
            SetText(TextGenerator());
        }

        public DynamicLabel SetText(Func<string> textGenerator)
        {
            this.TextGenerator = textGenerator;
            return this;
        }
    }
}
