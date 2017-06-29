using DotNetStarter.Abstractions;
using DotNetStarter.Framework.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace DotNetStarter.Framework.Tests
{
    [TestClass]
    public class EnvironmentTests
    {
        private Import<IEnvironmentSettings> Environment;

        [TestMethod]
        public void ShouldBeTestEnvironment()
        {
            Assert.IsTrue(Environment.Service.IsEnvironment("Test"));
        }

        [TestMethod]
        public void ShouldHaveMultipleSettings()
        {
            var settings = Environment.AllServices;

            Assert.IsTrue(settings.Count() > 1);
        }
    }
}