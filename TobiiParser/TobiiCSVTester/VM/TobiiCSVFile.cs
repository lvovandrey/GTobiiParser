using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TobiiCSVTester.Abstract;

namespace TobiiCSVTester.VM
{
    public struct XY
    {
        public long X;
        public double Y;
    }



    public class TobiiCSVFile: INPCBase
    {
        int SmoothInterval = 1000;

        public TobiiCSVFile(string name, int smoothInterval)
        {
            Name = name;
            SmoothInterval = smoothInterval;
            Xs = new List<long>();
            Ys = new List<double>();
        }

        public string Name { get; set; }

        public List<long> Xs
        {
            get;
            set;
        }

        public List<double> Ys
        {
            get;
            set;
        }

        public int aOIHitsColumnsCount = 0;
        public int AOIHitsColumnsCount
        {
            get { return aOIHitsColumnsCount; }
            set { aOIHitsColumnsCount = value; OnPropertyChanged("AOIHitsColumnsCount"); }
        }


        public async void ReadTestingInfoAsync()
        {
            Console.WriteLine(this.Name + " ReadTestingInfoAsync BEGIN");
            await Task.Run(() => ReadTestingInfo());
            Console.WriteLine(this.Name + " ReadTestingInfoAsync END");
        }

        public void ReadTestingInfo()
        {
            TobiiCSVRead(Name, tobiiList);
            Dictionary<long, double> avgs = CompactifyTobiiList(tobiiList, SmoothInterval);
            foreach (var item in avgs)
            {
                Ys.Add(item.Value);
                Xs.Add(item.Key);
            }
        }

        Dictionary<long, double> CompactifyTobiiList(List<TobiiRecord> TobiiRecords, int SmoothInterval)
        {
            Dictionary<long, double> Avgs = new Dictionary<long, double>();
            int Pices = (int)Math.Ceiling((double)(TobiiRecords.Count() / SmoothInterval));
            for (int i = 0; i < Pices; i++)
            {
                double CurSumm = 0;
                for (int j = i * SmoothInterval; j < (i + 1) * SmoothInterval; j++)
                {
                    CurSumm += TobiiRecords[j].zones.Count();
                }
                Avgs.Add(TobiiRecords[i * SmoothInterval].time_ms, CurSumm / SmoothInterval);
            }
            return Avgs;
        }


        public List<TobiiRecord> tobiiList = new List<TobiiRecord>();


        int ZoneColCountCalc(string[] first_string_arr)
        {
            int count = 0;
            foreach (var colName in first_string_arr)
            {
                if (colName.Contains("AOI hit [")) count++;
            }
            if (count == 0)
                throw new Exception("Не могу определить количество столбцов AOIhit");
            return count;
        }

        public void TobiiCSVRead(string filename, List<TobiiRecord> tobiiList)
        {

            char separator = '\n';
            char delimiter = '\t';

            int N_timestampCol = 0, N_firstZoneCol = 0;
            int ZoneColCount = 53;
            long i = 0;
            using (StreamReader rd = new StreamReader(new FileStream(filename, FileMode.Open)))
            {
                string[] first_string_arr = { "" };
                first_string_arr = rd.ReadLine().Split(delimiter);
                ZoneColCount = ZoneColCountCalc(first_string_arr);
                AOIHitsColumnsCount = ZoneColCount;
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

            }



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
    }
}