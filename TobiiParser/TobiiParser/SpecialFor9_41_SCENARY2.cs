﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Excel = Microsoft.Office.Interop.Excel;

namespace TobiiParser
{

    [Serializable]
    public class SeparatorIntervals
    {
        public List<Interval> Intervals = new List<Interval>();
        public string Id;
        public List<string> tags;
        public string filename;
        public SeparatorIntervals()
        {

        }
    }

    [Serializable]
    public class KadrIntervals
    {
        public List<KadrInterval> Intervals = new List<KadrInterval>();
        public string Id;
        public List<string> tags;
        public string filename;
        public KadrIntervals()
        {

        }
    }

   


    internal class SpecialFor9_41_SCENARY2
    {
        /// <summary>
        /// Создание RFile по другому немного
        /// </summary>
        internal static void CreateRFilesTest()
        {
            List<SeparatorIntervals> SeparatorIntervalsList = SeparatorIntervalsReadFromExcel(@"c:\_\1\test.xlsx");
            foreach (var SeparatorIntervals in SeparatorIntervalsList)
            {
                string Header = "FileID = " + SeparatorIntervals.Id + "\t";
                foreach (var tag in SeparatorIntervals.tags)
                    Header += tag + "\t";
                Interval.AppendWriteResult(@"c:\_\1\RFile.txt", SeparatorIntervals.Intervals, Header);
            }

        }

        /// <summary>
        /// Создание KFile (раньше не делал) из excel. 
        /// </summary>
        internal static void CreateKFilesTest()
        {
            List<KadrIntervals> KadrIntervalsList = KadrIntervalsReadFromExcel(@"c:\_\1\testK.xlsx");
            foreach (var KadrIntervals in KadrIntervalsList)
            {
                string Header = "FileID = " + KadrIntervals.Id + "\t";
                foreach (var tag in KadrIntervals.tags)
                    Header += tag + "\t";
                KadrInterval.AppendWriteResult(@"c:\_\1\KFile.txt", KadrIntervals.Intervals, Header);
            }

        }


        /// <summary>
        /// Создание через сериализацию RFile.xml
        /// </summary>
        internal static void SerializeRFiles(string sourceFilename, string targetRFileName)
        {
            List<SeparatorIntervals> SeparatorIntervalsList = SeparatorIntervalsReadFromExcel(@sourceFilename);
            XmlSerializer formatter = new XmlSerializer(typeof(List<SeparatorIntervals>));

            using (FileStream fs = new FileStream(@targetRFileName, FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, SeparatorIntervalsList);
            }
        }

        /// <summary>
        /// Создание через сериализацию KFile.xml (раньше не делал) из excel. 
        /// </summary>
        internal static void SerializeKFiles(string sourceFilename, string targetKFileName)
        {
            List<KadrIntervals> KadrIntervalsList = KadrIntervalsReadFromExcel(@sourceFilename);
            XmlSerializer formatter = new XmlSerializer(typeof(List<KadrIntervals>));

            using (FileStream fs = new FileStream(@targetKFileName, FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, KadrIntervalsList);
            }
        }


        /// <summary>
        /// Десериализация RFile.xml
        /// </summary>
        internal static List<SeparatorIntervals> DeserializeRFiles(string sourceFilename)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(List<SeparatorIntervals>));
            List<SeparatorIntervals> SeparatorIntervalsList; 

