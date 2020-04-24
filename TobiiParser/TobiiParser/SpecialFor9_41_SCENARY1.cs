using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TobiiParser
{
    class SpecialFor9_41_SCENARY1: SpecialFor9_41_POSADKI
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="WhereMustFind"></param>
        /// <param name="WhatMustFind"></param>
        /// <returns></returns>
        static string ContainsAny(string WhereMustFind, string[] WhatMustFind)
        {
            foreach (var s in WhatMustFind)
            {
                if (WhereMustFind.Contains(s)) return s;
            }
            return "";
        }

        static string IsAnyContains(string[] WhereMustFind, string WhatMustFind)
        {
            foreach (var s in WhereMustFind)
            {
                if (s.Contains(WhatMustFind)) return s;
            }
            return "";
        }

        public static void UnionFilesOnRegims(string dirMain)
        {
            string[] dirs = Directory.GetDirectories(dirMain,"*", SearchOption.AllDirectories);
            string[] regims = 
                {
                "_Подготовка",
                "_Взлёт",
                "_Маршрут",
                "_Ввод_ОПЦ_(РЛС)",
                "_Ввод_ОПЦ_(ОЛС)",
                "_Ввод_ОПЦ_(ИКШ)",
                "_ДБВ_РЛС",
                "_ДБВ_ОЛС",
                "_ББВ",
                "_ББВ_ВПУ",
                "_Возврат",
                "_Посадка"};

            string[] regims_new =
                {
                "№01-Подготовка",
                "№02-Взлёт",
                "№03-Маршрут",
                "№04-Ввод_ОПЦ_(РЛС)",
                "№05-Ввод_ОПЦ_(ОЛС)",
                "№06-Ввод_ОПЦ_(ИКШ)",
                "№07-ДБВ_РЛС",
                "№08-ДБВ_ОЛС",
                "№09-ББВ",
                "№10-ББВ_ВПУ",
                "№11-Возврат",
                "№12-Посадка"};


            foreach (var dir in dirs)
            {
                string regim = ContainsAny(dir, regims).Trim('_');
                if (regim != "")
                {
                    string HigherDir = Path.GetDirectoryName(dir);
                    string newRegim = IsAnyContains(regims_new, regim);
                    string NewDir = Path.Combine(HigherDir, newRegim);
                    if(!Directory.Exists(NewDir))
                        Directory.CreateDirectory(NewDir);

                    string[] files = Directory.GetFiles(dir);
                    foreach (var file in files)
                    {
                        string newFileName = Path.Combine(NewDir,Path.GetFileName(file));
                        File.Move(file, newFileName);
                    }
                    Directory.Delete(dir, false);
                }
            }

           
        }
    }
}
