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
    public static class TouchFunction
    {
        public static Command command;
        public static ShellConfig shellConfig;

        static string pathSource;
        static int index;

        public static int Touch()
        {
            command = Main.Command;
            shellConfig = Main.shellConfig;
            pathSource = "";
            index = 1;

            int res;
            if((res = IsPath()) == 2 || res == 0)
            {
                return 0;
            }
            
            if (!File.Exists(pathSource))
            {
                File.Create(pathSource).Dispose();
            }
            else
            {
                Console.WriteLine("Fichier déjà existant.");
                return 0;
            }
            return 1;
        }

        public static int IsPath()
        {
            string val = command.GetBaseValue(index).Replace("\"", "");
            string val2 = command.GetBaseValue(index + 1);
            if (val == "" && val2 != "")
            {
                Console.WriteLine("Fichier non reconnu.");
                return 2;
            }

            pathSource = DirectoryTool.SetPath(val);

            return 1;
        }

    }
}
