﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagicDustLibrary.Organization;

namespace MagicDustLibrary.Logic
{
    public interface IPlacement
    {
        Type GetLayerType();
    }
    public class Placement<T> : IPlacement where T : Layer
    {
        public Type GetLayerType()
        {
            return typeof(T);
        }

        public static Placement<T> On()
        {
            return new Placement<T>();
        }
    }

    public class Placement(Type layerType) : IPlacement
    {
        private readonly Type _storedType = layerType;
        public Type GetLayerType()
        {
            return _storedType;
        }
    }
}