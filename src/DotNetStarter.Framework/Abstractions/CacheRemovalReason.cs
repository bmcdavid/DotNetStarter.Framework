namespace DotNetStarter.Framework.Abstractions
{
    /// <summary>
    /// Cache removal reason
    /// </summary>
    public enum CacheRemovalReason
    {
        /// <summary>
        /// Removed
        /// </summary>
        Removed = 1,

        /// <summary>
        /// Expired
        /// </summary>
        Expired = 2,

        /// <summary>
        /// Underused
        /// </summary>
        Underused = 3,

        /// <summary>
        /// Dependency changed
        /// </summary>
        DependencyChanged = 4
    }
}