﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Av.API
{
    public abstract class AvProvider
    {
        public static readonly string BASE_URL = "https://www.alphavantage.co/query?";
        public static readonly string FUNC_PARAM = "function=";
        public static readonly string APIKEY_PARAM = "apikey=";
        public static readonly string SYMBOL_PARAM = "symbol=";


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

        public JObject request(String url)
        {
            string content = request_async(url).Result;
            var obj = JsonConvert.DeserializeObject(content);
            if (! (obj is JObject)) return null;
            JObject json = (JObject)obj;
            return json;
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
