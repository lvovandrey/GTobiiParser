using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TobiiCSVTester.Abstract;

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
            
            TobiiCSVFiles = new ObservableCollection<TobiiCSVFile>()
            {
                new TobiiCSVFile("First"), new TobiiCSVFile("Second"), new TobiiCSVFile("Last"),
                new TobiiCSVFile("First"), new TobiiCSVFile("Second"), new TobiiCSVFile("Last"),
                new TobiiCSVFile("First"), new TobiiCSVFile("Second"), new TobiiCSVFile("Last"),
                new TobiiCSVFile("First"), new TobiiCSVFile("Second"), new TobiiCSVFile("Last"),
                new TobiiCSVFile("First"), new TobiiCSVFile("Second"), new TobiiCSVFile("Last"),
                new TobiiCSVFile("First"), new TobiiCSVFile("Second"), new TobiiCSVFile("Last"),
                new TobiiCSVFile("First"), new TobiiCSVFile("Second"), new TobiiCSVFile("Last"),
                new TobiiCSVFile("First"), new TobiiCSVFile("Second"), new TobiiCSVFile("Last"),
                new TobiiCSVFile("First"), new TobiiCSVFile("Second"), new TobiiCSVFile("Last"),
                new TobiiCSVFile("First"), new TobiiCSVFile("Second"), new TobiiCSVFile("Last"),
                new TobiiCSVFile("First"), new TobiiCSVFile("Second"), new TobiiCSVFile("Last"),
                new TobiiCSVFile("First"), new TobiiCSVFile("Second"), new TobiiCSVFile("Last")
            };
            MainWindow = mainWindow;
        }



        string directoryCSVFiles="";
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
                MainWindow.RefreshChart();
            }

        }


        private void BuildFilesFillingDiagrams()
        {
            throw new NotImplementedException("BuildFilesFillingDiagrams() not implement");
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
    }
}
