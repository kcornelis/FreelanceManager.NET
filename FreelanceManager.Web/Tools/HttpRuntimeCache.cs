using System;
using System.Web;

namespace FreelanceManager.Web.Tools
{
    interface ICacheService
    {
        T Get<T>(string cacheID, Func<T> getItemCallback) where T : class;
        void Insert<T>(string cacheID, T value) where T : class;
    }

    public class HttpRuntimeCache : ICacheService
    {
        public T Get<T>(string cacheID, Func<T> getItemCallback) where T : class
        {
            T item = HttpRuntime.Cache.Get(cacheID) as T;
            if (item == null)
            {
                item = getItemCallback();
                HttpRuntime.Cache.Insert(cacheID, item);
            }
            return item;
        }

        public T Get<T>(string cacheID, Func<T> getItemCallback, DateTime absoluteExpiration) where T : class
        {
            T item = HttpRuntime.Cache.Get(cacheID) as T;
            if (item == null)
            {
                item = getItemCallback();
                HttpRuntime.Cache.Insert(cacheID, item, null, absoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration);
            }
            return item;
        }

        public void Insert<T>(string cacheID, T value) where T : class
        {
            HttpRuntime.Cache.Insert(cacheID, value);
        }

        public void Remove(string cacheId)
        {
            HttpRuntime.Cache.Remove(cacheId);
        }
    }
}