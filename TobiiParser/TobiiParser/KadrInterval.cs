﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TobiiParser
{
    /// <summary>
    /// В целом то же что KadrInTime только более обобщенное
    /// </summary>
    [Serializable]
    public class KadrInterval
    {


        public string[] KadrOnMFI;
        public long Time_ms_beg, Time_ms_end;

        public KadrInterval()
        {

        }
        public KadrInterval(string[] Kadrs, long TimeBeg, long TimeEnd)
        {
            int i;
            KadrOnMFI = Kadrs;  
            Time_ms_beg = TimeBeg;
            Time_ms_end = TimeEnd;
        }




        //время в этом промежутке?
        public bool IsTimeHere(long time_ms)
        {
            if (time_ms >= Time_ms_beg && time_ms <= Time_ms_end) return true;
            else return false;
        }


        public static KadrInterval FindTimeInList(List<KadrInterval> kadrInTimes, long time_ms)
        {
            foreach (var k in kadrInTimes)
            {
                if (k.IsTimeHere(time_ms)) { return k; }
            }

            return null;
        }

        internal string GetKadrOnMFI(int mFINumber)
        {
            if (mFINumber > KadrOnMFI.GetUpperBound(0))
                throw new Exception("МФИ номер (номера от нуля) " + mFINumber +"в таблице разбивки кадров по времени нет. Там только " + 
                                            (KadrOnMFI.GetUpperBound(0) + 1).ToString() + " штук МФИ");
            return KadrOnMFI[mFINumber];
        }

        public static async void WriteResult(string filename, List<KadrInterval> intervals)
        {
            using (StreamWriter writer = File.CreateText(filename))
            {
                await Task.Run(() => Write(writer, intervals));
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        static void Write(StreamWriter writer, List<KadrInterval> intervals)
        {
            foreach (var interval in intervals)
            {
                long time = interval.Time_ms_beg;
                string s = "";
                int i;
                for (i = 0; i <= interval.KadrOnMFI.GetUpperBound(0); i++)
                    s += interval.KadrOnMFI[i] + "\t";
                s += interval.Time_ms_beg.ToString() + "\t"
                    + interval.Time_ms_end.ToString();
                writer.WriteLine(s);
            }
        }

        internal static async void AppendWriteResultAsync(string filename, List<KadrInterval> intervals, string Header = "")
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

        internal static void AppendWriteResult(string filename, List<KadrInterval> intervals, string Header = "")
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