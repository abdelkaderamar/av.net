// Copyright (c) Abdelkader Amar. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace Av.API
{
    public enum StockRequestType { Daily, DailyFull, DailyAdjusted, DailyAdjustedFull, Weekly, WeeklyAdjusted, Monthly, MonthlyAdjusted}

    public enum CurrencyRequestType { ExchangeRate }

    public abstract class AvProvider
    {
        public static readonly string BASE_URL = "https://www.alphavantage.co/query?";
        public static readonly string FUNC_PARAM = "function=";
        public static readonly string APIKEY_PARAM = "apikey=";
        public static readonly string SYMBOL_PARAM = "symbol=";
        public static readonly string SYMBOLS_PARAM = "symbols=";

        private static readonly ILog log = LogManager.GetLogger(typeof(AvProvider));

        private string _key;

        private HttpClient _httpClient;

        public AvProvider(string key)
        {
            _key = key;
            _httpClient = new HttpClient();
        }

        public string GetUrl(string symbol, string function)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(BASE_URL).Append(FUNC_PARAM).Append(function)
                .Append("&").Append(SYMBOL_PARAM).Append(symbol).Append("&")
                .Append(APIKEY_PARAM).Append(_key);
            var url = builder.ToString();
            log.DebugFormat("Get Url of {0} | {1} = {2}", symbol, function, url);
            return url;
        }

        public string GetUrl(string[] symbols, string function)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(BASE_URL).Append(FUNC_PARAM).Append(function)
                .Append("&").Append(SYMBOLS_PARAM).Append(string.Join(",", symbols)).Append("&")
                .Append(APIKEY_PARAM).Append(_key);

            var url = builder.ToString();
            log.DebugFormat("Get Url of {0} | {1} = {2}", string.Join(",", symbols), function, url);
            return url;
        }

        public string GetUrl(string function, IList<KeyValuePair<string, string>> args)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(BASE_URL).Append(FUNC_PARAM).Append(function);
            foreach (var kvp in args)
            {
                builder.Append("&").Append(kvp.Key).Append("=").Append(kvp.Value);
            }
            builder.Append("&").Append(APIKEY_PARAM).Append(_key);
            var url = builder.ToString();
            log.DebugFormat("Get Url of {0} = {1}", function, url);
            return url;
        }

        public async Task<JObject> Request(String url)
        {
            string content = await RequestAsync(url);
            var obj = JsonConvert.DeserializeObject(content);
            if (!(obj is JObject))
            {
                log.Warn("Received object is not a valid JSON object");
                log.Warn(obj);
                return null;
            }
            JObject json = (JObject)obj;
            return json;
        }

        public string RequestSync(String url)
        {
            Task<string> task = RequestAsync(url);
            var result = task.Result;
            return result;
        }

        public async Task<string> RequestAsync(String url)
        {
            using (HttpResponseMessage response = await _httpClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    return content;
                }
                log.WarnFormat("The request to {0} failed with error code {1}", url, response.StatusCode);
                return string.Empty;
            }
        }


    }
}
