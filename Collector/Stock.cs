using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Collector
{
    public class Stock : WebApiSample
    {
        
        public Stock(JToken sampleType, JToken indexName, JToken isActive, string generalUrl, JToken stockName, JToken interval)
        {
            this.SampleType = sampleType.ToString();
            this.IndexName = indexName.ToString();
            this.IsActive = Convert.ToBoolean(isActive);
            this.Url = generalUrl.Replace("{StockName}", stockName.ToString());
            this.Interval = (int)interval;            
        }

        public string Ticker { get; set; }
        public string Market { get; set; }
        public double LastPrice { get; set; }
        public double Change { get; set; }
        public double ChangePercent { get; set; }
        public DateTime? UpdateTime { get; set; }


        public override bool MapFields(JObject StockData)
        {            
            Ticker = StockData["t"].ToString();
            Market = StockData["e"].ToString();
            LastPrice = (double)StockData["l"];
            Change = (double)StockData["c"];
            ChangePercent = (double)StockData["cp"];
            UpdateTime = (DateTime)StockData["lt_dts"];

            return true;
        }

        public override string ToString()
        {
            return String.Format("Sample ID: {0}, Ticker: {1}, Market:{2}, LastPrice:{3}, UpdateTime:{4}", SampleId, Ticker, Market, LastPrice, Change);
        }
    }
}
