using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;

namespace TobiiParser
{

    internal class SpecialFor9_41_SCENARY3 : SpecialFor9_41_SCENARY2
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
                    var arrData = (object[,])sheet.Range["A5:B" + iLastRow].Value; //берём данные с листа Excel

                    for (i = 1; i < arrData.GetUpperBound(0); i++)
                    {
                        double t = (double)arrData[i, 1] * 3_600_000 * 24;
                        long tbeg = (long)t;
                        double te = (double)arrData[i + 1, 1] * 3_600_000 * 24;
                        long tend = (long)te;

                        Interval I = new Interval(
                                        ((string)arrData[i, 2]).Trim(),
                                        tbeg,
                                        tend);
                        intervals.Add(I);
                    }
                    separatorIntervals.Intervals = intervals;
                    separatorIntervals.Id = sheet.Cells[2, "H"].Value.ToString();

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
                MessageBox.Show("SpecialFor9_41_SCENARY4.SeparatorIntervalsReadFromExcel.  Ошибка считывания файла " + filename + ":   " + e.Message + "    Stacktrace:" + e.StackTrace);
            }
            finally
            {
                xlWB.Close(false); //закрываем книгу, изменения не сохраняем
                xlApp.Quit(); //закрываем Excel
            }



            return SeparatorIntervalsList;
        }



        /// <summary>
        /// Считывание разбивки на режимы (используется для формирования R-file) из xlsx файла формата 9.41-сц2
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
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
                    var arrData = (object[,])sheet.Range["A5:H" + iLastRow].Value; //берём данные с листа Excel
                    KadrIntervals kadrIntervals = new KadrIntervals();


                    List<KadrInterval> intervals = new List<KadrInterval>();
                    //заполняем intervals данными из массива
                    int i;
                    for (i = 1; i <= arrData.GetUpperBound(0); i++)
                    {
                        double t = (double)arrData[i, 1] * 3_600_000 * 24;
                        long tbeg = (long)t;
                        double te = t + 3_600_000 * 24;
                        if (i != arrData.GetUpperBound(0))
                            te = (double)arrData[i + 1, 1] * 3_600_000 * 24;
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
                    kadrIntervals.Id = sheet.Cells[2, "H"].Value.ToString();

                    kadrIntervals.tags = new List<string>();
                    var arrDataTags = (object[,])sheet.Range["A2:H2"].Value;
                    for (i = 1; i <= 8; i++)
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


    }
}
