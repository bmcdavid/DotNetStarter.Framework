using DotNetStarter.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace DotNetStarter.Framework.Tests
{
    [TestClass]
    public class _Setup
    {
        [AssemblyInitialize]
        public static void InitialAssembly(TestContext context)
        {
            var testAssemblies = new Assembly[]
            {
                typeof(DotNetStarter.ApplicationContext).GetTypeInfo().Assembly,
                typeof(DotNetStarter.Abstractions.IAssemblyFilter).GetTypeInfo().Assembly,
                typeof(DotNetStarter.DryIocLocator).GetTypeInfo().Assembly,
                typeof(DotNetStarter.Framework.EnvironmentSettings).GetTypeInfo().Assembly,
                typeof(_Setup).GetTypeInfo().Assembly
            };

            DotNetStarter.ApplicationContext.Startup(assemblies: testAssemblies);
        }
    }
}