using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeTests
{
    [TestFixture]
    internal class ServiceCollectionTests
    {
        [Test]
        public void TestKeyedServiceDI()
        {
            var a = new ServiceCollection();
            a.AddSingleton<ITestService,TestService1>();
            a.AddSingleton<TestComponent>();
            a.AddSingleton<ITestService, TestService2>();
            var b = new DefaultServiceProviderFactory().CreateServiceProvider(a);
            a.AddSingleton<ITestService, TestService3>();
            var c = b.GetServices<ITestService>();
            Assert.IsTrue(c.Count() == 3);
        }

        [Test]
        public void TestSimpleInjector()
        {
            var a = new Container();
        }
    }

    internal class TestService1 : ITestService { }

    internal class TestService3 : ITestService { }

    internal class TestService2 : ITestService
    {
        private readonly TestComponent _test1;
        public TestService2(TestComponent test1)
        {
            _test1 = test1;
        }
    }

    internal class TestComponent
    {

    }

    internal interface ITestService { }
}
