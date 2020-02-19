using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TobiiParser
{
    public class TabOfKeys
    {
        public Dictionary<int, Dictionary<string, int>> Tab = new Dictionary<int, Dictionary<string, int>>();
     
        public int GetFuncZone(int TobiiZone, string Kadr)
        {
            Dictionary<string, int> d;
            if (Tab.ContainsKey(TobiiZone)) d = Tab[TobiiZone];
            else throw new Exception("Не обнаружена в ТАБ2(TabOfKeys) зона номер " + TobiiZone.ToString());

            int zone;
            if (d.ContainsKey(Kadr))  zone = d[Kadr];
            else throw new Exception("Не обнаружен в ТАБ2(TabOfKeys) для зоны " + TobiiZone.ToString() + "  кадр "+ Kadr);

            return zone;
        }
    }
}
