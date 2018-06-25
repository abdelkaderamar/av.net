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
    /// Logique d'interaction pour StockPanel.xaml
    /// </summary>
    public partial class StockPanel : UserControl
    {
        public StockPanel()
        {
            InitializeComponent();
        }

        private void delStockBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.Parent is Panel)
            {
                Panel parent = (Panel)this.Parent;
                parent.Children.Remove(this);
            }
        }
    }
}
