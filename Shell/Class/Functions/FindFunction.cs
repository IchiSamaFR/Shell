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
        static string pathSave;
        static string toFind;
        static bool showLine;
        static bool findName;
        static int index;

        public static int Find()
        {
            command = Main.Command;
            pathSource = "";
            toFind = "";
            index = 1;
            showLine = false;
            findName = false;

            if(GetCommand() == 0)
            {
                return 0;
            }
            if (pathSave != "")
            {
                if (File.Exists(pathSave))
                {
                    File.Create(pathSave);
                }
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
                else if (findName)
                {
                    if (Path.GetFileName(pathSource).ToLower().IndexOf(toFind.ToLower()) >= 0)
                    {
                        Console.WriteLine("Index trouvée.");
                    }
                    else
                    {
                        Console.WriteLine("Index non trouvée.");
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
                    else if (findName)
                    {
                        if(pathSave != "")
                        {
                            if (Path.GetFileName(item).ToLower().IndexOf(toFind.ToLower()) >= 0)
                            {
                                File.AppendAllText(pathSave, Environment.NewLine + item);
                            }
                        }
                        else
                        {
                            if (Path.GetFileName(item).ToLower().IndexOf(toFind.ToLower()) >= 0)
                            {
                                Console.WriteLine(item.Substring(pathSource.Length, item.Length - pathSource.Length));
                            }
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
                return 0;
            }

            return 1;
        }

        static string SetSource(string path)
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

        static int GetCommand()
        {
            if (command.IsCommandLike(index, "-sl") || command.IsCommandLike(index, "--showlines"))
            {
                showLine = true;
                index += 1;
                if (command.IsCommandLike(index, "$value $value $end"))
                {
                    toFind = command.GetBaseValue(index).Replace("\"", "");
                    pathSource = DirectoryTool.SetPath(command.GetBaseValue(index + 1).Replace("\"", ""));
                }
                else if (command.IsCommandLike(index, "$value $end"))
                {
                    toFind = command.GetBaseValue(index).Replace("\"", "");
                    pathSource = Main.shellConfig.actualDir;
                }
                else
                {
                    return 0;
                }
            }
            else if (command.IsCommandLike(index, "-n") || command.IsCommandLike(index, "--name"))
            {
                findName = true;
                index += 1;
                if (command.IsCommandLike(index, "$value $value > $value $end"))
                {
                    toFind = command.GetBaseValue(index).Replace("\"", "");
                    pathSource = DirectoryTool.SetPath(command.GetBaseValue(index + 1).Replace("\"", ""));
                    pathSave = DirectoryTool.SetPath(command.GetBaseValue(index + 3).Replace("\"", ""));
                }
                else if (command.IsCommandLike(index, "$value $value $end"))
                {
                    toFind = command.GetBaseValue(index).Replace("\"", "");
                    pathSource = DirectoryTool.SetPath(command.GetBaseValue(index + 1).Replace("\"", ""));
                }
                else if (command.IsCommandLike(index, "$value $end"))
                {
                    toFind = command.GetBaseValue(index).Replace("\"", "");
                    pathSource = Main.shellConfig.actualDir;
                }
                else
                {
                    return 0;
                }
            }
            else if (command.IsCommandLike(index, "$value $value $end"))
            {
                toFind = command.GetBaseValue(index).Replace("\"", "");
                pathSource = DirectoryTool.SetPath(command.GetBaseValue(index + 1).Replace("\"", ""));
            }
            else if (command.IsCommandLike(index, "$value $end"))
            {
                toFind = command.GetBaseValue(index).Replace("\"", "");
                pathSource = Main.shellConfig.actualDir;
            }
            else
            {
                return 0;
            }

            return 1;
        } 
    }
}
