using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Av.API
{
    public class StockData
    {
        public StockData(string symbol)
        {
            Symbol = symbol;
            Data = new SortedDictionary<DateTime, StockDataItem>();
        }

        public string Symbol { get; }

        public IDictionary<DateTime, StockDataItem> Data { get; }

        

        public void AddDataItem(StockDataItem dataItem)
        {
            this.Data[dataItem.DateTime] = dataItem;
        }


        public double[] Close()
        {
            IEnumerable<double> closes = from item in Data.Values select item.Close;
            return closes.ToArray<double>();
        }

        public long[] Volume()
        {
            IEnumerable<long> query = from item in Data.Values select item.Volume;
            return query.ToArray<long>();
        }
    }

    public class StockDataItem
    {
        public StockDataItem(DateTime dateTime)
        {
            DateTime = dateTime;
        }

        public DateTime DateTime { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public long Volume { get; set; }
        public double AdjustedClose { get; set; }
    }
}
