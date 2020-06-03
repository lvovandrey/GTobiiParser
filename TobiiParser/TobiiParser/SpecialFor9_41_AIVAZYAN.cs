using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

        public async void WriteTobiiRecordListToTxtAsync(string filename, List<TobiiRecord> TobiiRecords)
        {
            using (StreamWriter writer = File.CreateText(filename))
            {
                await Task.Run(() => WriteTobiiRecordListToTxt(writer, TobiiRecords));
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        void WriteTobiiRecordListToTxt(StreamWriter writer, List<TobiiRecord> TobiiRecords)
        {
            foreach (var tr in TobiiRecords)
            {
                int hour, min, sec, msec;
                double time = (double)tr.time_ms;
                if (time > 0)
                {
                    hour = (int)Math.Floor(time / 3_600_000);
                    time -= hour * 3_600_000;
                    min = (int)Math.Floor(time / 60_000);
                    time -= min * 60_000;
                    sec = (int)Math.Floor(time / 1_000);
                    time -= sec * 1_000;
                    msec = (int)Math.Floor(time);
                }
                else
                {
                    hour = (int)Math.Ceiling(time / 3_600_000);
                    time -= hour * 3_600_000;
                    min = (int)Math.Ceiling(time / 60_000);
                    time -= min * 60_000;
                    sec = (int)Math.Ceiling(time / 1_000);
                    time -= sec * 1_000;
                    msec = (int)Math.Ceiling(time);
                }
                string s = hour.ToString() + "\t" + min.ToString() + "\t" + sec.ToString() + "\t" + msec.ToString() + "\t" + tr.CurFZone.ToString();
                writer.WriteLine(s);
            }
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
                        sc.SaccadesCount += 2;
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



        public void ConvertTo5Hz(string dir)
        {
            string[] files = Directory.GetFiles(dir, "*.txt", SearchOption.TopDirectoryOnly);
            foreach (var filepath in files)
            {
                List<TobiiRecord> tobiiRecords = ReadTxtToTobiiRecordList(filepath);
                long time_beg = tobiiRecords.First().time_ms;
                long time_end = tobiiRecords.Last().time_ms;
                int lastIndex = tobiiRecords.IndexOf(tobiiRecords.Last());

                time_beg = 200 * (time_beg / 200);

                List<TobiiRecord> NewTobiiRecords = new List<TobiiRecord>();

                for (long t = time_beg; t < time_end; t += 200)
                {
                    TobiiRecord NewTR = new TobiiRecord();
                    int zone = -1;
                    foreach (var tr in tobiiRecords)
                    {
                        int index = tobiiRecords.IndexOf(tr);
                        if (index == lastIndex) break;
                        if ((tr.time_ms <= t) && (tobiiRecords[index + 1].time_ms > t))
                        {
                            NewTR.time_ms = t;
                            NewTR.CurFZone = tr.CurFZone;
                            break;
                        }
                    }
                    NewTobiiRecords.Add(NewTR);
                }

                string Newfilepath = Path.Combine(Path.GetDirectoryName(filepath), "Zones5Hz_" + Path.GetFileName(filepath));
                using (StreamWriter writer = File.CreateText(Newfilepath))
                {
                    foreach (var tr in NewTobiiRecords)
                    {
                        string s = tr.time_ms.ToString() + "\t" + tr.CurFZone.ToString();
                        writer.WriteLine(s);
                    }
                }

            }
        }



        public List<TobiiRecord> ReadTxtToTobiiRecordListZones5Hz(string filename)
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
                    tobiiRecord.time_ms = int.Parse(tmp[0]);
                    tobiiRecord.CurFZone = int.Parse(tmp[1]);

                    tobiiRecords.Add(tobiiRecord);
                }
            }
            return tobiiRecords;
        }



        public List<SaccadesCountOnInerval> ReadTxtToSaccadesCountOnInervalListSaccades(string filename)
        {
            List<SaccadesCountOnInerval> saccadesRecords = new List<SaccadesCountOnInerval>();
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

                    SaccadesCountOnInerval saccadesRecord = new SaccadesCountOnInerval();
                    saccadesRecord.time_beg = int.Parse(tmp[0]);
                    saccadesRecord.time_end = int.Parse(tmp[1]);
                    saccadesRecord.SaccadesCount = int.Parse(tmp[2]);

                    saccadesRecords.Add(saccadesRecord);
                }
            }
            return saccadesRecords;
        }

        public Dictionary<string, long> ReadSyncToIdToDictionary(string filename)
        {
            Dictionary<string, long> SyncToIDDict = new Dictionary<string, long>();
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
                    SyncToIDDict.Add(tmp[0], long.Parse(tmp[1]));
                }
            }
            return SyncToIDDict;
        }

        public void SyncronizeTxtFilesZones5Hz(string dir, string synctxtFilepath)
        {
            Dictionary<string, long> SyncDict = ReadSyncToIdToDictionary(synctxtFilepath);

            string[] files = Directory.GetFiles(dir, "*.txt", SearchOption.TopDirectoryOnly);
            foreach (var filepath in files)
            {
                List<TobiiRecord> tobiiRecords = ReadTxtToTobiiRecordListZones5Hz(filepath);
                string shortfilename = Path.GetFileNameWithoutExtension(filepath);
                string fileID = shortfilename.Substring(shortfilename.IndexOf("id") + 2, 3);
                long Time_delta = SyncDict[fileID];

                foreach (var tr in tobiiRecords)
                {
                    tr.time_ms += Time_delta;
                }


                string Newfilepath = Path.Combine(Path.GetDirectoryName(filepath), "Sync5Hz_"+Path.GetFileName(filepath));
                using (StreamWriter writer = File.CreateText(Newfilepath))
                {
                    foreach (var tr in tobiiRecords)
                    {
                        string s = tr.time_ms.ToString() + "\t" + tr.CurFZone.ToString();
                        writer.WriteLine(s);
                    }
                }

            }
        }


        public void SyncronizeCommonTxtFiles(string dir, string synctxtFilepath)
        {
            Dictionary<string, long> SyncDict = ReadSyncToIdToDictionary(synctxtFilepath);

            string[] files = Directory.GetFiles(dir, "*.txt", SearchOption.TopDirectoryOnly);
            foreach (var filepath in files)
            {
                List<TobiiRecord> tobiiRecords = ReadTxtToTobiiRecordList(filepath);
                string shortfilename = Path.GetFileNameWithoutExtension(filepath);
                string fileID = shortfilename.Substring(shortfilename.IndexOf("id") + 2, 3);
                long Time_delta;
                try
                {
                    Time_delta = SyncDict[fileID];
                }
                catch
                {
                    MessageBox.Show("В файле синхронизации " + synctxtFilepath + " не найден ID = " + fileID);
                    break;
                }
                foreach (var tr in tobiiRecords)
                {
                    tr.time_ms -= Time_delta;
                }

                string Newfilepath = Path.Combine(Path.GetDirectoryName(filepath), "Sync_" + Path.GetFileName(filepath));
                WriteTobiiRecordListToTxtAsync(Newfilepath, tobiiRecords);
            }
        }

    }
}
