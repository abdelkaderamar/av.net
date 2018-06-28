﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Av.API
{
    public enum RequestType { Daily, DailyAdjusted, Weekly, WeeklyAdjusted, Monthly, MonthlyAdjusted}

    public abstract class AvProvider
    {
        public static readonly string BASE_URL = "https://www.alphavantage.co/query?";
        public static readonly string FUNC_PARAM = "function=";
        public static readonly string APIKEY_PARAM = "apikey=";
        public static readonly string SYMBOL_PARAM = "symbol=";
        public static readonly string SYMBOLS_PARAM = "symbols=";


        private string _key;

        private HttpClient _httpClient;

        public AvProvider(string key)
        {
            _key = key;
            _httpClient = new HttpClient();
        }

        public string getUrl(string symbol, string function)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(BASE_URL).Append(FUNC_PARAM).Append(function)
                .Append("&").Append(SYMBOL_PARAM).Append(symbol).Append("&")
                .Append(APIKEY_PARAM).Append(_key);

            return builder.ToString();
        }

        public string getUrl(string[] symbols, string function)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(BASE_URL).Append(FUNC_PARAM).Append(function)
                .Append("&").Append(SYMBOLS_PARAM).Append(string.Join(",", symbols)).Append("&")
                .Append(APIKEY_PARAM).Append(_key);

            return builder.ToString();
        }

        public async Task<JObject> request(String url)
        {
            string content = await request_async(url);
            var obj = JsonConvert.DeserializeObject(content);
            if (! (obj is JObject)) return null;
            JObject json = (JObject)obj;
            return json;
        }

        public string request_sync(String url)
        {
            Task<string> task = request_async(url);
            var result = task.Result;
            return result;
        }

        public async Task<string> request_async(String url)
        {
            using (HttpResponseMessage response = await _httpClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    return content;
                }
                return string.Empty;
            }
        }


    }
}
