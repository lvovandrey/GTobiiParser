using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TobiiParser
{
    public class FZoneTab
    {
        public List<TobiiRecord> FZoneList;

        public void Calculate(List<TobiiRecord> tobiiRecords, List<KadrInTime> kadrInTimes, TabOfKeys tabOfKeys)
        {
            FZoneList = new List<TobiiRecord>();
            foreach (var TR in tobiiRecords)
            {
               
                string kadr = KadrInTime.GetKadr(kadrInTimes, TR.time_ms, TR.zone);
                if (kadr == "") continue;
                int FZone = tabOfKeys.GetFuncZone(TR.zone, kadr);
                FZoneList.Add(new TobiiRecord() { time_ms = TR.time_ms, zone = FZone });
            }
        }

        public async void WriteResult(string filename)
        {
            using (StreamWriter writer = File.CreateText(filename))
            {
                await Task.Run(() => Write(writer));
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        void Write(StreamWriter writer)
        {
            foreach (var tr in FZoneList)
            {
                double time = (double)tr.time_ms;
                int hour = (int)Math.Floor(time / 3_600_000);
                time -= hour * 3_600_000;
                int min = (int)Math.Floor(time / 60_000);
                time -= min * 60_000;
                int sec = (int)Math.Floor(time / 1_000);
                time -= sec * 1_000;
                int msec = (int)Math.Floor(time);

                string s = hour.ToString()+"\t"+min.ToString()+"\t"+sec.ToString()+ "\t" +msec.ToString() +"\t" + tr.zone.ToString();
                writer.WriteLine(s);
            }
        }

    }
}
