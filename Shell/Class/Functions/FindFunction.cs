using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Shell.Class.Tools;

namespace Shell.Class.Functions
{
    public static class FindFunction
    {
        public static Command command;
        static string pathSource;
        static string toFind;
        static bool showLine;
        static int index;

        public static int Find()
        {
            command = Main.Command;
            pathSource = "";
            toFind = "";
            index = 1;
            showLine = false;

            IsShowLine();
            if (IsFind() == 1)
            {
                return 1;
            }

            if (File.Exists(pathSource))
            {
                if (showLine)
                {
                    string[] line = DirectoryTool.GetLinesInFile(pathSource, toFind);
                    if (line.Length > 0)
                    {
                        Console.WriteLine(pathSource);
                        foreach (var item in line)
                        {
                            Console.WriteLine(item);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Ligne non trouvée.");
                    }
                }
                else
                {
                    if (DirectoryTool.FindInFile(pathSource, toFind))
                    {
                        Console.WriteLine("Ligne trouvée.");
                    }
                    else
                    {
                        Console.WriteLine("Ligne non trouvée.");
                    }
                }
            }
            else if (Directory.Exists(pathSource))
            {
                List<string> files = DirectoryTool.GetFiles(pathSource);

                bool bef = false;
                foreach (var item in files)
                {
                    if (showLine)
                    {
                        string[] lines = DirectoryTool.GetLinesInFile(item, toFind);
                        if (lines.Length > 0)
                        {
                            if (bef)
                            {
                                Console.WriteLine("");
                            }
                            Console.WriteLine(item.Substring(pathSource.Length, item.Length - pathSource.Length));
                            foreach (var line in lines)
                            {
                                Console.WriteLine(line);
                            }
                            bef = true;
                        }
                    }
                    else
                    {
                        if (DirectoryTool.FindInFile(item, toFind))
                        {
                            Console.WriteLine(item.Substring(pathSource.Length, item.Length - pathSource.Length));
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Chemin d'accès non reconnu.");
                return 1;
            }

            return 0;
        }

        static int IsShowLine()
        {
            if (command.GetBaseValue(index) == "-sl" || command.GetBaseValue(index) == "-showlines")
            {
                showLine = true;
                index += 1;
                return 0;
            }
            return 1;
        }
        static int IsFind()
        {
            string actualPath = Main.shellConfig.actualDir;

            string val = command.GetBaseValue(index);
            if(val != "")
            {
                toFind = val.Replace("\"","");

                val = command.GetBaseValue(index + 1);
                if (val != "")
                {
                    if (Path.IsPathRooted(val))
                    {
                        pathSource = val;
                    }
                    else
                    {
                        pathSource = actualPath + "\\" + val;
                    }
                    return 0;
                }
                else
                {
                    pathSource = actualPath;
                    return 0;
                }
            }
            return 1;
        }
    }
}
