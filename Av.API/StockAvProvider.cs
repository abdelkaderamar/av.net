using System;
using System.Collections.Generic;
using System.Text;

namespace Av.API
{
    public class StockAvProvider : AvProvider
    {
        public static readonly string DAILY_FUNC = "TIME_SERIES_DAILY";
        public static readonly string WEEKLY_FUNC = "TIME_SERIES_WEEKLY";
        public static readonly string MONTHLY_FUNC = "TIME_SERIES_MONTHLY";

        public StockAvProvider(string key) : base(key)
        {
        }

        public string requestDaily(string symbol)
        {
            var url = getUrl(symbol, DAILY_FUNC);
            return request(url);
        }

        public string requestWeekly(string symbol)
        {
            var url = getUrl(symbol, WEEKLY_FUNC);
            return request(url);
        }

        public string requestMonthly(string symbol)
        {
            var url = getUrl(symbol, MONTHLY_FUNC);
            return request(url);
        }

    }
}
