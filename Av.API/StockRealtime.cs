using System;
using System.Collections.Generic;
using System.Text;

namespace Av.API
{
    public class StockRealtime
    {
        public StockRealtime(string symbol)
        {
            Symbol = symbol;
        }

        public string Symbol { get; }

        public double Price { get; set; }

        public long Volume { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
