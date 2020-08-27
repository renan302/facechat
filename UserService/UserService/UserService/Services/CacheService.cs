using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace UserService.Services
{
    public class CacheService
    {
        private IDistributedCache _cache;

        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public T GetValue<T>(string key)
        {
            string cache = _cache.GetString(key);
            if (cache != null) 
            {
                var obj = JsonConvert.DeserializeObject<T>(cache);
                return obj;
            }
            else
            {
                return default(T);
            }
        }

        public void SetValue(string key, Object value)
        {
            DistributedCacheEntryOptions opcoesCache =
                   new DistributedCacheEntryOptions();

            opcoesCache.SetAbsoluteExpiration(
            TimeSpan.FromMinutes(15));

            string output = JsonConvert.SerializeObject(value);

            _cache.SetString(key, output, opcoesCache);
        }
    }
}
