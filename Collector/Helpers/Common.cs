using Nest;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Collector.Helpers
{
    public static class Common
    {
                
        public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        // Check if the app is able to reach elasticsearch server
        public static bool CheckElasticConnection(ConnectionSettings elasticSettings)
        {
            // Create elasticsearch client
            var elasticClient = new ElasticClient(elasticSettings);
            // Check if elasticsearch is available
            var elasticHealth = elasticClient.CatHealth();
            if (elasticHealth.OriginalException != null)
            {
                Console.WriteLine("[" + DateTime.Now + "] Error, Elasticsearch cannot be reached: " + elasticHealth.OriginalException.Message + ", " + elasticHealth.OriginalException.InnerException.Message);
                return false;
            }

            return true;                
        }
    }
}
