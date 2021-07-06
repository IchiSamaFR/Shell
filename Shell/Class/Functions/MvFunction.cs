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
    public static class MvFunction
    {
        public static Command command;
        public static ShellConfig shellConfig;

        static string pathSource;
        static string pathDest;
        static int index;

        public static int Mv()
        {
            command = Main.Command;
            shellConfig = Main.shellConfig;
            pathSource = "";
            pathDest = "";
            index = 1;
            
            if (command.IsCommandLike(index, "$value $end"))
            {
                pathSource = DirectoryTool.SetPath(command.GetBaseValue(index).Replace("\"", ""));
            }
            else
            {
                Console.WriteLine("Fichier ou dossier non reconnu.");
                return 0;
            }
            if (!DirectoryTool.IsValidPath(pathDest))
            {
                return 0;
            }

            if (File.Exists(pathSource))
            {
                if (Directory.Exists(pathDest))
                {
                    File.Move(pathSource, pathDest + "/" + Path.GetFileName(pathSource));
                }
                else
                {
                    File.Move(pathSource, pathDest);
                }
            }
            else if (Directory.Exists(pathSource))
            {
                Directory.Move(pathSource, pathDest);
            }
            else
            {
                Console.WriteLine("Fichier ou dossier introuvable.");
                return 0;
            }

            return 1;
        }

        static int IsMoving()
        {
            string val = command.GetBaseValue(index).Replace("\"", "");
            string val2 = command.GetBaseValue(index + 1).Replace("\"", "");
            string val3 = command.GetBaseValue(index + 2).Replace("\"", "");
            if (val == "" || val2 == "" || val3 != "")
            {
                Console.WriteLine("Fichier ou dossier non reconnu.");
                return 2;
            }

            pathSource = DirectoryTool.SetPath(val);
            pathDest = DirectoryTool.SetPath(val2);

            return 1;
        }
    }
}
