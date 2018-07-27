﻿// Copyright (c) Abdelkader Amar. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Av.API.Data
{
    public enum CryptoHistoDataType { Intraday, Daily, Weekly, Monthly };

    public class CryptoHistoData : AvData
    {
        #region JSON keys
        public const string META_DATA = "Meta Data";
        public const string INTRADY_TIME_SERIES = "Time Series (Digital Currency Intraday)";
        public const string DAILY_TIME_SERIES = "Time Series (Digital Currency Daily)";
        public const string WEEKLY_TIME_SERIES = "Time Series (Digital Currency Weekly)";
        public const string MONTHLY_TIME_SERIES = "Time Series (Digital Currency Monthly)";

        public const string DIGITAL_CURRENCY_CODE_KEY = "2. Digital Currency Code";
        public const string MARKET_CODE_KEY = "4. Market Code";

        /*
            "1a. open (ZAR)": "89720.47743110",
            "1b. open (USD)": "6535.06844094",
            "2a. high (ZAR)": "116562.17949600",
            "2b. high (USD)": "8786.16113151",
            "3a. low (ZAR)": "85536.87871940",
            "3b. low (USD)": "6406.38451049",
            "4a. close (ZAR)": "110655.65617300",
            "4b. close (USD)": "8349.80555512",
            "5. volume": "9419.42850037",
            "6. market cap (USD)": "69484110.88869999"
        */
        public const string VOLUME_KEY = "5. volume";
        public const string MARKET_CAP_KEY = "6. market cap (USD)";
        #endregion

        public CryptoHistoData(CryptoHistoDataType dataType)
        {
            DataType = dataType;
        }

        public override void Init(JObject json)
        {
            string timeSeriesKey = GetTypeJsonKey(DataType);
            bool containAllKeys = json.ContainsKey(META_DATA) && json.ContainsKey(timeSeriesKey);
            if (!containAllKeys)
                throw new FormatException("JSON result cannot be parsed (missing main properties");

            var metaData = (JObject)json.GetValue(META_DATA);
            var timeSeries = json.GetValue(timeSeriesKey);

            // TODO
            Currency = JsonHelper.GetValue(metaData, DIGITAL_CURRENCY_CODE_KEY);
            Market = JsonHelper.GetValue(metaData, MARKET_CODE_KEY);
            Data = new SortedDictionary<DateTime, CryptoDataItem>();

            foreach(var entry in timeSeries.Children<JProperty>())
            {
                Console.WriteLine(entry.Name);
                JObject entryData = entry.Value as JObject;
                CryptoDataItem dataItem = default(CryptoDataItem);
                if (ParseTimeSeriesEntry(ref dataItem, entryData))
                {
                    Data[dataItem._date] = dataItem;
                }
            }
        }

        public static bool ParseTimeSeriesEntry(ref CryptoDataItem dataItem, JObject jobject)
        {
            dataItem._volume = JsonHelper.GetDecimalValue(jobject, VOLUME_KEY);
            dataItem._marketCapUSD = JsonHelper.GetDecimalValue(jobject, MARKET_CAP_KEY);

            return true;
        }

        #region Main properties
        public CryptoHistoDataType DataType { get; }

        public string Currency { get; set; }

        public string Market { get; set; }

        public IDictionary<DateTime, CryptoDataItem> Data { get; set; }
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

        public static string GetTypeJsonKey(CryptoHistoDataType dataType)
        {
            switch (dataType)
            {
                case CryptoHistoDataType.Intraday: return INTRADY_TIME_SERIES;
                case CryptoHistoDataType.Daily: return DAILY_TIME_SERIES;
                case CryptoHistoDataType.Weekly: return WEEKLY_TIME_SERIES;
                case CryptoHistoDataType.Monthly: return MONTHLY_TIME_SERIES;
                default:
                    throw new NotSupportedException("DataType not supported " + dataType);
            }
        }
    }
}
