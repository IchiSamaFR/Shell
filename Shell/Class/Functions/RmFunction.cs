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

        public static int Rm()
        {
            command = Main.Command;
            shellConfig = Main.shellConfig;
            pathSource = "";
            index = 1;

            if(command.IsCommandLike(index, "$value $end"))
            {
                pathSource = DirectoryTool.SetPath(command.GetBaseValue(index).Replace("\"", ""));
            }
            else
            {
                Console.WriteLine("Fichier ou dossier non reconnu.");
                return 0;
            }

            if (File.Exists(pathSource))
            {
                File.Delete(pathSource);
            }
            else if (Directory.Exists(pathSource))
            {
                Directory.Delete(pathSource, true);
            }
            else
            {
                Console.WriteLine("Fichier ou dossier introuvable.");
                return 0;
            }

            return 1;
        }

    }
}
