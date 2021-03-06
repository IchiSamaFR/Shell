using Shell.Class.Tools;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell.Class.Functions
{
    public static class CatFunction
    {
        public static Command command;
        static CultureInfo culture = new CultureInfo("fr-FR");

        static string pathSource;
        static string pathSource2;
        static string pathDest;
        static string creatDateV;
        static string modifDateV;
        static int index; 

        public static int Cat()
        {
            command = Main.Command;
            pathSource = "";
            pathSource2 = "";
            pathDest = "";
            creatDateV = "";
            modifDateV = "";
            index = 1;

            if (GetCommand() == 0)
            {
                return 0;
            }
            if (!DirectoryTool.IsValidPath(pathSource) || !DirectoryTool.IsValidPath(pathDest))
            {
                return 0;
            }

            if (File.Exists(pathSource))
            {
                if (File.Exists(pathSource2))
                {
                    if (pathSource != "" && pathSource2 != "" && pathDest != "")
                    {
                        if (!Directory.Exists(pathDest))
                        {
                            string res = "";

                            foreach (var item in File.ReadAllLines(pathSource))
                            {
                                res += item + Environment.NewLine;
                            }
                            foreach (var item in File.ReadAllLines(pathSource2))
                            {
                                res += item + Environment.NewLine;
                            }

                            File.WriteAllText(pathDest, res);
                        }
                        else
                        {
                            Console.WriteLine("Dossier portant le même nom déjà existant.");
                            return 0;
                        }
                    }
                    else if (creatDateV == "" && modifDateV == "")
                    {
                        foreach (var item in File.ReadAllLines(pathSource))
                        {
                            Console.WriteLine(item);
                        }
                    }
                }
                else
                {
                    if (pathSource != "" && pathDest != "")
                    {
                        if (Directory.Exists(pathDest))
                        {
                            File.Copy(pathSource, pathDest + "/" + Path.GetFileName(pathSource), true);
                        }
                        else if (pathDest.Replace("/", "\\")[pathDest.Length - 1] == '\\')
                        {
                            Directory.CreateDirectory(pathDest);
                            File.Copy(pathSource, pathDest + "/" + Path.GetFileName(pathSource), true);
                        }
                        else
                        {
                            string dir = Path.GetDirectoryName(pathDest);
                            if (!Directory.Exists(dir))
                            {
                                Directory.CreateDirectory(dir);
                            }
                            File.Copy(pathSource, pathDest, true);
                        }
                    }
                    else if (creatDateV == "" && modifDateV == "")
                    {
                        foreach (var item in File.ReadAllLines(pathSource))
                        {
                            Console.WriteLine(item);
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Chemin d'accès non reconnu.");
                return 0;
            }

            string pathDate = pathDest != "" ? pathDest : pathSource;

            if (creatDateV != "")
            {
                DateTime tempDate = Convert.ToDateTime(creatDateV, culture);
                File.SetCreationTime(pathDate, tempDate);
            }
            if (modifDateV != "")
            {
                DateTime tempDate = Convert.ToDateTime(modifDateV, culture);
                File.SetLastWriteTime(pathDate, tempDate);
            }

            return 1;
        }
        static int GetCommand()
        {
            if (command.IsCommandLike(index, "$value > $value"))
            {
                pathSource = DirectoryTool.SetPath(command.GetBaseValue(index).Replace("\"", ""));
                pathDest = DirectoryTool.SetPath(command.GetBaseValue(index + 2).Replace("\"", ""));
                index += 3;
                if (IsDateModif() == 0)
                {
                    return 0;
                }
                else if (!command.IsCommandLike(index, "$end"))
                {
                    Console.WriteLine("c");
                    return 0;
                }
            }
            else if(command.IsCommandLike(index, "$value $value > $value"))
            {
                pathSource = DirectoryTool.SetPath(command.GetBaseValue(index).Replace("\"", ""));
                pathSource2 = DirectoryTool.SetPath(command.GetBaseValue(index + 1).Replace("\"", ""));
                pathDest = DirectoryTool.SetPath(command.GetBaseValue(index + 3).Replace("\"", ""));
                index += 4;
                if (IsDateModif() == 0)
                {
                    return 0;
                }
                else if (!command.IsCommandLike(index, "$end"))
                {
                    Console.WriteLine("c");
                    return 0;
                }
            }
            else if (command.IsCommandLike(index, "$value -m $value") || command.IsCommandLike(index, "$value --modifdate $value"))
            {
                pathSource = DirectoryTool.SetPath(command.GetBaseValue(index).Replace("\"", ""));
                modifDateV = command.GetBaseValue(index + 2).Replace("\"", "");
                index += 3;
                if (command.IsCommandLike(index, "-c $value") || command.IsCommandLike(index, "--createdate $value"))
                {
                    creatDateV = command.GetBaseValue(index + 1).Replace("\"", "");
                }
                else if (!command.IsCommandLike(index, "$end"))
                {
                    return 0;
                }
            }
            else if (command.IsCommandLike(index, "$value -c $value") || command.IsCommandLike(index, "$value --createdate $value"))
            {
                pathSource = DirectoryTool.SetPath(command.GetBaseValue(index).Replace("\"", ""));
                creatDateV = command.GetBaseValue(index + 2).Replace("\"", "");
                index += 3;
                if (command.IsCommandLike(index, "-m $value") || command.IsCommandLike(index, "--modifdate $value"))
                {
                    modifDateV = command.GetBaseValue(index + 1).Replace("\"", "");
                }
                else if (!command.IsCommandLike(index, "$end"))
                {
                    return 0;
                }
            }
            else if (command.IsCommandLike(index, "> $value $end"))
            {
                string name = command.GetBaseValue(index + 1).Replace("\"", "");
                string line = "";
                while (TextTool.CancelableReadLine(out line))
                {
                    File.AppendAllText(name, Environment.NewLine + line);
                }

                return 0;
            }
            else if (command.IsCommandLike(index, "$value $end"))
            {
                string name = command.GetBaseValue(index).Replace("\"", "");
                if (File.Exists(name))
                {
                    foreach (var item in File.ReadAllLines(name))
                    {
                        Console.WriteLine(item);
                    }
                }
                else
                {
                    return 0;
                }

                return 0;
            }
            else if (!command.IsCommandLike(index, "$end"))
            {
                return 0;
            }

            return 1;
        }

        static int IsDateModif()
        {
            if (command.IsCommandLike(index, "-m $value") || command.IsCommandLike(index, "--modifdate $value"))
            {
                modifDateV = command.GetBaseValue(index + 1).Replace("\"", "");
                index += 2;
                if (command.IsCommandLike(index, "-c $value") || command.IsCommandLike(index, "--createdate $value"))
                {
                    creatDateV = command.GetBaseValue(index + 1).Replace("\"", "");
                }
                else if (!command.IsCommandLike(index, "$end"))
                {
                    Console.WriteLine("a");
                    return 0;
                }
            }
            else if (command.IsCommandLike(index, "-c $value") || command.IsCommandLike(index, "--createdate $value"))
            {
                creatDateV = command.GetBaseValue(index + 1).Replace("\"", "");
                index += 2;
                if (command.IsCommandLike(index, "-m $value") || command.IsCommandLike(index, "--modifdate $value"))
                {
                    modifDateV = command.GetBaseValue(index + 1).Replace("\"", "");
                }
                else if (!command.IsCommandLike(index, "$end"))
                {
                    Console.WriteLine("b");
                    return 0;
                }
            }
            return 1;
        }
    }
}
