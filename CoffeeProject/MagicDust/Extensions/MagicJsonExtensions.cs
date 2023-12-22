using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Extensions
{
    public static class MagicJsonExtensions
    {
        public static Rectangle ToRectangle(this int[] array)
        {
            return new Rectangle(array[0], array[1], array[2], array[3]);
        }

        public static Vector3 ToVector3(this float[] array)
        {
            return new Vector3(array[0], array[1], array[2]);
        }

        public static Rectangle ReadRectangle(dynamic field)
        {
            if (field is List<object> array)
            {
                return array.Select(it => Convert.ToInt32((long)it)).Take(4).ToArray().ToRectangle();
            }
            throw new Exception("field was not list");
        }

        public static Vector3 ReadVector3(dynamic field)
        {
            if (field is List<object> array)
            {
                return array.Select(it => Convert.ToSingle(it.ToString())).Take(3).ToArray().ToVector3();
            }
            throw new Exception("field was not list");
        }
    }
}
