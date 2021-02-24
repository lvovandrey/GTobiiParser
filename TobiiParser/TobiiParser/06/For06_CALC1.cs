using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;

namespace TobiiParser._06
{
    internal class For06_CALC1 : SpecialFor9_41_SP_NewProcessing
    {
        /// <summary>
        /// Считывание разбивки на режимы (используется для формирования R-file) из xlsx файла формата 9.41-сц2
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public override List<SeparatorIntervals> SeparatorIntervalsReadFromExcel(string filename)
        {
            //считываем данные из Excel файла в двумерный массив
            Excel.Application xlApp = new Excel.Application(); //Excel
            Excel.Workbook xlWB; //рабочая книга              
            Excel.Worksheet xlSht; //лист Excel   
            xlWB = xlApp.Workbooks.Open(filename); //название файла Excel    
            int NShts = xlWB.Worksheets.Count;
            List<SeparatorIntervals> SeparatorIntervalsList = new List<SeparatorIntervals>();
            // xlSht = xlWB.Worksheets[1]; //название листа или 1-й лист в книге xlSht = xlWB.Worksheets[1];

            try
            {

                foreach (Excel.Worksheet sheet in xlWB.Worksheets)
                {
                    SeparatorIntervals separatorIntervals = new SeparatorIntervals();
                    List<Interval> intervals = new List<Interval>();
                    int i;

                    int iLastRow = sheet.Cells[sheet.Rows.Count, "A"].End[Excel.XlDirection.xlUp].Row;
                    var arrData = (object[,])sheet.Range["A5:D" + iLastRow].Value; //берём данные с листа Excel

                    for (i = 1; i <= arrData.GetUpperBound(0); i++)
                    {
                        double t = (double)arrData[i, 1] * 3_600_000 * 24;
                        long tbeg = (long)t;
                        double te;
                        if (i < arrData.GetUpperBound(0))
                            te = (double)arrData[i + 1, 1] * 3_600_000 * 24;
                        else
                            te = 10 * 3_600_000 * 24;
                        long tend = (long)te;

                        Interval I = new Interval(
                                        ((string)arrData[i, 3]).Trim(),
                                        tbeg,
                                        tend);
                        intervals.Add(I);
                    }
                    separatorIntervals.Intervals = intervals;
                    separatorIntervals.Id = sheet.Cells[2, "G"].Value.ToString();
                    separatorIntervals.Id = separatorIntervals.Id.Replace("\"", "");

                    separatorIntervals.tags = new List<string>();
                    var arrDataTags = (object[,])sheet.Range["A2:H2"].Value;
                    for (i = 1; i <= 8; i++)
                        if (arrDataTags[1, i] != null)
                            separatorIntervals.tags.Add(arrDataTags[1, i].ToString());

                    separatorIntervals.filename = "NONE!";
                    SeparatorIntervalsList.Add(separatorIntervals);
                }



            }
            catch (Exception e)
            {
                MessageBox.Show("For06_CALC1.SeparatorIntervalsReadFromExcel.  Ошибка считывания файла " + filename + ":   " + e.Message + "    Stacktrace:" + e.StackTrace);
            }
            finally
            {
                xlWB.Close(false); //закрываем книгу, изменения не сохраняем
                xlApp.Quit(); //закрываем Excel
            }



            return SeparatorIntervalsList;
        }



