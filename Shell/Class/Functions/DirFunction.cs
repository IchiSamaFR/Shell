using Shell.Class.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell.Class.Functions
{
    public class DirFunction
    {
        public static Command command;
        public static ShellConfig shellConfig;

        public static int CurrentDir()
        {
            command = Main.Command;
            shellConfig = Main.shellConfig;

            string pathToGo = shellConfig.actualDir;
            string path = "";

            int x = 0;
            foreach (var item in command.baseValues)
            {
                if (x > 0)
                    path += item + " ";
                x++;
            }

            path = path.Replace("\"", "").Replace("/", "\\");
            if(path == "")
            {
                Console.WriteLine(pathToGo);
            }
            else if (Path.IsPathRooted(path) && Directory.Exists(path))
            {
                shellConfig.actualDir = Path.GetFullPath(path);
            }
            else if (Directory.Exists(pathToGo + "\\" + path))
            {
                shellConfig.actualDir = Path.GetFullPath(pathToGo + "\\" + path);
            }
            else
            {
                Console.WriteLine(path);
                Console.WriteLine("Chemin d'accès non reconnu.");
                return 0;
            }

            return 1;
        }

    }
}
