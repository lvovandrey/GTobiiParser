using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TobiiParser
{

    public class TobiiRecord
    {
        public long time_ms;
        public List<int> zones;
        public List<int> fzones;
        public int CurFZone;

        public TobiiRecord()
        {
            time_ms = 0;
            zones = new List<int>();
            fzones = new List<int>();
            CurFZone = -1;
        }

        public TobiiRecord(TobiiRecord TR)
        {
            time_ms = TR.time_ms;
            zones = TR.zones;
            fzones = TR.fzones;
            CurFZone = TR.CurFZone;
        }
    }
}
