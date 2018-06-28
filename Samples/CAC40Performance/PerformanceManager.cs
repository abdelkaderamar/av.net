using Av.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAC40Performance
{
    public class PerformanceManager
    {
        public PerformanceManager()
        {
            StocksData = new Dictionary<string, StockData>();
            StocksPerformance = new Dictionary<string, StockPerformance>();
        }

        public Dictionary<string, StockData> StocksData { get; }

        public Dictionary<string, StockPerformance> StocksPerformance { get; }

        public StockPerformance ReferencePerformance { get; set; }

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
            var first = stockData.Data.Values.ElementAt(FindNearest(referenceDate, stockData));
            var last = stockData.Data.Values.Last();
            return (last.Close - first.Close) / first.Close;
        }

        public List<StockPerformance> ComputeAllPerformances(string referenceStock, DateTime referenceDate)
        {
            StocksPerformance.Clear();
            ReferencePerformance = new StockPerformance(referenceStock)
            {
                Performance = ComputePerformance(referenceStock, referenceDate)
            };

            foreach (var data in StocksData)
            {
                var symbol = data.Key;
                try
                {
                    StockPerformance stockPerformance = new StockPerformance(symbol)
                    {
                        Performance = ComputePerformance(symbol, referenceDate)
                    };
                    StocksPerformance.Add(symbol, stockPerformance);
                }
                catch (ApplicationException e)
                {
                    Console.WriteLine(e.StackTrace);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
            }

            List<StockPerformance> stockPerfList = new List<StockPerformance>(StocksPerformance.Values);
            stockPerfList.Sort();
            return stockPerfList;
        }
    }
}
