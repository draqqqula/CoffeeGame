using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MagicDustLibrary.Extensions
{
    public interface ITestServiceContainer
    {
        public void AddService<T>(T service);

        public T? GetService<T>();
    }

    public class NoReflectionServiceContainer : ITestServiceContainer
    {
        private readonly Dictionary<IServiceDescriptor, IServiceDescriptor> _services;

        public void AddService<T>(T service)
        {
            var descriptor = new ServiceDescriptor<T>();
            descriptor.Service = service;
            _services.Add(descriptor, descriptor);
        }

        public T? GetService<T>()
        {
            var descriptor = new ServiceDescriptor<T>();
            if (_services.TryGetValue(descriptor, out var service))
            {
                if (service is ServiceDescriptor<T> desc)
                {
                    return desc.Service;
                }
            }
            return default;
        }
    }

    public class ReflectionServiceContainer : ITestServiceContainer
    {
        private readonly Dictionary<Type, object> _services;

        public void AddService<T>(T service)
        {
            _services.Add(typeof(T), service);
        }

        public T? GetService<T>()
        {
            if (_services.TryGetValue(typeof(T), out var service))
            {
                return (T)service;
            }
            return default;
        }
    }

    public interface IServiceDescriptor
    {
    }

    public struct ServiceDescriptor<T> : IServiceDescriptor
    {
        public T Service { get; set; }
        public override bool Equals(object? obj)
        {
            if (obj is ServiceDescriptor<T>)
            {
                return true;
            }
            return false;
        }

        public override readonly int GetHashCode()
        {
            return typeof(T).GetHashCode();
        }
    }
}
