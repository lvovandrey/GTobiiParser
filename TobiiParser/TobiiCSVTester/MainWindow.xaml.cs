﻿using MaterialDesignThemes.Wpf;
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

            var myMessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(400));
            
            MySnackbar.MessageQueue = myMessageQueue;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //chart.ChartAreas.Add(new ChartArea("Default"));

            //// Добавим линию, и назначим ее в ранее созданную область "Default"
            //chart.Series.Add(new Series("Series1"));
            //chart.Series["Series1"].ChartArea = "Default";
            //chart.Series["Series1"].ChartType = SeriesChartType.Line;

        //    RefreshChart();
        }

        internal void MessageShowOnSnackBar(string infoMessage)
        {
            MySnackbar.MessageQueue.Enqueue(infoMessage);
        }

        //internal void RefreshChart()
        //{
        //    // добавим данные линии
        //    int i = 0;
        //    int[] axisXData = new int[30];
        //    double[] axisYData = new double[30];
        //    Random rnd = new Random();

        //    for (i = 0; i < 30; i++)
        //    {
        //        axisXData[i] = i + 1;
        //        axisYData[i] = 0;
        //    }
        //    chart.Series["Series1"].Points.DataBindXY(axisXData, axisYData);
        //}

        //internal void RefreshChart(TobiiCSVFile tobiiCSVFile)
        //{
        //    chart.Series["Series1"].Points.DataBindXY(tobiiCSVFile.Xs, tobiiCSVFile.Ys);
        //}
    }
}
