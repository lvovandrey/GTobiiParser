using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
            TabOfKeys tabOfKeys = ExcelReader.ReadTabOfKeys(tab2File);
            List<KadrInTime> kadrInTimes = ExcelReader.ReadKadrSets(file_k);
            FZoneTab fZoneTab = new FZoneTab();
            fZoneTab.Calculate(FiltredTobiiList, kadrInTimes, tabOfKeys);
            fZoneTab.FZoneList = tobiiCsvReader.ClearFromGarbageZone(fZoneTab.FZoneList, -1, 500);
            fZoneTab.FZoneList = tobiiCsvReader.CompactTobiiRecords(fZoneTab.FZoneList);

            fZoneTab.WriteResult(file_csv.Replace(".csv", ".txt"));
            
            List<Interval> intervals = ExcelReader.SeparatorIntervalsReadFromExcel(file_reg);
            ResultSeparator resultSeparator = new ResultSeparator(dir+@"\reg\", intervals, fZoneTab.FZoneList, Path.GetFileName(file_csv).Replace(".csv", "_"));
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
    }
}
