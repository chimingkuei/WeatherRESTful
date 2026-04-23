using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WeatherRESTful
{
    public class WeatherService
    {
        // 在 .NET 4.8 中，HttpClient 建議宣告為 static 避免 Socket 耗盡
        private static readonly HttpClient _client = new HttpClient();
        private readonly string _apiKey;

        public WeatherService(string apiKey)
        {
            _apiKey = apiKey;
        }

        /// <summary>
        /// 取得指定城市的天氣資訊
        /// </summary>
        public async Task GetWeatherAsync(string city)
        {
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric&lang=zh_tw";

            try
            {
                HttpResponseMessage response = await _client.GetAsync(url);
                string result = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var data = JObject.Parse(result);

                    // 提取資料
                    string cityName = data["name"]?.ToString();
                    double temp = data["main"]?["temp"]?.Value<double>() ?? 0;
                    // 新增：提取體感溫度
                    double feelsLike = data["main"]?["feels_like"]?.Value<double>() ?? 0;
                    int humidity = data["main"]?["humidity"]?.Value<int>() ?? 0;
                    string description = data["weather"]?[0]?["description"]?.ToString();

                    // 格式化輸出
                    Console.WriteLine("------------------------------");
                    Console.WriteLine($"【{cityName}】當前氣象");
                    Console.WriteLine($"氣溫：{temp} °C");
                    Console.WriteLine($"體感：{feelsLike} °C"); // 新增輸出
                    Console.WriteLine($"濕度：{humidity} %");
                    Console.WriteLine($"描述：{description}");
                    Console.WriteLine("------------------------------");
                }
                else
                {
                    Console.WriteLine($"[失敗] 伺服器回傳代碼: {response.StatusCode}");
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"[網路錯誤] 請檢查連線或 API 地址: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[系統錯誤] {ex.Message}");
            }
        }

        /// <summary>
        /// 梧棲區 lat︰24.2562、lon︰120.5332
        /// </summary>
        public async Task GetWeatherByCoordsAsync(double lat, double lon)
        {
            // 修改 URL 為經緯度模式 (lat={lat}&lon={lon})
            string url = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={_apiKey}&units=metric&lang=zh_tw";

            try
            {
                HttpResponseMessage response = await _client.GetAsync(url);
                string result = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var data = JObject.Parse(result);

                    // 提取資料
                    string cityName = data["name"]?.ToString();
                    double temp = data["main"]?["temp"]?.Value<double>() ?? 0;
                    double feelsLike = data["main"]?["feels_like"]?.Value<double>() ?? 0;
                    int humidity = data["main"]?["humidity"]?.Value<int>() ?? 0;
                    string description = data["weather"]?[0]?["description"]?.ToString();

                    // 格式化輸出
                    Console.WriteLine("------------------------------");
                    Console.WriteLine($"【{cityName} (座標:{lat}, {lon})】區域氣象");
                    Console.WriteLine($"氣溫：{temp} °C");
                    Console.WriteLine($"體感：{feelsLike} °C");
                    Console.WriteLine($"濕度：{humidity} %");
                    Console.WriteLine($"描述：{description}");
                    Console.WriteLine("------------------------------");
                }
                else
                {
                    Console.WriteLine($"[失敗] 伺服器回傳代碼: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[系統錯誤] {ex.Message}");
            }
        }
    }
}
