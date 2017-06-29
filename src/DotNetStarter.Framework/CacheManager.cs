namespace DotNetStarter.Framework
{
    using Abstractions;
    using DotNetStarter.Abstractions;
    using System;
    using System.Collections.Generic;
    using System.Linq;

#if NET35
    using System.Web;
    using System.Web.Caching;
#elif NET40 || NET45
    using System.Runtime.Caching;
#elif NETSTANDARD1_3
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Options;
    using System.Collections.Concurrent;
#endif

#if NETSTANDARD1_3
    /// <summary>
    /// to fix dryioc constuctor should not be static error
    /// </summary>
    internal static class StandardCacheHelper
    {
        internal static ConcurrentDictionary<string, string> CacheKeys = new ConcurrentDictionary<string, string>();
    }
#endif

    /// <summary>
    /// Default implementation using HttpRuntime
    /// </summary>
    [Register(typeof(ICacheManager), LifeTime.Singleton)]
    public class CacheManager : ICacheManager
    {
        private IReflectionHelper _ReflectionHelper;

#if NET35 || NET40 || NET45

        /// <summary>
        /// Constructor with injected cache settings
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="reflectionHelper"></param>
        public CacheManager(ICacheSettings settings, IReflectionHelper reflectionHelper)
        {
            Settings = settings;
            _ReflectionHelper = reflectionHelper;
        }

#elif NETSTANDARD1_3
        private IMemoryCache _MemoryCache;

        /// <summary>
        /// Constructor with injected cache settings
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="reflectionHelper"></param>
        /// <param name="memoryCacheOptions"></param>
        public CacheManager(ICacheSettings settings, IReflectionHelper reflectionHelper, IOptions<MemoryCacheOptions> memoryCacheOptions = null)
        {
            Settings = settings;
            _ReflectionHelper = reflectionHelper;
            _MemoryCache = new MemoryCache(memoryCacheOptions ?? new OptionsWrapper<MemoryCacheOptions>(new MemoryCacheOptions()));
        }
#endif

        /// <summary>
        /// Reference to container cache settings
        /// </summary>
        public ICacheSettings Settings { get; }

        /// <summary>
        /// Cache item with given Func&lt;object>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="cacheFunction"></param>
        /// <param name="interval"></param>
        /// <param name="dependencies"></param>
        /// <param name="priority"></param>
        /// <param name="removalCallback"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public virtual T CacheItem<T>(string key, Func<object> cacheFunction, double interval = 30, ICacheDependency dependencies = null,
            CachePriority priority = CachePriority.Default, Action<string, object, CacheRemovalReason> removalCallback = null, ICacheSettings settings = null)
        {
            if (cacheFunction == null)
                return default(T);

            settings = settings ?? Settings;
            bool cacheEnabled = settings.Enabled;

            // Handle Structs
            if (_ReflectionHelper.IsValueType(typeof(T)))
            {
                // Boxing needed to avoid nullable checks on structs
                object o = GetCrossPlatformCacheData(key);
                T cachedValue = default(T);

                if (o != null)
                    cachedValue = (T)o;

                // !EnableCache allows previewing to disable per user
                if (!cacheEnabled || o == null)
                {
                    cachedValue = (T)cacheFunction();

                    if (cacheEnabled && interval > 0)
                    {
                        SetCrossPlatformCache(key, cachedValue, interval, dependencies, priority, removalCallback);
                    }
                }

                return cachedValue;
            }

            // Handle Classes
            var cachedData = GetCrossPlatformCacheData(key);

            // !EnableCache allows previewing to disable per user
            if (!cacheEnabled || cachedData == null)
            {
                cachedData = cacheFunction();

                // If cached data is bad, store for a minute.
                if (cachedData == null)
                {
                    interval = 60;
                    cachedData = new NullData();
                }

                if (cacheEnabled && interval > 0)
                    SetCrossPlatformCache(key, cachedData, interval, dependencies, priority, removalCallback);
            }

            return (cachedData is NullData) ? default(T) : (T)cachedData;
        }

        /// <summary>
        /// Gets all cached keys
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<string> GetAllKeys() => EnumerateCrossPlatformCacheKeys();

        /// <summary>
        /// Gets a typed item from cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual T GetItem<T>(string key)
        {
            object CacheData = GetCrossPlatformCacheData(key);

            return (CacheData == null || CacheData is NullData) ? default(T) : (T)CacheData;
        }

        /// <summary>
        /// Removes a single key from cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual object Remove(string key) => RemoveCrossPlatformCacheData(key);

        /// <summary>
        /// Removes all items in the cache and returns them as a dictionary, or null if no keys match prefix
        /// </summary>
        /// <param name="keyPrefix"></param>
        /// <returns></returns>
        public virtual IDictionary<string, object> RemoveAll(string keyPrefix = null)
        {
            List<string> cacheKeys = EnumerateCrossPlatformCacheKeys().ToList();
            var cachedData = new Dictionary<string, object>();

            if (cacheKeys.Count < 1)
            {
                return null;
            }

            foreach (string cacheKey in cacheKeys)
            {
                if (keyPrefix == null || cacheKey.StartsWith(keyPrefix, StringComparison.CurrentCultureIgnoreCase))
                {
                    cachedData[cacheKey] = RemoveCrossPlatformCacheData(cacheKey);
                }
            }

            return cachedData;
        }

#if NET40 || NET45
        /// <summary>
        /// Converts cache priority to implementations priority
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        protected virtual CacheItemPriority ConvertPriority(CachePriority priority)
        {
            switch (priority)
            {
                case CachePriority.NotRemovable:
                    return CacheItemPriority.NotRemovable;

                case CachePriority.Low:
                case CachePriority.High:
                case CachePriority.Default:
                default:
                    return CacheItemPriority.Default;
            }
        }

        /// <summary>
        /// Converts implementation removal to abstraction
        /// </summary>
        /// <param name="reason"></param>
        /// <returns></returns>
        protected virtual CacheRemovalReason ConvertRemovedReason(CacheEntryRemovedReason reason)
        {
            switch (reason)
            {
                case CacheEntryRemovedReason.ChangeMonitorChanged:
                    return CacheRemovalReason.DependencyChanged;

                case CacheEntryRemovedReason.Expired:
                    return CacheRemovalReason.Expired;

                case CacheEntryRemovedReason.Removed:
                    return CacheRemovalReason.Removed;
            }

            return CacheRemovalReason.Expired;
        }
#endif

#if NET35

        /// <summary>
        /// Converts cache priority to implementations priority
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        protected virtual CacheItemPriority ConvertPriority(CachePriority priority)
        {
            switch (priority)
            {
                case CachePriority.Low:
                    return CacheItemPriority.Low;

                case CachePriority.High:
                    return CacheItemPriority.High;

                case CachePriority.NotRemovable:
                    return CacheItemPriority.NotRemovable;

                case CachePriority.Default:
                default:
                    return CacheItemPriority.Default;
            }
        }

        /// <summary>
        /// Converts abstraction to implementation
        /// </summary>
        /// <param name="dependency"></param>
        /// <returns></returns>
        protected virtual CacheDependency ConvertDependency(ICacheDependency dependency)
        {
            if (dependency == null) return null;

            return new CacheDependency(dependency.FileNames?.ToArray(), dependency.CacheKeys?.ToArray(), dependency.SubCacheDependency != null ? ConvertDependency(dependency.SubCacheDependency) : null);
        }

        /// <summary>
        /// Converts implementation removal to abstraction
        /// </summary>
        /// <param name="reason"></param>
        /// <returns></returns>
        protected virtual CacheRemovalReason ConvertRemovedReason(CacheItemRemovedReason reason)
        {
            switch (reason)
            {
                case CacheItemRemovedReason.DependencyChanged:
                    return CacheRemovalReason.DependencyChanged;

                case CacheItemRemovedReason.Expired:
                    return CacheRemovalReason.Expired;

                case CacheItemRemovedReason.Removed:
                    return CacheRemovalReason.Removed;

                case CacheItemRemovedReason.Underused:
                    return CacheRemovalReason.Underused;
            }

            return CacheRemovalReason.Expired;
        }

#endif

#if NETSTANDARD1_3

        /// <summary>
        /// Converts implementation removal to abstraction
        /// </summary>
        /// <param name="reason"></param>
        /// <returns></returns>
        protected static CacheRemovalReason ConvertRemovedReason(EvictionReason reason)
        {
            switch (reason)
            {
                case EvictionReason.TokenExpired:
                    return CacheRemovalReason.DependencyChanged;

                case EvictionReason.Expired:
                    return CacheRemovalReason.Expired;

                case EvictionReason.Removed:
                    return CacheRemovalReason.Removed;

                case EvictionReason.None:
                    return CacheRemovalReason.Underused;
            }

            return CacheRemovalReason.Expired;
        }

        /// <summary>
        /// Converts cache priority to implementations priority
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        protected virtual CacheItemPriority ConvertPriority(CachePriority priority)
        {
            switch (priority)
            {
                case CachePriority.Low:
                    return CacheItemPriority.Low;

                case CachePriority.High:
                    return CacheItemPriority.High;

                case CachePriority.NotRemovable:
                    return CacheItemPriority.NeverRemove;

                case CachePriority.Default:
                default:
                    return CacheItemPriority.Normal;
            }
        }

        private static void PostEvictionDelegateWrapper(object key, object value, EvictionReason reason, object state, Action<string, object, CacheRemovalReason> removalCallback)
        {
            string s;
            bool removed = StandardCacheHelper.CacheKeys.TryRemove(key.ToString(), out s); // track independently of memory cache
            removalCallback?.Invoke(key.ToString(), value, ConvertRemovedReason(reason));
        }
#endif

        /// <summary>
        /// Used to cache nulls for a brief interval
        /// </summary>
        private class NullData { }

        /// <summary>
        /// Common get cache key cross platform
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected virtual object GetCrossPlatformCacheData(string key)
        {
#if NET35
            return HttpRuntime.Cache[key];
#elif NET40 || NET45
            return MemoryCache.Default[key];
#elif NETSTANDARD1_3
            object o = null;
            _MemoryCache.TryGetValue(key, out o);

            return o;
#endif
        }

        /// <summary>
        /// Common get cache key cross platform
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected virtual object RemoveCrossPlatformCacheData(string key)
        {
#if NET35
            return HttpRuntime.Cache.Remove(key);
#elif NET40 || NET45
            return MemoryCache.Default.Remove(key);
#elif NETSTANDARD1_3
            object o = null;
            _MemoryCache.TryGetValue(key, out o);
            _MemoryCache.Remove(key);

            return o;
#endif
        }

        /// <summary>
        /// Common enumeration for cache keys
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<string> EnumerateCrossPlatformCacheKeys()
        {
#if NET35
            var enumerator = HttpRuntime.Cache.GetEnumerator();

            while (enumerator.MoveNext())
            {
                yield return enumerator.Key.ToString();
            }
#elif NET40 || NET45
            return MemoryCache.Default.Select(x => x.Key);
#elif NETSTANDARD1_3
            return StandardCacheHelper.CacheKeys?.Select(x => x.Key) ?? Enumerable.Empty<string>();
#endif
        }

        /// <summary>
        /// Cross platform cache set/add/insert
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cachedValue"></param>
        /// <param name="interval"></param>
        /// <param name="dependencies"></param>
        /// <param name="priority"></param>
        /// <param name="removalCallback"></param>
        /// <param name="settings"></param>
        protected virtual void SetCrossPlatformCache(string key, object cachedValue, double interval = 30, ICacheDependency dependencies = null,
            CachePriority priority = CachePriority.Default, Action<string, object, CacheRemovalReason> removalCallback = null, ICacheSettings settings = null)
        {
#if NET35
            HttpRuntime.Cache.Insert(key, cachedValue, ConvertDependency(dependencies), DateTime.UtcNow.AddSeconds(interval), Cache.NoSlidingExpiration,
                ConvertPriority(priority), (cKey, cData, reason) => removalCallback?.Invoke(cKey, cData, ConvertRemovedReason(reason)));
#elif NET40 || NET45
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.Priority = ConvertPriority(priority);
            policy.AbsoluteExpiration = DateTime.UtcNow.AddSeconds(interval);
            policy.RemovedCallback = (args) => removalCallback?.Invoke(args.CacheItem?.Key, args?.CacheItem.Value, ConvertRemovedReason(args.RemovedReason));

            if (dependencies?.FileNames?.Any() == true)
            {
                policy.ChangeMonitors.Add(new HostFileChangeMonitor(dependencies?.FileNames.ToList()));
            }

            if (dependencies?.CacheKeys?.Any() == true)
            {
                policy.ChangeMonitors.Add(MemoryCache.Default.CreateCacheEntryChangeMonitor(dependencies?.CacheKeys.ToList()));
            }

            MemoryCache.Default.Set(key, cachedValue, policy);
#elif NETSTANDARD1_3
            // store in the cache
            _MemoryCache.Set
            (
                key,
                cachedValue,
                new MemoryCacheEntryOptions()
                {
                    Priority = ConvertPriority(priority),
                    AbsoluteExpiration = DateTime.UtcNow.AddSeconds(interval)
                }
                .RegisterPostEvictionCallback((key2, value, reason, state) => PostEvictionDelegateWrapper(key2, value, reason, state, removalCallback))
            );

            //todo: how to handle cache dependencies?
            if (dependencies != null)
                throw new NotSupportedException("ICacheDependencies not supported in netcore at this time!");

            StandardCacheHelper.CacheKeys.TryAdd(key, key); // track independently of memory cache
#endif
        }
    }
}