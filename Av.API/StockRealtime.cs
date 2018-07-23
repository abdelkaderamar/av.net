// Copyright (c) Abdelkader Amar. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


﻿using System;
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

        public override string ToString()
        {
            // TODO
            return base.ToString();
        }
    }
}
