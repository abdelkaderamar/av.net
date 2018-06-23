using Av.API;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public SeriesCollection StockSeriesCollection { get; }

        public ChartValues<StockDataItem> ChartValues { get; set; }

        public StockData StockData { get; set; }

        public Func<double, string> DateTimeFormatter { get; set; }

        public String[] StockSerieTypes { get; set; }

        public String StockSerieType { get; set; }

        public void init()
        {
            if (StockData == null) return;

            // Old method
            //var dates = (from item in Data.Data.Keys select item.ToString("yyyy-MM-dd")).ToArray<string>();
            //var volumes = Data.Volume();
            //StockSeriesCollection[0].Values = new ChartValues<long>(volumes);
            //Dates = dates;
            //this.AxisX.Labels = Dates;

            if (StockSerieType.Equals("Open"))
            {
                var mapper = Mappers.Xy<StockDataItem>().X(model => model.DateTime.Ticks).Y(model => model.Open);
                Charting.For<StockDataItem>(mapper);
            }
            else if (StockSerieType.Equals("High"))
            {
                var mapper = Mappers.Xy<StockDataItem>().X(model => model.DateTime.Ticks).Y(model => model.High);
                Charting.For<StockDataItem>(mapper);
            }
            else if (StockSerieType.Equals("Low"))
            {
                var mapper = Mappers.Xy<StockDataItem>().X(model => model.DateTime.Ticks).Y(model => model.Low);
                Charting.For<StockDataItem>(mapper);
            }
            else if (StockSerieType.Equals("Close"))
            {
                var mapper = Mappers.Xy<StockDataItem>().X(model => model.DateTime.Ticks).Y(model => model.Close);
                Charting.For<StockDataItem>(mapper);
            }
            else if (StockSerieType.Equals("Volume"))
            {
                var mapper = Mappers.Xy<StockDataItem>().X(model => model.DateTime.Ticks).Y(model => model.Volume);
                Charting.For<StockDataItem>(mapper);
            }

            StockSeriesCollection[0].Values = new ChartValues<StockDataItem>(StockData.Data.Values);
        }

        private bool _handle = true;
        private void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (_handle) Handle();
            _handle = true;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            _handle = !cmb.IsDropDownOpen;
            Handle();
        }

        private void Handle()
        {
            StockSerieType = StockSerieTypesCombo.SelectedItem.ToString();
            init();
        }

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}
