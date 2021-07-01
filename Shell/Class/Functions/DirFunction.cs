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
    public class DirFunction
    {
        public static Command command;
        public static ShellConfig shellConfig;

        static string path;
        static int index;

        public static int CurrentDir()
        {
            command = Main.Command;
            shellConfig = Main.shellConfig;
            index = 1;

            string pathToGo = shellConfig.actualDir;
            path = "";

            if (command.IsCommandLike(index, "$value $end"))
            {
                path = DirectoryTool.SetPath(command.GetBaseValue(index).Replace("\"", ""));
            }
            else if (!command.IsCommandLike(index, "$end"))
            {
                return 0;
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
