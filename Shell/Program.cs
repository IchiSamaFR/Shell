using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Shell.Class;
using System.Security.AccessControl;

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
            else if (args[0] == "cd")
            {
                CurrentDir(command);
            }
            else if (args[0] == "exit")
            {
                Environment.Exit(0);
            }
            else if (args[0] == "clear")
            {
                Console.Clear();
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
            List<string> speArgs = GetArgs(args);
            List<string> valArgs = GetValues(args);
            bool list = false;

            string path = shellConfig.actualDir;
            if (argsStr.Length > 1)
            {
                path = argsStr[1];
            }
            else if(valArgs.Count > 1)
            {
                path = "";
                int i = 0;
                foreach (var item in valArgs)
                {
                    if(i > 0)
                    {
                        path += item + " ";
                    }
                    i++;
                }
            }

            foreach (var item in speArgs)
            {
                if(item.ToLower() == "-l" || item.ToLower() == "-list")
                {
                    list = true;
                }
            }

            if (argsStr.Length >= 2 && argsStr.Length != 3)
            {
                Console.WriteLine("Chemin d'accès non reconnu.");
                return;
            }

            if (path.Replace(" ", "") == "") path = shellConfig.actualDir;

            if (Directory.Exists(shellConfig.actualDir + "\\" + path))
            {
                path = shellConfig.actualDir + "\\" + path;
            }
            if (!Directory.Exists(path))
            {
                Console.WriteLine("Chemin d'accès non reconnu.");
                return;
            }
            if (!CanRead(path))
            {
                Console.WriteLine("Vous n'avez pas l'autorisation de lire ce dossier.");
                return;
            }

            if (list)
            {
                Console.WriteLine(AddBlankLeft(" ", 10) +
                                 "  " + AddBlankRight(" ", 20) +
                                 " " + "..");
                foreach (var item in Directory.GetDirectories(path))
                {
                    DirectoryInfo _dir = new DirectoryInfo(item);
                    if (CanRead(_dir.FullName))
                    {
                        Console.WriteLine(_dir.CreationTime.ToString("dd/mm/yyyy") +
                                         "  " + AddBlankRight("<DIR>", 20) +
                                         "  " + _dir.Name + "\\");
                    }
                }
                foreach (var item in Directory.GetFiles(path))
                {
                    FileInfo _file = new FileInfo(item);
                    IFormatProvider frenchFormatProvider =
                        new System.Globalization.CultureInfo("fr-FR");
                    if (CanRead(_file.FullName))
                    {
                        Console.WriteLine(_file.CreationTime.ToString("dd/mm/yyyy") +
                                         "  " + AddBlankLeft(_file.Length.ToString("#,##0", frenchFormatProvider), 20) +
                                         "  " + _file.Name + "");
                    }
                }
            }
            else
            {
                int _x = 0;
                int _y = 0;
                int col = 4;
                int amount = Directory.GetDirectories(path).Length + Directory.GetFiles(path).Length + 1;
                string[,] colValues = new string[col, (int)Math.Ceiling((float)amount / (float)col)];
                int[] charCol = new int[col];

                colValues[_x, _y] = "..";
                _x++;
                foreach (var item in Directory.GetDirectories(path))
                {
                    DirectoryInfo _dir = new DirectoryInfo(item);
                    if (CanRead(_dir.FullName))
                    {
                        colValues[_x, _y] = _dir.Name + "\\";
                        if (charCol[_x] < (_dir.Name + "\\").Length)
                        {
                            charCol[_x] = _dir.Name.Length;
                        }

                        _x++;
                        if (_x == col)
                        {
                            _y++;
                            _x = 0;
                        }
                    }
                }
                foreach (var item in Directory.GetFiles(path))
                {
                    FileInfo _file = new FileInfo(item);
                    if (CanRead(_file.FullName))
                    {
                        colValues[_x, _y] = _file.Name;
                        if (charCol[_x] < _file.Name.Length)
                        {
                            charCol[_x] = _file.Name.Length;
                        }

                        _x++;
                        if (_x == col)
                        {
                            _y++;
                            _x = 0;
                        }
                    }
                }


                for (int y = 0; y < colValues.GetLength(1); y++)
                {
                    for (int x = 0; x < col; x++)
                    {
                        Console.Write(AddBlankRight(colValues[x, y], charCol[x]) + " ");
                    }
                    Console.WriteLine("");
                }
            }
        }
        static void CurrentDir(string command)
        {
            string[] argsStr = command.Split('\"');
            string[] args = command.Split(' ');

            string pathToGo = shellConfig.actualDir;
            string path = "";
            if (argsStr.Length > 1)
            {
                path = argsStr[1];
            }
            else if (args.Length > 1)
            {
                path = "";
                int i = 0;
                foreach (var item in args)
                {
                    if (i > 0)
                    {
                        path += item + " ";
                    }
                    i++;
                }
            }
            if(path.Replace(" ", "").Length == 0)
            {
                return;
            }

            string[] directories = path.Replace("/", "\\").Split('\\');
            if (path[0] == '\\')
            {
                pathToGo = "";
            }

            foreach (var item in directories)
            {
                if (item.Replace(" ", "") == "..")
                {
                    pathToGo = RemoveLastPathDir(pathToGo);
                }
                else if (item.Replace(" ", "") != ".")
                {
                    pathToGo += "\\" + item;
                }
            }
            if(directories[directories.Length - 1].Replace(" ", "") == "")
            {
                pathToGo = pathToGo.Substring(0, pathToGo.LastIndexOf("\\"));
            }
            if(pathToGo[pathToGo.Length - 1] == ' ')
            {
                pathToGo = pathToGo.Substring(0, pathToGo.Length - 1);
            }

            if (Directory.Exists(pathToGo) && CanRead(pathToGo))
            {
                shellConfig.actualDir = ClearBlank(pathToGo);
            }
            else if (Directory.Exists(path) && CanRead(path))
            {
                path = ClearBlank(path).Replace('/', '\\');
                string nPath = "";
                foreach (var item in path.Split('\\'))
                {
                    if (item.Replace(" ", "") != "") nPath += item + "\\";
                }

                shellConfig.actualDir = nPath;
            }
            else
            {
                Console.WriteLine("Chemin d'accès non reconnu.");
            }
        }
        static string RemoveLastPathDir(string path)
        {
            int x = path.LastIndexOf('\\');
            if(x == path.Length)
            {
                path.Substring(0, x - 1);
                x = path.LastIndexOf('\\');
            }

            if(x == 0 || x == -1)
            {
                return path;
            }
            return path.Substring(0, x);
        }

        static string AddSpaceThousand(string item)
        {
            string[] _item = item.Split('.');



            return _item.Length == 1 ? _item[0] : _item[0] + "." + _item[1];
        }
        static string AddBlankRight(string item, int length)
        {
            if (item == null) return item;

            for (int i = item.Length - 1; i < length; i++)
            {
                item += " ";
            }
            return item;
        }
        static string AddBlankLeft(string item, int length)
        {
            if (item == null) return item;

            for (int i = item.Length - 1; i < length; i++)
            {
                item = " " + item;
            }
            return item;
        }
        static string ClearBlank(string item)
        {
            while (true)
            {
                int x = item.LastIndexOf(" ");
                if (x == item.Length - 1)
                {
                    item = item.Substring(0, x - 1);
                }
                else
                {
                    break;
                }
            }
            return item;
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
        public static bool CanRead(string path)
        {
            try
            {

                DirectorySecurity ds = Directory.GetAccessControl(path);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }
    }
}
