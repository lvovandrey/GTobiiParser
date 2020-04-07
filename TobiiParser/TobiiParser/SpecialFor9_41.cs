using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Excel = Microsoft.Office.Interop.Excel;

namespace TobiiParser
{
    public class SpecialFor9_41
    {
        /// <summary>
        /// Сортировка файлов по папкам с учетом отличительного признака - для 9.41 делаем
        /// </summary>
        /// <param name="mainDir"></param>
        /// <param name="mark"></param>
        /// <param name="filemask"></param>
        internal static void SortAndUnionFilesInDirs_SpecialFor9_41(string mainDir, string targetDir)
        {

            string[] files = Directory.GetFiles(mainDir, "*.txt", SearchOption.TopDirectoryOnly);
            foreach (string fullfilepath1 in files)
            {
                if (!File.Exists(fullfilepath1)) continue;

                string filepath1 = Path.GetFileName(fullfilepath1);


                int N_pos1 = filepath1.IndexOf("№"); // ищем позицию номера 
                string N_subs1 = filepath1.Substring(N_pos1 + 1, 2);     //AAA  берем подстроку после номера - ну сам номер порядковый
                string Regim_subs1 = filepath1.Substring(0, N_pos1 - 1);  //ZZZ  берем подстроку до номера - в ней весь режим

                int SP_pos1 = filepath1.IndexOf("СП");   //ищем позицию сложного положения
                int EXT_pos1 = filepath1.IndexOf(".txt");      //ищем позицию расширения
                string SP_subs1 = filepath1.Substring(SP_pos1 + 2, EXT_pos1 - SP_pos1 - 2); //XX     берем номер сложного положения

                string NewDirName = Path.Combine(targetDir, Regim_subs1 + "СП" + SP_subs1);
                string NewFile1Name = Path.Combine(NewDirName, filepath1);

                if (!Directory.Exists(NewDirName))
                {
                    Directory.CreateDirectory(NewDirName);
                    FileInfo fi = new FileInfo(fullfilepath1);
                    fi.MoveTo(NewFile1Name);
                }


                string[] files2 = Directory.GetFiles(mainDir, "*.txt", SearchOption.TopDirectoryOnly);
                foreach (string fullfilepath2 in files2)
                {
                    string filepath2 = Path.GetFileName(fullfilepath2);


                    int N_pos2 = filepath2.IndexOf("№");
                    string N_subs2 = filepath2.Substring(N_pos2 + 1, 2);     //AAA
                    string Regim_subs2 = filepath2.Substring(0, N_pos2 - 1);  //ZZZ

                    int SP_pos2 = filepath2.IndexOf("СП");
                    int EXT_pos2 = filepath2.IndexOf(".txt");
                    string SP_subs2 = filepath2.Substring(SP_pos2 + 2, EXT_pos2 - SP_pos2 - 2); //XX

                    string NewFile2Name = Path.Combine(NewDirName, filepath2);
                    if ((Regim_subs1 == Regim_subs2) && (SP_subs1 == SP_subs2) && (N_subs1 != N_subs2))
                    {
                        if (Directory.Exists(NewDirName))
                        {
                            FileInfo fi2 = new FileInfo(fullfilepath2);
                            fi2.MoveTo(NewFile2Name);
                        }
                    }
                }

            }
        }

