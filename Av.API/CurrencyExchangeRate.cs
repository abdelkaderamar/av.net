using System;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Av.API
{
    /*
    {
        "Realtime Currency Exchange Rate": {
            "1. From_Currency Code": "BTC",
            "2. From_Currency Name": "Bitcoin",
            "3. To_Currency Code": "CNY",
            "4. To_Currency Name": "Chinese Yuan",
            "5. Exchange Rate": "41214.50945560",
            "6. Last Refreshed": "2018-07-12 08:05:55",
            "7. Time Zone": "UTC"
        }
    }
    */
    public class CurrencyExchangeRate
    {
        #region JSON keys
        public static readonly string REALTIME_CURRENCY_EXCHANGE_RATE_KEY = "Realtime Currency Exchange Rate";
        public static readonly string FROM_CURRENCY_CODE_KEY = "1. From_Currency Code";
        public static readonly string FROM_CURRENCY_NAME_KEY = "2. From_Currency Name";
        public static readonly string TO_CURRENCY_CODE_KEY = "3. To_Currency Code";
        public static readonly string TO_CURRENCY_NAME_KEY = "4. To_Currency Name";
        public static readonly string EXCHANGE_RATE_KEY = "5. Exchange Rate";
        public static readonly string LAST_REFRESHED_KEY = "6. Last Refreshed";
        public static readonly string TIME_ZONE_KEY = "7. Time Zone";
        #endregion

        public static readonly string LAST_REFRESHED_FORMAT = "yyyy-MM-dd hh:mm:ss";

        public CurrencyExchangeRate(string fromCurrCode, string toCurrCode)
        {
            FromCurrencyCode = fromCurrCode;
            ToCurrencyCode = toCurrCode;
        }

        public string FromCurrencyCode { get; }

        public string FromCurrencyName { get; set; }

        public string ToCurrencyCode { get; }

        public string ToCurrencyName { get; set; }

        public decimal ExchangeRate { get; set; }

        public DateTime LastRefreshed { get; set; }

        public string TimeZone { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Currency Exchange Rate of {0}/{1} to {2}/{3} = {4}", 
                FromCurrencyName, FromCurrencyCode, ToCurrencyName, ToCurrencyCode, ExchangeRate).Append(Environment.NewLine);
            sb.AppendFormat("\tTimestamp = {0}, TimeZone = {1}", LastRefreshed, TimeZone);
            return sb.ToString();
        }

        public static CurrencyExchangeRate FromJson(JObject parentJson)
        {
            var jtoken = parentJson.GetValue(REALTIME_CURRENCY_EXCHANGE_RATE_KEY);
            if (!(jtoken is JObject)) return null;

            JObject json = jtoken as JObject;
            string from = JsonHelper.GetValue(json, FROM_CURRENCY_CODE_KEY);
            string to = JsonHelper.GetValue(json, TO_CURRENCY_CODE_KEY);

            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to)) return null;

            CurrencyExchangeRate currExchangeRate = new CurrencyExchangeRate(from, to)
            {
                ExchangeRate = JsonHelper.GetDecimalValue(json, EXCHANGE_RATE_KEY),
                FromCurrencyName = JsonHelper.GetValue(json, FROM_CURRENCY_NAME_KEY),
                ToCurrencyName = JsonHelper.GetValue(json, TO_CURRENCY_CODE_KEY),
                TimeZone = JsonHelper.GetValue(json, TIME_ZONE_KEY),
                LastRefreshed = JsonHelper.GetDateTimeValue(json, LAST_REFRESHED_KEY)
            };
            try
            {
                currExchangeRate.LastRefreshed = JsonHelper.GetDateTimeValue(json, LAST_REFRESHED_KEY, LAST_REFRESHED_FORMAT);
            }
            catch (Exception) { return null; }

            return currExchangeRate;
        }
    }
}
