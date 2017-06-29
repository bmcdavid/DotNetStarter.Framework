using DotNetStarter.Abstractions;
using DotNetStarter.Framework.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DotNetStarter.Framework.Tests
{
    [TestClass]
    public class LoggerTests
    {
        private Import<ILogger> Logger;

        [TestMethod]
        public void ShouldLogException()
        {
            Logger.Service.LogException(new NullReferenceException(), typeof(LoggerTests), ErrorLevel.Error);

            Assert.IsTrue(Logger.Service.ToString().Contains(nameof(NullReferenceException)));
        }

        [TestMethod]
        public void ShouldLogMessage()
        {
            string testMessage = "Testing Message Logging";
            Logger.Service.LogMessage(testMessage, typeof(LoggerTests), ErrorLevel.Information);

            Assert.IsTrue(Logger.Service.ToString().Contains(testMessage));
        }
    }
}