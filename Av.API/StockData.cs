using System;
using System.Collections.Generic;
using System.Text;

namespace Av.API
{
    public class StockData
    {
        public StockData(string symbol)
        {
            Symbol = symbol;
        }

        string Symbol { get; }
    }
}
