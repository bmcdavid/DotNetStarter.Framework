namespace DotNetStarter.Framework.Abstractions
{
    /// <summary>
    /// Caching priority
    /// </summary>
    public enum CachePriority
    {
        /// <summary>
        /// Default
        /// </summary>
        Default = 0,

        /// <summary>
        /// High
        /// </summary>
        High = 100,

        /// <summary>
        /// Low
        /// </summary>
        Low = 200,

        /// <summary>
        /// Cannot be removed
        /// </summary>
        NotRemovable = 300
    }
}