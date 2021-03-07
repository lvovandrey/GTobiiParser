using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using TobiiParser._06;

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
            int NZones = 0;
            if (!int.TryParse(TextBoxNZones.Text, out NZones))
            {
                System.Windows.MessageBox.Show("Задайте кол-во зон");
                return;
            }
            tobiiCsvReader.TobiiCSCRead(filename, tobiiCsvReader.tobiiList,NZones);



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
            int NZones = 0;
            if (!int.TryParse(TextBoxNZones.Text, out NZones))
            {
                System.Windows.MessageBox.Show("Задайте кол-во зон");
                return;
            }
            MultipleDirsWorker.PassAllDIrs_OneRegFile(@TextBoxTarget.Text, this.TextBoxCurDir, this.TextBox1, @"C:\_\Tab2new2.xlsx",NZones, TextBoxKadrDefault.Text);
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
            new SpecialFor9_41_SCENARY2().SerializeRFiles(Path.Combine(TextBoxTarget.Text,"R.xlsx"), Path.Combine(TextBoxTarget.Text, "RFile.xml"));
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            new SpecialFor9_41_SCENARY2().SerializeKFiles(Path.Combine(TextBoxTarget.Text, "K.xlsx"), Path.Combine(TextBoxTarget.Text, "KFile.xml"));
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            MultipleDirsWorker.FixationAddition = 500;
            if (!int.TryParse(TextBoxFixationAddition.Text, out MultipleDirsWorker.FixationAddition))
            {
                System.Windows.MessageBox.Show("Введите корректное значение добавки к фиксациям, в милисекундах"); return;
            }
            int NZones = 0;
            if (!int.TryParse(TextBoxNZones.Text, out NZones))
            {
                System.Windows.MessageBox.Show("Задайте кол-во зон");
                return;
            }
            MultipleDirsWorker.PassAllDIrs_OneRegFile(@TextBoxTarget.Text, this.TextBoxCurDir, this.TextBox1, @"C:\_\Tab2new2.xlsx", NZones, TextBoxKadrDefault.Text);

        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            new SpecialFor9_41_SCENARY4().SerializeRFiles(Path.Combine(TextBoxTarget.Text, "R.xlsx"), Path.Combine(TextBoxTarget.Text, "RFile.xml"));

        }

        private void MenuItem_Click_6(object sender, RoutedEventArgs e)
        {
            new SpecialFor9_41_SCENARY4().SerializeKFiles(Path.Combine(TextBoxTarget.Text, "K.xlsx"), Path.Combine(TextBoxTarget.Text, "KFile.xml"));
        }

        private void MenuItem_Click_7(object sender, RoutedEventArgs e)
        {
            new SpecialFor9_41_SCENARY3().SerializeRFiles(Path.Combine(TextBoxTarget.Text, "R.xlsx"), Path.Combine(TextBoxTarget.Text, "RFile.xml"));
        }

        private void MenuItem_Click_8(object sender, RoutedEventArgs e)
        {
            new SpecialFor9_41_SCENARY3().SerializeKFiles(Path.Combine(TextBoxTarget.Text, "K.xlsx"), Path.Combine(TextBoxTarget.Text, "KFile.xml"));
        }

        //Переименовать файлы в соответствии с ID и таблицей тегов
        private void MenuItem_Click_9(object sender, RoutedEventArgs e)
        {
            SpecialFor9_41.RenameCsvFileAccordingToTagTable(@TextBoxTarget.Text, @"C:\_\TagToId.xlsx", TextBox1);
        }

        //Переименовать файлы в соответствии с ID котрый найден в их имени
        private void MenuItem_Click_10(object sender, RoutedEventArgs e)
        {
            SpecialFor9_41.RenameCsvFileOnlyToID(@TextBoxTarget.Text, TextBox1);
        }

        private void MenuItem_Click_11(object sender, RoutedEventArgs e)
        {
            SpecialFor9_41.OutputIdFromRFile(Path.Combine(TextBoxTarget.Text, "RFile.xml"), TextBox1);
        }

        private void MenuItem_Click_12(object sender, RoutedEventArgs e)
        {
            SpecialFor9_41.OutputIdFromKFile(Path.Combine(TextBoxTarget.Text, "KFile.xml"), TextBox1);
        }

        private void MenuItem_Click_13(object sender, RoutedEventArgs e)
        {
            SpecialFor9_41.SyncronizeRFileAccordingToSyncToIdTable(Path.Combine(TextBoxTarget.Text, "RFile.xml"), @"C:\_\SyncToId.xlsx", TextBox1);
        }

        private void MenuItem_Click_14(object sender, RoutedEventArgs e)
        {
            SpecialFor9_41.SyncronizeKFileAccordingToSyncToIdTable(Path.Combine(TextBoxTarget.Text, "KFile.xml"), @"C:\_\SyncToId.xlsx", TextBox1);
        }

        private void MenuItem_Click_15(object sender, RoutedEventArgs e)
        {
            new SpecialFor9_41_POSADKI().SerializeRFiles(Path.Combine(TextBoxTarget.Text, "R.xlsx"), Path.Combine(TextBoxTarget.Text, "RFile.xml"));
        }

        private void MenuItem_Click_16(object sender, RoutedEventArgs e)
        {
            new SpecialFor9_41_POSADKI().SerializeKFiles(Path.Combine(TextBoxTarget.Text, "K.xlsx"), Path.Combine(TextBoxTarget.Text, "KFile.xml"));
        }

        private void MenuItem_Click_17(object sender, RoutedEventArgs e)
        {
            SpecialFor9_41.CopyFilesInDirsAccordingTagsInFilename(TextBoxTarget.Text, TextBoxTarget_Copy.Text, TextBox1);
        }

        private void MenuItem_Click_18(object sender, RoutedEventArgs e)
        {
            SpecialFor9_41_SCENARY1.UnionFilesOnRegims(TextBoxTarget.Text);
        }

        private void MenuItem_Click_19(object sender, RoutedEventArgs e)
        {
            SpecialFor9_41_SCENARY2.UnionFilesOnRegims(TextBoxTarget.Text);
        }

        private void MenuItem_Click_20(object sender, RoutedEventArgs e)
        {
            SpecialFor9_41_SCENARY3.UnionFilesOnRegims(TextBoxTarget.Text);
        }

        private void MenuItem_Click_21(object sender, RoutedEventArgs e)
        {
            new SpecialFor9_41_AIVAZYAN().CalculateSaccades(TextBoxTarget.Text);
        }

        private void MenuItem_Click_22(object sender, RoutedEventArgs e)
        {
            new SpecialFor9_41_AIVAZYAN().ConvertTo5Hz(TextBoxTarget.Text);
        }

        private void MenuItem_Click_23(object sender, RoutedEventArgs e)
        {
            new SpecialFor9_41_AIVAZYAN().SyncronizeCommonTxtFiles(TextBoxTarget.Text, @"c:\_\SyncToID.txt");
        }

        private void MenuItem_Click_24(object sender, RoutedEventArgs e)
        {
            new SpecialFor9_41_SP_NewProcessing().SerializeRFiles(Path.Combine(TextBoxTarget.Text, "R.xlsx"), Path.Combine(TextBoxTarget.Text, "RFile.xml"));
        }

        private void MenuItem_Click_25(object sender, RoutedEventArgs e)
        {
            new SpecialFor9_41_SP_NewProcessing().SerializeKFiles(Path.Combine(TextBoxTarget.Text, "K.xlsx"), Path.Combine(TextBoxTarget.Text, "KFile.xml"));
        }

        private void MenuItem_Click_26(object sender, RoutedEventArgs e)
        {
            MultipleDirsWorker.FixationAddition = 500;
            if (!int.TryParse(TextBoxFixationAddition.Text, out MultipleDirsWorker.FixationAddition))
            {
                System.Windows.MessageBox.Show("Введите корректное значение добавки к фиксациям, в милисекундах"); return;
            }
            int NZones = 0;
            if (!int.TryParse(TextBoxNZones.Text, out NZones))
            {
                System.Windows.MessageBox.Show("Задайте кол-во зон");
                return;
            }
            MultipleDirsWorker.PassAllDIrs_WithoutKAndRFiles(@TextBoxTarget.Text, this.TextBoxCurDir, this.TextBox1, @"C:\__\Tab2new2.xlsx", NZones, TextBoxKadrDefault.Text);


        }

        private void MenuItem_Click_27(object sender, RoutedEventArgs e)
        {
            SpecialFor9_41_OPERATOR.ParseForSpecialGazeParams(@TextBoxTarget.Text, this.TextBoxCurDir, this.TextBox1);
        }

        private void MenuItem_Click_28(object sender, RoutedEventArgs e)
        {
            MultipleDirsWorker.FixationAddition = 500;
            if (!int.TryParse(TextBoxFixationAddition.Text, out MultipleDirsWorker.FixationAddition))
            {
                System.Windows.MessageBox.Show("Введите корректное значение добавки к фиксациям, в милисекундах"); return;
            }
            int NZones = 0;
            if (!int.TryParse(TextBoxNZones.Text, out NZones))
            {
                System.Windows.MessageBox.Show("Задайте кол-во зон");
                return;
            }
            MultipleDirsWorker.PassAllDIrs_OneRegFile(@TextBoxTarget.Text, this.TextBoxCurDir, this.TextBox1, @"C:\_\Tab2new2.xlsx", NZones, TextBoxKadrDefault.Text, "E");

        }

        private void MenuItem_Click_29(object sender, RoutedEventArgs e)
        {
            new For06_CALC1().SerializeRFilesFromCSVinDir(TextBoxTarget.Text, Path.Combine(TextBoxTarget.Text, "RFile.xml"));
        }

        private void MenuItem_Click_30(object sender, RoutedEventArgs e)
        {
            new For06_CALC1().SerializeKFiles(Path.Combine(TextBoxTarget.Text, "K.xlsx"), Path.Combine(TextBoxTarget.Text, "KFile.xml"));
        }
    }
}

