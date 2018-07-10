﻿using Av.API;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MultipleStockVolumeChart
{
    /// <summary>
    /// Logique d'interaction pour MultipleStockVolumeControl.xaml
    /// </summary>
    public partial class MultipleStockVolumeControl : UserControl
    {
        public MultipleStockVolumeControl()
        {
            InitializeComponent();


            var mapper = Mappers.Xy<StockDataItem>().X(model => model.DateTime.Ticks).Y(model => model.Volume);
            Charting.For<StockDataItem>(mapper);

            StockSeriesCollection = new SeriesCollection
            {
                // new LineSeries{Title = "Volume", Values = new ChartValues<StockDataItem>() }
            };


            DateTimeFormatter = value => new DateTime((long)value).ToString("yy-MM-dd");

            DataContext = this;

        }

        public SeriesCollection StockSeriesCollection { get; }

        public Func<double, string> DateTimeFormatter { get; set; }

        private async void addStockBtn_Click(object sender, RoutedEventArgs e)
        {
            string symbol = this.stockTextBox.Text;
            if (string.IsNullOrEmpty(symbol)) return;

            foreach (var line in StockSeriesCollection)
            {
                if (line.Title.Equals(symbol)) return;
            }

            AvStockProvider stockProvider = new AvStockProvider("XD6HTE47G8ZZIDRB");
            StockData stockData = await stockProvider.RequestDailyAsync(symbol);

            if (stockData.Data.Values.Count == 0) return;

            StockPanel stockPanel = new StockPanel();
            stockPanel.stockLabel.Content = symbol;
            stockPanel.delStockBtn.Click += new RoutedEventHandler(delegate(object s, RoutedEventArgs args)
            {
                removeStock(symbol);
            });

            this.allStocksPanel.Children.Add(stockPanel);

            var lineSeries = new LineSeries { Title = symbol, Values = new ChartValues<StockDataItem>(stockData.Data.Values) };
            StockSeriesCollection.Add(lineSeries);
        }

        private void removeStock(string symbol)
        {
            for (int i=0; i< StockSeriesCollection.Count; ++i)
            {
                if (StockSeriesCollection[i].Title.Equals(symbol))
                {
                    StockSeriesCollection.RemoveAt(i);
                    break;
                }
            }
        }
    }
}
