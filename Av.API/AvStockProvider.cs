using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using log4net;

namespace Av.API
{
    public class AvStockProvider : AvProvider
    {
        #region Functions constants
        public static readonly string DAILY_FUNC = "TIME_SERIES_DAILY";
        public static readonly string WEEKLY_FUNC = "TIME_SERIES_WEEKLY";
        public static readonly string MONTHLY_FUNC = "TIME_SERIES_MONTHLY";
        public static readonly string BATCH_FUNC = "BATCH_STOCK_QUOTES";
        #endregion

        #region JSON keys
        public static readonly string META_DATA = "Meta Data";
        public static readonly string DAILY_TIME_SERIES = "Time Series (Daily)";
        public static readonly string WEEKLY_TIME_SERIES = "Weekly Time Series";
        public static readonly string MONTHLY_TIME_SERIES = "Monthly Time Series";
        public static readonly string STOCK_QUOTES = "Stock Quotes";

        public static readonly string SYMBOL_KEY = "2. Symbol";

        public static readonly string OPEN_KEY = "1. open";
        public static readonly string HIGH_KEY = "2. high";
        public static readonly string LOW_KEY = "3. low";
        public static readonly string CLOSE_KEY = "4. close";
        public static readonly string VOLUME_KEY = "5. volume";
        #endregion

        #region AlphaVantage format
        public static readonly string AV_DATE_FORMAT = "yyyy-MM-dd";
        public static readonly string AV_DATETIME_FORMAT = "yyyy-MM-dd HH:mm:ss";
        #endregion

        private static readonly ILog log = LogManager.GetLogger(typeof(AvStockProvider));

        public AvStockProvider(string key) : base(key)
        {
        }

        public StockData RequestDaily(string symbol, bool full = false)
        {
            var stockData = RequestDailyAsync(symbol, full).Result;
            return stockData;
        }

        public async Task<StockData> RequestDailyAsync(string symbol, bool full = false)
        {
            var url = GetUrl(symbol, DAILY_FUNC);
            if (full) url += "&outputsize=full";

            var json = await Request(url);

            var stockData = RequestData(json, DAILY_TIME_SERIES);

            if (stockData == null)
            {
                log.WarnFormat("Failed to parse the result of request {0}", url);
                log.WarnFormat("The JSON content is {0}", json);
            }

            return stockData;
        }


        public StockData RequestWeekly(string symbol)
        {
            var url = GetUrl(symbol, WEEKLY_FUNC);
            var json = Request(url).Result;

            var stockData = RequestData(json, WEEKLY_TIME_SERIES);

            if (stockData == null)
            {
                log.WarnFormat("Failed to parse the result of request {0}", url);
                log.WarnFormat("The JSON content is {0}", json);
            }

            return stockData;

        }

        public StockData requestMonthly(string symbol)
        {
            var url = GetUrl(symbol, MONTHLY_FUNC);
            var json = Request(url).Result;

            var stockData = RequestData(json, MONTHLY_TIME_SERIES);

            if (stockData == null)
            {
                log.WarnFormat("Failed to parse the result of request {0}", url);
                log.WarnFormat("The JSON content is {0}", json);
            }

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

            foreach(var data in timeSeries.Children<JProperty>())
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
