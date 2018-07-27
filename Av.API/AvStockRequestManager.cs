// Copyright (c) Abdelkader Amar. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


﻿using System;
using System.Collections.Concurrent;
using Av.API.Provider;
using Av.API.Data;
using log4net;

namespace Av.API
{
    using StockRequestData = Tuple<StockRequestType, string, Action<StockRequestType, string, StockData>>;

    public class AvStockRequestManager : AvRequestManager<StockRequestType, StockRequestData>
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(AvStockRequestManager));

        private AvStockProvider _avStcokProvider;



        public AvStockRequestManager(AvStockProvider avStockProvider)
        {
            _avStcokProvider = avStockProvider;
            _requests = new ConcurrentQueue<StockRequestData>();
        }

        public void Add(StockRequestType requestType, string symbol, Action<StockRequestType, string, StockData> callback)
        {
            _requests.Enqueue(new StockRequestData(requestType, symbol, callback));
        }

        protected override void Execute(StockRequestData requestData)
        {
            var requestType = requestData.Item1;
            var symbol = requestData.Item2;
            var callback = requestData.Item3;

            try
            {
                switch (requestType)
                {
                    case StockRequestType.Daily:
                    case StockRequestType.DailyFull:
                    case StockRequestType.DailyAdjusted:
                    case StockRequestType.DailyAdjustedFull:
                        bool full = (requestType == StockRequestType.DailyFull || requestType == StockRequestType.DailyAdjustedFull);
                        var stockData = _avStcokProvider.RequestDaily(symbol, full);
                        callback(requestType, symbol, stockData);
                        break;
                    case StockRequestType.Weekly:
                    case StockRequestType.WeeklyAdjusted:
                        callback(requestType, symbol, _avStcokProvider.RequestWeekly(symbol));
                        break;
                    case StockRequestType.Monthly:
                    case StockRequestType.MonthlyAdjusted:
                        callback(requestType, symbol, _avStcokProvider.requestMonthly(symbol));
                        break;
                }
                ResetDelay();
            }
            catch (HighUsageException e)
            {
                if (CanRetry()) ReExecute(requestData);
                else
                {
                    log.ErrorFormat("Max retry count reached => Failed to execute request {0}/{1}",
                        requestType, symbol);
                    ResetDelay();
                }
            }

        }

    }
}
