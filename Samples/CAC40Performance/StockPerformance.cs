using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAC40Performance
{
    public class StockPerformance : IComparable<StockPerformance>
    {
        public StockPerformance(string symbol)
        {
            Symbol = symbol;
        }

        public string Symbol { get; }

        public double Performance { get; set; }

        public int CompareTo(StockPerformance other)
        {
            return Performance.CompareTo(other.Performance);
        }
    }
}
