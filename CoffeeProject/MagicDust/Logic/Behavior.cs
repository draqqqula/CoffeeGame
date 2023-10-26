using MagicDustLibrary.Display;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Logic
{
    public abstract class Behavior<T> : IBehavior where T : GameObject
    {
        public bool Enabled { get; set; } = true;
        public DrawingParameters ChangeAppearance(GameObject parent, DrawingParameters parameters)
        {
            if (parent.GetType() is T)
            {
                return ChangeAppearance((T)parent, parameters);
            }
            return parameters;
        }
        protected virtual DrawingParameters ChangeAppearance(T parent, DrawingParameters parameters)
        {
            return parameters;
        }
        public void Act(GameObject parent, TimeSpan deltaTime)
        {
            if (parent.GetType() is T)
            {
                Act((T)parent, deltaTime);
            }
        }
        protected virtual void Act(T parent, TimeSpan deltaTime)
        {
        }
    }

    public interface IBehavior
    {
        public bool Enabled { get; set; }

        public DrawingParameters ChangeAppearance(GameObject parent, DrawingParameters parameters);

        public void Act(GameObject parent, TimeSpan deltaTime);
    }
}
