using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TobiiParser
{


    public class ResultSeparator
    {
        string DirectoryForFiles = "";
        string prefixfilename = "";
        List<TobiiRecord> tobiiRecords;
        List<Interval> intervals;
        public ResultSeparator(string directoryForFiles, List<Interval> intervals, List<TobiiRecord> tobiiRecords, string prefixfilename)
        {
            DirectoryForFiles = directoryForFiles;
            this.tobiiRecords = tobiiRecords;
            this.prefixfilename = prefixfilename;
            this.intervals = intervals;
        }

        public void Separate()
        {
            List<List<TobiiRecord>> SuperList = new List<List<TobiiRecord>>();

            foreach (var TR in tobiiRecords)
            {
                foreach (var interval in intervals)
                {
                    if (TR.time_ms >= interval.Time_ms_beg && TR.time_ms <= interval.Time_ms_end)
                    {
                        if (interval.records.Count() == 0)//Ставим первую фиксацию разрезав предыдущую
                        {
                            int TRindex = tobiiRecords.IndexOf(TR);
                            if (TRindex != 0)
                            {
                                TobiiRecord TRfirst = new TobiiRecord(tobiiRecords[TRindex - 1]);
                                TRfirst.time_ms = interval.Time_ms_beg;
                                interval.records.Add(TRfirst);
                            }
                        }

                        interval.records.Add(TR); //Ставим саму фиксацию
                    }
                }
                foreach (var interval in intervals) //Если в пределах одной фиксации помещается весь интервал - надо найти эту фиксацию
                {
                    if (interval.records.Count() == 0)
                    {
                        int i = 0;
                        for (i = 1; i < tobiiRecords.Count; i++)
                        {
                            if (tobiiRecords[i].time_ms > interval.Time_ms_beg) //ищем первую фиксацию, который началась после начала интервала
                            {
                                TobiiRecord TRPrev = new TobiiRecord(tobiiRecords[i - 1]); // и берем предыдущую
                                if (tobiiRecords[i - 1].time_ms < interval.Time_ms_beg)    // если предыдущая фиксация - до начала интервала началась
                                    TRPrev.time_ms = interval.Time_ms_beg;                 // считаем что фиксация все таки началась с началом интервала

                                interval.records.Add(TRPrev);
                                break;                                                     // одну добавили - и хватить
                            }
                        }
                    }
                }
            }

            foreach (var interval in intervals) // Всем раздаем по заглушке в конце интервала - по пустой фиксации.
            {
                TobiiRecord TR = new TobiiRecord();
                TR.time_ms = interval.Time_ms_end + 10;
                interval.records.Add(TR);
            }

            DirectoryInfo di = new DirectoryInfo(DirectoryForFiles);
            if (!di.Exists) di.Create();

            foreach (var interval in intervals)
            {
                //Сначала запишем файл в ту же папку
                int number = intervals.IndexOf(interval);
                string filename = DirectoryForFiles + prefixfilename + "_№" + number.ToString() + " " + interval.Name + ".txt";
                WriteResult(filename, interval.records);

                //А теперь в свою собственную
                string Dir_innerName = DirectoryForFiles + @"\" + prefixfilename + "_№" + number.ToString() + " " + interval.Name + @"\";
                string filename2 = Dir_innerName + "Inner " + prefixfilename + "_№" + number.ToString() + " " + interval.Name + ".txt";
                DirectoryInfo di_inner = new DirectoryInfo(Dir_innerName);
                if (!di_inner.Exists) di_inner.Create();
                WriteResult(filename2, interval.records);

            }
        }
        public async void WriteResult(string filename, List<TobiiRecord> RecList)
        {
            using (StreamWriter writer = File.CreateText(filename))
            {
                await Task.Run(() => Write(writer, RecList));
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();

        }

        void Write(StreamWriter writer, List<TobiiRecord> RecList)
        {
            foreach (var tr in RecList)
            {
                double time = (double)tr.time_ms;
                int hour = (int)Math.Floor(time / 3_600_000);
                time -= hour * 3_600_000;
                int min = (int)Math.Floor(time / 60_000);
                time -= min * 60_000;
                int sec = (int)Math.Floor(time / 1_000);
                time -= sec * 1_000;
                int msec = (int)Math.Floor(time);

                string s = hour.ToString() + "\t" + min.ToString() + "\t" + sec.ToString() + "\t" + msec.ToString() + "\t" + tr.CurFZone.ToString();
                writer.WriteLine(s);
            }
        }
    }

}
