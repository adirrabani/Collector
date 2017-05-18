# Collector
Application that collect data from web apis and write it to elasticsearch, other sample methods can be added as well (performance monitor samples for example).

**Current integrations:**
1. Google finance API in order to retrieve stocks data.
2. Openweathermap API in order to retrieve weather data, api key is necessary in order to use this API.


**Configuration files:**
- App.config : Conatains general settings for the application.
  ```xml
  <appSettings>
		<add key="ElasticAddress" value="http://localhost:9200" />	
		<add key="StocksGeneralUrl" value="https://www.google.com/finance/info?q={StockName}" />
		<add key="WeatherGeneralUrl" value="http://api.openweathermap.org/data/2.5/weather?q={cityName}&amp;units=metric&amp;appid={API Key}" />				
	</appSettings>
  ```
  1. _ElasticAddress_ - elasticsearch server and port
  2. _StocksGeneralUrl_ - Base URL for stock samples, should not be changed since the app will change {StockName} during the execution.
  3. _WeatherGeneralUrl_ - Base URL for weather samples, ```{API Key}``` should be replaced with relevant API key from Openweathermap, the app will change {cityName} during the execution.

- settings.json : Json file that conatains configuration of each sample (stock/weather).
```json
  {
  "configuration": {
    "samples": [
      {
        "type": "WebApi",
        "isActive": "true",
        "indexName": "stocks",
        "stockTicker": "GD",
        "interval": "60000"
      },
      {
        "type": "WebApi",
        "isActive": "true",
        "indexName": "weather",
        "cityName": "berlin",
        "interval": "60000"
      }
    ]
  }
}
```
  1. _type_ : Specifies sample type, should not be changed since stock/weather samples are Web API samples.
  2. _isActive_ : in order to enable or disable specific sample.
  3. _indexName_ : Specifies which sample is this (stock/weather).
  4. _stockTicker/cityName_ : Specifies which stock/city we want to sample.  
  5. _interval_ : Specifies the interval between sample to sample in milliseconds. 
  
**Prerequisites :**
1. Elasticearch server up and running.
2. API key for Openweathermap.

**Example of Kibana Dashboard - Weather Samples**
![Weather Dashboard](http://i.imgur.com/BDyjXc5.png)


