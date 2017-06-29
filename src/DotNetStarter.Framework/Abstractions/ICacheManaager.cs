namespace DotNetStarter.Framework.Abstractions
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides access to cached data
    /// </summary>
    public interface ICacheManager
    {
        /// <summary>
        /// Reference to container cache settings
        /// </summary>
        ICacheSettings Settings { get; }

        /// <summary>
        /// Cache item with given Func&lt;object>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">string key</param>
        /// <param name="cacheFunction"></param>
        /// <param name="interval">time in seconds to cache</param>
        /// <param name="dependencies"></param>
        /// <param name="priority"></param>
        /// <param name="removalCallback"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        T CacheItem<T>
        (
            string key,
            Func<object> cacheFunction,
            double interval = 30,
            ICacheDependency dependencies = null,
            CachePriority priority = CachePriority.Default,
            Action<string, object, CacheRemovalReason> removalCallback = null,
            ICacheSettings settings = null
        );

        /// <summary>
        /// Remove cached item of given key
        /// </summary>
        /// <param name="key"></param>
        object Remove(string key);

        /// <summary>
        /// Removes all items in the cache and returns them as a dictionary, or null if no keys match prefix
        /// </summary>
        /// <param name="keyPrefix"></param>
        IDictionary<string, object> RemoveAll(string keyPrefix = null);

        /// <summary>
        /// Gets a typed cached item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T GetItem<T>(string key);

        /// <summary>
        /// Gets all cache keys in use
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetAllKeys();
    }
}