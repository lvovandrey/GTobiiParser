using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TobiiParser
{
    public class SpecialFor9_41
    {
        /// <summary>
        /// Сортировка файлов по папкам с учетом отличительного признака - для 9.41 делаем
        /// </summary>
        /// <param name="mainDir"></param>
        /// <param name="mark"></param>
        /// <param name="filemask"></param>
        internal static void SortAndUnionFilesInDirs_SpecialFor9_41(string mainDir, string targetDir)
        {

            string[] files = Directory.GetFiles(mainDir, "*.txt", SearchOption.TopDirectoryOnly);
            foreach (string fullfilepath1 in files)
            {
                if (!File.Exists(fullfilepath1)) continue;

                string filepath1 = Path.GetFileName(fullfilepath1);


                int N_pos1 = filepath1.IndexOf("№"); // ищем позицию номера 
                string N_subs1 = filepath1.Substring(N_pos1 + 1, 2);     //AAA  берем подстроку после номера - ну сам номер порядковый
                string Regim_subs1 = filepath1.Substring(0, N_pos1 - 1);  //ZZZ  берем подстроку до номера - в ней весь режим

                int SP_pos1 = filepath1.IndexOf("СП");   //ищем позицию сложного положения
                int EXT_pos1 = filepath1.IndexOf(".txt");      //ищем позицию расширения
                string SP_subs1 = filepath1.Substring(SP_pos1 + 2, EXT_pos1 - SP_pos1 - 2); //XX     берем номер сложного положения

                string NewDirName = Path.Combine(targetDir, Regim_subs1 + "СП" + SP_subs1);      
                string NewFile1Name = Path.Combine(NewDirName, filepath1);

                if (!Directory.Exists(NewDirName))
                {
                    Directory.CreateDirectory(NewDirName);
                    FileInfo fi = new FileInfo(fullfilepath1);
                    fi.MoveTo(NewFile1Name);
                }


                string[] files2 = Directory.GetFiles(mainDir, "*.txt", SearchOption.TopDirectoryOnly);
                foreach (string fullfilepath2 in files2)
                {
                    string filepath2 = Path.GetFileName(fullfilepath2);


                    int N_pos2 = filepath2.IndexOf("№");
                    string N_subs2 = filepath2.Substring(N_pos2 + 1, 2);     //AAA
                    string Regim_subs2 = filepath2.Substring(0, N_pos2 - 1);  //ZZZ

                    int SP_pos2 = filepath2.IndexOf("СП");
                    int EXT_pos2 = filepath2.IndexOf(".txt");
                    string SP_subs2 = filepath2.Substring(SP_pos2 + 2, EXT_pos2 - SP_pos2 - 2); //XX

                    string NewFile2Name = Path.Combine(NewDirName, filepath2);
                    if ((Regim_subs1 == Regim_subs2) && (SP_subs1 == SP_subs2) && (N_subs1 != N_subs2))
                    {
                        if (Directory.Exists(NewDirName))
                        {
                            FileInfo fi2 = new FileInfo(fullfilepath2);
                            fi2.MoveTo(NewFile2Name);
                        }
                    }
                }

            }
        }
    }
}
