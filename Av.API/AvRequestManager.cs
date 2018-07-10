using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;


namespace Av.API
{
    using RequestData = Tuple<RequestType, string, System.Action<RequestType, string, StockData>>;

    public class AvRequestManager
    {
        private bool _run;

        private Thread _thread;

        public static int DEFAULT_DELAY = 2000;

        public static int YIELD_DELAY = 100;
        // Delay betwwen two successive requests

        ConcurrentQueue<RequestData> _requests;

        private AvStockProvider _avProvider;

        public AvRequestManager(AvStockProvider avProvider)
        {
            _avProvider = avProvider;
            _run = false;
            _thread = new Thread(Run);
            Delay = DEFAULT_DELAY;
            _requests = new ConcurrentQueue<RequestData>();
        }

        int Delay { get; set; }

        public void Start()
        {
            _run = true;
            _thread.Start();
        }

        public void Stop(bool finish)
        {
            _run = false;
            Finish();
            _thread.Join();
        }

        public void Add(RequestType requestType, string symbol, Action<RequestType, string, StockData> callback)
        {
            _requests.Enqueue(new RequestData(requestType, symbol, callback));
        }

        protected void Run()
        {
            while (_run)
            {
                RequestData requestData;
                if (_requests.TryDequeue(out requestData))
                {
                    Execute(requestData);
                    Thread.Sleep(Delay);
                }
                else
                {
                    Thread.Sleep(YIELD_DELAY);
                }
            }
        }

        private void Execute(RequestData requestData)
        {
            var requestType = requestData.Item1;
            var symbol = requestData.Item2;
            var callback = requestData.Item3;

            switch (requestType)
            {
                case RequestType.Daily:
                case RequestType.DailyAdjusted:
                    callback(requestType, symbol, _avProvider.RequestDaily(symbol));
                    break;
                case RequestType.Weekly:
                case RequestType.WeeklyAdjusted:
                    callback(requestType, symbol, _avProvider.RequestWeekly(symbol));
                    break;
                case RequestType.Monthly:
                case RequestType.MonthlyAdjusted:
                    callback(requestType, symbol, _avProvider.requestMonthly(symbol));
                    break;
            }
        }

        private void Finish()
        {
            RequestData requestData;
            while (_requests.TryDequeue(out requestData))
            {
                Execute(requestData);
                Thread.Sleep(Delay);
            }
        }
    }
}
