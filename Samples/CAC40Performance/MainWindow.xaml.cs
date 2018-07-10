using Av.API;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace CAC40Performance
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
            PerfManager = new PerformanceManager();

            UpdateData(true);
        }

        void UpdateData(bool download)
        { 
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DownloadData;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync(download);
        }


        public string ReferenceStock { get; set; }

        public DateTime ReferenceDate { get; set; }

        public SeriesCollection StockSeriesCollection { get; set; }

        public PerformanceManager PerfManager { get; }

        public Func<double, string> PerformanceFormatter { get; }

        public static readonly string CAC40 = "^FCHI";

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }

        private void worker_DownloadData(object sender, DoWorkEventArgs e)
        {
            StockSeriesCollection[0].Values.Clear();
            AvStockProvider stockProvider = new AvStockProvider("XD6HTE47G8ZZIDRB");
            bool download = (bool)e.Argument;
            if (download)
            {
                foreach (var symbol in CAC40Helper.CAC40_STOCKS)
                {
                    StockData stockData = stockProvider.RequestDaily(symbol);
                    PerfManager.StocksData[symbol] = stockData;
                    Thread.Sleep(2000);
                }
            }
            var stocksPerformances = PerfManager.ComputeAllPerformances(ReferenceStock, ReferenceDate);
            string[] labels = new string[stocksPerformances.Count];
            for (int i=0; i<stocksPerformances.Count; ++i)
            {
                labels[i] = stocksPerformances[i].Symbol;
                StockSeriesCollection[0].Values.Add(stocksPerformances[i].Performance - PerfManager.ReferencePerformance.Performance);
            }
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.AxisX.Labels = labels;
            }));
            Console.WriteLine("Download completed");
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
