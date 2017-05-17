# Collector
Application that collect data from web apis and write it to elastic search, other sample methods can be added as well (performance monitor samples for example).

Current integrations :
1. Google finance API in order to retrieve stocks data.
2. Openweathermap API in order to retrieve weather data, api key is necessary in order to use this API.


Configuration files:
- App.config : Conatains general settings for the application.
  ```xml
  <appSettings>
		<add key="ElasticAddress" value="http://localhost:9200" />	
		<add key="StocksGeneralUrl" value="https://www.google.com/finance/info?q={StockName}" />
		<add key="WeatherGeneralUrl" value="http://api.openweathermap.org/data/2.5/weather?q={cityName}&amp;units=metric&amp;appid={API Key}" />				
	</appSettings>
  ```
  1. ElasticAddress - elasticsearch server and port
  2. StocksGeneralUrl - Base URL for stock samples, should not be changed since the app change {StockName} during execution.
  3. WeatherGeneralUrl - Base URL for weather samples, {API Key} should be replace with relevant API key from Openweathermap, the will change {cityName} during the execution.

Prerequisites :
1. Elasticearch server up and running.
2. API key for Openweathermap.
