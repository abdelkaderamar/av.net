// Copyright (c) Abdelkader Amar. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


﻿using Av.API;
using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Av.API.Data;

namespace StockPerformance
{
    public class PerformanceManager
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public PerformanceManager()
        {
            StocksData = new Dictionary<string, StockData>();
            StocksPerformance = new Dictionary<string, StockPerformanceData>();
        }

        public Dictionary<string, StockData> StocksData { get; }

        public Dictionary<string, StockPerformanceData> StocksPerformance { get; }

        public StockPerformanceData ReferencePerformance { get; set; }

        public int FindNearest(DateTime date, StockData stockData)
        {
            var data = stockData.Data.Values.ToList<StockDataItem>();
            int i = data.Count - 1;
            while (i> 0)
            {
                if (data[i].DateTime <= date) return i;
                --i;
            }
            return 0;
        }

        public double ComputePerformance(string symbol, DateTime referenceDate)
        {
            if (!StocksData.ContainsKey(symbol)) throw new ApplicationException(symbol + " data not found");
            var stockData = StocksData[symbol];
            if (stockData.Data == null || stockData.Data.Values == null)
            {
                throw new ApplicationException(symbol + " data empty");
            }
            var first = stockData.Data.Values.ElementAt(FindNearest(referenceDate, stockData));
            var last = stockData.Data.Values.Last();
            return (last.Close - first.Close) / first.Close;
        }

        public List<StockPerformanceData> ComputeAllPerformances(string referenceStock, DateTime referenceDate)
        {
            StocksPerformance.Clear();
            ReferencePerformance = new StockPerformanceData(referenceStock)
            {
                Performance = ComputePerformance(referenceStock, referenceDate)
            };

            foreach (var data in StocksData)
            {
                var symbol = data.Key;
                try
                {
                    StockPerformanceData stockPerformance = new StockPerformanceData(symbol)
                    {
                        Performance = ComputePerformance(symbol, referenceDate)
                    };
                    StocksPerformance.Add(symbol, stockPerformance);
                }
                catch (ApplicationException e)
                {
                    Log.Error("ApplicationException when computing performances " + e.Message);
                    Log.Error(e.StackTrace);
                }
                catch (Exception e)
                {
                    Log.Error("Exception when computing performances " + e.Message);
                    Log.Error(e.StackTrace);
                }
            }

            List<StockPerformanceData> stockPerfList = new List<StockPerformanceData>(StocksPerformance.Values);
            stockPerfList.Sort();
            return stockPerfList;
        }
    }
}
