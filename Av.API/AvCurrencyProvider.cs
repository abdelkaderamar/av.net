﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Newtonsoft.Json.Linq;

namespace Av.API
{
    public class AvCurrencyProvider : AvProvider
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AvCurrencyProvider));

        public static readonly string FROM_CURRENCY_ARG = "from_currency";
        public static readonly string TO_CURRENCY_ARG = "to_currency";

        public static readonly string CURRENCY_EXCHANGE_RATE_FUNC = "CURRENCY_EXCHANGE_RATE";

        public AvCurrencyProvider(string key) : base(key)
        {
        }

        public CurrencyExchangeRate RequestExchangeRate(string fromCurrency, string toCurrency)
        {
            return RequestExchangeRateAsync(fromCurrency, toCurrency).Result;
        }

        public async Task<CurrencyExchangeRate> RequestExchangeRateAsync(string fromCurrency, string toCurrency)
        {
            var args = new List<KeyValuePair<string, string>>();
            args.Add(new KeyValuePair<string, string>(FROM_CURRENCY_ARG, fromCurrency));
            args.Add(new KeyValuePair<string, string>(TO_CURRENCY_ARG, toCurrency));

            var url = GetUrl(CURRENCY_EXCHANGE_RATE_FUNC, args);

            var json = await Request(url);

            if (json is JObject)
            {
                return CurrencyExchangeRate.FromJson((JObject)json);
            }
            log.WarnFormat("Unable to parse requet result {0}", json);
            return null;
        }
    }
}
