using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Security.AccessControl;

using Shell.Class;
using Shell.Tools;

namespace Shell
{
    class Program
    {
        static ShellConfig shellConfig;
        static SQLConfig sqlConfig;
        static List<string> Command;
        static bool Logged = false;
        static Dictionary<string, ConsoleColor> colors = new Dictionary<string, ConsoleColor>();

        static Dictionary<string, Func<int>> functions = new Dictionary<string, Func<int>>();

        static void Main(string[] args)
        {
            Init();
            Loop();
        }

        static void Init()
        {
            shellConfig = new ShellConfig(Environment.CurrentDirectory, ConsoleColor.Green, ConsoleColor.White);
            sqlConfig = new SQLConfig();

            colors.Add("red", ConsoleColor.Red);
            colors.Add("blue", ConsoleColor.Blue);
            colors.Add("green", ConsoleColor.Green);
            colors.Add("darkred", ConsoleColor.DarkRed);
            colors.Add("darkblue", ConsoleColor.DarkBlue);
            colors.Add("darkgreen", ConsoleColor.DarkGreen);
            colors.Add("cyan", ConsoleColor.Cyan);
            colors.Add("darkcyan", ConsoleColor.DarkCyan);
            colors.Add("white", ConsoleColor.White);
            colors.Add("black", ConsoleColor.Black);

            functions.Add("fcolor", new Func<int>(ChangeForeColor));
            functions.Add("colors", new Func<int>(ShowColors));
            functions.Add("color", new Func<int>(ShowColors));
            functions.Add("echo", new Func<int>(Echo));
            functions.Add("ls", new Func<int>(LsFolders));
            functions.Add("cd", new Func<int>(CurrentDir));
            functions.Add("exit", new Func<int>(Exit));
            functions.Add("clear", new Func<int>(Clear));
            functions.Add("help", new Func<int>(Help));
            functions.Add("sql", new Func<int>(SQL));
        }

        static void Loop()
        {
            while (true)
            {
                Console.ForegroundColor = shellConfig.pathColor;
                Console.WriteLine(shellConfig.actualDir + ">");
                if(Logged)
                    Console.Write("logged~$ ");
                else
                    Console.Write("~$ ");

                Console.ForegroundColor = shellConfig.textColor;
                CallCommand(Console.ReadLine());
            }
        }

        static void CallCommand(string command)
        {
            Command = TextTool.CommandToArgs(command);
            bool find = false;

            foreach (var item in functions)
            {
                if(item.Key == Command[0])
                {
                    item.Value.Invoke();
                    find = true;
                    break;
                }
            }
            if (command.Replace(" ", "") != "" && !find)
            {
                Console.WriteLine("Commande non reconnu.");
            }
            Console.WriteLine("");
        }

        static int ChangeForeColor()
        {
            bool find = false;
            if (Command.Count <= 1)
            {
                Console.WriteLine("'fcolor' a beoins d'une valeur pour fonctionner.");
                Console.WriteLine("Exemple : 'fcolor red'.");
                return 1;
            }
            else if (Command.Count > 2)
            {
                Console.WriteLine("'fcolor' a beoins d'une seule valeur pour fonctionner.");
                Console.WriteLine("Exemple : 'fcolor red'.");
                return 1;
            }

            foreach (var item in colors)
            {
                if (item.Key == Command[1])
                {
                    shellConfig.textColor = item.Value;
                    find = true;
                    break;
                }
            }
            if (!find)
            {
                Console.WriteLine("Couleur non trouvé.");
                return 1;
            }
            return 0;
        }
        static int ShowColors()
        {
            foreach (var item in colors)
            {
                Console.WriteLine(item.Key);
            }
            return 0;
        }
        static int Echo()
        {
            int x = 0;
            string ret = "";
            foreach (var item in Command)
            {
                if (x > 0)
                {
                    ret += item + " ";
                }
                x++;
            }
            Console.WriteLine(ret);
            return 0;
        }
        static int LsFolders()
        {
            List<string> speArgs = TextTool.GetArgs(Command);
            List<string> valArgs = TextTool.GetValues(Command);
            bool list = false;

            string path = shellConfig.actualDir;
            if (speArgs.Count > 1)
            {
                path = Command[1];
            }
            else if(valArgs.Count > 1)
            {
                path = "";
                int i = 0;
                foreach (var item in valArgs)
                {
                    if(i > 0)
                    {
                        path += item + "";
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

            if (speArgs.Count >= 2 && speArgs.Count != 3)
            {
                Console.WriteLine("Chemin d'accès non reconnu.");
                return 1;
            }

            if (path.Replace(" ", "") == "") path = shellConfig.actualDir;

            if (Directory.Exists(shellConfig.actualDir + "\\" + path))
            {
                path = shellConfig.actualDir + "\\" + path;
            }
            if (!Directory.Exists(path) && !File.Exists(path))
            {
                Console.WriteLine("Chemin d'accès non reconnu.");
                return 1;
            }
            if (!CanRead(path))
            {
                Console.WriteLine("Vous n'avez pas l'autorisation de lire ce dossier.");
                return 1;
            }

            if (list)
            {
                Console.Write(AddBlankLeft(" ", 9) +
                              "  " + AddBlankRight("<DIR>", 20));
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("  " + "..");
                Console.ForegroundColor = shellConfig.textColor;
                Console.WriteLine("");

                foreach (var item in Directory.GetDirectories(path))
                {
                    if (CanRead(item))
                    {
                        DirectoryInfo _dir = new DirectoryInfo(item);
                        Console.Write(_dir.CreationTime.ToString("dd/mm/yyyy") +
                                         "  " + AddBlankRight("<DIR>", 20));

                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("  " + _dir.Name + "\\");
                        Console.ForegroundColor = shellConfig.textColor;
                        Console.WriteLine("");
                    }
                }
                foreach (var item in Directory.GetFiles(path))
                {
                    if (CanRead(item))
                    {
                        FileInfo _file = new FileInfo(item);
                        IFormatProvider frenchFormatProvider =
                            new System.Globalization.CultureInfo("fr-FR");
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
                int _countDir = 0;
                int col = 4;
                int amount = Directory.GetDirectories(path).Length + Directory.GetFiles(path).Length + 1;
                string[,] colValues = new string[col, (int)Math.Ceiling((float)amount / (float)col)];
                int[] charCol = new int[col];

                colValues[_x, _y] = "..";
                _countDir++;
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
                        _countDir++;
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
                        if(_countDir > 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            _countDir--;
                        }
                        else
                        {
                            Console.ForegroundColor = shellConfig.textColor;
                        }
                        Console.Write(AddBlankRight(colValues[x, y], charCol[x]) + " ");
                    }
                    Console.WriteLine("");
                }
            }
            return 0;
        }
        static int CurrentDir()
        {
            string pathToGo = shellConfig.actualDir;
            string path = "";
            if (Command.Count == 2)
            {
                path = Command[1];
            }
            else
            {
                path = "";
                int i = 0;
                foreach (var item in Command)
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
                return 1;
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
            return 0;
        }
        static int Exit()
        {
            Environment.Exit(0);
            return 0;
        }
        static int Clear()
        {
            Console.Clear();
            return 0;
        }
        static int Help()
        {
            foreach (var item in functions)
            {
                if(item.Key != "help")
                {
                    Console.WriteLine(TextTool.StringToWidth(">" + item.Key, 10));
                }
            }
            return 0;
        }
        static int SQL()
        {
            foreach (var item in Command)
            {
                if(item == "-add")
                {

                    break;
                }
            }


            return 0;
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
