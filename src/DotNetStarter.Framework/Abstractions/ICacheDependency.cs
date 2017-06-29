namespace DotNetStarter.Framework.Abstractions
{
    using System.Collections.Generic;

    /// <summary>
    /// Cache dependency
    /// </summary>
    public interface ICacheDependency
    {
        /// <summary>
        /// For extensible usage to handle unkown dependency types
        /// </summary>
        object CustomDependency { get; set; }

        /// <summary>
        /// Dependent files
        /// </summary>
        IEnumerable<string> FileNames { get; set; }

        /// <summary>
        /// Dependent cache keys
        /// </summary>
        IEnumerable<string> CacheKeys { get; set; }

        /// <summary>
        /// Sub dependency
        /// </summary>
        ICacheDependency SubCacheDependency { get; set; }
    }
}