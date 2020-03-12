using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TobiiCSVTester.Abstract;
using TobiiCSVTester.View;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Media;

namespace TobiiCSVTester.VM
{
    public class MainWindowViewModel : INPCBase
    {

        MainWindow MainWindow;

        public DelegateCommand BuildFilesFillingDiagramsCommand { get; set; }
        public DelegateCommand AppCloseCommand { get; set; }
        public DelegateCommand OpenDirectoryCSVFilesCommand { get; set; }
        public DelegateCommand TestsStopCommand { get; set; }


        public MainWindowViewModel(MainWindow mainWindow)
        {
            BuildFilesFillingDiagramsCommand = new DelegateCommand(o => BuildFilesFillingDiagramsAsync());
            AppCloseCommand = new DelegateCommand(o => AppClose());
            OpenDirectoryCSVFilesCommand = new DelegateCommand(o => OpenDirectoryCSVFiles());
            TestsStopCommand = new DelegateCommand(o => TestsStop());


            TobiiCSVFiles = new ObservableCollection<TobiiCSVFile>();
            MainWindow = mainWindow;

            StackedAreaExampleRun();
        }


        string directoryCSVFiles = @"";
        public string DirectoryCSVFiles
        {
            get
            {
                return directoryCSVFiles;
            }
            set
            {
                directoryCSVFiles = value;
                OnPropertyChanged("DirectoryCSVFiles");
            }
        }


        int smoothInterval = 1000;
        public int SmoothInterval
        {
            get
            {
                return smoothInterval;
            }
            set
            {
                smoothInterval = value;
                OnPropertyChanged("SmoothInterval");
            }
        }

        string infoMessage = "";
        public string InfoMessage
        {
            get
            {
                return infoMessage;
            }
            set
            {
                infoMessage = value;
                OnPropertyChanged("InfoMessage");

                System.Windows.Application.Current.Dispatcher.Invoke(() => 
                {
                    MainWindow.MessageShowOnSnackBar(infoMessage);
                });

            }
        }

        public ObservableCollection<TobiiCSVFile> TobiiCSVFiles { get; set; }

        private TobiiCSVFile _SelectedTobiiCSVFile;
        public TobiiCSVFile SelectedTobiiCSVFile
        {
            get
            { return _SelectedTobiiCSVFile; }
            set
            {
                _SelectedTobiiCSVFile = value;
                OnPropertyChanged("SelectedTobiiCSVFile");
           //     MainWindow.RefreshChart(SelectedTobiiCSVFile);

                StackedAreaExampleRefresh(SelectedTobiiCSVFile);

            }

        }


        static CancellationTokenSource cur_cts;
        private void TestsStop()
        {
            cur_cts?.Cancel();
            InfoMessage = "Тест прерван";
        }


        private async void BuildFilesFillingDiagramsAsync()
        {
            cur_cts?.Cancel();

            cur_cts = new CancellationTokenSource();
            await Task.Run(() => BuildFilesFillingDiagrams(cur_cts.Token));
        }

        private async Task BuildFilesFillingDiagrams(CancellationToken cancellationToken)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() => { TobiiCSVFiles.Clear(); });

            if (!Directory.Exists(DirectoryCSVFiles)) { MessageBox.Show("Директории " + DirectoryCSVFiles + " не существует"); return; }

            List<string> files = Directory.GetFiles(DirectoryCSVFiles, "*.csv", SearchOption.AllDirectories).ToList();

            if (!Directory.Exists(DirectoryCSVFiles)) { MessageBox.Show("В директории и ее поддиректориях " + DirectoryCSVFiles + " нет csv-файлов"); return; }

            InfoMessage = "Обработка " + files.Count() + " файлов началась";