        /// <summary>
        /// Сортировка файлов по папкам по признаку сложных положений - для 9.41 делаем. Поиск файлов для сортировки ведется только внутри указанной директории - без захода во вложенные директории
        /// </summary>
        /// <param name="mainDir"></param>
        internal static void SortAndUnionFilesInDirsOnSP_SpecialFor9_41(string mainDir)
        {

            string[] files = Directory.GetFiles(mainDir, "*.txt", SearchOption.TopDirectoryOnly);
            List<string> SPNames = new List<string>()
            {
                "СП 1",
                "СП 2",
                "СП 3",
                "СП 4",
                "СП 5",
                "СП 6",
                "СП 7",
                "СП 8",
                "СП 9",
                "СП 10"
            };

            foreach (string fullfilepath in files)
            {
                if (!File.Exists(fullfilepath)) continue;

                string filepath = Path.GetFileName(fullfilepath);

                foreach (var SPName in SPNames)
                {
                    if (filepath.Contains(SPName + ".txt"))
                    {
                        string NewDirPath = Path.Combine(mainDir, SPName);
                        if (!Directory.Exists(NewDirPath))
                            Directory.CreateDirectory(NewDirPath);
                        string NewFileName = Path.Combine(NewDirPath, filepath);
                        FileInfo fi = new FileInfo(fullfilepath);
                        fi.MoveTo(NewFileName);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Сортировка файлов по папкам по признаку Фамилия летчика - для 9.41 делаем. Поиск файлов для сортировки ведется только внутри указанной директории - без захода во вложенные директории
        /// </summary>
        /// <param name="mainDir"></param>
        internal static void SortAndUnionFilesInDirsOnPilotName_SpecialFor9_41(string mainDir)
        {

            string[] files = Directory.GetFiles(mainDir, "*.txt", SearchOption.AllDirectories);

            List<string> Pilots = new List<string>()
            {
                "Петруша",
                "Коноваленко",
                "Рыков",
                "Виноградов",
                "Скоромнов",
                "Журавлев"
            };

            foreach (string fullfilepath in files)
            {
                if (!File.Exists(fullfilepath)) continue;

                string filepath = Path.GetFileName(fullfilepath);
                string dir = Path.GetDirectoryName(fullfilepath);

                bool isFind = false;
                foreach (var Pilot in Pilots)
                {
                    if (filepath.Contains(Pilot))
                    {
                        string NewDirPath = Path.Combine(dir, Pilot);
                        if (!Directory.Exists(NewDirPath))
                            Directory.CreateDirectory(NewDirPath);
                        string NewFileName = Path.Combine(NewDirPath, filepath);
                        FileInfo fi = new FileInfo(fullfilepath);
                        fi.MoveTo(NewFileName);
                        isFind = true;
                        break;
                    }
                }
                if (!isFind)
                {
                    MessageBox.Show("Не найдена фамилия в файле " + fullfilepath);
                    Console.WriteLine(fullfilepath);
                }
            }
        }





        public class TxtFileResult
        {
            public List<TobiiRecord> tobiiRecords = new List<TobiiRecord>();
            public List<string> tags = new List<string>();
            public string OrderNumber;
            public string filename;

        }


        internal static void ParseAllTxtToUnionTable(string mainDir)
        {
            List<TxtFileResult> txtFileResults = new List<TxtFileResult>();

            string[] files = Directory.GetFiles(mainDir, "*.txt", SearchOption.AllDirectories);



            foreach (string fullfilepath in files)
            {
                char separator = '\n';
                char delimiter = '\t';
                char separatorDirs = '\\';

                TxtFileResult txtFileResult = new TxtFileResult();

                string filepath = Path.GetFileName(fullfilepath);
                string dir = Path.GetDirectoryName(fullfilepath);


                string dirToTagsTmp = dir.Replace(mainDir, "");
                txtFileResult.tags = dirToTagsTmp.Split(separatorDirs).ToList();

                int N_pos = filepath.IndexOf("№"); // ищем позицию номера 
                txtFileResult.OrderNumber = filepath.Substring(N_pos, 3);

                txtFileResult.filename = filepath;


                using (StreamReader rd = new StreamReader(new FileStream(fullfilepath, FileMode.Open)))
                {

                    string[] str_arr = { "" };
                    string big_str = "";
                    TobiiCsvReader.ReadPartOfFile(rd, out big_str); // TODO: я расчитываю что файл режимов будет меньше 10000 строк
                    str_arr = big_str.Split(separator);

                    int i = 0;

                    for (i = 0; i < str_arr.Length; i++)
                    {
                        if (str_arr[i] == "") continue;
                        string[] tmp = { "" };
                        tmp = str_arr[i].Split(delimiter);
                        int timeInMs = int.Parse(tmp[0]) * 3_600_000 + int.Parse(tmp[1]) * 60_000 + int.Parse(tmp[2]) * 1000 + int.Parse(tmp[3]);
                        TimeSpan timeOfCurFixation = TimeSpan.FromMilliseconds(timeInMs);

                        txtFileResult.tobiiRecords.Add(new TobiiRecord() { time_ms = timeInMs, CurFZone = int.Parse(tmp[4]) });

                    }


                }
                txtFileResults.Add(txtFileResult);
            }

            WriteTxtFileResultAsync(@"C:\_\1.csv", txtFileResults);

        }


        //Файл ZonesInterpretation.txt у меня такого формата
        //  -1	    ?
        //  0	    ?
        //  1	    Курс
        //  2	    АГ
        //  3	    Вариометр
        //  4	    Внекаб.обст.
        //  5 	    Высота
        //  6	    Перегрузка
        //  7	    Скорость
        //  8	    Угол атаки
        //  9	    ПЛТ другое
        //  10	    ИКШ-Скорость
        //  11	    ИКШ-Высота
        //  12	    ИКШ-Авиагоризонт
        //  13	    Другое

        /// <summary>
        /// Читает файл с интерпретацией зон - перевод номеров зон в их названия
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ReadZonesInterpretation(string filename)
        {
            Dictionary<string, string> ZonesInterpretation = new Dictionary<string, string>();
            using (StreamReader rd = new StreamReader(new FileStream(filename, FileMode.Open)))
            {
                char separator = '\n';
                char delimiter = '\t';
                string[] str_arr = { "" };
                string big_str = "";
                TobiiCsvReader.ReadPartOfFile(rd, out big_str);
                str_arr = big_str.Split(separator);

                int i = 0;
                for (i = 0; i < str_arr.Length; i++)
                {
                    if (str_arr[i] == "") continue;
                    string[] tmp = { "" };
                    tmp = str_arr[i].Split(delimiter);

                    ZonesInterpretation.Add(tmp[0], tmp[1]);
                }
            }
            return ZonesInterpretation;
        }


        public static async void WriteTxtFileResultAsync(string filename, List<TxtFileResult> TxtFileResults)
        {
            using (StreamWriter writer = new StreamWriter(new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write), Encoding.Unicode))
            {
                await Task.Run(() => WriteTxtFileResult(writer, TxtFileResults));
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        static void WriteTxtFileResult(StreamWriter writer, List<TxtFileResult> TxtFileResults)
        {
            Dictionary<string, string> ZonesInterpretation = ReadZonesInterpretation(@"C:\_\ZonesInterpretation.txt");
            foreach (var r in TxtFileResults)
            {

                long timeBegin = r.tobiiRecords.First().time_ms;
                foreach (var tr in r.tobiiRecords)
                {
                    string s = "";
                    foreach (var tag in r.tags)
                        s += tag + "\t";
                    s += r.OrderNumber + "\t";
                    s += r.filename + "\t";
                    s += (r.tobiiRecords.IndexOf(tr) + 1).ToString() + "\t";
                    s += tr.CurFZone + "\t";
                    s += ZonesInterpretation[tr.CurFZone.ToString()] + "\t";
                    s += ((double)tr.time_ms) / (24 * 3_600_000) + "\t";
                    s += (tr.time_ms - timeBegin) + "\t";

                    if (r.tobiiRecords.IndexOf(tr) + 1 >= r.tobiiRecords.Count())
                    {
                        s += "-------";
                        writer.WriteLine(s);
                        break;
                    }
                    long nextTime = r.tobiiRecords[r.tobiiRecords.IndexOf(tr) + 1].time_ms;
                    s += (nextTime - tr.time_ms).ToString();

                    writer.WriteLine(s);
                }
                writer.WriteLine("");
            }
        }

        /// <summary>
        /// Переименовать файлы в соответствии с ID и таблицей тегов
        /// </summary>
        /// <param name="dir">Директория с csv-файлами</param>
        /// <param name="TagTableFilename">Путь к таблице тегов файлов по ID</param>
        public static void RenameCsvFileAccordingToTagTable(string dir, string TagTableFilename, TextBox OuterTextBox, string LastColumn = "G")
        {

            string[] filescsv = Directory.GetFiles(dir, "*.csv", SearchOption.TopDirectoryOnly);
            List<string> filesIds = new List<string>();
            Regex regex = new Regex(@"\d{3}");//в файле будем искать id по признаку три цифры подряд - типа того "123"
            bool CheckSuccess = true;

            //Перед выполнением проверяем файлы - чтобы не было повторов, чтоб id везде были и только по 1 на файл
            foreach (var filecsv in filescsv)
            {
                string fileShortName = Path.GetFileName(filecsv);
                MatchCollection matches = regex.Matches(fileShortName);
                if (matches.Count != 1)
                {
                    OuterTextBox.Text += "Не найден id (или более 1) в файле " + fileShortName + "\n";
                    CheckSuccess = false;
                }
                else
                {
                    if (filesIds.Contains(matches[0].Value))
                    {
                        OuterTextBox.Text += "Повторный id в еще одном файле: " + fileShortName + "\n";
                        CheckSuccess = false;
                    }
                    else
                    {
                        filesIds.Add(matches[0].Value);
                    }
                }
            }

            if (!CheckSuccess) return;

            //считываем данные из Excel файла в двумерный массив
            Excel.Application xlApp = new Excel.Application(); //Excel
            Excel.Workbook xlWB; //рабочая книга              
            Excel.Worksheet xlSht; //лист Excel   
            xlWB = xlApp.Workbooks.Open(TagTableFilename); //название файла Excel    
            xlSht = xlWB.Worksheets[1]; //название листа или 1-й лист в книге xlSht = xlWB.Worksheets[1];

            int iLastRow = xlSht.Cells[xlSht.Rows.Count, "A"].End[Excel.XlDirection.xlUp].Row;
            var arrData = (object[,])xlSht.Range["A1:" + LastColumn + iLastRow].Value; //берём данные с листа Excel

            int i;
            List<string> ids = new List<string>();

            foreach (var filecsv in filescsv)
            {
                string fileShortName = Path.GetFileName(filecsv);
                MatchCollection matches = regex.Matches(fileShortName);
                string curId = matches[0].Value;
                bool curIdFindInTagTable = false;

                for (i = 1; i <= arrData.GetUpperBound(0); i++)
                {
                    string idFromTagTable = ((string)arrData[i, 4]).Replace("id", "");
                    if (curId == idFromTagTable)
                    {
                        curIdFindInTagTable = true;

                        string NewFileName = (string)arrData[i, 2] + " " +
                            (string)arrData[i, 3] + " " +
                            (string)arrData[i, 4] + " " +
                            (string)arrData[i, 5] + " " +
                            (string)arrData[i, 6] + " " +
                            (string)arrData[i, 7] + ".csv";

                        File.Move(filecsv, Path.Combine(dir, NewFileName));

                        break;
                    }
                }

                if (!curIdFindInTagTable)
                    OuterTextBox.Text += "Id файла:     " + fileShortName + "   не найден в " + Path.GetFileName(TagTableFilename) + " \n";
            }
        }

        /// <summary>
        /// Переименовать файлы в соответствии с найденными в них ID 
        /// </summary>
        /// <param name="dir">Директория с csv-файлами</param>
        /// <param name="TagTableFilename">Путь к таблице тегов файлов по ID</param>
        public static void RenameCsvFileOnlyToID(string dir, TextBox OuterTextBox)
        {

            string[] filescsv = Directory.GetFiles(dir, "*.csv", SearchOption.TopDirectoryOnly);
            List<string> filesIds = new List<string>();
            Regex regex = new Regex(@"\d{3}");//в файле будем искать id по признаку три цифры подряд - типа того "123"
            bool CheckSuccess = true;

            //Перед выполнением проверяем файлы - чтобы не было повторов, чтоб id везде были и только по 1 на файл
            foreach (var filecsv in filescsv)
            {
                string fileShortName = Path.GetFileName(filecsv);
                MatchCollection matches = regex.Matches(fileShortName);
                if (matches.Count != 1)
                {
                    OuterTextBox.Text += "Не найден id (или более 1) в файле " + fileShortName + "\n";
                    CheckSuccess = false;
                }
                else
                {
                    if (filesIds.Contains(matches[0].Value))
                    {
                        OuterTextBox.Text += "Повторный id в еще одном файле: " + fileShortName + "\n";
                        CheckSuccess = false;
                    }
                    else
                    {
                        filesIds.Add(matches[0].Value);
                    }
                }
            }

            if (!CheckSuccess) return;

            OuterTextBox.Text += "----------" + "\n" + "Переименовать файлы в соответствии с найденными в них ID в папке " + dir + "\n" + "-----------------------" + "\n";

            foreach (var filecsv in filescsv)
            {
                string fileShortName = Path.GetFileName(filecsv);
                MatchCollection matches = regex.Matches(fileShortName);
                string curId = matches[0].Value;
                string NewFileName = "id" + curId + ".csv";
                 File.Move(filecsv, Path.Combine(dir, NewFileName));
                OuterTextBox.Text += curId + "\n";
            }

            OuterTextBox.Text += "\n" + "-----------------------";
        }

        /// <summary>
        /// Вывод всех ID, которые есть в R-файле
        /// </summary>
        /// <param name="file_r"></param>
        /// <param name="OuterTextBox"></param>
        public static void OutputIdFromRFile(string file_r, TextBox OuterTextBox)
        {
            List<SeparatorIntervals> Lst = SpecialFor9_41_SCENARY2.DeserializeRFiles(file_r);
            OuterTextBox.Text += "----------" + "\n" + "Перечень ID в файле " + file_r + "\n" + "-----------------------" + "\n";
            foreach (var Interval in Lst)
            {
                OuterTextBox.Text+= Interval.Id+"\n";
            }
            OuterTextBox.Text += "\n" + "-----------------------";
        }

        /// <summary>
        /// Вывод всех ID, которые есть в К-файле
        /// </summary>
        /// <param name="file_r"></param>
        /// <param name="OuterTextBox"></param>
        public static void OutputIdFromKFile(string file_k, TextBox OuterTextBox)
        {
            List<KadrIntervals> Lst = SpecialFor9_41_SCENARY2.DeserializeKFiles(file_k);
            OuterTextBox.Text += "----------" + "\n" + "Перечень ID в файле " + file_k + "\n" + "-----------------------" + "\n";
            foreach (var Interval in Lst)
            {
                OuterTextBox.Text += Interval.Id + "\n";
            }
            OuterTextBox.Text += "\n" + "-----------------------";
        }


        /// <summary>
        /// Синхронизировать R-файл в соответствии с ID и таблицей синхронизации
        /// </summary>
        /// <param name="file_r">Путь к Rфайлу</param>
        /// <param name="SyncToIdTableFilename">Путь к таблице синхронизации по ID</param>
        public static void SyncronizeRFileAccordingToSyncToIdTable(string file_r, string SyncToIdTableFilename, TextBox OuterTextBox)
        {
            List<SeparatorIntervals> R = SpecialFor9_41_SCENARY2.DeserializeRFiles(file_r);

            //считываем данные из Excel файла в двумерный массив
            Excel.Application xlApp = new Excel.Application(); //Excel
            Excel.Workbook xlWB; //рабочая книга              
            Excel.Worksheet xlSht; //лист Excel   
            xlWB = xlApp.Workbooks.Open(SyncToIdTableFilename); //название файла Excel    
            xlSht = xlWB.Worksheets[1]; //название листа или 1-й лист в книге xlSht = xlWB.Worksheets[1];

            int iLastRow = xlSht.Cells[xlSht.Rows.Count, "A"].End[Excel.XlDirection.xlUp].Row;
            var arrData = (object[,])xlSht.Range["A1:B" + iLastRow].Value; //берём данные с листа Excel

            int i;
      
            foreach (var Intervals in R)
            {
                string curId = Intervals.Id;
                bool curIdFindInSyncTable = false;
                for (i = 1; i <= arrData.GetUpperBound(0); i++)
                {
                    string idFromSyncTable = ((string)arrData[i, 1]);
                    if (curId == idFromSyncTable)
                    {
                        long delta_t = (long)(24 * 3_600_00 * (double)arrData[i, 2]);
                        curIdFindInSyncTable = true;
                        foreach (var interval in Intervals.Intervals)
                        {
                            interval.Time_ms_beg += delta_t;
                            interval.Time_ms_end += delta_t;
                        }
                        break;
                    }
                }

                if (!curIdFindInSyncTable)
                    OuterTextBox.Text += "Id файла:     " + curId + "   не найден в " + Path.GetFileName(SyncToIdTableFilename) + " \n";
            }
            new SpecialFor9_41_SCENARY2().SerializeRFiles(R, file_r.Replace(".xml","_sync.xml"));            
        }

        /// <summary>
        /// Синхронизировать K-файл в соответствии с ID и таблицей синхронизации
        /// </summary>
        /// <param name="file_k">Путь к Rфайлу</param>
        /// <param name="SyncToIdTableFilename">Путь к таблице синхронизации по ID</param>
        public static void SyncronizeKFileAccordingToSyncToIdTable(string file_k, string SyncToIdTableFilename, TextBox OuterTextBox)
        {
            List<KadrIntervals> K = SpecialFor9_41_SCENARY2.DeserializeKFiles(file_k);

            //считываем данные из Excel файла в двумерный массив
            Excel.Application xlApp = new Excel.Application(); //Excel
            Excel.Workbook xlWB; //рабочая книга              
            Excel.Worksheet xlSht; //лист Excel   
            xlWB = xlApp.Workbooks.Open(SyncToIdTableFilename); //название файла Excel    
            xlSht = xlWB.Worksheets[1]; //название листа или 1-й лист в книге xlSht = xlWB.Worksheets[1];

            int iLastRow = xlSht.Cells[xlSht.Rows.Count, "A"].End[Excel.XlDirection.xlUp].Row;
            var arrData = (object[,])xlSht.Range["A1:B" + iLastRow].Value; //берём данные с листа Excel

            int i;

            foreach (var Intervals in K)
            {
                string curId = Intervals.Id;
                bool curIdFindInSyncTable = false;
                for (i = 1; i <= arrData.GetUpperBound(0); i++)
                {
                    string idFromSyncTable = ((string)arrData[i, 1]);
                    if (curId == idFromSyncTable)
                    {
                        long delta_t = (long)(24 * 3_600_00 * (double)arrData[i, 2]);
                        curIdFindInSyncTable = true;
                        foreach (var interval in Intervals.Intervals)
                        {
                            interval.Time_ms_beg += delta_t;
                            interval.Time_ms_end += delta_t;
                        }
                        break;
                    }
                }

                if (!curIdFindInSyncTable)
                    OuterTextBox.Text += "Id файла:     " + curId + "   не найден в " + Path.GetFileName(SyncToIdTableFilename) + " \n";
            }
            new SpecialFor9_41_SCENARY2().SerializeKFiles(K, file_k.Replace(".xml", "_sync.xml"));
        }
    }
}
