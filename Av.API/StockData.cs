// Copyright (c) Abdelkader Amar. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


﻿using System;
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

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Historical Data for ").Append(Symbol).Append(Environment.NewLine);
            foreach(var kvp in Data)
            {
                sb.AppendFormat("\tDate = [{0}]", kvp.Value.DateTime).Append("\t:").
                    AppendFormat("\tVolume = [{0}]", kvp.Value.Volume).
                    AppendFormat("\tOpen = [{0}]", kvp.Value.Open).
                    AppendFormat("\tHigh = [{0}]", kvp.Value.High).
                    AppendFormat("\tLow = [{0}]", kvp.Value.Low).
                    AppendFormat("\tClose = [{0}]", kvp.Value.Close).
                    Append(Environment.NewLine);
            }
            return sb.ToString();
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

        public override string ToString()
        {
            // TODO
            return base.ToString();
        }
    }
}
