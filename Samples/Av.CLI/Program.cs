using Av.API;
using System;
using System.Threading;
using System.Collections.Generic;
using log4net;
using log4net.Config;

namespace Av.CLI
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        static void Usage()
        {
            Console.WriteLine("Syntax error");
            Console.WriteLine("Usage : ");
            Console.WriteLine("\tAv.CLI <AlphaVantage key> [CURRENCY]");
        }

        static void RequestCurrencies(string avKey)
        {
            AvCurrencyProvider currencyProvider = new AvCurrencyProvider(avKey);
            var cryptoCurrencies = new List<string>() { "BTC", "ETH", "XRP", "BCH", "EOS", "LTC", "XLM", "ADA" };
            foreach (var cryptoCurr in cryptoCurrencies)
            {
                var rate = currencyProvider.RequestExchangeRate(cryptoCurr, "USD");
                if (rate != null)
                    Console.WriteLine(rate);
                Thread.Sleep(2000);
            }
            Console.ReadKey();
        }

        static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            if (args.Length == 0)
            {
                Usage();
                return;
            }
            if (args[1].Equals("CURRENCY"))
            {
                RequestCurrencies(args[0]);
                return;
            }
            AvStockProvider provider = new AvStockProvider(args[0]);
            var sgoDailyData = provider.RequestDaily("SGO.PA", true);
            log.Info(sgoDailyData);
            var sgoWeeklyData = provider.RequestWeekly("SGO.PA");
            log.Info(sgoWeeklyData);
            var sgoMonthlyData = provider.requestMonthly("SGO.PA");
            log.Info(sgoMonthlyData);

            IDictionary<string, StockRealtime> batchData = provider.BatchRequest(new string[] { "MSFT", "IBM", "AAPL" });
            log.Info("Batch Request for MSFT, IBM and AAPL");
            foreach (var kvp in batchData)
            {
                log.InfoFormat("{0} : Price = {1}, Volume = {2}, Date = {3}", kvp.Key, kvp.Value.Price, kvp.Value.Volume, kvp.Value.Timestamp);
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
                log.InfoFormat("No {0} data received for symbol {1}", requestType.ToString(), symbol);

            }
            else
            {
                log.InfoFormat("Receiving {0} data for symbol {1}", requestType.ToString(), symbol);
                log.InfoFormat("\t{0} data", stockData.Data.Count);
                log.InfoFormat("\t{0}", stockData);
            }
        }

    }
}
