using MagicDustLibrary.ComponentModel;
using MagicDustLibrary.Content;
using MagicDustLibrary.Display;
using MagicDustLibrary.Factorys;
using MagicDustLibrary.Organization;
using MagicDustLibrary.Organization.StateManagement;
using MagicDustLibrary.StateManagement.DefualtImplementations;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Logic.Controllers
{
    public interface IFactoryController : IStateController
    {
        public T CreateAsset<T>(params string[] names);
        public T CreateObject<T>() where T : ComponentBase;
        public void AddToState<T>(T obj) where T : ComponentBase;
    }

    internal class DefaultFactoryController : IFactoryController
    {
        private IContentStorage _storage;
        private IGameObjectFactory _factory;
        private StateHooker _hooker;
        public DefaultFactoryController(IContentStorage storage, IGameObjectFactory factory, StateHooker hooker)
        {
            _storage = storage;
            _hooker = hooker;
            _factory = factory;
        }
        public T CreateAsset<T>(params string[] names)
        {
            return AssetExtensions.Create<T>(_storage, names);
        }

        public T CreateObject<T>() where T : ComponentBase
        {
            var obj = _factory.CreateObject<T>();
            return obj;
        }

        public void AddToState<T>(T obj) where T : ComponentBase
        {
            _hooker.Hook(obj);
        }
    }
    public static class FactoryExtensions
    {
        public static T1 AddComponent<T1,T2>(this T1 obj, T2 component) where T1 : ComponentBase where T2 : ComponentBase
        {
            obj.CombineWith(component);
            return obj;
        }
    }
}