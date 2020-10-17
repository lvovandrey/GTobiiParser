using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TobiiParser
{
    public class TobiiCsvReader
    {
        public List<TobiiRecord> tobiiList;
        public List<TobiiRecord> FiltredTobiiList;

        public void TobiiCSCRead(string filename, List<TobiiRecord> tobiiList, int ZoneColCount)
        {

            char separator = '\n';
            char delimiter = '\t';

            int N_timestampCol = 0, N_firstZoneCol = 0;
            long i = 0;
            using (StreamReader rd = new StreamReader(new FileStream(filename, FileMode.Open)))
            {
                string[] first_string_arr = { "" };
                first_string_arr = rd.ReadLine().Split(delimiter);
                N_timestampCol = SearchColFirst(first_string_arr, "Recording timestamp");
                N_firstZoneCol = SearchColFirst(first_string_arr, "AOI hit [");

                bool EndOfFile = false;
                while (!EndOfFile)
                {
                    string[] str_arr = { "" };
                    string big_str = "";
                    EndOfFile = ReadPartOfFile(rd, out big_str);

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


        internal List<Interval> TobiiIntervalRead(string file_csv)
        {

            List<Interval> intervals = new List<Interval>();

            char separator = '\n';
            char delimiter = '\t';

            int N_timestampCol = 0, N_eventsCol = 0;
            long i = 0;
            using (StreamReader rd = new StreamReader(new FileStream(file_csv, FileMode.Open)))
            {
                string[] first_string_arr = { "" };
                first_string_arr = rd.ReadLine().Split(delimiter);
                N_timestampCol = SearchColFirst(first_string_arr, "Recording timestamp");
                N_eventsCol = SearchColFirst(first_string_arr, "Event");

                long RecordingEndTime = 0;

                bool EndOfFile = false;
                while (!EndOfFile)
                {
                    string[] str_arr = { "" };
                    string big_str = "";
                    EndOfFile = ReadPartOfFile(rd, out big_str);

                    str_arr = big_str.Split(separator);
                    foreach (string s in str_arr)
                    {
                        string[] tmp = { "" };
                        i++;
                        tmp = s.Split(delimiter);
                        if (tmp.Count() < 3) continue;

                        string EventName = tmp[N_eventsCol];

                        if (EventName == "RecordingEnd")
                        {
                            if (!long.TryParse(tmp[N_timestampCol], out RecordingEndTime))
                                throw new Exception("Не могу преобразовать в timestamp строку  " + tmp[N_timestampCol]);
                        }

                        if (EventName != "" &&
                            EventName != "RecordingStart" &&
                            EventName != "SyncPortOutHigh" &&
                            EventName != "SyncPortOutLow" &&
                            EventName != "RecordingEnd" &&
                            EventName != "RecordingPause" &&
                            !EventName.Contains("IntervalStart") &&
                            !EventName.Contains("IntervalEnd")
                            )
                        {
                            long TimeBeg = 0;
                            long TimeEnd = 0;

                            if (!long.TryParse(tmp[N_timestampCol], out TimeBeg))
                                throw new Exception("Не могу преобразовать в timestamp строку  " + tmp[N_timestampCol]);

                            TimeEnd = TimeBeg + 30000;
                            if (intervals.Count > 0) intervals.Last().Time_ms_end = TimeBeg;

                            Interval interval = new Interval(tmp[N_eventsCol], TimeBeg, TimeEnd);
                            intervals.Add(interval);
                        }
                    }

                }

                if (RecordingEndTime != 0 &&
                    intervals.Last().Time_ms_end > RecordingEndTime)
                    intervals.Last().Time_ms_end = RecordingEndTime - 1000;


            }

            return intervals;
        }


        public static bool ReadPartOfFile(StreamReader rd, out string str)
        {
            bool endOfFile = false;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i <= 10000; i++)
            {
                string s = rd.ReadLine();
                if (s == null) { endOfFile = true; break; }
                sb.Append(s);
                sb.Append("\n");
            }
            str = sb.ToString();
            return endOfFile;
        }

        /// <summary>
        /// Найти номера заполненных колонок
        /// </summary>
        /// <param name="row"></param>
        /// <param name="colName"></param>
        /// <returns></returns>
        List<int> SearchCol(string[] row, string colName)
        {
            List<int> zones = new List<int>();
            int ii = 0;
            foreach (string s in row)
            {
                if (s == null) continue;
                if (s.IndexOf(colName) > -1)
                {
                    zones.Add(ii + 1);
                }
                ii++;
            }
            return zones;
        }

        int SearchColFirst(string[] row, string colName)
        {
            int ii = 0;
            bool find = false;
            foreach (string s in row)
            {
                if (s == null) continue;
                if (s.IndexOf(colName) > -1)
                { find = true; break; }
                ii++;
            }
            if (find) return ii;
            else { return -1; }
        }


        private bool IsEqual(List<int> a, List<int> b)
        {
            if (a.Count() != b.Count) return false;
            for (int i = 0; i < a.Count; i++)
                if (a[i] != b[i]) return false;
            return true;
        }

        //Убираем повторы из записи тоби - компактифицируем ее
        public List<TobiiRecord> CompactTobiiRecords(List<TobiiRecord> tRs, string mode = "MultZones")
        {
            if (mode == "FZones")
            {
                List<TobiiRecord> TRSNew = new List<TobiiRecord>();
                int ZoneBefore = -2;
                foreach (var tr in tRs)
                {
                    if (tr.CurFZone != ZoneBefore)
                    {
                        TRSNew.Add(tr);
                        ZoneBefore = tr.CurFZone;
                    }
                }
                return TRSNew;
            }
            else if (mode == "MultZones")
            {
                List<TobiiRecord> TRSNew = new List<TobiiRecord>();
                List<int> ZonesBefore = tRs[0].zones;

                for (int i = 1; i < tRs.Count; i++)
                {
                    var tr = tRs[i];
                    if (!IsEqual(tr.zones, ZonesBefore))
                    {
                        TRSNew.Add(tr);
                        ZonesBefore = tr.zones;
                    }
                }
                return TRSNew;
            }
            else
                throw new Exception("CompactTobiiRecords - нет варианта с параметром mode = " + mode);


        }



        public List<TobiiRecord> ClearFromGarbageZone(List<TobiiRecord> tRs, int GarbageZone, long UPBoundFiltrationOfGarbage)
        {
            List<TobiiRecord> TRSNew = new List<TobiiRecord>();
            foreach (var tr in tRs)
            {
                bool NotGarbage = tr.CurFZone != GarbageZone;
                bool IsFirst = tRs.IndexOf(tr) == 0;
                bool IsLast = tRs.IndexOf(tr) > tRs.Count - 1;
                bool IsPreLast = tRs.IndexOf(tr) > tRs.Count - 2;

                long dt = 0;
                if (!IsLast && !IsPreLast)
                    dt = tRs[tRs.IndexOf(tr) + 1].time_ms - tr.time_ms;
                // dt = tr.time_ms - tRs[tRs.IndexOf(tr) - 1].time_ms;
                bool IsdtMoreUpBound = dt > UPBoundFiltrationOfGarbage;

                if (NotGarbage ||
                    IsFirst ||
                    IsLast ||
                    IsdtMoreUpBound)
                    TRSNew.Add(tr);
            }
            return TRSNew;
        }

    }

    public class TobiiPupilDiametersCsvReader
    {

        public void ReadFromFile(string filename, out List<PupilDiameterRecord> tobiiList)
        {
            tobiiList = new List<PupilDiameterRecord>();
            char separator = '\n';
            char delimiter = '\t';

            int N_timestampCol = 0, N_PupilDiameterLeftCol = 0, N_PupilDiameterRightCol = 0;
            long i = 0;
            using (StreamReader rd = new StreamReader(new FileStream(filename, FileMode.Open)))
            {
                string[] first_string_arr = { "" };
                first_string_arr = rd.ReadLine().Split(delimiter);
                N_timestampCol = SearchColFirst(first_string_arr, "Recording timestamp");
                N_PupilDiameterLeftCol = SearchColFirst(first_string_arr, "Pupil diameter left");
                N_PupilDiameterRightCol = SearchColFirst(first_string_arr, "Pupil diameter right");

                bool EndOfFile = false;
                while (!EndOfFile)
                {
                    string[] str_arr = { "" };
                    string big_str = "";
                    EndOfFile = ReadPartOfFile(rd, out big_str);

                    str_arr = big_str.Split(separator);
                    foreach (string s in str_arr)
                    {
                        string[] tmp = { "" };
                        i++;
                        tmp = s.Split(delimiter);
                        if (tmp.Count() < 3) continue;
                        PupilDiameterRecord PDR = new PupilDiameterRecord();
                        if (!long.TryParse(tmp[N_timestampCol], out PDR.time_ms))
                            throw new Exception("Не могу преобразовать в timestamp строку  " + tmp[N_timestampCol]);

                        if (!double.TryParse(tmp[N_PupilDiameterLeftCol], out PDR.DiameterLeft) && tobiiList.Count > 0)
                            PDR.DiameterLeft = tobiiList.Last().DiameterLeft;

                        if (!double.TryParse(tmp[N_PupilDiameterRightCol], out PDR.DiameterRight) && tobiiList.Count > 0)
                            PDR.DiameterRight = tobiiList.Last().DiameterRight;

                        tobiiList.Add(PDR);
                    }

                }

            }
        }


        public static bool ReadPartOfFile(StreamReader rd, out string str)
        {
            bool endOfFile = false;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i <= 10000; i++)
            {
                string s = rd.ReadLine();
                if (s == null) { endOfFile = true; break; }
                sb.Append(s);
                sb.Append("\n");
            }
            str = sb.ToString();
            return endOfFile;
        }

        /// <summary>
        /// Найти номера заполненных колонок
        /// </summary>
        /// <param name="row"></param>
        /// <param name="colName"></param>
        /// <returns></returns>
        List<int> SearchCol(string[] row, string colName)
        {
            List<int> zones = new List<int>();
            int ii = 0;
            foreach (string s in row)
            {
                if (s == null) continue;
                if (s.IndexOf(colName) > -1)
                {
                    zones.Add(ii + 1);
                }
                ii++;
            }
            return zones;
        }

        int SearchColFirst(string[] row, string colName)
        {
            int ii = 0;
            bool find = false;
            foreach (string s in row)
            {
                if (s == null) continue;
                if (s.IndexOf(colName) > -1)
                { find = true; break; }
                ii++;
            }
            if (find) return ii;
            else { return -1; }
        }


        private bool IsEqual(List<int> a, List<int> b)
        {
            if (a.Count() != b.Count) return false;
            for (int i = 0; i < a.Count; i++)
                if (a[i] != b[i]) return false;
            return true;
        }

        //Убираем повторы из записи тоби - компактифицируем ее
        public List<TobiiRecord> CompactTobiiRecords(List<TobiiRecord> tRs, string mode = "MultZones")
        {
            if (mode == "FZones")
            {
                List<TobiiRecord> TRSNew = new List<TobiiRecord>();
                int ZoneBefore = -2;
                foreach (var tr in tRs)
                {
                    if (tr.CurFZone != ZoneBefore)
                    {
                        TRSNew.Add(tr);
                        ZoneBefore = tr.CurFZone;
                    }
                }
                return TRSNew;
            }
            else if (mode == "MultZones")
            {
                List<TobiiRecord> TRSNew = new List<TobiiRecord>();
                List<int> ZonesBefore = tRs[0].zones;

                for (int i = 1; i < tRs.Count; i++)
                {
                    var tr = tRs[i];
                    if (!IsEqual(tr.zones, ZonesBefore))
                    {
                        TRSNew.Add(tr);
                        ZonesBefore = tr.zones;
                    }
                }
                return TRSNew;
            }
            else
                throw new Exception("CompactTobiiRecords - нет варианта с параметром mode = " + mode);


        }



        public List<TobiiRecord> ClearFromGarbageZone(List<TobiiRecord> tRs, int GarbageZone, long UPBoundFiltrationOfGarbage)
        {
            List<TobiiRecord> TRSNew = new List<TobiiRecord>();
            foreach (var tr in tRs)
            {
                bool NotGarbage = tr.CurFZone != GarbageZone;
                bool IsFirst = tRs.IndexOf(tr) == 0;
                bool IsLast = tRs.IndexOf(tr) > tRs.Count - 1;
                bool IsPreLast = tRs.IndexOf(tr) > tRs.Count - 2;

                long dt = 0;
                if (!IsLast && !IsPreLast)
                    dt = tRs[tRs.IndexOf(tr) + 1].time_ms - tr.time_ms;
                // dt = tr.time_ms - tRs[tRs.IndexOf(tr) - 1].time_ms;
                bool IsdtMoreUpBound = dt > UPBoundFiltrationOfGarbage;

                if (NotGarbage ||
                    IsFirst ||
                    IsLast ||
                    IsdtMoreUpBound)
                    TRSNew.Add(tr);
            }
            return TRSNew;
        }

    }

    public class TobiiEyeMoveventEventsCsvReader
    {

        public void ReadFromFile(string filename, out List<EyeMovementEventRecord> tobiiList, out string Time, out string Date, out long fulltime_ms)
        {
            tobiiList = new List<EyeMovementEventRecord>();
            char separator = '\n';
            char delimiter = '\t';

            Time = "";
            Date = "";
            fulltime_ms = 0;

            int N_timestampCol = -1, N_EyeMovementTypeCol = -1, N_TimeCol = -1, N_DateCol = -1;
            long i = 0;
            using (StreamReader rd = new StreamReader(new FileStream(filename, FileMode.Open)))
            {
                string[] first_string_arr = { "" };
                first_string_arr = rd.ReadLine().Split(delimiter);
                N_timestampCol = SearchColFirst(first_string_arr, "Recording timestamp");
                N_EyeMovementTypeCol = SearchColFirst(first_string_arr, "Eye movement type");
                N_TimeCol = SearchColFirst(first_string_arr, "Recording start time");
                N_DateCol= SearchColFirst(first_string_arr, "Recording date");

                bool EndOfFile = false;
                while (!EndOfFile)
                {

                    string[] str_arr = { "" };
                    string big_str = "";
                    EndOfFile = ReadPartOfFile(rd, out big_str);

                    str_arr = big_str.Split(separator);
                    foreach (string s in str_arr)
                    {
                        string[] tmp = { "" };
                        i++;
                        tmp = s.Split(delimiter);
                        if (tmp.Count() < 3) continue;
                        EyeMovementEventRecord EMER = new EyeMovementEventRecord();
                        if (!long.TryParse(tmp[N_timestampCol], out EMER.time_ms))
                            throw new Exception("Не могу преобразовать в timestamp строку  " + tmp[N_timestampCol]);

                        switch (tmp[N_EyeMovementTypeCol])
                        {
                            case "Fixation": EMER.Type = EyeMovementType.Fixation; break;
                            case "Saccade": EMER.Type = EyeMovementType.Saccade; break;
                            default: EMER.Type = EyeMovementType.Other; break;
                        }

                        if (tobiiList.Count() == 0) tobiiList.Add(EMER);

                        tobiiList.Last().duraion_ms = EMER.time_ms - tobiiList.Last().time_ms;
                        if (!(tobiiList.Last().Type == EMER.Type))
                        {
                            tobiiList.Add(EMER);
                        }

                        fulltime_ms = EMER.time_ms;

                        if (N_TimeCol >= 0 && Time == "" && tmp[N_TimeCol] != "") Time = tmp[N_TimeCol];
                        if (N_DateCol >= 0 && Date == "" && tmp[N_DateCol] != "") Date = tmp[N_DateCol];

                    }

                }

            }

            //чистим от мелких неопределенных событий до 60 мс которые
            for (int ii = 0; ii < tobiiList.Count(); ii++)
            {
                if (tobiiList[ii].Type == EyeMovementType.Other && tobiiList[ii].duraion_ms < 60)
                {
                    if (ii > 0) tobiiList[ii].Type = tobiiList[ii - 1].Type;
                }
            }

            tobiiList = CompactRecords(tobiiList);
            CalcDurations(ref tobiiList);

            foreach (var item in tobiiList)
            {
                if (item.duraion_ms > 80 && item.Type == EyeMovementType.Saccade)
                    item.Type = EyeMovementType.Search;
            }

        }


        public static bool ReadPartOfFile(StreamReader rd, out string str)
        {
            bool endOfFile = false;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i <= 10000; i++)
            {
                string s = rd.ReadLine();
                if (s == null) { endOfFile = true; break; }
                sb.Append(s);
                sb.Append("\n");
            }
            str = sb.ToString();
            return endOfFile;
        }

        /// <summary>
        /// Найти номера заполненных колонок
        /// </summary>
        /// <param name="row"></param>
        /// <param name="colName"></param>
        /// <returns></returns>
        List<int> SearchCol(string[] row, string colName)
        {
            List<int> zones = new List<int>();
            int ii = 0;
            foreach (string s in row)
            {
                if (s == null) continue;
                if (s.IndexOf(colName) > -1)
                {
                    zones.Add(ii + 1);
                }
                ii++;
            }
            return zones;
        }

        int SearchColFirst(string[] row, string colName)
        {
            int ii = 0;
            bool find = false;
            foreach (string s in row)
            {
                if (s == null) continue;
                if (s.IndexOf(colName) > -1)
                { find = true; break; }
                ii++;
            }
            if (find) return ii;
            else { return -1; }
        }


        private bool IsEqual(List<int> a, List<int> b)
        {
            if (a.Count() != b.Count) return false;
            for (int i = 0; i < a.Count; i++)
                if (a[i] != b[i]) return false;
            return true;
        }

        //Убираем повторы из записи тоби 
        public List<EyeMovementEventRecord> CompactRecords(List<EyeMovementEventRecord> records)
        {

            List<EyeMovementEventRecord> RecordsNew = new List<EyeMovementEventRecord>();
            EyeMovementType Before = EyeMovementType.Other;
            foreach (var tr in records)
            {
                if (tr.Type != Before)
                {
                    RecordsNew.Add(tr);
                    Before = tr.Type;
                }
            }
            return RecordsNew;
        }

        public void CalcDurations(ref List<EyeMovementEventRecord> records)
        {
            foreach (var tr in records)
            {
                int i = records.IndexOf(tr);
                if (i < records.Count() - 1)
                {
                    tr.duraion_ms = records[i + 1].time_ms - tr.time_ms;
                }

            }
        }


        public List<TobiiRecord> ClearFromGarbageZone(List<TobiiRecord> tRs, int GarbageZone, long UPBoundFiltrationOfGarbage)
        {
            List<TobiiRecord> TRSNew = new List<TobiiRecord>();
            foreach (var tr in tRs)
            {
                bool NotGarbage = tr.CurFZone != GarbageZone;
                bool IsFirst = tRs.IndexOf(tr) == 0;
                bool IsLast = tRs.IndexOf(tr) > tRs.Count - 1;
                bool IsPreLast = tRs.IndexOf(tr) > tRs.Count - 2;

                long dt = 0;
                if (!IsLast && !IsPreLast)
                    dt = tRs[tRs.IndexOf(tr) + 1].time_ms - tr.time_ms;
                // dt = tr.time_ms - tRs[tRs.IndexOf(tr) - 1].time_ms;
                bool IsdtMoreUpBound = dt > UPBoundFiltrationOfGarbage;

                if (NotGarbage ||
                    IsFirst ||
                    IsLast ||
                    IsdtMoreUpBound)
                    TRSNew.Add(tr);
            }
            return TRSNew;
        }

    }

}
