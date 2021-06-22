using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell.Class.Tools
{
    public static class DirectoryTool
    {
        public static List<string> GetFiles(string folder)
        {
            List<string> files = new List<string>();

            foreach (var item in Directory.GetFiles(folder))
            {
                files.Add(item);
            }
            foreach (var item in Directory.GetDirectories(folder))
            {
                files.AddRange(GetFiles(item));
            }
            return files;
        }
        public static bool FindInFile(string file, string toFind)
        {
            try
            {
                foreach (var item in File.ReadAllLines(file))
                {
                    if (item.IndexOf(toFind) >= 0)
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }

            return false;
        }
        public static string GetLinesInFile(string file, string toFind)
        {
            try
            {
                foreach (var item in File.ReadAllLines(file))
                {
                    if (item.IndexOf(toFind) >= 0)
                    {
                        return item;
                    }
                }
            }
            catch
            {
                return "";
            }

            return "";
        }
    }
}
