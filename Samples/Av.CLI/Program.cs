using Av.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Av.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            AvStockProvider provider = new AvStockProvider(args[0]);
            provider.RequestDaily("SGO.PA");
            provider.RequestWeekly("SGO.PA");
            provider.requestMonthly("SGO.PA");

            provider.BatchRequest(new string[] { "MSFT", "IBM", "AAPL" });

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
