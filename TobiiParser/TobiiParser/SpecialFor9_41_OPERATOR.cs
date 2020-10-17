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
            using (StreamWriter wr = new StreamWriter(new FileStream(@"C:\__\SpecialParams.csv", FileMode.Append)))
            {
                wr.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t{14}\t{15}",
                            "Испытуемый",
                            "Дата",
                            "Время",
                            "Диаметр левого зрачка, мм",
                            "Диаметр правого зрачка",
                            "% времени фиксаций",
                            "% времени саккад(менее 80 мс)",
                            "% времени зрительного поиск(saccade более 80 мс)",
                            "Суммарное время зрительной фиксации за всю запись, секунды",
                            "Суммарное время саккад за всю запись, секунды",
                            "Суммарное время зрительного поиска за всю запись, секунды",
                            "Число фиксаций за всю запись",
                            "Число саккад за всю запись",
                            "Число зрительных поисков за всю запись",
                            "Полное время записи, мсек",
                            "Полное число саккад, фиксаций, поисков и неидентифицированных событий за всю запись, шт."


                             );

            }

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
            TobiiPupilDiametersCsvReader PDreader = new TobiiPupilDiametersCsvReader();
            List<PupilDiameterRecord> PDrecords;
            PDreader.ReadFromFile(file_csv, out PDrecords);

            double PupilAvgDiameterLeft, PupilAvgDiameterRight;
            PupilDiameterRecord.GetAvgDiametersInList(PDrecords, out PupilAvgDiameterLeft, out PupilAvgDiameterRight);



            List<EyeMovementEventRecord> EMErecords;
            TobiiEyeMoveventEventsCsvReader EMEreader = new TobiiEyeMoveventEventsCsvReader();
            string Date, Time;
            long fulltime_ms;
            EMEreader.ReadFromFile(file_csv, out EMErecords, out Time, out Date, out fulltime_ms);

            var fixations = from i in EMErecords
                            where i.Type == EyeMovementType.Fixation
                            select i;

            var saccades = from i in EMErecords
                           where i.Type == EyeMovementType.Saccade
                           select i;

            var searches = from i in EMErecords
                           where i.Type == EyeMovementType.Search
                           select i;


            double FixationsDurationSumm = fixations.Sum(n => n.duraion_ms);
            double SaccadesDurationSumm = saccades.Sum(n => n.duraion_ms);
            double SearchesDurationSumm = searches.Sum(n => n.duraion_ms);

            double FixationsPercent = FixationsDurationSumm / fulltime_ms;
            double SaccadesPercent = SaccadesDurationSumm / fulltime_ms;
            double SearchesPercent = SearchesDurationSumm / fulltime_ms;

            Console.WriteLine("{0} ......... {1:f1}-{2:f1} .... {3:d} ... {4} {5} .. {6:d}",
                Path.GetFileName(file_csv), PupilAvgDiameterLeft, PupilAvgDiameterRight, EMErecords.Count, Date, Time, fulltime_ms);

            Console.WriteLine("fix {0:f1} {3:p1}    sac {1:f1} {4:p1}    search{2:f1} {5:p1}",
               FixationsDurationSumm, SaccadesDurationSumm, SearchesDurationSumm, FixationsPercent, SaccadesPercent, SearchesPercent);

            using (StreamWriter wr = new StreamWriter(new FileStream(@"C:\__\SpecialParams.csv", FileMode.Append)))
            {
                wr.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t{14}\t{15}",
                             Path.GetFileNameWithoutExtension(file_csv),
                             Date,
                             Time,
                             PupilAvgDiameterLeft,
                             PupilAvgDiameterRight,
                             FixationsPercent,
                             SaccadesPercent,
                             SearchesPercent,
                             FixationsDurationSumm,
                             SaccadesDurationSumm,
                             SearchesDurationSumm,
                             fixations.Count(),
                             saccades.Count(),
                             searches.Count(),
                             fulltime_ms,
                             EMErecords.Count()                             
                             );

            }


        }


    }
}
