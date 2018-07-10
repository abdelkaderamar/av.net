using Av.API;
using System;
using System.Collections.Generic;


namespace Av.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            AvStockProvider provider = new AvStockProvider(args[0]);
            var sgoDailyData = provider.RequestDaily("SGO.PA", true);
            Console.WriteLine(sgoDailyData);
            var sgoWeeklyData = provider.RequestWeekly("SGO.PA");
            var sgoMonthlyData = provider.requestMonthly("SGO.PA");

            IDictionary<string, StockRealtime> batchData = provider.BatchRequest(new string[] { "MSFT", "IBM", "AAPL" });
            Console.WriteLine("Batch Request for MSFT, IBM and AAPL");
            foreach (var kvp in batchData)
            {
                Console.WriteLine("{0} : Price = {1}, Volume = {2}, Date = {3}", kvp.Key, kvp.Value.Price, kvp.Value.Volume, kvp.Value.Timestamp);
            }
            AvRequestManager requestManager = new AvRequestManager(provider);
            string[] stocks = new string[] { "SGO.PA", "GLE.PA", "BNP.PA", "VIV.PA", "RNO.PA", "CS.PA" };
            requestManager.Start();
            foreach (var stock in stocks)
            {
                requestManager.Add(RequestType.Daily, stock, Callback);
                requestManager.Add(RequestType.Weekly, stock, Callback);
                requestManager.Add(RequestType.Monthly, stock, Callback);
            }
            requestManager.Stop(true);

        }

        private static void Callback(RequestType requestType, string symbol, StockData stockData)
        {

            if (stockData == null)
            {
                Console.WriteLine("No {0} data received for symbol {1}", requestType.ToString(), symbol);

            }
            else
            {
                Console.WriteLine("Receiving {0} data for symbol {1}", requestType.ToString(), symbol);
                Console.WriteLine("\t{0} data", stockData.Data.Count);
            }
        }

    }
}
