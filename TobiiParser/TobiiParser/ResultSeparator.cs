using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TobiiParser
{
    public class Interval
    {
        public string Name;
        public long Time_ms_beg, Time_ms_end;


        public List<TobiiRecord> records;

        public Interval(string name, long time_ms_beg, long time_ms_end)
        {
            Name = name;
            Time_ms_beg = time_ms_beg;
            Time_ms_end = time_ms_end;
            records = new List<TobiiRecord>();
        }
    }

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
                        interval.records.Add(TR);
                }
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

                string s = hour.ToString() + "\t" + min.ToString() + "\t" + sec.ToString() + "\t" + msec.ToString() + "\t" + tr.zone.ToString();
                writer.WriteLine(s);
            }
        }
    }

}
