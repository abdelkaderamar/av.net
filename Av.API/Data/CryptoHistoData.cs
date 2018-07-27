// Copyright (c) Abdelkader Amar. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Av.API.Data
{
    public class CryptoHistoData
    {
        public CryptoHistoData(string currency, string market)
        {
            Currency = currency;
            Market = market;
            Data = new SortedDictionary<DateTime, CryptoDataItem>();
        }

        #region Main properties
        public string Currency { get; }

        public string Market { get; }

        public IDictionary<DateTime, CryptoDataItem> Data { get; }
        #endregion

        #region Additional properties
        public string Information { get; set; }

        public string CurrencyName { get; set; }

        public string MarketName { get; set; }

        public DateTime LastRefreshed { get; set; }

        public string TimeZone { get; set; }

        #endregion

        public struct CryptoDataItem
        {
            public DateTime _date;
            public decimal _open;
            public decimal _openUSD;
            public decimal _high;
            public decimal _highUSD;
            public decimal _low;
            public decimal _lowUSD;
            public decimal _close;
            public decimal _closeUSD;
            public decimal _volume;
            public decimal _marketCapUSD;
        }
    }
}
