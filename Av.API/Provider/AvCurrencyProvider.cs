// Copyright (c) Abdelkader Amar. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Newtonsoft.Json.Linq;
using Av.API.Data;

namespace Av.API.Provider
{
    public class AvCurrencyProvider : AvProvider
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AvCurrencyProvider));

        public const string FROM_CURRENCY_ARG = "from_currency";
        public const string TO_CURRENCY_ARG = "to_currency";

        public const string CURRENCY_EXCHANGE_RATE_FUNC = "CURRENCY_EXCHANGE_RATE";

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

            CurrencyExchangeRate rate = null;
            if (json is JObject)
            {
                rate = CurrencyExchangeRate.FromJson((JObject)json);
            }
            if (rate == null)
            {
                log.WarnFormat("Failed to parse the result of request {0}", url);
                log.WarnFormat("The JSON content is {0}", json);
            }
            return rate;
        }
    }
}
