// Copyright (c) Abdelkader Amar. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using log4net;
using Av.API.Data;

namespace Av.API.Provider
{
    public class AvStockProvider : AvProvider
    {
        #region Functions constants
        public const string DAILY_FUNC = "TIME_SERIES_DAILY";
        public const string WEEKLY_FUNC = "TIME_SERIES_WEEKLY";
        public const string MONTHLY_FUNC = "TIME_SERIES_MONTHLY";
        public const string BATCH_FUNC = "BATCH_STOCK_QUOTES";
        #endregion

        #region JSON keys
        public const string META_DATA = "Meta Data";
        public const string DAILY_TIME_SERIES = "Time Series (Daily)";
        public const string WEEKLY_TIME_SERIES = "Weekly Time Series";
        public const string MONTHLY_TIME_SERIES = "Monthly Time Series";
        public const string STOCK_QUOTES = "Stock Quotes";

        public const string SYMBOL_KEY = "2. Symbol";

        public const string OPEN_KEY = "1. open";
        public const string HIGH_KEY = "2. high";
        public const string LOW_KEY = "3. low";
        public const string CLOSE_KEY = "4. close";
        public const string VOLUME_KEY = "5. volume";
        #endregion

        #region AlphaVantage format
        public const string AV_DATE_FORMAT = "yyyy-MM-dd";
        public const string AV_DATETIME_FORMAT = "yyyy-MM-dd HH:mm:ss";
        #endregion

        private static readonly ILog log = LogManager.GetLogger(typeof(AvStockProvider));

        public AvStockProvider(string key) : base(key)
        {
        }

        public StockData RequestDaily(string symbol, bool full = false)
        {
            try
            {
                var stockData = RequestDailyAsync(symbol, full).Result;
                return stockData;
            }
            catch (AggregateException e)
            {
                if (e.InnerException is HighUsageException)
                {
                    log.WarnFormat("Highusage exception when requestion daily data for {0}", symbol);
                    throw e.InnerException;
                }
            }
            return null;
        }

        public async Task<StockData> RequestDailyAsync(string symbol, bool full = false)
        {
            var url = GetUrl(symbol, DAILY_FUNC);
            if (full) url += "&outputsize=full";

            var json = await Request(url);

            var stockData = RequestData(json, DAILY_TIME_SERIES);

            CheckStockData(stockData, url, json, symbol);

            return stockData;
        }


        public StockData RequestWeekly(string symbol)
        {
            var url = GetUrl(symbol, WEEKLY_FUNC);
            var json = Request(url).Result;

            var stockData = RequestData(json, WEEKLY_TIME_SERIES);

            CheckStockData(stockData, url, json, symbol);

            return stockData;

        }

        public StockData requestMonthly(string symbol)
        {
            var url = GetUrl(symbol, MONTHLY_FUNC);
            var json = Request(url).Result;

            var stockData = RequestData(json, MONTHLY_TIME_SERIES);

            CheckStockData(stockData, url, json, symbol);

            return stockData;
        }

        public IDictionary<string, StockRealtime> BatchRequest(string[] symbols)
        {
            var url = GetUrl(symbols, BATCH_FUNC);
            var json = Request(url).Result;

            return RequestBatchData(json);
        }

