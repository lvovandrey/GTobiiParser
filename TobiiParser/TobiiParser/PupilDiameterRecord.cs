using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TobiiParser
{
    public class PupilDiameterRecord
    {
        public long time_ms;
        public double DiameterLeft;
        public double DiameterRight;


        public static void GetAvgDiametersInList(List<PupilDiameterRecord> records, out double AvgLeft, out double AvgRight)
        {
            double summL = 0, summR=0;
            foreach (var item in records)
            {
                summL += item.DiameterLeft;
                summR += item.DiameterRight;
            }
            AvgLeft = summL / records.Count();
            AvgRight = summR / records.Count();
        }
    }



}
