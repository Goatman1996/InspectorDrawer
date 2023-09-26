using System.Collections.Generic;

namespace GMToolKit.Inspector
{
    internal class InspectorDrawerCache
    {
        private static InspectorDrawerCache _instance;
        public static InspectorDrawerCache Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new InspectorDrawerCache();
                    _instance.OnCreate();
                }
                return _instance;
            }
        }

        private Dictionary<string, DrawerCacheObject> cache;

        private void OnCreate()
        {
            cache = new Dictionary<string, DrawerCacheObject>();
        }

        public void Set(string feildName, object value)
        {
            if (!this.cache.ContainsKey(feildName))
            {
                this.cache.Add(feildName, new DrawerCacheObject());
            }
            this.cache[feildName].Set(value);
        }

        public T Get<T>(string fieldName)
        {
            var obj = this.Get(fieldName);
            if (obj == null)
            {
                return default(T);
            }
            return (T)obj;
        }

        public object Get(string fieldName)
        {
            if (!this.cache.ContainsKey(fieldName))
            {
                return null;
            }
            return this.cache[fieldName].Get();
        }
    }

    public class DrawerCacheObject
    {
        private object value;

        public void Set(object value)
        {
            this.value = value;
        }

        public object Get()
        {
            return value;
        }
    }
}