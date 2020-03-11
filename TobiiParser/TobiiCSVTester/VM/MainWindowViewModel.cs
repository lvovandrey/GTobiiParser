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

namespace TobiiCSVTester.VM
{
    public class MainWindowViewModel:INPCBase
    {

        MainWindow MainWindow;

        public DelegateCommand BuildFilesFillingDiagramsCommand { get; set; }
        public DelegateCommand AppCloseCommand { get; set; }
        public DelegateCommand OpenDirectoryCSVFilesCommand { get; set; }
        

        public MainWindowViewModel(MainWindow mainWindow)
        {
            BuildFilesFillingDiagramsCommand = new DelegateCommand(o => BuildFilesFillingDiagrams());
            AppCloseCommand = new DelegateCommand(o => AppClose());
            OpenDirectoryCSVFilesCommand = new DelegateCommand(o => OpenDirectoryCSVFiles());

            TobiiCSVFiles = new ObservableCollection<TobiiCSVFile>();
            MainWindow = mainWindow;

            StackedAreaExampleRun();
        }



        string directoryCSVFiles= @"g:\0. 941 TOBII Обработка2020\0. ИСХОДНИКИ\TOBII\1. сп\1. Виноградов\";
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
                MainWindow.RefreshChart(SelectedTobiiCSVFile);

                StackedAreaExampleRefresh(SelectedTobiiCSVFile);

            }

        }



        private async void BuildFilesFillingDiagrams()
        {
            TobiiCSVFiles.Clear();

            if (!Directory.Exists(DirectoryCSVFiles)) { MessageBox.Show("Директории " + DirectoryCSVFiles + " не существует"); return; }

            List<string> files = Directory.GetFiles(DirectoryCSVFiles, "*.csv", SearchOption.AllDirectories).ToList();

            if (!Directory.Exists(DirectoryCSVFiles)) { MessageBox.Show("В директории и ее поддиректориях " + DirectoryCSVFiles + " нет csv-файлов"); return; }

            foreach (string fullfilepath in files)
            {
                InfoMessage = "Файл " + files.IndexOf(fullfilepath) + "из" + files.Count() + " обработан";

                TobiiCSVFile tobiiCSVFile = new TobiiCSVFile(fullfilepath);
                await Task.Run(() => tobiiCSVFile.ReadTestingInfoAsync());
                TobiiCSVFiles.Add(tobiiCSVFile);
            }
            InfoMessage = "";
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

            List<ObservablePoint> xy_list = new List<ObservablePoint>();
            foreach (var x in selectedTobiiCSVFile.Xs)
            {
                XY xy = new XY();
                xy.X = x;
                xy.Y = selectedTobiiCSVFile.Ys[selectedTobiiCSVFile.Xs.IndexOf(x)];
                xy_list.Add(new ObservablePoint(xy.X, xy.Y));
            }

            SeriesCollection = new SeriesCollection
            {
                new StackedAreaSeries
                {
                    Title = "Africa",
                    Values = new ChartValues<ObservablePoint>
                    {
                    }
                }
            };



            foreach (var xy in xy_list)
            {
                SeriesCollection[0].Values.Add(xy);
            }

            OnPropertyChanged("SeriesCollection");
        }

        public void StackedAreaExampleRun()
        {
  
            SeriesCollection = new SeriesCollection
            {
                new StackedAreaSeries
                {
                    Title = "Africa",
                    Values = new ChartValues<DateTimePoint>
                    {
                        new DateTimePoint(new DateTime(1950, 1, 1), .228),
                        new DateTimePoint(new DateTime(1960, 1, 1), .285),
                        new DateTimePoint(new DateTime(1970, 1, 1), .366),
                        new DateTimePoint(new DateTime(1980, 1, 1), .478),
                        new DateTimePoint(new DateTime(1990, 1, 1), .629),
                        new DateTimePoint(new DateTime(2000, 1, 1), .808),
                        new DateTimePoint(new DateTime(2010, 1, 1), 1.031),
                        new DateTimePoint(new DateTime(2013, 1, 1), 1.110)
                    },
                    LineSmoothness = 0
                }
                
            };

            XFormatter = val => new DateTime((long)val).ToString("yyyy");
            YFormatter = val => val.ToString("N") + " M";
                        OnPropertyChanged("SeriesCollection");
        }

        public SeriesCollection SeriesCollection { get; set; }
        public Func<double, string> XFormatter { get; set; }
        public Func<double, string> YFormatter { get; set; }

    }






    public partial class StackedAreaExample : UserControl
    {


    }
}
