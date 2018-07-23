// Copyright (c) Abdelkader Amar. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


﻿using Av.API;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace VolumeChart
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AvStockProvider stockProvider = new AvStockProvider("XD6HTE47G8ZZIDRB");
            StockData stockData = await stockProvider.RequestDailyAsync("SGO.PA");
            ((MainWindow)Application.Current.MainWindow).volumeChart.StockData = stockData;
            ((MainWindow)Application.Current.MainWindow).volumeChart.init();
        }
    }
}