        protected StockData RequestData(JObject json, string timeSeriesKey)
        {
            bool containAllKeys = json.ContainsKey(META_DATA) && json.ContainsKey(timeSeriesKey);
            if (!containAllKeys)
            {
                log.WarnFormat("{0} or {1} keys are missing in the resulting JSON", META_DATA, timeSeriesKey);
                return null;
            }

            var metaData = (JObject)json.GetValue(META_DATA);
            var timeSeries = json.GetValue(timeSeriesKey);

            string symbol = GetValue(metaData, SYMBOL_KEY);
            if (string.IsNullOrEmpty(symbol))
            {
                log.WarnFormat("Missing symbol (key is {0})", SYMBOL_KEY);
                return null;
            }

            StockData stockData = new StockData(symbol);

            foreach (var data in timeSeries.Children<JProperty>())
            {
                if (!(data.Value is JObject)) continue;
                JObject jdata = (JObject)(data.Value);

                try
                {
                    DateTime dateTime = DateTime.ParseExact(data.Name, AV_DATE_FORMAT, CultureInfo.InvariantCulture);

                    var open = GetDoubleValue(jdata, OPEN_KEY);
                    var high = GetDoubleValue(jdata, HIGH_KEY);
                    var low = GetDoubleValue(jdata, LOW_KEY);
                    var close = GetDoubleValue(jdata, CLOSE_KEY);
                    var volume = GetLongValue(jdata, VOLUME_KEY);

                    StockDataItem dataItem = new StockDataItem(dateTime)
                    {
                        Open = open,
                        High = high,
                        Low = low,
                        Close = close,
                        Volume = volume
                    };

                    stockData.AddDataItem(dataItem);
                }
                catch (Exception e)
                {
                    log.ErrorFormat("Exception when parsing json data {0}", e.StackTrace);
                }
            } // end foreach
            return stockData;
        }

        public IDictionary<string, StockRealtime> RequestBatchData(JObject json)
        {
            IDictionary<string, StockRealtime> realtimes = new Dictionary<string, StockRealtime>();

            bool containAllKeys = json.ContainsKey(META_DATA) && json.ContainsKey(STOCK_QUOTES);
            if (!containAllKeys)
            {
                log.WarnFormat("{0} or {1} keys are missing in the resulting JSON", META_DATA, STOCK_QUOTES);
                return null;
            }

            JToken jtoken = json.GetValue(STOCK_QUOTES);
            if (jtoken is JArray)
            {
                JArray stockQuotes = (jtoken as JArray);
                foreach (var stockQuote in stockQuotes.Children<JObject>())
                {
                    string symbol = GetValue(stockQuote, BatchKeys.SYMBOL);
                    double price = GetDoubleValue(stockQuote, BatchKeys.PRICE);
                    long volume = GetLongValue(stockQuote, BatchKeys.VOLUME);
                    DateTime timestamp = DateTime.ParseExact(GetValue(stockQuote, BatchKeys.TIMESTAMP),
                        AV_DATETIME_FORMAT, CultureInfo.InvariantCulture);
                    realtimes[symbol] = new StockRealtime(symbol) { Price = price, Volume = volume, Timestamp = timestamp };
                }
            }
            return realtimes;
        }

        protected void CheckStockData(StockData stockData, string url, JObject json, string symbol)
        {
            if (stockData == null)
            {
                log.WarnFormat("Failed to parse the result of request {0}", url);
                log.WarnFormat("The JSON content is {0}", json);
                CheckHighUsage(json, symbol);
            }
        }

        protected void CheckHighUsage(JObject json, string symbol)
        {
            var props = json.Children<JProperty>();
            foreach (var prop in props)
            {
                if (prop.Value.ToString().Contains("if you would like to have a higher API call volume"))
                    throw new HighUsageException("High Usage error when requesting daily data for " + symbol);
            }
        }

        static string GetValue(JObject json, string property)
        {
            if (json.ContainsKey(property)) return json.GetValue(property).ToString();
            return string.Empty;
        }

        static double GetDoubleValue(JObject json, string property, double defaultValue = 0.0)
        {
            if (json.ContainsKey(property))
            {
                string str = json.GetValue(property).ToString();
                double value;
                if (Double.TryParse(str, NumberStyles.Number, CultureInfo.InvariantCulture, out value))
                    return value;
            }
            return defaultValue;
        }

        static long GetLongValue(JObject json, string property, long defaultValue = 0)
        {
            if (json.ContainsKey(property))
            {
                string str = json.GetValue(property).ToString();
                long value;
                if (long.TryParse(str, NumberStyles.Number, CultureInfo.InvariantCulture, out value))
                    return value;
            }
            return defaultValue;
        }

    }
}
