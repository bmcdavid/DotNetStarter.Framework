#if NETCOREAPP1_1

using System;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Hosting;
using DotNetStarter.Abstractions;

namespace DotNetStarter.Framework.Tests.Mocks
{
    [Register(typeof(IHostingEnvironment), LifeTime.Singleton)]
    public class HostingEnvironment : Microsoft.AspNetCore.Hosting.IHostingEnvironment
    {
        public string EnvironmentName { get; set; } = "MockEnv";
        public string ApplicationName { get; set; }
        public string WebRootPath { get; set; }
        public IFileProvider WebRootFileProvider { get; set; }
        public string ContentRootPath { get; set; }
        public IFileProvider ContentRootFileProvider { get; set; }
    }
}
#endif