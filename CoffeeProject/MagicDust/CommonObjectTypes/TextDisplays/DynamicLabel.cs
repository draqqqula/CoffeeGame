using MagicDustLibrary.Display;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Logic.Behaviors;
using MagicDustLibrary.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.CommonObjectTypes.TextDisplays
{

    public class DynamicLabel : Label, IUpdateComponent, IMultiBehaviorComponent
    {
        private const string ErrorText = "Error";

        public event Action<IControllerProvider, TimeSpan, IMultiBehaviorComponent> OnAct = delegate { };

        public enum OnException
        {
            UseErrorText,
            ThrowException,
            Dispose
        }
        public OnException ExceptionAction { get; set; } = OnException.UseErrorText;
        protected Func<string> TextGenerator {  get; set; } = () => "Text";
        public void Update(IControllerProvider state, TimeSpan deltaTime)
        {
            OnAct(state, deltaTime, this);
            string text;
            if (ExceptionAction == OnException.ThrowException)
            {
                text = TextGenerator();
                SetText(text);
                return;
            }
            try
            {
                text = TextGenerator();
            }
            catch (Exception e)
            {
                text = ErrorText;
                if (ExceptionAction == OnException.Dispose)
                {
                    Dispose();
                }
            }

            SetText(text);
        }

        public override DrawingParameters GetDrawingParameters()
        {
            var info =  base.GetDrawingParameters();
            foreach (var filter in this.GetComponents<IDisplayFilter>())
            {
                info = filter.ApplyFilter(info);
            }
            return info;
        }

        public DynamicLabel SetText(Func<string> textGenerator, OnException exceptionAction)
        {
            this.TextGenerator = textGenerator;
            ExceptionAction = exceptionAction;
            return this;
        }

        public DynamicLabel SetText(Func<string> textGenerator)
        {
            return SetText(textGenerator, OnException.UseErrorText);
        }
    }
}
