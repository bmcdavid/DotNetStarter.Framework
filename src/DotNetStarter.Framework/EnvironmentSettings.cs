using DotNetStarter.Abstractions;
using DotNetStarter.Framework.Abstractions;

namespace DotNetStarter.Framework
{
    /// <summary>
    /// CrossPlatform access for environment settings
    /// </summary>
    [Register(typeof(IEnvironmentSettings), LifeTime.Singleton)]
    public class EnvironmentSettings : IEnvironmentSettings
    {
#if NETSTANDARD1_3
        /// <summary>
        /// Constructor for netstandard
        /// </summary>
        public EnvironmentSettings(Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            EnvironmentName = env?.EnvironmentName;
            ContentRootPath = env?.ContentRootPath;
            WebRootPath = env?.WebRootPath;
            ApplicationBasePath = System.AppContext.BaseDirectory;
        }
#else

        /// <summary>
        /// Constructor for netframework
        /// </summary>
        public EnvironmentSettings()
        {
            var appDomain = System.AppDomain.CurrentDomain;
            ApplicationBasePath = appDomain.BaseDirectory;
            WebRootPath = !string.IsNullOrEmpty(appDomain.SetupInformation.PrivateBinPath) ? appDomain.BaseDirectory : null;
            ContentRootPath = WebRootPath;
            EnvironmentName = System.Configuration.ConfigurationManager.AppSettings[$"{typeof(IEnvironmentSettings).FullName}.{nameof(IEnvironmentSettings.EnvironmentName)}"]?.ToString() ?? "";
        }

#endif

        /// <summary>
        /// Full base directory path
        /// </summary>
        public virtual string ApplicationBasePath { get; }

        /// <summary>
        /// Full content root path
        /// </summary>
        public virtual string ContentRootPath { get; }

        /// <summary>
        /// Environment name
        /// </summary>
        public virtual string EnvironmentName { get; }

        /// <summary>
        /// Full webroot path
        /// </summary>
        public virtual string WebRootPath { get; }

        /// <summary>
        /// Determines if environment name is 'Development'
        /// </summary>
        /// <returns></returns>
        public virtual bool IsDevelopment() => IsEnvironment("Development");

        /// <summary>
        /// Determines if environment name matches given environment name.
        /// </summary>
        /// <param name="environmentName"></param>
        /// <returns></returns>
        public virtual bool IsEnvironment(string environmentName) => string.CompareOrdinal(EnvironmentName, environmentName) == 0;

        /// <summary>
        /// Determines if environment name is production
        /// </summary>
        /// <returns></returns>
        public virtual bool IsProduction() => IsEnvironment("Production");

        /// <summary>
        /// Determines if environment name is staging
        /// </summary>
        /// <returns></returns>
        public virtual bool IsStaging() => IsEnvironment("Staging");
    }
}