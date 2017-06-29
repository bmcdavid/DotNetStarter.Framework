namespace DotNetStarter.Framework.Abstractions
{
    /// <summary>
    /// Environment settings for netframework and netcoreapps
    /// </summary>
    public interface IEnvironmentSettings
    {
        /// <summary>
        /// Environment name
        /// </summary>
        string EnvironmentName { get; }

        /// <summary>
        /// Full webroot path of environment
        /// </summary>
        string WebRootPath { get; }

        /// <summary>
        /// Full content root path of environment
        /// </summary>
        string ContentRootPath { get; }

        /// <summary>
        /// Full application base path of environment
        /// </summary>
        string ApplicationBasePath { get; }

        /// <summary>
        /// Determines if current environment matches given environment.
        /// </summary>
        /// <param name="environmentName"></param>
        /// <returns></returns>
        bool IsEnvironment(string environmentName);

        /// <summary>
        /// Determines if environment is 'Staging'
        /// </summary>
        /// <returns></returns>
        bool IsStaging();

        /// <summary>
        /// Determines if environ,ent is 'Development'
        /// </summary>
        /// <returns></returns>
        bool IsDevelopment();

        /// <summary>
        /// Determines if environ,ent is 'Production'
        /// </summary>
        /// <returns></returns>
        bool IsProduction();
    }
}