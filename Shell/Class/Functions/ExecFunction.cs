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

            int res = 0;
            if ((res = IsPath()) == 2 || res == 0)
            {
                return 0;
            }

            Process proc;
            if (File.Exists(pathSource))
            {
                proc = Process.Start(pathSource);
                proc.WaitForInputIdle();
                proc.WaitForExit(100);
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

        public static int IsPath()
        {
            string val = command.GetBaseValue(index).Replace("\"", "");
            string val2 = command.GetBaseValue(index + 1).Replace("\"", "");
            if (val == "")
            {
                Console.WriteLine("Fichier ou dossier non reconnu.");
                return 2;
            }

            pathSource = DirectoryTool.SetPath(val);
            arguments = DirectoryTool.SetPath(val);

            return 1;
        }
    }
}
