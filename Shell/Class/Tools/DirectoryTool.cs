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
            try
            {
                foreach (var item in Directory.GetFiles(folder))
                {
                    files.Add(item);
                }
                foreach (var item in Directory.GetDirectories(folder))
                {
                    files.AddRange(GetFiles(item));
                }
            }
            catch
            {

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
        public static string[] GetLinesInFile(string file, string toFind, bool lower = true)
        {
            List<string> lines = new List<string>();
            try
            {
                int count = 0;
                foreach (var item in File.ReadAllLines(file))
                {
                    if (lower)
                    {
                        if (item.ToLower().IndexOf(toFind.ToLower()) >= 0)
                        {
                            lines.Add(TextTool.AddBlankRight(count.ToString(), 4) + item);
                        }
                    }
                    else
                    {
                        if (item.IndexOf(toFind) >= 0)
                        {
                            lines.Add(TextTool.AddBlankRight(count.ToString(), 4) + item);
                        }
                    }
                    count++;
                }
            }
            catch
            {
                return lines.ToArray();
            }

            return lines.ToArray();
        }
        
        public static string SetPath(string path)
        {
            if (Path.IsPathRooted(path))
            {
                return path;
            }
            else
            {
                return Main.shellConfig.actualDir + "\\" + path;
            }
        }
    }
}
