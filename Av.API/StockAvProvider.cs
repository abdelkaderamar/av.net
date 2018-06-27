using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace Av.API
{
    public class StockAvProvider : AvProvider
    {
        #region Functions constants
        public static readonly string DAILY_FUNC = "TIME_SERIES_DAILY";
        public static readonly string WEEKLY_FUNC = "TIME_SERIES_WEEKLY";
        public static readonly string MONTHLY_FUNC = "TIME_SERIES_MONTHLY";
        #endregion

        #region JSON keys
        public static readonly string META_DATA = "Meta Data";
        public static readonly string DAILY_TIME_SERIES = "Time Series (Daily)";
        public static readonly string WEEKLY_TIME_SERIES = "Weekly Time Series";
        public static readonly string MONTHLY_TIME_SERIES = "Monthly Time Series";

        public static readonly string SYMBOL_KEY = "2. Symbol";

        public static readonly string OPEN_KEY = "1. open";
        public static readonly string HIGH_KEY = "2. high";
        public static readonly string LOW_KEY = "3. low";
        public static readonly string CLOSE_KEY = "4. close";
        public static readonly string VOLUME_KEY = "5. volume";
        #endregion

        #region AlphaVantage format
        public static readonly string AV_DATE_FORMAT = "yyyy-MM-dd";
        #endregion

        public StockAvProvider(string key) : base(key)
        {
        }

        public StockData requestDaily(string symbol, bool full = false)
        {
            var stockData = requestDailyAsync(symbol, full).Result;
            return stockData;
        }

        public async Task<StockData> requestDailyAsync(string symbol, bool full = false)
        {
            var url = getUrl(symbol, DAILY_FUNC);
            if (full) url += "&outputsize=full";

            var json = await request(url);

            var stockData = requestData(json, DAILY_TIME_SERIES);

            return stockData;
        }


        public StockData requestWeekly(string symbol)
        {
            var url = getUrl(symbol, WEEKLY_FUNC);
            var json = request(url).Result;

            var stockData = requestData(json, WEEKLY_TIME_SERIES);

            return stockData;

        }

        public StockData requestMonthly(string symbol)
        {
            var url = getUrl(symbol, MONTHLY_FUNC);
            var json = request(url).Result;

            var stockData = requestData(json, MONTHLY_TIME_SERIES);

            return stockData;
        }

        protected StockData requestData(JObject json, string timeSeriesKey)
        {
            bool containAllKeys = json.ContainsKey(META_DATA) && json.ContainsKey(timeSeriesKey);
            if (!containAllKeys) return null;

            var metaData = (JObject)json.GetValue(META_DATA);
            var timeSeries = json.GetValue(timeSeriesKey);

            string symbol = getValue(metaData, SYMBOL_KEY);
            if (string.IsNullOrEmpty(symbol)) return null; 

            StockData stockData = new StockData(symbol);

            foreach(var data in timeSeries.Children<JProperty>())
            {
                if (!(data.Value is JObject)) continue;
                JObject jdata = (JObject)(data.Value);

                try
                {
                    DateTime dateTime = DateTime.ParseExact(data.Name, AV_DATE_FORMAT, CultureInfo.InvariantCulture);

                    var open = getDoubleValue(jdata, OPEN_KEY);
                    var high = getDoubleValue(jdata, HIGH_KEY);
                    var low = getDoubleValue(jdata, LOW_KEY);
                    var close = getDoubleValue(jdata, CLOSE_KEY);
                    var volume = getLongValue(jdata, VOLUME_KEY);

                    StockDataItem dataItem = new StockDataItem(dateTime)
                    {
                        Open = open,
                        High = high,
                        Low = low,
                        Close = close,
                        Volume = volume
                    };

                    stockData.addDataItem(dataItem);
                }
                catch (Exception)
                {
                    // TODO
                }
            } // end foreach
            return stockData;
        }

        static string getValue(JObject json, string property)
        {
            if (json.ContainsKey(property)) return json.GetValue(property).ToString();
            return string.Empty;
        }

        static double getDoubleValue(JObject json, string property, double defaultValue = 0.0)
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

        static long getLongValue(JObject json, string property, long defaultValue = 0)
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
