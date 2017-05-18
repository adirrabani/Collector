using Nest;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Collector.Helpers
{
    class Sample
    {
        // Function that read configuration from config files and return list of samples
        public static JArray GetSamples()
        {
            JArray samples = null;
            try
            {
                // Get samples configuration from settings.json
                JObject configuration = JObject.Parse(File.ReadAllText(@"settings.json"));
                samples = (JArray)configuration["configuration"]["samples"];                
            }
            catch (Exception e)
            {
                Console.WriteLine("[" + DateTime.Now + "] Error when opening settings.json file : " + System.Environment.NewLine + e);
            }
            // If samples wasn't retrieved return null
            return samples;
        }

        public static List<EntityBase> PrepareSamples(string stocksGeneralUrl, string weatherGeneralUrl)
        {
            // Get all samples from config files
            JArray samples = Sample.GetSamples();

            List<EntityBase> sampleObjects = new List<EntityBase>();

            if (samples != null)
            {                
                // running on each sample, create relevant object and add it to list of sample objects
                foreach (var sample in samples)
                {
                    // Handling stock objects
                    if (sample["indexName"].ToString().Equals("stocks"))
                    {

                        try
                        {
                            // Preparing stock object with the data that was retrieved from the configuration file
                            Stock stock = new Stock(sample["type"], sample["indexName"], sample["isActive"], stocksGeneralUrl, sample["stockTicker"], sample["interval"]);
                            sampleObjects.Add(stock);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("[" + DateTime.Now + "] Error when trying to prepare stock object : " + System.Environment.NewLine + e);
                        }

                    }

                    // Handling weather objects
                    else if (sample["indexName"].ToString().Equals("weather"))
                    {
                        try
                        {
                            Weather weather = new Weather(sample["type"], sample["indexName"], sample["isActive"], weatherGeneralUrl, sample["cityName"], sample["interval"]);
                            sampleObjects.Add(weather);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("[" + DateTime.Now + "] Error when trying to prepare weather object : " + System.Environment.NewLine + e);
                        }                        
                    }
                }

            }
            return sampleObjects;
        }

        // Create and start task for each sample
        public static bool StartSamples(List<EntityBase> sampleObjects, ConnectionSettings elasticSettings)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            var token = cts.Token;

            foreach (var sample in sampleObjects)
            {                
                // Currently handling only WebApi samples
                if (sample.SampleType.Equals("WebApi") && sample.Validate())
                {
                    // Add sample id for each valid sample
                    EntityBase.Id += 1;
                    sample.SampleId = EntityBase.Id;

                    // Create and start a task
                    Task t = Task.Factory.StartNew(
                    () =>
                    {
                        WebApiSample.StartApiSample(sample as WebApiSample, token, elasticSettings);
                    },
                    token,
                    TaskCreationOptions.LongRunning,
                    TaskScheduler.Default
                );                    
                }
            }
            return true;
        }
    }
}
