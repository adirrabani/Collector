using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Collector.Helpers;


namespace Collector
{
    public class Weather : WebApiSample
    {        
        public Weather(JToken sampleType, JToken indexName, JToken isActive, string generalUrl, JToken cityName, JToken interval)
        {
            this.SampleType = sampleType.ToString();
            this.IndexName = indexName.ToString();
            this.IsActive = Convert.ToBoolean(isActive);
            this.Url = generalUrl.Replace("{cityName}", cityName.ToString());           
            this.Interval = (int)interval;
        }

        public string CityName { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }        
        public DateTime? Sunrise { get; set; }
        public DateTime? Sunset { get; set; }
        public DateTime? CalcDate { get; set; }
        
        private double _coordLongitude { get; set; }
        private double _coordLatitude { get; set; }
        // Create location string for elasticsearch
        public string Location
        {
            get { return _coordLatitude + "," + _coordLongitude; }
        }

        public override bool MapFields(JObject WeatherData)
        {
            CityName = WeatherData["name"].ToString();
            Temperature = (double)WeatherData["main"]["temp"];
            Humidity = (double)WeatherData["main"]["humidity"];
            _coordLongitude = (double)WeatherData["coord"]["lon"];
            _coordLatitude = (double)WeatherData["coord"]["lat"];
            Sunrise = Common.UnixTimeStampToDateTime((long)WeatherData["sys"]["sunrise"]);
            Sunset  = Common.UnixTimeStampToDateTime((long)WeatherData["sys"]["sunset"]);
            CalcDate = Common.UnixTimeStampToDateTime((long)WeatherData["dt"]);
            
            return true;
        }

        public override string ToString()
        {
            return String.Format("Sample ID: {0}, CityName: {1}, Temperature:{2}, Humidity:{3}, CalcDate:{4}", SampleId, CityName, Temperature, Humidity, CalcDate);
        }
    }
}
