using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;



namespace TobiiParser
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }





        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<KadrInTime> kadrInTimes = ExcelReader.ReadKadrSets(@"C:\tmp\1.xlsx");
            foreach (var item in kadrInTimes)
            {
                TextBox1.Text += item.ToString() + Environment.NewLine;
            }
            TobiiCsvReader tobiiCsvReader = new TobiiCsvReader();

            string filename = @"C:\tmp\22.csv";
            tobiiCsvReader.tobiiList = new List<TobiiRecord>(500000);
            tobiiCsvReader.FiltredTobiiList = new List<TobiiRecord>();
            tobiiCsvReader.TobiiCSCRead(filename, tobiiCsvReader.tobiiList);



        }

     

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
        }
        private bool IsEqual(List<int> a, List<int> b)
        {
            if (a.Count() != b.Count) return false;
            for (int i = 0; i < a.Count; i++)
                if (a[i] != b[i]) return false;
            return true;
        }

        //private void Button_Click_2(object sender, RoutedEventArgs e)
        //{

            

        ////    TobiiCsvReader tobiiCsvReader = new TobiiCsvReader();
        ////    List<TobiiRecord> tobiiRecords = new List<TobiiRecord>();
        ////    tobiiCsvReader.TobiiCSCRead(@"C:\_\1\1.csv", tobiiRecords);
        ////    List<TobiiRecord> FiltredTobiiList = tobiiCsvReader.CompactTobiiRecords(tobiiRecords);
        ////    TabOfKeys tabOfKeys =  ExcelReader.ReadTabOfKeys(@"C:\_\Tab2new.xlsx");
        ////    List<KadrInTime> kadrInTimes = ExcelReader.ReadKadrSets(@"C:\_\1\1_k.xls");
        ////    FZoneTab fZoneTab = new FZoneTab();
        ////    fZoneTab.Calculate(FiltredTobiiList, kadrInTimes, tabOfKeys);
        ////    fZoneTab.FZoneList = tobiiCsvReader.ClearFromGarbageZone(fZoneTab.FZoneList, -1, 500);
        ////    fZoneTab.WriteResult(@"C:\tmp\1\1.txt");

        ////    List<Interval> intervals = ExcelReader.SeparatorIntervalsReadFromExcel(@"C:\_\1\1_reg.xls");
        ////    ResultSeparator resultSeparator = new ResultSeparator(@"C:\_\1\reg\", intervals, fZoneTab.FZoneList, "1");
        ////    resultSeparator.Separate();
        //}

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            // MultipleDirsWorker.PassAllDIrs(@TextBoxTarget.Text, this.TextBoxCurDir,this.TextBox1, @"C:\_\Tab2new2.xlsx");
            MultipleDirsWorker.FixationAddition = 500;
            if (!int.TryParse(TextBoxFixationAddition.Text, out MultipleDirsWorker.FixationAddition))
            {
                System.Windows.MessageBox.Show("Введите корректное значение добавки к фиксациям, в милисекундах"); return;
            }
            MultipleDirsWorker.PassAllDIrs_OneRegFile(@TextBoxTarget.Text, this.TextBoxCurDir, this.TextBox1, @"C:\_\Tab2new2.xlsx", TextBoxKadrDefault.Text);
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            if (folderBrowserDialog1.ShowDialog()==System.Windows.Forms.DialogResult.OK)
            {
                TextBoxTarget.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TextBoxTarget_Copy.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {

        }

        private void CopyTxtToDir(object sender, RoutedEventArgs e)
        {
            FilesOperations.DeepCopyFilesToDir(TextBoxTarget.Text, TextBoxTarget_Copy.Text, "*.txt");
        }

        private void RenameAndAddSufficsAndUID(object sender, RoutedEventArgs e)
        {
            FilesOperations.RenameAndAddSufficsAndUIDAndPath(TextBoxTarget.Text, TextBoxRename.Text, "*.txt");
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            FilesOperations.OutFilepaths(TextBoxTarget.Text, TextBox1);
        }

        private void ButtonTab2ReadClick(object sender, RoutedEventArgs e)
        {
            TabOfKeys tabOfKeys = ExcelReader.ReadTabOfKeys(@"C:\_\Tab2new2.xlsx", "T");
        }

        private void R_filesGenerateButtonClick(object sender, RoutedEventArgs e)
        {
            MultipleDirsWorker.RFilesGenerate(TextBoxTarget.Text, this.TextBoxCurDir, this.TextBox1); 
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            SpecialFor9_41.SortAndUnionFilesInDirs_SpecialFor9_41(TextBoxTarget.Text, TextBoxTarget_Copy.Text);
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            string[] dirs = Directory.GetDirectories(TextBoxTarget.Text,"*", SearchOption.TopDirectoryOnly);
            foreach (var dir in dirs)
            {
                SpecialFor9_41.SortAndUnionFilesInDirsOnSP_SpecialFor9_41(dir);
            }
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            SpecialFor9_41.SortAndUnionFilesInDirsOnPilotName_SpecialFor9_41(TextBoxTarget.Text);
        }


        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            SpecialFor9_41.ParseAllTxtToUnionTable(TextBoxTarget.Text);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            SpecialFor9_41_SCENARY2.SerializeRFiles(Path.Combine(TextBoxTarget.Text,"R.xlsx"), Path.Combine(TextBoxTarget.Text, "RFile.xml"));
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            SpecialFor9_41_SCENARY2.SerializeKFiles(Path.Combine(TextBoxTarget.Text, "K.xlsx"), Path.Combine(TextBoxTarget.Text, "KFile.xml"));
        }
    }
}
