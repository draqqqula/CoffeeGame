using MagicDustLibrary.Display;
using MagicDustLibrary.Extensions.Collections;
using MagicDustLibrary.Logic;
using System.Reflection;
using System.Runtime.Serialization;

namespace MagicDustLibrary.Organization
{
    public abstract class Layer : RelativeCollection<IDisplayProvider>
    {
        public readonly byte Priority;
        public abstract DrawingParameters Process(DrawingParameters arguments, GameCamera camera);

        public Layer()
        {
            var attr = GetType().GetCustomAttribute<LayerPriorityAttribute>();
            if (attr is null)
                throw new ApplicationException(string.Format("{0} must define priority using LayerPriorityAttribute", GetType().Name));
            Priority = attr.priority;
        }

        public Layer(byte priority)
        {
            Priority = priority;
        }
    }
    /// <summary>
    /// Inherit this class to define parlax layer quickly. ParalaxAttribute required. If not found sets xFactor and yFactor to 1.
    /// </summary>
    public abstract class ParalaxLayer : Layer
    {
        public readonly float xf;
        public readonly float yf;
        public ParalaxLayer()
        {
            var attr = GetType().GetCustomAttribute<ParalaxAttribute>();
            if (attr is null)
            {
                xf = 1;
                yf = 1;
                return;
            }
            xf = attr.xf;
            yf = attr.yf;
        }
        public override DrawingParameters Process(DrawingParameters arguments, GameCamera camera)
        {
            return camera.ApplyParalax(arguments, xf, yf);
        }
    }

    public class ParalaxAttribute : Attribute
    {
        public float xf;
        public float yf;
        public ParalaxAttribute(float xf, float yf)
        {
            this.xf = xf;
            this.yf = yf;
        }
    }


    public class FrameState
    {
        public readonly List<IDisplayable> Pictures = new List<IDisplayable>();

        public void Clear()
        {
            Pictures.Clear();
        }

        public void AddPicture(IDisplayable picture)
        {
            Pictures.Add(picture);
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DefinedLayerAttribute<T> : Attribute where T : Layer
    {
        public byte newPriority;

        public DefinedLayerAttribute(byte newPriority)
        {
            this.newPriority = newPriority;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class LayerPriorityAttribute : Attribute
    {
        public byte priority;

        public LayerPriorityAttribute(byte priority)
        {
            this.priority = priority;
        }
    }

    public class LayerComparer : IComparer<Layer>
    {
        public int Compare(Layer? x, Layer? y)
        {
            return x.Priority - y.Priority;
        }
    }
}
