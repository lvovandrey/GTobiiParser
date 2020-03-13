using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TobiiParser
{
    [Serializable]
    public class Interval
    {
        public string Name;
        public long Time_ms_beg, Time_ms_end;


        public List<TobiiRecord> records;

        public Interval() { }
        public Interval(string name, long time_ms_beg, long time_ms_end)
        {
            Name = name;
            Time_ms_beg = time_ms_beg;
            Time_ms_end = time_ms_end;
            records = new List<TobiiRecord>();
        }


        public static async void WriteResult(string filename, List<Interval> intervals)
        {
            using (StreamWriter writer = File.CreateText(filename))
            {
                await Task.Run(() => Write(writer, intervals));
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        static void Write(StreamWriter writer, List<Interval> intervals)
        {
            foreach (var interval in intervals)
            {
                long time = interval.Time_ms_beg;
                string s = interval.Name.ToString() + "\t"
                    + interval.Time_ms_beg.ToString() + "\t"
                    + interval.Time_ms_end.ToString();
                writer.WriteLine(s);
            }
        }

        internal static async void AppendWriteResultAsync(string filename, List<Interval> intervals, string Header = "")
        {
            using (StreamWriter writer = new StreamWriter(File.Open(filename, FileMode.Append)))
            {
                writer.WriteLine();
                if (Header != "") writer.WriteLine(Header);
                await Task.Run(() => Write(writer, intervals));
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        internal static void AppendWriteResult(string filename, List<Interval> intervals, string Header = "")
        {
            using (StreamWriter writer = new StreamWriter(File.Open(filename, FileMode.Append)))
            {
                writer.WriteLine();
                if (Header != "") writer.WriteLine(Header);
                Write(writer, intervals);
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

    }
}