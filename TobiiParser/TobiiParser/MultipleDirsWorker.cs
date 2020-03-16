using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TobiiParser
{
    class MultipleDirsWorker
    {

        public static  void ParseInDirectory(string dir, string file_csv, string file_k, string file_reg, string tab2File)
        {
            TobiiCsvReader tobiiCsvReader = new TobiiCsvReader();
            List<TobiiRecord> tobiiRecords = new List<TobiiRecord>();
            tobiiCsvReader.TobiiCSCRead(file_csv, tobiiRecords);
            List<TobiiRecord> FiltredTobiiList = tobiiCsvReader.CompactTobiiRecords(tobiiRecords);
            TabOfKeys tabOfKeys = ExcelReader.ReadTabOfKeys(tab2File, "T");
            List<KadrInTime> kadrInTimes = ExcelReader.ReadKadrSets(file_k);
            FZoneTab fZoneTab = new FZoneTab();
            List<TobiiRecord> FZoneList = fZoneTab.Calculate(FiltredTobiiList, kadrInTimes, tabOfKeys);
            FZoneList = tobiiCsvReader.ClearFromGarbageZone(FZoneList, -1, 500);
            FZoneList = tobiiCsvReader.CompactTobiiRecords(FZoneList, "FZones");

            fZoneTab.WriteResult(file_csv.Replace(".csv", ".txt"), FZoneList);
            
            List<Interval> intervals = ExcelReader.SeparatorIntervalsReadFromExcel(file_reg);
            ResultSeparator resultSeparator = new ResultSeparator(dir+@"\reg\", intervals, FZoneList, Path.GetFileName(file_csv).Replace(".csv", "_"));
            resultSeparator.Separate();
        }

        public static async void PassAllDIrs(string mainDir, TextBox textBox, TextBox Big_textBox, string tab2File)
        {
            string[] dirs = Directory.GetDirectories(mainDir, "*", SearchOption.AllDirectories);
            foreach (var dir in dirs)
            {
                string file_csv, file_k, file_reg;
                string[] filescsv = Directory.GetFiles(dir, "*.csv", SearchOption.TopDirectoryOnly);
                if(filescsv.Count()>1) { Big_textBox.Text += "В директории " + dir + "       содержится более 1 файла csv"+Environment.NewLine; continue; }
                else if (filescsv.Count() < 1) { Big_textBox.Text += "В директории " + dir + "          нет файла csv" + Environment.NewLine; continue; }
                file_csv = filescsv[0];
                file_k = file_csv.Replace("1.csv", "k.xls");
                file_reg = file_csv.Replace("1.csv", "r.xls");

                if (!File.Exists(file_k) || !File.Exists(file_reg)) { Big_textBox.Text += "В директории " + dir + "      не полный комплект файлов xls" + Environment.NewLine; continue; }


                textBox.Text = "Обрабатываю " + dir;
                await Task.Run(()=>ParseInDirectory(dir,file_csv, file_k, file_reg, tab2File));
            }

            textBox.Text = "Обработка завершена";
        }


        public static async void PassAllDIrs_OneRegFile(string mainDir, TextBox textBox, TextBox Big_textBox, string tab2File, string KadrDefault="")
        {
            string[] dirs = Directory.GetDirectories(mainDir, "*", SearchOption.AllDirectories);
            foreach (var dir in dirs)
            {
                string file_csv, file_k, file_r;
                string[] filescsv = Directory.GetFiles(dir, "*.csv", SearchOption.TopDirectoryOnly);
                if (filescsv.Count() > 1) { Big_textBox.Text += "В директории " + dir + "       содержится более 1 файла csv" + Environment.NewLine; continue; }
                else if (filescsv.Count() < 1) { Big_textBox.Text += "В директории " + dir + "          нет файла csv" + Environment.NewLine; continue; }
                file_csv = filescsv[0];

                file_k = Path.Combine(mainDir, "KFile.xml");
                file_r = Path.Combine(mainDir, "RFile.xml"); 

                if (!File.Exists(file_r))
                {
                    Big_textBox.Text += "Не могу найти файл с разбивкой режимов" + Environment.NewLine;
                    break;
                }
                if (!File.Exists(file_k))
                {
                    Big_textBox.Text += "Не могу найти файл с разбивкой кадров" + Environment.NewLine;
                    break;
                }

                textBox.Text = "Обрабатываю " + dir;
                await Task.Run(() => ParseInDirectory_OneRegFile(dir, file_csv, file_k, file_r, tab2File, KadrDefault));
            }

            textBox.Text = "Обработка завершена";
        }

        private static void ParseInDirectory_OneRegFile(string dir, string file_csv, string file_k, string file_r, string tab2File, string kadrDefault="")
        {
            TobiiCsvReader tobiiCsvReader = new TobiiCsvReader();
            List<TobiiRecord> tobiiRecords = new List<TobiiRecord>();
            tobiiCsvReader.TobiiCSCRead(file_csv, tobiiRecords);
            List<TobiiRecord> FiltredTobiiList = tobiiCsvReader.CompactTobiiRecords(tobiiRecords);
            TabOfKeys tabOfKeys = ExcelReader.ReadTabOfKeys(tab2File, "T");

            Regex regex = new Regex(@"id\d{3}");
            MatchCollection matches = regex.Matches(Path.GetFileName(file_csv));
            if (matches.Count > 1 || matches.Count==0) { MessageBox.Show("В имени файла " + file_csv + " найдено неверное кол-во id (0 или более 1)"); return;  }
            string FileId = matches[0].Value.Replace("id", "");

            KadrIntervals kadrIntervals;
            kadrIntervals = SpecialFor9_41_SCENARY2.GetKadrIntervalsInXmlKFile(file_k, FileId);

           
            FZoneTab fZoneTab = new FZoneTab();
            List<TobiiRecord> FZoneList = fZoneTab.Calculate(FiltredTobiiList, kadrIntervals, tabOfKeys);
            FZoneList = tobiiCsvReader.ClearFromGarbageZone(FZoneList, -1, 100);
            FZoneList = tobiiCsvReader.CompactTobiiRecords(FZoneList, "FZones");

            fZoneTab.WriteResult(file_csv.Replace(".csv", ".txt"), FZoneList);

            SeparatorIntervals separatorIntervals = SpecialFor9_41_SCENARY2.GetSeparatorIntervalsInXmlKFile(file_r, FileId);

            ResultSeparator resultSeparator = new ResultSeparator(dir + @"\reg\", separatorIntervals.Intervals, FZoneList, Path.GetFileName(file_csv).Replace(".csv", "_"));
            resultSeparator.Separate();
        }

        internal static async void RFilesGenerate(string mainDir, TextBox textBox, TextBox Big_textBox)
        {
           string[] dirs = Directory.GetDirectories(mainDir, "*", SearchOption.AllDirectories);
            foreach (var dir in dirs)
            {
                string file_csv, file_reg;
                string[] filescsv = Directory.GetFiles(dir, "*.csv", SearchOption.TopDirectoryOnly);
                if (filescsv.Count() > 1) { Big_textBox.Text += "В директории " + dir + "       содержится более 1 файла csv" + Environment.NewLine; continue; }
                else if (filescsv.Count() < 1) { Big_textBox.Text += "В директории " + dir + "          нет файла csv" + Environment.NewLine; continue; }
                file_csv = filescsv[0];
                file_reg = file_csv.Replace(".csv", "_r.txt");

                textBox.Text = "Обрабатываю " + dir;
                await Task.Run(() => RFilesGenerateInDirectory(mainDir, dir, file_csv, file_reg));
            }

            textBox.Text = "Обработка завершена";
        }

        private static async void RFilesGenerateInDirectory(string MainDir, string dir, string file_csv, string file_reg)
        {
            TobiiCsvReader tobiiCsvReader = new TobiiCsvReader();
            List<Interval> Intervals = new List<Interval>();
            Intervals =  tobiiCsvReader.TobiiIntervalRead(file_csv);
           
            Interval.WriteResult(file_csv.Replace(".csv", ".txt"), Intervals);
            string mainFileName = Path.Combine(MainDir, "RFile.txt");
            Interval.AppendWriteResultAsync(mainFileName, Intervals, file_csv);

        }
    }
}
