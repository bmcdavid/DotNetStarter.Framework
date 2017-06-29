using DotNetStarter.Abstractions;
using DotNetStarter.Framework.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DotNetStarter.Framework.Tests
{
    [TestClass]
    public class LoggerTests
    {
        Import<ILogger> Logger;

        [TestMethod]
        public void ShouldLogException()
        {
            Logger.Service.LogException(new NullReferenceException(), typeof(LoggerTests), ErrorLevel.Error);

            Assert.IsTrue(Logger.Service.ToString().Contains(nameof(NullReferenceException)));
        }
    }
}
