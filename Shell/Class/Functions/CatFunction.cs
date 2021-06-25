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
            string pathToGo = Main.shellConfig.actualDir;
            pathSource = "";
            pathDest = "";
            creatDateV = "";
            modifDateV = "";
            index = 1;


            if (command.values.Count > 0)
            {
                int res = 0;
                if ((res = IsCopy()) == 0)
                {
                    if ((res = IsModifDate()) == 0)
                    {
                        res = IsCreateDate();
                        if (res == 2) return 1;
                    }
                    else if (res != 2 && (res = IsCreateDate()) == 0)
                    {
                        res = IsModifDate();
                        if (res == 2) return 1;
                    }
                    else if (command.GetBaseValue(index) != "")
                    {
                        if(res != 2)
                        {
                            Console.WriteLine("Arguments non reconnu.");
                        }
                        return 1;
                    }
                }
                else if (res != 2 && (res = IsModifDate()) == 0)
                {
                    res = IsCreateDate();
                    if (res == 2) return 1;
                }
                else if (res != 2 && (res = IsCreateDate()) == 0)
                {
                    res = IsModifDate();
                    if (res == 2) return 1;
                }
                else if (res != 2 && (res = IsShow()) == 0)
                {
                    return 0;
                }
                else
                {
                    if (res != 2)
                    {
                        Console.WriteLine("Valeurs non reconnu.");
                    }
                    return 1;
                }
            }
            else
            {
                Console.WriteLine("Valeurs non reconnu.");
                return 1;
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
                return 1;
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

            return 0;
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
            if (command.GetBaseValue(index) != "" && command.GetBaseValue(index + 1) == ">")
            {
                if (command.GetBaseValue(index + 2) != "")
                {
                    pathSource = command.GetBaseValue(index);
                    pathDest = command.GetBaseValue(index + 2);
                    index += 3;
                    return 0;
                }
                else
                {
                    Console.WriteLine("Chemin de copie non reconnu.");
                    return 2;
                }
            }
            else
            {
                return 1;
            }
        }
        static int IsModifDate()
        {
            if (index != 1 && (command.GetBaseValue(index) == "-md" || command.GetBaseValue(index) == "-modifdate"))
            {
                if (TextTool.IsDateTime(command.GetBaseValue(index + 1)))
                {
                    modifDateV = command.GetBaseValue(index + 1);
                    index += 2;
                    return 0;
                }
                else
                {
                    Console.WriteLine("Date non reconnu.");
                    return 2;
                }
            }
            else if (command.GetBaseValue(index) != "" && (command.GetBaseValue(index + 1) == "-md" || command.GetBaseValue(index + 1) == "-modifdate"))
            {
                Console.WriteLine(command.GetBaseValue(index + 2));
                if (TextTool.IsDateTime(command.GetBaseValue(index + 2)))
                {
                    pathSource = SetSource(command.GetBaseValue(index));
                    modifDateV = command.GetBaseValue(index + 2);
                    index += 3;
                    return 0;
                }
                else
                {
                    Console.WriteLine("Date non reconnu.");
                    return 2;
                }
            }
            else
            {
                return 1;
            }
        }
        static int IsCreateDate()
        {
            if (index != 1 && (command.GetBaseValue(index) == "-cd" || command.GetBaseValue(index) == "-createdate"))
            {
                if (TextTool.IsDateTime(command.GetBaseValue(index + 1)))
                {
                    creatDateV = command.GetBaseValue(index + 1);
                    index += 2;
                    Console.WriteLine(creatDateV);
                    return 0;
                }
                else
                {
                    Console.WriteLine("Date non reconnu.");
                    return 2;
                }
            }
            else if (command.GetBaseValue(index) != "" && (command.GetBaseValue(index + 1) == "-cd" || command.GetBaseValue(index + 1) == "-createdate"))
            {
                if (TextTool.IsDateTime(command.GetBaseValue(index + 2)))
                {
                    pathSource = SetSource(command.GetBaseValue(index));
                    creatDateV = command.GetBaseValue(index + 2);
                    index += 3;
                    return 0;
                }
                else
                {
                    Console.WriteLine("Date non reconnu.");
                    return 2;
                }
            }
            else
            {
                return 1;
            }
        }
        static int IsShow()
        {
            if (command.GetBaseValue(index) != "")
            {
                if (command.GetBaseValue(index + 1) == "" && File.Exists(command.GetBaseValue(index)))
                {
                    foreach (var item in File.ReadAllLines(command.GetBaseValue(index)))
                    {
                        Console.WriteLine(item);
                    }
                    index += 1;
                    return 0;
                }
                else
                {
                    Console.WriteLine("Fichier non reconnu.");
                    return 2;
                }
            }
            else
            {
                return 1;
            }
        }
    }
}
