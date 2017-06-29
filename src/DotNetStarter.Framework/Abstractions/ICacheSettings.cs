namespace DotNetStarter.Framework.Abstractions
{
    /// <summary>
    /// Cache manager settings
    /// </summary>
    public interface ICacheSettings
    {
        /// <summary>
        /// Is caching enabled
        /// </summary>
        bool Enabled { get; }

        /// <summary>
        /// Shortest interval
        /// </summary>
        double QuickInterval { get; }

        /// <summary>
        /// default interval
        /// </summary>
        double DefaultInterval { get; }

        /// <summary>
        /// longest interval
        /// </summary>
        double LongInterval { get; }
    }
}