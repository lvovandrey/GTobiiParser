using System.Collections.Generic;

namespace TobiiParser
{
    /// <summary>
    /// В целом то же что KadrInTime только более обобщенное
    /// </summary>
    public class KadrInterval
    {


        public Dictionary<int, string> KadrOnMFI = new Dictionary<int, string>();
        public long time_ms_beg, time_ms_end;

        public KadrInterval(List<string> Kadrs, long TimeBeg, long TimeEnd)
        {
            foreach (var Kadr in Kadrs)
                KadrOnMFI.Add(Kadrs.IndexOf(Kadr) + 1, Kadr);
            time_ms_beg = TimeBeg;
            time_ms_end = TimeEnd;
        }

        //время в этом промежутке?
        public bool IsTimeHere(long time_ms)
        {
            if (time_ms >= time_ms_beg && time_ms <= time_ms_end) return true;
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

    }
}