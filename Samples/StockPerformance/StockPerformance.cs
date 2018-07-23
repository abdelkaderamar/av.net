// Copyright (c) Abdelkader Amar. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


﻿using System;

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
