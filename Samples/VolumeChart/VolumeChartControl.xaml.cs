// Copyright (c) Abdelkader Amar. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


﻿using Av.API;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;


namespace VolumeChart
{
    /// <summary>
    /// Logique d'interaction pour VolumeChart.xaml
    /// </summary>
    public partial class VolumeChartControl : UserControl, INotifyPropertyChanged
    {
        public VolumeChartControl()
        {
            InitializeComponent();

            var mapper = Mappers.Xy<StockDataItem>().X(model => model.DateTime.Ticks).Y(model => model.Volume);
            Charting.For<StockDataItem>(mapper);

            StockSeriesCollection = new SeriesCollection
            {
                new LineSeries{Title = "Volume", Values = new ChartValues<StockDataItem>() }
            };


            DateTimeFormatter = value => new DateTime((long)value).ToString("yy-MM-dd");

            StockSerieTypes = new string[] { "Open", "High", "Low", "Close", "Volume" };
            StockSerieType = "";

            DataContext = this;
        }

        #region Properties
        public SeriesCollection StockSeriesCollection { get; }

        public ChartValues<StockDataItem> ChartValues { get; set; }

        public StockData StockData { get; set; }

        public Func<double, string> DateTimeFormatter { get; set; }

        public string[] StockSerieTypes { get; set; }

        public string StockSerieType { get; set; }

        public void init()
        {
            if (StockData == null) return;

            StockSeriesCollection[0].Values = new ChartValues<StockDataItem>(StockData.Data.Values);
        }
        #endregion

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private async void addStockBtn_Click(object sender, RoutedEventArgs e)
        {
            string symbol = this.stockTextBox.Text;
            if (string.IsNullOrEmpty(symbol)) return;

            AvStockProvider stockProvider = new AvStockProvider("XD6HTE47G8ZZIDRB");
            StockData stockData = await stockProvider.RequestDailyAsync(symbol);
            ((MainWindow)Application.Current.MainWindow).volumeChart.StockData = stockData;
            ((MainWindow)Application.Current.MainWindow).volumeChart.init();
        }
    }
}
