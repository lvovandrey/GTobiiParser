using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TobiiParser
{
    public class FilesOperations
    {
        /// <summary>
        /// Осуществляет копирование файлов в новую директорию из всех субдиректорий, причем сохраняет структуру первичной директории (копирует также сами директории)
        /// Но не копирует файлы, которые не совпадают с маской
        /// </summary>
        /// <param name="mainDir"></param>
        /// <param name="targetDir"></param>
        /// <param name="filemask"></param>
        public static void DeepCopyFilesToDir(string mainDir, string targetDir, string filemask)
        {
            string[] dirs = Directory.GetDirectories(mainDir, "*", SearchOption.AllDirectories);
            foreach (var dir in dirs)
            {

                string newDir = dir.Replace(mainDir, targetDir);
                try
                {
                    DirectoryInfo di = new DirectoryInfo(newDir);
                    if (!di.Exists) di.Create();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    return;
                }

                string[] files = Directory.GetFiles(dir, filemask, SearchOption.TopDirectoryOnly);

                foreach (string filepath in files)
                {
                    try
                    {
                        string newfilepath = filepath.Replace(mainDir, targetDir);
                        FileInfo fi = new FileInfo(filepath);
                        fi.CopyTo(newfilepath);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                        return;
                    }
                }
            }
        }

        internal static void RenameAndAddSufficsAndUID(string mainDir, string suffics, string filemask)
        {
            Random R = new Random(32312123);

            string[] dirs = Directory.GetDirectories(mainDir, "*", SearchOption.AllDirectories);
            foreach (var dir in dirs)
            {

                string[] files = Directory.GetFiles(dir, filemask, SearchOption.TopDirectoryOnly);

                foreach (string filepath in files)
                {
                    try
                    {

                        string ext = Path.GetExtension(filepath);
                        string newfilepath = filepath.Replace(ext, "_" + suffics + "_" + R.Next(100000, 999999).ToString() + ext);
                        FileInfo fi = new FileInfo(filepath);
                        fi.MoveTo(newfilepath);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                        return;
                    }
                }
            }
        }

        internal static void RenameAndAddSufficsAndUIDAndPath(string mainDir, string suffics, string filemask)
        {
            Random R = new Random(32312123);

            string[] dirs = Directory.GetDirectories(mainDir, "*", SearchOption.AllDirectories);
            foreach (var dir in dirs)
            {

                string[] files = Directory.GetFiles(dir, filemask, SearchOption.TopDirectoryOnly);

                foreach (string filepath in files)
                {
                    try
                    {

                        string ext = Path.GetExtension(filepath);
                        string path = filepath.Replace(Path.GetFileName(filepath), "").Replace(mainDir, "").Replace(@"\", "_");
                        string newfilepath = filepath.Replace(ext, "_" + suffics + "_" + path + "_" + R.Next(100000, 999999).ToString() + ext);
                        FileInfo fi = new FileInfo(filepath);
                        fi.MoveTo(newfilepath);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                        return;
                    }
                }
            }
        }

        internal static void OutFilepaths(string mainDir, TextBox textBox)
        {


            string[] dirs = Directory.GetDirectories(mainDir, "*", SearchOption.AllDirectories);
            foreach (var dir in dirs)
            {

                string[] files = Directory.GetFiles(dir, "*.*", SearchOption.TopDirectoryOnly);

                foreach (string filepath in files)
                {
                    try
                    {
                        System.IO.FileInfo file = new System.IO.FileInfo(filepath);
                        long size = file.Length;

                        textBox.Text += filepath.Replace(mainDir, "") + "\t" + size.ToString() + "\t" + Path.GetExtension(filepath).Replace(".", "") + System.Environment.NewLine;
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                        return;
                    }
                }
            }
        }



      
    }

}

