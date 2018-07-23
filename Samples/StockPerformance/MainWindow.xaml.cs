// Copyright (c) Abdelkader Amar. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


﻿using Av.API;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using log4net;

namespace StockPerformance
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private BackgroundWorker _worker;

        public MainWindow()
        {
            InitializeComponent();

            ReferenceStock = CAC40;
            ReferenceDate = new DateTime(DateTime.Now.Year, 1, 1);
            StockSeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Performance vs " + ReferenceStock,
                    Values = new ChartValues<double>()
                }
            };
            PerformanceFormatter = value => value.ToString("P");
            DataContext = this;

            refStockCB.ItemsSource = CAC40Helper.CAC40_STOCKS;

            PerfManager = new PerformanceManager();

            UpdateData(true);
        }

        void UpdateData(bool download)
        {
            this.refStockCB.IsEnabled = false;
            this.datePicker.IsEnabled = false;

            _worker = new BackgroundWorker();
            _worker.WorkerReportsProgress = true;
            _worker.DoWork += worker_DownloadData;
            _worker.ProgressChanged += worker_ProgressChanged;
            _worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            _worker.RunWorkerAsync(download);
        }


        public string ReferenceStock { get; set; }

        public DateTime ReferenceDate { get; set; }

        public SeriesCollection StockSeriesCollection { get; set; }

        public PerformanceManager PerfManager { get; }

        public Func<double, string> PerformanceFormatter { get; }

        public static readonly string CAC40 = "^FCHI";

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Log.Info("Download completed");
            UpdatePerformances();
        }

        private void UpdatePerformances()
        { 
            var stocksPerformances = PerfManager.ComputeAllPerformances(ReferenceStock, ReferenceDate);
            string[] labels = new string[stocksPerformances.Count];
            for (int i = 0; i < stocksPerformances.Count; ++i)
            {
                labels[i] = stocksPerformances[i].Symbol;
                StockSeriesCollection[0].Values.Add(stocksPerformances[i].Performance - PerfManager.ReferencePerformance.Performance);
            }
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.AxisX.Labels = labels;
            }));

            refStockCB.IsEnabled = true;
            datePicker.IsEnabled = true;

            refStockCB.SelectedItem = ReferenceStock;
            datePicker.SelectedDate = ReferenceDate;

        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void worker_DownloadData(object sender, DoWorkEventArgs e)
        {
            StockSeriesCollection[0].Values.Clear();
            AvStockProvider stockProvider = new AvStockProvider("XD6HTE47G8ZZIDRB");
            AvStockRequestManager stockRequestMgr = new AvStockRequestManager(stockProvider);
            stockRequestMgr.EnabledRetry = true;
            stockRequestMgr.Delay = 8000;
            stockRequestMgr.MaxRetry = 10;
            stockRequestMgr.Start();
            bool download = (bool)e.Argument;
            if (download)
            {
                foreach (var symbol in CAC40Helper.CAC40_STOCKS)
                {
                    stockRequestMgr.Add(StockRequestType.Daily, symbol, StockDailyCallback);
                }
            } // end if (download)
            stockRequestMgr.Stop(true);
        }

        protected void StockDailyCallback(StockRequestType requestType, string symbol, StockData stockData)
        {
            if (stockData != null)
            {
                Log.InfoFormat("Receiving {0} data for symbol {1}", requestType, symbol);
                PerfManager.StocksData[symbol] = stockData;
                _worker.ReportProgress((PerfManager.StocksData.Count * 100) / CAC40Helper.CAC40_STOCKS.Length);
            }
            else
            {
                Log.WarnFormat("Failed to download {0} data for symbol {1}", requestType, symbol);
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender != this.refStockCB) return;
            if (this.refStockCB.SelectedValue == null) return;
            string newRefStock = this.refStockCB.SelectedValue.ToString();
            if (string.IsNullOrEmpty(newRefStock)) return;

            ReferenceStock = newRefStock;
            UpdateData(false);
        }

        private void DatePicker_CalendarClosed(object sender, RoutedEventArgs e)
        {
            var selectedDate = this.datePicker.SelectedDate;
            if (selectedDate == null) return;

            ReferenceDate = (DateTime)selectedDate;
            UpdateData(false);
        }
    }
}
