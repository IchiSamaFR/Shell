using Shell.Class.Config;
using Shell.Class.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell.Class.Functions
{
    public static class ExecFunction
    {
        public static Command command;
        public static ShellConfig shellConfig;

        static string pathSource;
        static string arguments;
        static int index;

        public static int Exec()
        {
            command = Main.Command;
            shellConfig = Main.shellConfig;
            pathSource = "";
            index = 1;
            
            if (command.IsCommandLike(index, "$value $value $end"))
            {
                pathSource = DirectoryTool.SetPath(command.GetBaseValue(index).Replace("\"", ""));
                arguments = DirectoryTool.SetPath(command.GetBaseValue(index + 1).Replace("\"", ""));
            }
            else if (command.IsCommandLike(index, "$value $end"))
            {
                pathSource = DirectoryTool.SetPath(command.GetBaseValue(index).Replace("\"", ""));
            }
            else
            {
                return 0;
            }

            Process proc;
            if (File.Exists(pathSource))
            {
                proc = Process.Start(pathSource);
            }
            else if (Directory.Exists(pathSource))
            {
                proc = Process.Start(pathSource);
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
