using DotNetStarter.Abstractions;
using DotNetStarter.Framework.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading;

namespace DotNetStarter.Framework.Tests
{
    [TestClass]
    public class CacheTests
    {
        private static volatile bool CalledBack = false;
        private Import<ICacheManager> CacheManager;

        [TestMethod]
        public void ShouldCacheValueType()
        {
            var key = nameof(ShouldCacheValueType);
            DateTime o = CacheManager.Service.CacheItem<DateTime>(key, () => new DateTime(2000, 10, 1), 1);
            var cached = CacheManager.Service.GetItem<DateTime>(key);

            Assert.AreEqual(o, cached);
        }

        [TestMethod]
        public void ShouldCacheObject()
        {
            var key = nameof(ShouldCacheObject);
            object o = CacheManager.Service.CacheItem<object>(key, () => new System.Text.StringBuilder("hello"), 1);
            var cached = CacheManager.Service.GetItem<object>(key);

            Assert.AreEqual(o, cached);
        }

        [TestMethod]
        public void ShouldGetAllCacheKeys()
        {
            InsertCacheItems(nameof(ShouldRemoveAllCachedItems));
            var sut = CacheManager.Service.GetAllKeys();

            Assert.IsTrue(sut?.Any() == true);
        }

        [TestMethod]
        public void ShouldGetCacheManager()
        {
            Assert.IsNotNull(CacheManager);
        }

        [TestMethod]
        public void ShouldCacheNullData()
        {
            object x = null;
            CacheManager.Service.CacheItem<object>(nameof(ShouldCacheNullData), () => x, 500);
            object sut = CacheManager.Service.GetItem<object>(nameof(ShouldCacheNullData));

            Assert.IsNull(sut);
        }

        [TestMethod]
        public void ShouldInvokeRemovalCallback()
        {
            var key = nameof(ShouldInvokeRemovalCallback);
            object o = CacheManager.Service.CacheItem<object>(key, () => new object(), interval: 100, removalCallback: RemovalCallback);
            CacheManager.Service.Remove(key); // force removal
            Thread.Sleep(5);

            Assert.IsTrue(CalledBack);
        }

        [TestMethod]
        public void ShouldRemoveAllCachedItems()
        {
            InsertCacheItems(nameof(ShouldRemoveAllCachedItems));
            var sut = CacheManager.Service.RemoveAll(nameof(ShouldRemoveAllCachedItems));

            Assert.IsTrue(sut?.Count > 1);
        }

        private void InsertCacheItems(string prefix)
        {
            for (int i = 0; i < 5; i++)
            {
                CacheManager.Service.CacheItem<object>(prefix + i, () => new object(), interval: 1000);
            }
        }

        private void RemovalCallback(string key, object o, CacheRemovalReason reason)
        {
            CalledBack = true;
        }
    }
}