            using (FileStream fs = new FileStream(@sourceFilename, FileMode.OpenOrCreate))
            {
                SeparatorIntervalsList = (List<SeparatorIntervals>)formatter.Deserialize(fs);
            }
            return SeparatorIntervalsList;
        }

        /// <summary>
        /// Десериализация KFile.xml 
        /// </summary>
        internal static List<KadrIntervals> DeserializeKFiles(string sourceFilename)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(List<KadrIntervals>));
            List<KadrIntervals> SeparatorIntervalsList;

            using (FileStream fs = new FileStream(@sourceFilename, FileMode.OpenOrCreate))
            {
                SeparatorIntervalsList = (List<KadrIntervals>)formatter.Deserialize(fs);
            }
            return SeparatorIntervalsList;
        }


        /// <summary>
        /// Считывание разбивки на режимы (используется для формирования R-file) из xlsx файла формата 9.41-сц2
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static List<SeparatorIntervals> SeparatorIntervalsReadFromExcel(string filename)
        {
            //считываем данные из Excel файла в двумерный массив
            Excel.Application xlApp = new Excel.Application(); //Excel
            Excel.Workbook xlWB; //рабочая книга              
            Excel.Worksheet xlSht; //лист Excel   
            xlWB = xlApp.Workbooks.Open(filename); //название файла Excel    
            int NShts = xlWB.Worksheets.Count;
            List<SeparatorIntervals> SeparatorIntervalsList = new List<SeparatorIntervals>();
            // xlSht = xlWB.Worksheets[1]; //название листа или 1-й лист в книге xlSht = xlWB.Worksheets[1];

            foreach (Excel.Worksheet sheet in xlWB.Worksheets)
            {
                int iLastRow = sheet.Cells[sheet.Rows.Count, "A"].End[Excel.XlDirection.xlUp].Row;
                var arrData = (object[,])sheet.Range["A5:B" + iLastRow].Value; //берём данные с листа Excel
                SeparatorIntervals separatorIntervals = new SeparatorIntervals();


                List<Interval> intervals = new List<Interval>();
                //заполняем intervals данными из массива
                int i;
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
                separatorIntervals.Id = sheet.Cells[2, "I"].Value.ToString();

                separatorIntervals.tags = new List<string>();
                var arrDataTags = (object[,])sheet.Range["A2:I2"].Value;
                for (i = 1; i <= 9; i++)
                    if (arrDataTags[1, i] != null)
                        separatorIntervals.tags.Add(arrDataTags[1, i].ToString());

                separatorIntervals.filename = "NONE!";
                SeparatorIntervalsList.Add(separatorIntervals);
            }


            xlWB.Close(false); //закрываем книгу, изменения не сохраняем
            xlApp.Quit(); //закрываем Excel

            return SeparatorIntervalsList;
        }

        /// <summary>
        /// Считывание разбивки на режимы (используется для формирования R-file) из xlsx файла формата 9.41-сц2
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static List<KadrIntervals> KadrIntervalsReadFromExcel(string filename)
        {
            //считываем данные из Excel файла в двумерный массив
            Excel.Application xlApp = new Excel.Application(); //Excel
            Excel.Workbook xlWB; //рабочая книга              
            Excel.Worksheet xlSht; //лист Excel   
            xlWB = xlApp.Workbooks.Open(filename); //название файла Excel    
            int NShts = xlWB.Worksheets.Count;
            List<KadrIntervals> KadrIntervalsList = new List<KadrIntervals>();
         
            foreach (Excel.Worksheet sheet in xlWB.Worksheets)
            {
                int iLastRow = sheet.Cells[sheet.Rows.Count, "A"].End[Excel.XlDirection.xlUp].Row;
                var arrData = (object[,])sheet.Range["A5:H" + iLastRow].Value; //берём данные с листа Excel
                KadrIntervals kadrIntervals = new KadrIntervals();


                List<KadrInterval> intervals = new List<KadrInterval>();
                //заполняем intervals данными из массива
                int i;
                for (i = 1; i < arrData.GetUpperBound(0); i++)
                {
                    double t = (double)arrData[i, 1] * 3_600_000 * 24;
                    long tbeg = (long)t;
                    double te = (double)arrData[i + 1, 1] * 3_600_000 * 24;
                    long tend = (long)te;
                    string[] kadrs = new string[arrData.GetUpperBound(1)-1];
                    int j;
                    for (j=2; j<=arrData.GetUpperBound(1); j++)
                        kadrs[j-2] = (string)arrData[i, j];
                    
                    KadrInterval I = new KadrInterval(kadrs,
                                    tbeg,
                                    tend);
                    intervals.Add(I);
                }
                kadrIntervals.Intervals = intervals;
                kadrIntervals.Id = sheet.Cells[2, "I"].Value.ToString();

                kadrIntervals.tags = new List<string>();
                var arrDataTags = (object[,])sheet.Range["A2:I2"].Value;
                for (i = 1; i <= 9; i++)
                    if (arrDataTags[1, i] != null)
                        kadrIntervals.tags.Add(arrDataTags[1, i].ToString());

                kadrIntervals.filename = "NONE!";
                KadrIntervalsList.Add(kadrIntervals);
            }


            xlWB.Close(false); //закрываем книгу, изменения не сохраняем
            xlApp.Quit(); //закрываем Excel

            return KadrIntervalsList;
        }



    }
}