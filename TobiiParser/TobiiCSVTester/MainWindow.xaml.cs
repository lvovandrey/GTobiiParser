using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TobiiCSVTester.VM;

namespace TobiiCSVTester
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(this);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            chart.ChartAreas.Add(new ChartArea("Default"));

            // Добавим линию, и назначим ее в ранее созданную область "Default"
            chart.Series.Add(new Series("Series1"));
            chart.Series["Series1"].ChartArea = "Default";
            chart.Series["Series1"].ChartType = SeriesChartType.Line;

            RefreshChart();
        }

        internal void RefreshChart()
        {
            // добавим данные линии
            int i = 0;
            int[] axisXData = new int[30];
            double[] axisYData = new double[30];
            Random rnd = new Random();

            for (i = 0; i < 30; i++)
            {
                axisXData[i] = i + 1;
                axisYData[i] = rnd.Next(0, 100);
            }
            chart.Series["Series1"].Points.DataBindXY(axisXData, axisYData);
        }
    }
}
