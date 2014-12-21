using System;

namespace Eca.Commons.Caching
{
    public class CacheInfo
    {
        #region Properties

        public string CacheName { get; set; }
        public string InstanceName { get; set; }
        public bool IsEnabled { get; set; }
        public int ItemCount { get; set; }
        public DateTime LastAdd { get; set; }
        public DateTime LastRetrieve { get; set; }
        public DateTime LastUse { get; set; }

        #endregion
    }
}