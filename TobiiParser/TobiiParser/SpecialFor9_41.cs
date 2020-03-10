using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

                    int  i = 0;

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
                    s+=r.OrderNumber + "\t";
                    s += r.filename + "\t";
                    s += (r.tobiiRecords.IndexOf(tr) + 1).ToString() + "\t";
                    s += tr.CurFZone + "\t";
                    s += ZonesInterpretation[tr.CurFZone.ToString()] + "\t";
                    s += ((double)tr.time_ms)/(24*3_600_000) + "\t";
                    s += (tr.time_ms- timeBegin) + "\t";

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
    }
}
