using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;

namespace TobiiParser
{


    public class ExcelReader
    {


        public static List<KadrInTime> ReadKadrSets(string FileName)
        {
            //считываем данные из Excel файла в двумерный массив
            Excel.Application xlApp = new Excel.Application(); //Excel
            Excel.Workbook xlWB; //рабочая книга              
            Excel.Worksheet xlSht; //лист Excel   
            xlWB = xlApp.Workbooks.Open(FileName); //название файла Excel                                             
            xlSht = xlWB.Worksheets[1]; //название листа или 1-й лист в книге xlSht = xlWB.Worksheets[1];



            int iLastRow = xlSht.Cells[xlSht.Rows.Count, "B"].End[Excel.XlDirection.xlUp].Row;  //последняя заполненная строка в столбце А      
            var arrData = (object[,])xlSht.Range["B4:M" + iLastRow].Value; //берём данные с листа Excel
            //xlApp.Visible = true; //отображаем Excel     
            xlWB.Close(false); //закрываем книгу, изменения не сохраняем
            xlApp.Quit(); //закрываем Excel


            List<KadrInTime> kadrInTimes = new List<KadrInTime>();
            //заполняем DataGridView данными из массива
            int i;
            for (i = 1; i <= arrData.GetUpperBound(0); i++)
            {
                double t = (double)arrData[i, 1] * 3_600_000 * 24;
                long tbeg = (long)t;
                double te = (double)arrData[i, 2] * 3_600_000 * 24;
                long tend = (long)te;

                KadrInTime K = new KadrInTime(
                                (string)arrData[i, 9],
                                (string)arrData[i, 10],
                                (string)arrData[i, 11],
                                (string)arrData[i, 12],
                                tbeg,
                                tend);
                kadrInTimes.Add(K);
            }
            return kadrInTimes;
        }

        public static List<KadrInTime> GenerateKadrSets(string kadrDefault)
        {
            List<KadrInTime> kadrInTimes = new List<KadrInTime>();
            KadrInTime K = new KadrInTime(kadrDefault, kadrDefault, kadrDefault, "левый", 0, 86_400_000);
            kadrInTimes.Add(K);
            return kadrInTimes;
        }



        public static TabOfKeys ReadTabOfKeys(string FileName)
        {
            //считываем данные из Excel файла в двумерный массив
            Excel.Application xlApp = new Excel.Application(); //Excel
            Excel.Workbook xlWB; //рабочая книга              
            Excel.Worksheet xlSht; //лист Excel   
            xlWB = xlApp.Workbooks.Open(FileName); //название файла Excel                                             
            xlSht = xlWB.Worksheets[1]; //название листа или 1-й лист в книге xlSht = xlWB.Worksheets[1];



            int iLastRow = xlSht.Cells[xlSht.Rows.Count, "A"].End[Excel.XlDirection.xlUp].Row;  //последняя заполненная строка в столбце А      
            var arrData = (object[,])xlSht.Range["A1:B" + iLastRow].Value; //берём данные с листа Excel
            //xlApp.Visible = true; //отображаем Excel     
            xlWB.Close(false); //закрываем книгу, изменения не сохраняем
            xlApp.Quit(); //закрываем Excel


            Dictionary<int, Dictionary<string, int>> Tab = new Dictionary<int, Dictionary<string, int>>();

            int i, j;
            for (i = 2; i <= arrData.GetUpperBound(0); i++)
            {
                int KeyTobiiKadr = (int)((double)arrData[i, 1]);
                Dictionary<string, int> row = new Dictionary<string, int>();
                for (j = 2; j <= arrData.GetUpperBound(1); j++)
                {
                    string KeyKadr = (string)arrData[1, j];
                    int ValFuncZone = (int)((double)arrData[i, j]);
                    row.Add(KeyKadr, ValFuncZone);
                }
                Tab.Add(KeyTobiiKadr, row);
            }

            TabOfKeys tabOfKeys = new TabOfKeys { Tab = Tab };

            return tabOfKeys;
        }

