using System;
using System.Collections.Concurrent;

namespace Av.API
{
    using StockRequestData = Tuple<StockRequestType, string, Action<StockRequestType, string, StockData>>;

    public class AvStockRequestManager : AvRequestManager<StockRequestType, StockRequestData>
    {
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

            switch (requestType)
            {
                case StockRequestType.Daily:
                case StockRequestType.DailyFull:
                case StockRequestType.DailyAdjusted:
                case StockRequestType.DailyAdjustedFull:
                    bool full = (requestType == StockRequestType.DailyFull || requestType == StockRequestType.DailyAdjustedFull);
                    callback(requestType, symbol, _avStcokProvider.RequestDaily(symbol, full));
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
        }

    }
}
