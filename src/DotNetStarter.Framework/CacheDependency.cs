namespace DotNetStarter.Framework
{
    using Abstractions;
    using DotNetStarter.Abstractions;
    using System.Collections.Generic;

    /// <summary>
    /// Cache dependency
    /// </summary>
    [Register(typeof(ICacheDependency), LifeTime.Transient)]
    public class DefaultCacheDependency : ICacheDependency
    {
        /// <summary>
        /// Dependent cache keys
        /// </summary>
        public virtual IEnumerable<string> CacheKeys { get; set; }

        /// <summary>
        /// For extensible usage to handle unkown dependency types
        /// </summary>
        public object CustomDependency { get; set; }

        /// <summary>
        /// Dependent files
        /// </summary>
        public IEnumerable<string> FileNames { get; set; }

        /// <summary>
        /// Sub dependency
        /// </summary>
        public virtual ICacheDependency SubCacheDependency { get; set; }
    }
}