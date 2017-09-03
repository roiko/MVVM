using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RESTClient
{
	public class Client
	{
		//weather example: http://api.openweathermap.org/data/2.5/weather?lat=45.7982741&lon=9.0954442&APPID=f9c37aafc23e975a2625e2119a9219d9
		//const string URL_weather = "http://api.openweathermap.org/data/2.5/";
		const string URL_weather = "http://146.185.181.89/data/2.5/";
		const string API_KEY_weather = "f9c37aafc23e975a2625e2119a9219d9";

		HttpClient _httpClient;

		public Client()
		{
			_httpClient = new HttpClient();
		}

		public async Task<WeatherResult> GetCurrentWeather(double latitude, double longitude)
		{
			try
			{
				WeatherResult weather = new WeatherResult();
				string lat = latitude.ToString().Replace(",", ".");
				string lon = longitude.ToString().Replace(",", ".");
				string completeUrl = string.Format("{0}weather?lat={1}&lon={2}&APPID={3}", URL_weather, lat, lon, API_KEY_weather);
				_httpClient.Timeout = new TimeSpan(0, 0, 30);
				string jsonResult = await _httpClient.GetStringAsync(completeUrl);
				var jsonParsed = Newtonsoft.Json.Linq.JObject.Parse(jsonResult);
				weather.Main = jsonParsed.SelectToken("weather[0].main").ToString();
				weather.Description = jsonParsed.SelectToken("weather[0].description").ToString();
				return weather;
			}
			catch (Exception ex)
			{
				throw (ex);
			}

		}
	}
}
