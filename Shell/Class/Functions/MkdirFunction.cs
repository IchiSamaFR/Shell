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
    public static class MkdirFunction
    {
        public static Command command;
        public static ShellConfig shellConfig;

        static string pathSource;
        static int index;

        public static int Mkdir()
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
            if (!DirectoryTool.IsValidPath(pathSource))
            {
                return 0;
            }

            if (!Directory.Exists(pathSource) && !File.Exists(pathSource))
            {
                Directory.CreateDirectory(pathSource);
            }
            else
            {
                Console.WriteLine("Fichier ou Dossier déjà existant avec ce nom.");
                return 0;
            }
            return 1;
        }
    }
}