        public override List<KadrIntervals> KadrIntervalsReadFromExcel(string filename)
        {
            //считываем данные из Excel файла в двумерный массив
            Excel.Application xlApp = new Excel.Application(); //Excel
            Excel.Workbook xlWB; //рабочая книга              
            Excel.Worksheet xlSht; //лист Excel   
            xlWB = xlApp.Workbooks.Open(filename); //название файла Excel    
            int NShts = xlWB.Worksheets.Count;
            List<KadrIntervals> KadrIntervalsList = new List<KadrIntervals>();

            try
            {
                foreach (Excel.Worksheet sheet in xlWB.Worksheets)
                {
                    int iLastRow = sheet.Cells[sheet.Rows.Count, "A"].End[Excel.XlDirection.xlUp].Row;
                    var arrData = (object[,])sheet.Range["A5:D" + iLastRow].Value; //берём данные с листа Excel
                    KadrIntervals kadrIntervals = new KadrIntervals();


                    List<KadrInterval> intervals = new List<KadrInterval>();
                    //заполняем intervals данными из массива
                    int i;
                    for (i = 1; i <= arrData.GetUpperBound(0); i++)
                    {
                        double t = (double)arrData[i, 1] * 3_600_000 * 24;
                        long tbeg = (long)t;
                        double te;
                        if (i < arrData.GetUpperBound(0))
                            te = (double)arrData[i + 1, 1] * 3_600_000 * 24;
                        else
                            te = 10 * 3_600_000 * 24;
                        long tend = (long)te;

                        string[] kadrs = new string[arrData.GetUpperBound(1) - 1];
                        int j;
                        for (j = 2; j <= arrData.GetUpperBound(1); j++)
                            kadrs[j - 2] = (string)arrData[i, j];

                        KadrInterval I = new KadrInterval(kadrs,
                                        tbeg,
                                        tend);
                        intervals.Add(I);
                    }
                    kadrIntervals.Intervals = intervals;
                    kadrIntervals.Id = sheet.Cells[2, "G"].Value.ToString().Replace("\"", ""); ;

                    kadrIntervals.tags = new List<string>();
                    var arrDataTags = (object[,])sheet.Range["A2:G2"].Value;
                    for (i = 1; i <= 7; i++)
                        if (arrDataTags[1, i] != null)
                            kadrIntervals.tags.Add(arrDataTags[1, i].ToString());

                    kadrIntervals.filename = "NONE!";
                    KadrIntervalsList.Add(kadrIntervals);
                }


            }
            catch (Exception e)
            {
                MessageBox.Show("SpecialFor9_41_SCENARY4.KadrIntervalsReadFromExcel -  Ошибка считывания файла " + filename + "  :   " + e.Message + "    Stacktrace:" + e.StackTrace);
            }
            finally
            {
                xlWB.Close(false); //закрываем книгу, изменения не сохраняем
                xlApp.Quit(); //закрываем Excel
            }


            return KadrIntervalsList;
        }


        private static bool IsWrongEvent(string[] wrongEventsNames, string eventName)
        {
            foreach (var item in wrongEventsNames)
            {
                if (eventName == item) return true;
            }
            return false;
        }


        public void ReadCSVForRFiles(string filename)
        {
            string[] wrongEvents = new string[]{null, "", "RecordingStart",
                   "SyncPortOutHigh", "SyncPortOutLow","под зоны IntervalEnd",
                    "под зоны IntervalStart"};


            char separator = '\n';
            char delimiter = '\t';

            int N_timestampCol = 0, N_eventCol = 0;
            long i = 0;
            using (StreamReader rd = new StreamReader(new FileStream(filename, FileMode.Open)))
            {
                string[] first_string_arr = { "" };
                first_string_arr = rd.ReadLine().Split(delimiter);
                N_timestampCol = TobiiCsvReader.SearchColFirst(first_string_arr, "Recording timestamp");
                N_eventCol = TobiiCsvReader.SearchColFirst(first_string_arr, "Event");

                bool EndOfFile = false;
                while (!EndOfFile)
                {
                    string[] str_arr = { "" };
                    string big_str = "";
                    EndOfFile = TobiiCsvReader.ReadPartOfFile(rd, out big_str);

                    str_arr = big_str.Split(separator);
                    foreach (string s in str_arr)
                    {
                        string[] tmp = { "" };
                        i++;
                        tmp = s.Split(delimiter);
                        if (tmp.Count() < 3) continue;
                        TobiiRecord TR = new TobiiRecord();
                        if (!long.TryParse(tmp[N_timestampCol], out TR.time_ms))
                            throw new Exception("Не могу преобразовать в timestamp строку  " + tmp[N_timestampCol]);

                        string[] Hits = new string[tmp.Count()];
                        try
                        {
                            Array.Copy(tmp, N_firstZoneCol, Hits, 0, ZoneColCount);
                        }
                        catch
                        { Console.WriteLine("!!!"); }
                        TR.zones = SearchCol(Hits, "1");
                        tobiiList.Add(TR);
                    }

                }

                FiltredTobiiList = CompactTobiiRecords(tobiiList);
            }
        }

    }
}
