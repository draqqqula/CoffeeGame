using CoffeeTests;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

[TestFixture]
public class ServiceContainerPerformanceTests
{
    private const int Iterations = 10000;

    [Test]
    public void NoReflectionServiceContainer_AddService_PerformanceTest()
    {
        ITestServiceContainer container = new NoReflectionServiceContainer();

        var stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < Iterations; i++)
        {
            Type dynamicType = CreateDynamicType();
            dynamic service = Activator.CreateInstance(dynamicType);
            container.AddService(service);
        }

        stopwatch.Stop();

        Console.WriteLine($"NoReflectionServiceContainer_AddService: {stopwatch.ElapsedMilliseconds} ms");
    }

    [Test]
    public void ReflectionServiceContainer_AddService_PerformanceTest()
    {
        ITestServiceContainer container = new ReflectionServiceContainer();

        var stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < Iterations; i++)
        {
            Type dynamicType = CreateDynamicType();
            dynamic service = Activator.CreateInstance(dynamicType);
            container.AddService(service);
        }

        stopwatch.Stop();

        Console.WriteLine($"ReflectionServiceContainer_AddService: {stopwatch.ElapsedMilliseconds} ms");
    }

    [Test]
    public void NoReflectionServiceContainer_GetService_PerformanceTest()
    {
        ITestServiceContainer container = new NoReflectionServiceContainer();

        // Заполним контейнер сервисами
        for (int i = 0; i < Iterations; i++)
        {
            Type dynamicType = CreateDynamicType();
            dynamic service = Activator.CreateInstance(dynamicType);
            container.AddService(service);
        }

        var stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < Iterations; i++)
        {
            dynamic service = container.GetService<dynamic>();
            Assert.IsNotNull(service);
        }

        stopwatch.Stop();

        Console.WriteLine($"NoReflectionServiceContainer_GetService: {stopwatch.ElapsedMilliseconds} ms");
    }

    [Test]
    public void ReflectionServiceContainer_GetService_PerformanceTest()
    {
        ITestServiceContainer container = new ReflectionServiceContainer();

        // Заполним контейнер сервисами
        for (int i = 0; i < Iterations; i++)
        {
            Type dynamicType = CreateDynamicType();
            dynamic service = Activator.CreateInstance(dynamicType);
            container.AddService(service);
        }

        var stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < Iterations; i++)
        {
            dynamic service = container.GetService<dynamic>();
            Assert.IsNotNull(service);
        }

        stopwatch.Stop();

        Console.WriteLine($"ReflectionServiceContainer_GetService: {stopwatch.ElapsedMilliseconds} ms");
    }

    private Type CreateDynamicType()
    {
        AssemblyName assemblyName = new AssemblyName("DynamicAssembly");
        AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicModule");
        TypeBuilder typeBuilder = moduleBuilder.DefineType($"DynamicType_{Guid.NewGuid()}", TypeAttributes.Public);
        return typeBuilder.CreateType();
    }
}
