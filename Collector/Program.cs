using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using Collector.Helpers;
using System.Collections.Specialized;
using Nest;


namespace Collector
{
    class Program
    {
        static void Main(string[] args)
        {            

            NameValueCollection appSettings = null;
            try
            {
                // Get general settings from app.config
                appSettings = ConfigurationManager.AppSettings;
            }
            catch (Exception e)
            {
                Console.WriteLine("[" + DateTime.Now + "] Error, Collector cannot be started since configuration couldn't be retrieved : " + e);
                Console.ReadLine();
            }


            // App will continue only if configuration was retrieved from config file
            if (appSettings.Count != 0)
            {                
                //Console.WriteLine(appSettings["ElasticAddress"]);
                var elasticSettings = new ConnectionSettings(new Uri(appSettings["ElasticAddress"]));
                // App will continue only if Elasticsearch is available
                if (!Common.CheckElasticConnection(elasticSettings))
                {
                    Console.WriteLine("[" + DateTime.Now + "] Collector cannot be started since elasticsearch cannot be reached, Please check that elasticsearch server is up and running.");
                    Console.ReadLine();
                }
                else
                {
                    Console.WriteLine("[" + DateTime.Now + "] Collector started" + Environment.NewLine + "[" + DateTime.Now + "] Collector is connected to elasticsearch server - " + appSettings["ElasticAddress"]);
                    string stocksGeneralUrl = appSettings["StocksGeneralUrl"];
                    string weatherGeneralUrl = appSettings["WeatherGeneralUrl"];
                    
                    // Create list that contains the sample objects.
                    List<EntityBase> sampleObjects = new List<EntityBase>();
                    sampleObjects = Sample.PrepareSamples(stocksGeneralUrl, weatherGeneralUrl);

                    // Start sampling if sample objects was retrieved succufully
                    if (sampleObjects != null) { Sample.StartSamples(sampleObjects, elasticSettings); }

                    Console.ReadLine();
                }                
            }
            else
            {
                Console.WriteLine("[" + DateTime.Now + "] Error, App cannot be started since configuration couldn't be retrieved.");
                Console.ReadLine();
            }

        }
    }
}
