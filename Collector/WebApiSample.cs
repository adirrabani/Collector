using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Threading;
using Nest;

namespace Collector
{
    public abstract class WebApiSample : EntityBase
    {

        public string Url{ get; set; }
        public string Response { get; set; }

        // Send get request to specific url
        public bool HttpGet(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    // copy the response to Response field
                    Response = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[" + DateTime.Now + "] Problem with GET request, URL : " + url  + System.Environment.NewLine + e);
                return false;
            }
           
            return true;

        }

        public abstract bool MapFields(JObject retrievedJson);

        // Validate existing object, will be in use before starting a task
        public override bool Validate()
        {
            if (!base.Validate()) { return false; } ;

            var isValid = true;

            if (!SampleType.Equals("WebApi")) { isValid = false; }
            if (Url == null) { isValid = false; }

            return isValid;
        }

        // Running web api sample every X seconds (configurable)
        public static void StartApiSample(WebApiSample webApiSample, CancellationToken token, ConnectionSettings elasticSettings)
        {
            Console.WriteLine("[" + DateTime.Now + "] Starting " + webApiSample.SampleType + " sample, API URL : " + webApiSample.Url);
            // Create elasticsearch client
            var elasticClient = new ElasticClient(elasticSettings);
            
            int failures = 0;
            // Running until cancellation requested , task that fails 5 time in a row will be cancelled
            while (!token.IsCancellationRequested && failures < 5)
            {                
                bool getResponse = webApiSample.HttpGet(webApiSample.Url);

                // Check if response was retrieved
                if (getResponse)
                {                    
                    // Check if current object is stock in order to manipulate the response
                    if (webApiSample.GetType() == typeof(Collector.Stock)) { webApiSample.Response = webApiSample.Response.Replace("// [", "").Replace("]", ""); }

                    JObject retrievedJson = JObject.Parse(webApiSample.Response);

                    // Map the relevant fields to the object
                    webApiSample.MapFields(retrievedJson);
                    webApiSample.TimeStamp = DateTime.Now;

                    webApiSample.Response = "";                                        

                    //Console.WriteLine(webApiSample.ToString());
                    var res = elasticClient.Index(webApiSample, p => p.Index(webApiSample.IndexName));
                    
                    // If there is problem with elasticsearch server increase failures count and write the error to the console.
                    if(res.OriginalException != null)
                    {
                        Console.WriteLine("[" + DateTime.Now + "] Error when trying to insert " + webApiSample.IndexName + " data to elasticsearch: " + res.OriginalException.Message);
                        if(res.OriginalException.InnerException != null) { Console.Write(", " + res.OriginalException.InnerException.Message); }                            
                        failures += 1;
                    }
                    else { failures = 0; }
                }
                else
                {
                    failures += 1;                    
                }

                Thread.Sleep(webApiSample.Interval);
            }

        }
    }
}
