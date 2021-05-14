using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Shell.Class;


namespace Shell
{
    class Program
    {
        static ShellConfig shellConfig;
        static Dictionary<string, ConsoleColor> colors = new Dictionary<string, ConsoleColor>();

        static void Main(string[] args)
        {
            Init();
            Loop();
        }

        static void Init()
        {
            shellConfig = new ShellConfig(Environment.CurrentDirectory, ConsoleColor.DarkGreen, ConsoleColor.White);
            colors.Add("red", ConsoleColor.Red);
            colors.Add("blue", ConsoleColor.Blue);
            colors.Add("green", ConsoleColor.Green);
            colors.Add("darkred", ConsoleColor.DarkRed);
            colors.Add("darkblue", ConsoleColor.DarkBlue);
            colors.Add("darkgreen", ConsoleColor.DarkGreen);
            colors.Add("white", ConsoleColor.White);
            colors.Add("black", ConsoleColor.Black);
        }

        static void Loop()
        {
            while (true)
            {
                Console.ForegroundColor = shellConfig.pathColor;
                Console.WriteLine(shellConfig.actualDir + ">");
                Console.Write("~$ ");

                Console.ForegroundColor = shellConfig.textColor;
                ReturnCommandRes(Console.ReadLine());
            }
        }

        static void ReturnCommandRes(string command)
        {
            string[] args = command.Split(' ');

            if (args[0] == "fcolor")
            {
                ChangeForeColor(args);
            }
            else if (args[0] == "colors" || args[0] == "color")
            {
                ShowColors();
            }
            else if (args[0] == "echo")
            {
                Echo(command);
            }
            else if (args[0] == "ls")
            {
                LsFolders(command);
            }
            else if(command.Replace(" ", "") != "")
            {
                Console.WriteLine("Commande non reconnu.");
            }
        }


        static void ChangeForeColor(string[] args)
        {
            bool find = false;
            List<string> _args = GetArgs(args);
            List<string> _values = GetValues(args);

            if (_values.Count <= 1)
            {
                Console.WriteLine("'fcolor' a beoins d'une valeur pour fonctionner.");
                Console.WriteLine("Exemple : 'fcolor red'.");
                return;
            }
            else if (_values.Count > 2)
            {
                Console.WriteLine("'fcolor' a beoins d'une seule valeur pour fonctionner.");
                Console.WriteLine("Exemple : 'fcolor red'.");
                return;
            }
            else if (_args.Count > 0)
            {
                Console.WriteLine("Arguments non reconnus.");
                return;
            }
            
            foreach (var item in colors)
            {
                if (item.Key == _values[1])
                {
                    shellConfig.textColor = item.Value;
                    find = true;
                    break;
                }
            }
            if (!find)
            {
                Console.WriteLine("Couleur non trouvé.");
            }
        }
        static void ShowColors()
        {
            foreach (var item in colors)
            {
                Console.WriteLine(item.Key);
            }
        }
        static void Echo(string command)
        {
            command = command.Replace("\\\"", "&quote").Replace("\\", "&slash");
            string[] argsStr = command.Split('\"');
            int i = 0;
            foreach (var item in argsStr)
            {
                argsStr[i] = item.Replace("&quote", "\"");
                argsStr[i] = argsStr[i].Replace("&slash", "\\");
                i++;
            }

            string[] args = command.Split(' ');
            i = 0;
            foreach (var item in args)
            {
                args[i] = item.Replace("&quote", "\"");
                args[i] = args[i].Replace("&slash", "\\");
                i++;
            }

            if (argsStr.Length > 1)
            {
                args = argsStr[0].Split(' ');
            }

            List<string> _args = GetArgs(args);
            List<string> _values = GetValues(args);

            if (_args.Count > 0)
            {
                Console.WriteLine("Arguments non reconnus.");
                return;
            }
            else if (argsStr.Length == 2)
            {
                Console.WriteLine("\"" + argsStr[1]);
                return;
            }
            else if (argsStr.Length == 3 && argsStr[2].Replace(" ", "") == "")
            {
                Console.WriteLine(argsStr[1]);
                return;
            }
            else if (argsStr.Length >= 3 && argsStr[2].Replace(" ", "") != "")
            {
                Console.WriteLine("Impossible de comprendre la commande, veuillez suivre ce format :");
                Console.WriteLine("'echo \"Mon texte\"");
                return;
            }
            else if (args.Length > 1)
            {
                int x = 0;
                string ret = "";
                foreach (var item in args)
                {
                    if(x > 0)
                    {
                        ret += item + " ";
                    }
                    x++;
                }
                Console.WriteLine(ret);
                return;
            }
        }
        static void LsFolders(string command)
        {
            string[] argsStr = command.Split('\"');
            string[] args = command.Split(' ');

            string path = shellConfig.actualDir;
            if (argsStr.Length > 1)
            {
                path = argsStr[1];
            }
            else if(args.Length > 1)
            {
                path = "";
                int i = 0;
                foreach (var item in args)
                {
                    if(i > 0)
                    {
                        path += item + " ";
                    }
                    i++;
                }
            }


            if (argsStr.Length >= 2 && argsStr.Length != 3)
            {
                Console.WriteLine("Chemin d'accès non recosnnu.");
                return;
            }

            if (!Directory.Exists(path))
            {
                Console.WriteLine(path);
                Console.WriteLine("Chemin d'accès non rseconnu.");
                return;
            }

            foreach (var item in Directory.GetFiles(path))
            {
                Console.WriteLine(item.Substring(path.Length));
            }
        }


        static List<string> GetArgs(string[] args)
        {
            List<string> _args = new List<string>();
            foreach (var item in args)
            {
                if(item.Length > 0 && item[0] == '-')
                {
                    _args.Add(item);
                }
            }

            return _args;
        }
        static List<string> GetValues(string[] args)
        {
            List<string> _args = new List<string>();
            foreach (var item in args)
            {
                if (item.Length > 0 && item[0] != '-')
                {
                    _args.Add(item);
                }
            }
            return _args;
        }
    }
}
