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
        static string pathDest;
        static string creatDateV;
        static string modifDateV;
        static int index; 

        public static int Cat()
        {
            command = Main.Command;
            pathSource = "";
            pathDest = "";
            creatDateV = "";
            modifDateV = "";
            index = 1;


            if (command.values.Count > 0)
            {
                int res = 0;
                if ((res = IsCopy()) == 1)
                {
                    if ((res = IsModifDate()) == 0)
                    {
                        res = IsCreateDate();
                        if (res == 2) return 0;
                    }
                    else if (res != 2 && (res = IsCreateDate()) == 0)
                    {
                        res = IsModifDate();
                        if (res == 2) return 0;
                    }
                    else if (command.GetBaseValue(index) != "")
                    {
                        if(res != 2)
                        {
                            Console.WriteLine("Arguments non reconnu.");
                        }
                        return 0;
                    }
                }
                else if (res != 2 && (res = IsModifDate()) == 1)
                {
                    res = IsCreateDate();
                    if (res == 2) return 0;
                }
                else if (res != 2 && (res = IsCreateDate()) == 1)
                {
                    res = IsModifDate();
                    if (res == 2) return 0;
                }
                else if (res != 2 && (res = IsWritting()) == 1)
                {
                    return 1;
                }
                else if (res != 2 && (res = IsShow()) == 1)
                {
                    return 1;
                }
                else
                {
                    if (res != 2)
                    {
                        Console.WriteLine("Valeurs non reconnu.");
                    }
                    return 0;
                }
            }
            else
            {
                Console.WriteLine("Valeurs non reconnu.");
                return 0;
            }


            if (File.Exists(pathSource))
            {
                if (pathSource != "" && pathDest != "")
                {
                    File.Copy(pathSource, pathDest, true);
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
        static string SetModifDate(string path)
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
        static int IsCopy()
        {
            string val = command.GetBaseValue(index).Replace("\"", "");
            string val2 = command.GetBaseValue(index + 2).Replace("\"", "");
            if (val != "" && command.GetBaseValue(index + 1) == ">")
            {
                if (val2 != "")
                {
                    pathSource = val;
                    pathDest = val2;
                    index += 3;
                    return 1;
                }
                else
                {
                    Console.WriteLine("Chemin de copie non reconnu.");
                    return 2;
                }
            }
            else
            {
                return 0;
            }
        }
        static int IsModifDate()
        {
            if (index != 1 && (command.GetBaseValue(index) == "-md" || command.GetBaseValue(index) == "-modifdate"))
            {
                string val = command.GetBaseValue(index + 1).Replace("\"", "");
                if (TextTool.IsDateTime(val))
                {
                    modifDateV = val;
                    index += 2;
                    return 1;
                }
                else
                {
                    Console.WriteLine("Date non reconnu.");
                    return 2;
                }
            }
            else if (command.GetBaseValue(index) != "" && (command.GetBaseValue(index + 1) == "-md" || command.GetBaseValue(index + 1) == "-modifdate"))
            {
                string val = command.GetBaseValue(index + 2).Replace("\"", "");
                if (TextTool.IsDateTime(val))
                {
                    pathSource = SetSource(command.GetBaseValue(index).Replace("\"", ""));
                    modifDateV = val;
                    index += 3;
                    return 1;
                }
                else
                {
                    Console.WriteLine("Date non reconnu.");
                    return 2;
                }
            }
            else
            {
                return 0;
            }
        }
        static int IsCreateDate()
        {
            if (index != 1 && (command.GetBaseValue(index) == "-cd" || command.GetBaseValue(index) == "-createdate"))
            {
                string val = command.GetBaseValue(index + 1).Replace("\"", "");
                if (TextTool.IsDateTime(val))
                {
                    creatDateV = val;
                    index += 2;
                    return 1;
                }
                else
                {
                    Console.WriteLine("Date non reconnu.");
                    return 2;
                }
            }
            else if (command.GetBaseValue(index) != "" && (command.GetBaseValue(index + 1) == "-cd" || command.GetBaseValue(index + 1) == "-createdate"))
            {
                string val = command.GetBaseValue(index + 2).Replace("\"", "");
                if (TextTool.IsDateTime(val))
                {
                    pathSource = SetSource(command.GetBaseValue(index).Replace("\"", ""));
                    creatDateV = val;
                    index += 3;
                    return 1;
                }
                else
                {
                    Console.WriteLine("Date non reconnu.");
                    return 2;
                }
            }
            else
            {
                return 0;
            }
        }
        static int IsShow()
        {
            if (command.GetBaseValue(index) != "")
            {
                string val = command.GetBaseValue(index).Replace("\"", "");
                if (command.GetBaseValue(index + 1) == "" && File.Exists(val))
                {
                    foreach (var item in File.ReadAllLines(val))
                    {
                        Console.WriteLine(item);
                    }
                    index += 1;
                    return 1;
                }
                else
                {
                    Console.WriteLine("Fichier non reconnu.");
                    return 2;
                }
            }
            else
            {
                return 0;
            }
        }
        static int IsWritting()
        {
            string name = command.GetBaseValue(index + 1);
            if (command.GetBaseValue(index) != ">" || name == "" || command.GetBaseValue(index + 2) != "")
            {
                return 0;
            }
            
            string line = "";
            while (TextTool.CancelableReadLine(out line))
            {
                File.AppendAllText(name, Environment.NewLine + line);
            }

            return 1;
        }
    }
}