            foreach (string fullfilepath in files)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("Операция прервана"); InfoMessage = "Операция прервана"; break;
                }


                TobiiCSVFile tobiiCSVFile = new TobiiCSVFile(fullfilepath, SmoothInterval);
                tobiiCSVFile.ReadTestingInfo();
                System.Windows.Application.Current.Dispatcher.Invoke(() => { TobiiCSVFiles.Add(tobiiCSVFile); });
                Console.WriteLine(fullfilepath);
                InfoMessage = "Файл " + (files.IndexOf(fullfilepath) + 1).ToString() + " из " + files.Count() + " обработан";

            }
            InfoMessage = "Обработка " + files.Count() + " файлов завершена";
        }

        private void AppClose()
        {
            MainWindow.Close();
        }

        private void OpenDirectoryCSVFiles()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    DirectoryCSVFiles = fbd.SelectedPath;
                }
            }
        }


        



        private void StackedAreaExampleRefresh(TobiiCSVFile selectedTobiiCSVFile)
        {

            XFormatter = val => val.ToString();
            YFormatter = val => val.ToString();
            XFormatter2 = val => TimeSpan.FromMilliseconds(val).ToString(@"mm\:ss");

            List<ObservablePoint> xy_list = new List<ObservablePoint>();
            foreach (var x in selectedTobiiCSVFile.Xs)
            {
                XY xy = new XY();
                xy.X = x;
                xy.Y = selectedTobiiCSVFile.Ys[selectedTobiiCSVFile.Xs.IndexOf(x)];
                xy_list.Add(new ObservablePoint(xy.X, xy.Y));
            }

            //Brush brush = new SolidColorBrush(Colors.Gray);
            //brush.Opacity = 0.5;

            //SeriesCollection = new SeriesCollection
            //{
            //    new StackedAreaSeries
            //    {
            //        Values = new ChartValues<ObservablePoint>
            //        {
            //        },
            //        LineSmoothness = 0 , Foreground = Brushes.Black, Fill = brush, PointForeground=Brushes.Black
            //    }
            //};

            Brush brush2 = new SolidColorBrush(Colors.DarkOrange);
            brush2.Opacity = 0.5;

            SeriesCollection2 = new SeriesCollection
            {
                new StackedAreaSeries
                {
                    Values = new ChartValues<ObservablePoint>
                    {
                    },
                    LineSmoothness = 0 , Foreground = Brushes.Black, Fill = brush2, PointForeground=Brushes.Black
                }
            };




            foreach (var xy in xy_list)
            {
         //       SeriesCollection[0].Values.Add(xy);
                SeriesCollection2[0].Values.Add(xy);
            }

            OnPropertyChanged("SeriesCollection");
            OnPropertyChanged("SeriesCollection2");
        }

        public void StackedAreaExampleRun()
        {

            //Brush brush = new SolidColorBrush(Colors.Gray);
            //brush.Opacity = 0.5;

            //SeriesCollection = new SeriesCollection
            //{
            //    new StackedAreaSeries
            //    {
            //        Values = new ChartValues<ObservablePoint>
            //        {
            //        },
            //        LineSmoothness = 0 , Foreground = Brushes.Black, Fill = brush, PointForeground=Brushes.Black
            //    }
            //};

            Brush brush2 = new SolidColorBrush(Colors.DarkOrange);
            brush2.Opacity = 0.5;

            SeriesCollection2 = new SeriesCollection
            {
                new StackedAreaSeries
                {
                    Values = new ChartValues<ObservablePoint>
                    {
                    },
                    LineSmoothness = 0 , Foreground = Brushes.Black, Fill = brush2, PointForeground=Brushes.Black
                }
            };

            XFormatter = val => val.ToString();
            YFormatter = val => val.ToString();
            XFormatter2 = val => TimeSpan.FromMilliseconds(val).ToString(@"mm\:ss");

            OnPropertyChanged("SeriesCollection");
        }

        public SeriesCollection SeriesCollection { get; set; }
        public SeriesCollection SeriesCollection2 { get; set; }

        public Func<double, string> XFormatter { get; set; }
        public Func<double, string> YFormatter { get; set; }
        public Func<double, string> XFormatter2 { get; set; }
    }






    public partial class StackedAreaExample : UserControl
    {


    }
}
