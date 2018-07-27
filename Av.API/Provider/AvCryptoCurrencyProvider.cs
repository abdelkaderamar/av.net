// Copyright (c) Abdelkader Amar. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Newtonsoft.Json.Linq;
using Av.API.Data;

namespace Av.API.Provider
{
    public class AvCryptoCurrencyProvider : AvProvider
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AvCryptoCurrencyProvider));

        public const string MARKET_PARAM = "market=";

        public const string DIGITAL_CURRENCY_INTRADAY_FUNC = "DIGITAL_CURRENCY_INTRADAY";
        public const string DIGITAL_CURRENCY_DAILY_FUNC = "DIGITAL_CURRENCY_DAILY";
        public const string DIGITAL_CURRENCY_WEEKLY_FUNC = "DIGITAL_CURRENCY_WEEKLY";
        public const string DIGITAL_CURRENCY_MONTHLY_FUNC = "DIGITAL_CURRENCY_MONTHLY";

        public AvCryptoCurrencyProvider(string key) : base(key)
        { }

        public CryptoHistoData RequestDaily(string currency, string market)
        {

            return RequestHistoData(currency, market, DIGITAL_CURRENCY_INTRADAY_FUNC).Result;
        }

        public CryptoHistoData RequestWeekly(string currency, string market)
        {
            return RequestHistoData(currency, market, DIGITAL_CURRENCY_WEEKLY_FUNC).Result;

        }

        public CryptoHistoData RequestMonthly(string currency, string market)
        {
            return RequestHistoData(currency, market, DIGITAL_CURRENCY_MONTHLY_FUNC).Result;
        }

        protected Task<CryptoHistoData> RequestHistoData(string currency, string market, string function)
        {
            var args = new List<KeyValuePair<string, string>>();
            args.Add(new KeyValuePair<string, string>(SYMBOL_PARAM, currency));
            args.Add(new KeyValuePair<string, string>(MARKET_PARAM, market));

            var url = GetUrl(function, args);

            throw new NotImplementedException();
        }

    }
}
