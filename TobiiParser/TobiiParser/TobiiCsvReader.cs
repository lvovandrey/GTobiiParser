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

        public void TobiiCSCRead(string filename, List<TobiiRecord> tobiiList)
        {
            
            char separator = '\n';
            char delimiter = '\t';

            int N_timestampCol = 0, N_firstZoneCol = 0;
            int ZoneColCount = 54;
            long i = 0;
            using (StreamReader rd = new StreamReader(new FileStream(filename, FileMode.Open)))
            {
                string[] first_string_arr = { "" };
                first_string_arr = rd.ReadLine().Split(delimiter);
                N_timestampCol = SearchColFirst(first_string_arr, "Recording timestamp");
                N_firstZoneCol = SearchColFirst(first_string_arr, "AOI hit [");

                bool EndOfFile=false;
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
                        Array.Copy(tmp, N_firstZoneCol, Hits, 0, ZoneColCount);
                        TR.zones = SearchCol(Hits, "1");
                        tobiiList.Add(TR);
                    }

                }

                FiltredTobiiList = CompactTobiiRecords(tobiiList);
            }





        }

        bool ReadPartOfFile(StreamReader rd, out string str)
        {
            bool endOfFile = false;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i <= 10000; i++)
            {
                string s = rd.ReadLine();
                if (s == null) { endOfFile = true; break;  }
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
                    zones.Add(ii+1);
                }
                ii++;
            }
            return zones;
        }

        int SearchColFirst(string[] row, string colName )
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
                if (a[i] != b[i])  return false;
            return true;
        }

        //Убираем повторы из записи тоби - компактифицируем ее
        public List<TobiiRecord> CompactTobiiRecords(List<TobiiRecord> tRs, string mode="MultZones")
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

                for (int i = 1; i<tRs.Count;i++)
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

                long dt =0;
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
