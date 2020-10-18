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
        //public List<TobiiRecord> FZoneList;

        public List<TobiiRecord> Calculate(List<TobiiRecord> tobiiRecords, List<KadrInTime> kadrInTimes, TabOfKeys tabOfKeys)
        {
            // FZoneList = new List<TobiiRecord>();
            foreach (var TR in tobiiRecords)
            {
                foreach (var zone in TR.zones)
                    TR.fzones.Add(tabOfKeys.GetFuncZone(zone, "ПИЛ"));

                TR.fzones = TR.fzones.Distinct().ToList();

                if (TR.fzones.Count() > 1)
                    if (TR.fzones.Contains(13)) TR.fzones.Remove(13);
                if (TR.fzones.Count() > 0)
                    TR.CurFZone = TR.fzones.First();
                if (TR.fzones.Count() == 0)
                    TR.CurFZone = -1;
                // string kadr = KadrInTime.GetKadr(kadrInTimes, TR.time_ms, TR.zone);
                // if (kadr == "") continue;
                //string kadr = KadrInTime.GetKadr(kadrInTimes, TR.time_ms);

                //int FZone = tabOfKeys.GetFuncZone(TR.zone, kadr);
                //FZoneList.Add(new TobiiRecord() { time_ms = TR.time_ms, zone = FZone });
            }
            return tobiiRecords;
        }


        public List<TobiiRecord> Calculate(List<TobiiRecord> tobiiRecords, TabOfKeys tabOfKeys)
        {

            foreach (var TR in tobiiRecords)
            {
                foreach (var zone in TR.zones)
                    TR.fzones.Add(tabOfKeys.GetFuncZone(zone, "ПИЛ"));

                TR.fzones = TR.fzones.Distinct().ToList();

                if (TR.fzones.Count() > 1) 
                    if (TR.zones.Contains(4))
                        TR.fzones.Remove(4);
                if (TR.fzones.Count() > 1)
                    if (TR.zones.Contains(5))
                        TR.fzones.Remove(5);
                if (TR.fzones.Count() > 1)
                    if (TR.zones.Contains(6))
                        TR.fzones.Remove(6);

                if (TR.fzones.Count() > 0)
                    TR.CurFZone = TR.fzones.Last();
                if (TR.fzones.Count() == 0)
                    TR.CurFZone = -1;
            }
            return tobiiRecords;
        }

        /// <summary>
        /// По моему это бесполезно комментировать.... 
        /// Короче тут происходит самое основное - вроде как смотрится каждая строка, потом выясняется в каком она кадре
        /// Потом .... вобщем ей назначается одна только зона в результате....
        /// </summary>
        /// <param name="tobiiRecords"></param>
        /// <param name="kadrIntervals"></param>
        /// <param name="tabOfKeys"></param>
        /// <returns></returns>
        public List<TobiiRecord> Calculate(List<TobiiRecord> tobiiRecords, KadrIntervals kadrIntervals, TabOfKeys tabOfKeys)
        {
            // FZoneList = new List<TobiiRecord>();
            foreach (var TR in tobiiRecords)//проходим по всем записям
            {

                foreach (var zone in TR.zones)
                {
                    int MFINumber;
                    string kadr;
                    int Fzone;
                    MFINumber = SpecialFor9_41_SCENARY2.GetMFINumber(zone);
                    kadr = kadrIntervals.GetKadr(TR.time_ms, MFINumber);
                    if (kadr == "")
                        throw new Exception("Не могу найти подходящий кадр для интервала id = " + kadrIntervals.Id + " при времени" + TR.time_ms);
                    Fzone = tabOfKeys.GetFuncZone(zone, kadr);
                    TR.fzones.Add(Fzone);

                }

                TR.fzones = TR.fzones.Distinct().ToList();

                if (TR.fzones.Count() > 1)
                    if (TR.fzones.Contains(0))
                        TR.fzones.Remove(0);
                if (TR.fzones.Count() > 0)
                    TR.CurFZone = TR.fzones.First();
                if (TR.fzones.Count() == 0)
                    TR.CurFZone = -1;



            }
            return tobiiRecords;
        }



        public async void WriteResult(string filename, List<TobiiRecord> FZoneList)
        {
            using (StreamWriter writer = File.CreateText(filename))
            {
                await Task.Run(() => Write(writer, FZoneList));
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        void Write(StreamWriter writer, List<TobiiRecord> FZoneList)
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

                string s = hour.ToString() + "\t" + min.ToString() + "\t" + sec.ToString() + "\t" + msec.ToString() + "\t" + tr.CurFZone.ToString();
                writer.WriteLine(s);
            }
        }

    }
}
