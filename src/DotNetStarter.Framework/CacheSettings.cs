namespace DotNetStarter.Framework
{
    using Abstractions;
    using DotNetStarter.Abstractions;

    /// <summary>
    /// Cache Settings
    /// </summary>
    [Register(typeof(ICacheSettings), LifeTime.Singleton)]
    public class CacheSettings : ICacheSettings
    {
        /// <summary>
        /// Default interval of 300 seconds
        /// </summary>
        public virtual double DefaultInterval => 300;

        /// <summary>
        /// Default enabled is true
        /// </summary>
        public virtual bool Enabled => true;

        /// <summary>
        /// Default long interval is 3600 seconds
        /// </summary>
        public virtual double LongInterval => 3600;

        /// <summary>
        /// Default quick interval is 30 seconds
        /// </summary>
        public virtual double QuickInterval => 30;
    }
}