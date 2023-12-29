using CoffeeProject.SurfaceMapping;
using MagicDustLibrary.ComponentModel;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Organization.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.Collision
{
    public class CollisionHandler : IComponentHandler, IUpdateService
    {
        private readonly Dictionary<ICollisionChecker, ICollisionHandlerNode> _nodes = [];
        private readonly List<IBodyComponent> _bodies = [];

        public bool RunOnPause => false;

        public void Hook(ComponentBase component)
        {
            component.InvokeEach<IBodyComponent>(HookBody);
            component.InvokeEach<ICollisionChecker>(HookChecker);
        }

        private void HookChecker(ICollisionChecker checker)
        {
            var genericType = typeof(ICollisionChecker<>);
            var checkerType = checker.GetType();
            var interfaces = checkerType.GetInterfaces()
                .Where(it => it.IsGenericType)
                .Where(it => it.GetGenericTypeDefinition() == genericType);
            foreach (var ichecker in interfaces)
            {
                var targetType = ichecker.GetGenericArguments().First();
                var parameterType = genericType.MakeGenericType(targetType);
                var nodeType = typeof(CollisionHandlerNode<>).MakeGenericType(targetType);
                var constructor = nodeType.GetConstructor([parameterType]);
                var handler = constructor.Invoke([checker]) as ICollisionHandlerNode;
                _nodes.Add(checker, handler);

                foreach (var body in _bodies)
                {
                    if (body is ComponentBase component)
                    {
                        handler.Hook(component);
                    }
                }
            }
        }

        private void HookBody(IBodyComponent body)
        {
            if (body is ComponentBase component)
            {
                _bodies.Add(body);
                foreach (var node in _nodes)
                {
                    node.Value.Hook(component);
                }
            }
        }

        public void Unhook(ComponentBase component)
        {
            component.InvokeEach<IBodyComponent>(UnhookBody);
            component.InvokeEach<ICollisionChecker>(UnhookChecker);
        }

        private void UnhookChecker(ICollisionChecker checker)
        {
            _nodes.Remove(checker);
        }

        private void UnhookBody(IBodyComponent body)
        {
            if (body is ComponentBase component)
            {
                _bodies.Remove(body);
                foreach (var node in _nodes)
                {
                    node.Value.Unhook(component);
                }
            }
        }

        public void Update(IControllerProvider state, TimeSpan deltaTime)
        {
            foreach (var node in _nodes.Values)
            {
                node.Update(state, deltaTime);
            }
        }
    }

    public class CollisionHandlerNode<T>(ICollisionChecker<T> checker) : 
        ComponentHandler<T>, ICollisionHandlerNode where T : IBodyComponent
    {
        private readonly ICollisionChecker<T> _checker = checker;
        private readonly List<T> _bodies = [];

        public bool RunOnPause => false;

        public override void Hook(T component)
        {
            _bodies.Add(component);
        }

        public override void Unhook(T component)
        {
            _bodies.Remove(component);
        }

        public void Update(IControllerProvider state, TimeSpan deltaTime)
        {
            var checkerLayout = _checker.GetLayout();
            foreach (var body in _bodies)
            {
                var bodyLayout = body.GetLayout();
                Rectangle.Intersect(ref checkerLayout, ref bodyLayout, out var intersection);
                if (intersection.IsEmpty)
                {
                    continue;
                }
                _checker.OnCollisionWith(state, deltaTime, body, intersection);
            }
        }
    }

    public interface ICollisionHandlerNode : IComponentHandler, IUpdateService
    {

    }

    public static class CollisionExtensions
    {
        public static void ConfigureCollisionHandler(IServiceCollection services, LevelSettings settings)
        {
            var collisionHandler = new CollisionHandler();
            services.AddSingleton<IUpdateService>(collisionHandler);
            services.AddSingleton<IComponentHandler>(collisionHandler);
        }
    }
}