        public static List<Interval> SeparatorIntervalsReadFromExcel(string filename)
        {
            //считываем данные из Excel файла в двумерный массив
            Excel.Application xlApp = new Excel.Application(); //Excel
            Excel.Workbook xlWB; //рабочая книга              
            Excel.Worksheet xlSht; //лист Excel   
            xlWB = xlApp.Workbooks.Open(filename); //название файла Excel                                             
            xlSht = xlWB.Worksheets[1]; //название листа или 1-й лист в книге xlSht = xlWB.Worksheets[1];



            int iLastRow = xlSht.Cells[xlSht.Rows.Count, "B"].End[Excel.XlDirection.xlUp].Row;  //последняя заполненная строка в столбце А      
            var arrData = (object[,])xlSht.Range["B4:E" + iLastRow].Value; //берём данные с листа Excel
            //xlApp.Visible = true; //отображаем Excel     
            xlWB.Close(false); //закрываем книгу, изменения не сохраняем
            xlApp.Quit(); //закрываем Excel


            List<Interval> intervals = new List<Interval>();
            //заполняем DataGridView данными из массива
            int i;
            for (i = 1; i <= arrData.GetUpperBound(0); i++)
            {
                double t = (double)arrData[i, 1] * 3_600_000 * 24;
                long tbeg = (long)t;
                double te = (double)arrData[i, 2] * 3_600_000 * 24;
                long tend = (long)te;

                Interval I = new Interval(
                                ((string)arrData[i, 4]).Trim(),
                                tbeg,
                                tend);
                intervals.Add(I);
            }
            return intervals;
        }

        internal static List<Interval> SeparatorIntervalsReadFromUnionTxt(string file_reg, string file_csv)
        {
            List<Interval> intervals = new List<Interval>();

            char separator = '\n';
            char delimiter = '\t';

            using (StreamReader rd = new StreamReader(new FileStream(file_reg, FileMode.Open)))
            {
                string[] first_string_arr = { "" };

                string[] str_arr = { "" };
                string big_str = "";
                TobiiCsvReader.ReadPartOfFile(rd, out big_str); // TODO: я расчитываю что файл режимов будет меньше 10000 строк
                str_arr = big_str.Split(separator);

                int RowFirst = 0, RowLast = 0, i = 0;

                for (i = 0; i < str_arr.Length; i++)
                {
                    string[] tmp = { "" };
                    tmp = str_arr[i].Split(delimiter);

                    if (tmp[0].Contains(file_csv))
                    {
                        int j;
                        RowFirst = i+1;
                        for (j = RowFirst; j < str_arr.Length; j++)
                        {
                            string[] tmp2 = { "" };
                            tmp2 = str_arr[j].Split(delimiter);
                            if (tmp2[0] == "")
                            {
                                RowLast = j - 1;
                                break;
                            }
                        }
                        break;
                    }
                }

                for (i = RowFirst; i <= RowLast; i++)
                {
                    string[] tmp = { "" };
                    tmp = str_arr[i].Split(delimiter);
                    string Name = tmp[0].Trim();
                    long TimeBeg = 0;
                    long TimeEnd = 0;
                    if (!long.TryParse(tmp[1], out TimeBeg)) { MessageBox.Show(" SeparatorIntervalsReadFromUnionTxt - не парсится время " + tmp[1]); return null; }
                    if (!long.TryParse(tmp[2], out TimeEnd)) { MessageBox.Show(" SeparatorIntervalsReadFromUnionTxt - не парсится время " + tmp[1]); return null; }

                    Interval interval = new Interval(Name, TimeBeg, TimeEnd);
                    intervals.Add(interval);
                }
            }
            return intervals;
        }







        internal static void R_filesGenerate(string text)
        {
            throw new NotImplementedException();
        }
    }
}
