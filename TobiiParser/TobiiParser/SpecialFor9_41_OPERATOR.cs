using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TobiiParser
{
    internal class SpecialFor9_41_OPERATOR : SpecialFor9_41_AIVAZYAN
    {



        public static async void ParseForSpecialGazeParams(string mainDir, TextBox textBox, TextBox Big_textBox)
        {
            string[] dirs = Directory.GetDirectories(mainDir, "*", SearchOption.AllDirectories);
            foreach (var dir in dirs)
            {
                string file_csv;
                string[] filescsv = Directory.GetFiles(dir, "*.csv", SearchOption.TopDirectoryOnly);
                if (filescsv.Count() > 1) { Big_textBox.Text += "В директории " + dir + "       содержится более 1 файла csv" + Environment.NewLine; continue; }
                else if (filescsv.Count() < 1) { Big_textBox.Text += "В директории " + dir + "          нет файла csv" + Environment.NewLine; continue; }
                file_csv = filescsv[0];

                textBox.Text = "Обрабатываю " + dir;
                await Task.Run(() => ParseInDirectory_SpecialGazeParams(file_csv));
            }

            textBox.Text = "Обработка завершена";
        }


        private static void ParseInDirectory_SpecialGazeParams(string file_csv)
        {
            TobiiPupilDiametersCsvReader reader = new TobiiPupilDiametersCsvReader();
            List<PupilDiameterRecord> records;
            reader.ReadFromFile(file_csv, out records);

            double PupilAvgDiameterLeft, PupilAvgDiameterRight;
            PupilDiameterRecord.GetAvgDiametersInList(records, out PupilAvgDiameterLeft, out PupilAvgDiameterRight);

            Console.WriteLine("{0} --- {1}-{2}", Path.GetFileName(file_csv), PupilAvgDiameterLeft, PupilAvgDiameterRight);

        }


    }
}
