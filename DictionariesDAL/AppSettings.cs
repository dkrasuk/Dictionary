using System;
using System.Configuration;

namespace DictionariesDAL
{
    public class AppSettings
    {
        public const int DefaultCacheUpdateDurationPeriodInMinutes = 5;

        public static int CacheUpdateDurationPeriodInMinutes
        {
            get
            {
                try
                {
                    return Convert.ToInt32(ConfigurationManager.AppSettings["CacheUpdateDurationPeriodInMinutes"]);
                }
                catch (Exception)
                {
                    return DefaultCacheUpdateDurationPeriodInMinutes;
                }
            }
        }

    }
}
