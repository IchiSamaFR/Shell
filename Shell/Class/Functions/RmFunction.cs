using Shell.Class.Config;
using Shell.Class.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell.Class.Functions
{
    public static class RmFunction
    {
        public static Command command;
        public static ShellConfig shellConfig;

        static string pathSource;
        static int index;
        static bool onlyEmpty;

        public static int Rm()
        {
            command = Main.Command;
            shellConfig = Main.shellConfig;
            pathSource = "";
            index = 1;
            onlyEmpty = false;

            if (command.IsCommandLike(index, "-r $value $end"))
            {
                onlyEmpty = true;
                pathSource = DirectoryTool.SetPath(command.GetBaseValue(index + 1).Replace("\"", ""));
            }
            else if (command.IsCommandLike(index, "$value $end"))
            {
                pathSource = DirectoryTool.SetPath(command.GetBaseValue(index).Replace("\"", ""));
            }
            else
            {
                Console.WriteLine("Fichier ou dossier non reconnu.");
                return 0;
            }

            if (File.Exists(pathSource) && !onlyEmpty)
            {
                File.Delete(pathSource);
            }
            else if (Directory.Exists(pathSource))
            {
                if (onlyEmpty)
                {
                    try
                    {
                        Directory.Delete(pathSource, false);
                    }
                    catch
                    {
                        Console.WriteLine("Impossible de supprimer le dossier.");
                        return 0;
                    }
                }
                else
                {
                    Directory.Delete(pathSource, true);
                }
            }
            else
            {
                if (onlyEmpty)
                {
                    Console.WriteLine("Dossier introuvable.");
                }
                else
                {
                    Console.WriteLine("Fichier ou dossier introuvable.");
                }
                return 0;
            }

            return 1;
        }

        public static int RmDir()
        {
            command = Main.Command;
            shellConfig = Main.shellConfig;
            pathSource = "";
            index = 1;

            if (command.IsCommandLike(index, "$value $end"))
            {
                pathSource = DirectoryTool.SetPath(command.GetBaseValue(index).Replace("\"", ""));
            }
            else
            {
                Console.WriteLine("Dossier non reconnu.");
                return 0;
            }

            if (Directory.Exists(pathSource))
            {
                try
                {
                    Directory.Delete(pathSource, false);
                }
                catch
                {
                    Console.WriteLine("Impossible de supprimer le dossier.");
                    return 0;
                }
            }
            else
            {
                Console.WriteLine("Dossier introuvable.");
                return 0;
            }

            return 1;
        }

    }
}
