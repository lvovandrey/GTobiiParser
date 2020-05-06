using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TobiiParser
{
    internal class SaccadesCountOnInerval
    {
        public long time_beg;
        public long time_end;
        public int SaccadesCount;
    }

    internal class SpecialFor9_41_AIVAZYAN : SpecialFor9_41_POSADKI
    {
        public List<TobiiRecord> ReadTxtToTobiiRecordList(string filename)
        {
            List<TobiiRecord> tobiiRecords = new List<TobiiRecord>();
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

                    TobiiRecord tobiiRecord = new TobiiRecord();
                    tobiiRecord.time_ms = int.Parse(tmp[0]) * 3_600_000 + int.Parse(tmp[1]) * 60_000 + int.Parse(tmp[2]) * 1_000 + int.Parse(tmp[3]);
                    tobiiRecord.CurFZone = int.Parse(tmp[4]);

                    tobiiRecords.Add(tobiiRecord);
                }
            }
            return tobiiRecords;
        }

        public List<TobiiRecord> FilterTobiiRecords(List<TobiiRecord> tobiiRecords)
        {
            List<TobiiRecord> newTobiiRecords = new List<TobiiRecord>();
            foreach (var tr in tobiiRecords)
                if (tr.CurFZone != -1)
                    newTobiiRecords.Add(new TobiiRecord(tr));

            List<TobiiRecord> new2TobiiRecords = new List<TobiiRecord>();
            new2TobiiRecords.Add(newTobiiRecords[0]);
            foreach (var tr in newTobiiRecords)
            {
                if (newTobiiRecords.IndexOf(tr) != 0)
                    if (new2TobiiRecords.Last().CurFZone != tr.CurFZone)
                        new2TobiiRecords.Add(tr);
            }

            return new2TobiiRecords;
        }

        public List<SaccadesCountOnInerval> CalcSaccadesOnSlidingWindow(List<TobiiRecord> tobiiRecords, long SlidingWindowSize_msec, long SlidingWindowTimeStep_msec)
        {
            List<SaccadesCountOnInerval> ListSaccadesCountOnInerval = new List<SaccadesCountOnInerval>();

            long tobiiRecordsBeginTime = tobiiRecords.First().time_ms + (long)(SlidingWindowSize_msec / 2);
            long tobiiRecordsEndTime = tobiiRecords.Last().time_ms - (long)(SlidingWindowSize_msec / 2);

            for (long curTime = tobiiRecordsBeginTime; curTime < tobiiRecordsEndTime; curTime += SlidingWindowTimeStep_msec)
            {
                SaccadesCountOnInerval sc = new SaccadesCountOnInerval();
                sc.time_beg = curTime;
                sc.time_end = curTime + SlidingWindowSize_msec;
                sc.SaccadesCount = 0;
                foreach (var tr in tobiiRecords)
                    if (tr.time_ms >= sc.time_beg && tr.time_ms < sc.time_end)
                        sc.SaccadesCount+=2;
                ListSaccadesCountOnInerval.Add(sc);
            }
            return ListSaccadesCountOnInerval;
        }

        public void SaccadesCountsOnInervalWriteToTxt(string filename, List<SaccadesCountOnInerval> saccadesCountOnInervals)
        {
            using (StreamWriter writer = File.CreateText(filename))
            {
                foreach (var sc in saccadesCountOnInervals)
                {
                    string s = sc.time_beg.ToString() + "\t" + sc.time_end.ToString() + "\t" + sc.SaccadesCount.ToString();
                    writer.WriteLine(s);
                }
            }
        }



        public void CalculateSaccades(string dir)
        {
            string[] files = Directory.GetFiles(dir, "*.txt", SearchOption.TopDirectoryOnly);
            foreach (var filepath in files)
            {
                List<TobiiRecord> tobiiRecords = ReadTxtToTobiiRecordList(filepath);
                tobiiRecords = FilterTobiiRecords(tobiiRecords);
                List<SaccadesCountOnInerval> saccadesCountOnInervals = CalcSaccadesOnSlidingWindow(tobiiRecords, 30000, 1000);
                string newFilepath = Path.Combine(Path.GetDirectoryName(filepath), "Saccades_" + Path.GetFileName(filepath));
                SaccadesCountsOnInervalWriteToTxt(newFilepath, saccadesCountOnInervals);
            }

        }
    }
}
