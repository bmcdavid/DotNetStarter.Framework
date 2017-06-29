using DotNetStarter.Abstractions;
using DotNetStarter.Framework.Abstractions;
using System;

namespace DotNetStarter.Framework.Tests.Mocks
{
    [Register(typeof(IEnvironmentSettings), LifeTime.Singleton, ConstructorType.Greediest, typeof(DotNetStarter.Framework.EnvironmentSettings))]
    public class TestEnvironmentSettings : IEnvironmentSettings
    {
        public string EnvironmentName => "Test";

        public string WebRootPath => throw new NotImplementedException();

        public string ContentRootPath => throw new NotImplementedException();

        public string ApplicationBasePath => throw new NotImplementedException();

        public bool IsDevelopment()
        {
            throw new NotImplementedException();
        }

        public bool IsEnvironment(string environmentName)
        {
            return string.CompareOrdinal(environmentName, this.EnvironmentName) == 0;
        }

        public bool IsProduction()
        {
            throw new NotImplementedException();
        }

        public bool IsStaging()
        {
            throw new NotImplementedException();
        }
    }
}