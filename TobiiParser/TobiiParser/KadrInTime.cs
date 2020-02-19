using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TobiiParser
{
    public class KadrInTime
    {



        public string KadrInLeftMFI;
        public string KadrInCenterMFI;
        public string KadrInRightMFI;
        public string RMLeftOrRight;
        public long time_ms_beg, time_ms_end;
        public KadrInTime(string LeftMFI, string CenterMFI, string RightMFI, string RM, long TimeBeg, long TimeEnd)
        {
            KadrInLeftMFI = LeftMFI;
            KadrInCenterMFI = CenterMFI;
            KadrInRightMFI = RightMFI;
            RMLeftOrRight = RM;
            //ВНИМАНИЕ!!! я тут посчитал что раз Денис кадры переворачивал, тут и заморачиваться не стоит с правым/левым  Условно говоря у нас всегда левое РМ
           // RMLeftOrRight = "левый";

            time_ms_beg = TimeBeg;
            time_ms_end = TimeEnd;
        }

        public override string ToString()
        {
            return time_ms_beg.ToString() + " - " + time_ms_end.ToString() + "   MFILeft-" + KadrInLeftMFI + "   MFICentr-" + KadrInCenterMFI + "   MFIRight-" + KadrInRightMFI + "   RM-" + RMLeftOrRight;
        }

        //что на основном мфи
        public string OnMainMFI
        {
            get
            {
                if (RMLeftOrRight == "левый")
                    return KadrInLeftMFI;
                else if (RMLeftOrRight == "правый")
                    return KadrInRightMFI;
                else throw new Exception("Не могу определить кадр на этом мфи - он не левый и не правый, а ..." + RMLeftOrRight);
            }
        }

        //что на дальнем мфи
        public string OnFarMFI
        {
            get
            {
                if (RMLeftOrRight == "левый")
                    return KadrInRightMFI;
                else if (RMLeftOrRight == "правый")
                    return KadrInLeftMFI;
                else throw new Exception("Не могу определить кадр на этом мфи - он не левый и не правый, а ..." + RMLeftOrRight);
            }
        }

        //что на центральном мфи
        public string OnCenterMFI
        {
            get
            {
                return KadrInCenterMFI;
            }
        }

        //время в этом промежутке?
        public bool IsTimeHere(long time_ms)
        {
            if (time_ms >= time_ms_beg && time_ms <= time_ms_end) return true;
            else return false;
        }


        public static KadrInTime FindTimeInList(List<KadrInTime> kadrInTimes, long time_ms)
        {
            foreach (var k in kadrInTimes)
            {
                if(k.IsTimeHere(time_ms)) { return k; }
            }

            return null;
        }

        public static string GetKadr(KadrInTime kadrInTime, int tobiiZone)
        {
            if (kadrInTime == null) return "";
            if (tobiiZone >= 1 && tobiiZone <= 13)
            { return kadrInTime.OnMainMFI; }
            if (tobiiZone >= 14 && tobiiZone <= 26)
            { return kadrInTime.OnCenterMFI; }
            if (tobiiZone == 27 )
            { return kadrInTime.OnFarMFI; }
            if ((tobiiZone >= 28 && tobiiZone <= 37) || (tobiiZone>=-1 && tobiiZone<=0))
            { return kadrInTime.OnMainMFI; }

            throw new Exception("Зона тобии номер " + tobiiZone.ToString() + " находится вне диапазона адресов - от -1 до 37");

        }
        public static string GetKadr(List<KadrInTime> kadrInTimes,long time_ms, int tobiiZone)
        {
            KadrInTime kit = FindTimeInList(kadrInTimes, time_ms);
            string kadr = GetKadr(kit, tobiiZone);
            return kadr;
        }
    }
}
