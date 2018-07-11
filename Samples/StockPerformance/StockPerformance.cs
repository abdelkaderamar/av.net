using System;

namespace StockPerformance
{
    public class StockPerformanceData : IComparable<StockPerformanceData>
    {
        public StockPerformanceData(string symbol)
        {
            Symbol = symbol;
        }

        public string Symbol { get; }

        public double Performance { get; set; }

        public int CompareTo(StockPerformanceData other)
        {
            return Performance.CompareTo(other.Performance);
        }
    }
}
