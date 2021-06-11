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
        static Command _Command;
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
            functions.Add("fg", new Func<int>(ChangeForeColor));
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
                if (Logged)
                {
                    Console.Write("logged");
                }
                Console.Write("~$ ");
                Console.ForegroundColor = shellConfig.textColor;

                CallCommand(Console.ReadLine());
            }
        }


        static void CallCommand(string command)
        {
            _Command = new Command(command);
            Command = TextTool.CommandToArgs(command);
            bool find = false;

            foreach (var item in functions)
            {
                if(item.Key == _Command.function)
                {
                    item.Value.Invoke();
                    find = true;
                    break;
                }
            }
            if (_Command.function.Replace(" ", "") != "" && !find)
            {
                Console.WriteLine("Commande non reconnu.");
            }
            Console.WriteLine("");
        }
        static int ChangeForeColor()
        {
            bool find = false;
            if (_Command.values.Count == 0)
            {
                Console.WriteLine("'fcolor' a besoin d'une valeur pour fonctionner.");
                Console.WriteLine("Exemple : 'fcolor red'.");
                return 1;
            }
            else if (_Command.values.Count > 1)
            {
                Console.WriteLine("'fcolor' a besoin d'une seule valeur pour fonctionner.");
                Console.WriteLine("Exemple : 'fcolor red'.");
                return 1;
            }

            foreach (var item in colors)
            {
                if (item.Key == _Command.values[0])
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
            foreach (var item in _Command.baseValues)
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
            bool list = false;

            string path = shellConfig.actualDir;

            if (_Command.values.Count > 0)
            {
                path = "";
                foreach (var item in _Command.values)
                {
                    path += item + " ";
                }
            }

            foreach (var item in _Command.arguments)
            {
                if(item.ToLower() == "-l" || item.ToLower() == "-list")
                {
                    list = true;
                }
                else
                {
                    Console.WriteLine("Argument non reconnu :");
                    Console.WriteLine("\"" + item + "\"");
                    return 1;
                }
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
                Console.Write(TextTool.AddBlankLeft(" ", 9) +
                              "  " + TextTool.AddBlankRight("<DIR>", 20));
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
                                         "  " + TextTool.AddBlankRight("<DIR>", 20));

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
                                         "  " + TextTool.AddBlankLeft(_file.Length.ToString("#,##0", frenchFormatProvider), 20) +
                                         "  " + _file.Name + "");
                    }
                }
            }
            else
            {
                int _x = 0;
                int _y = 0;
                int maxSize = 30;
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
                        colValues[_x, _y] = _dir.Name;
                        if (charCol[_x] < _dir.Name.Length)
                        {
                            if (_dir.Name.Length > maxSize && maxSize > 0)
                            {
                                charCol[_x] = maxSize;
                                colValues[_x, _y] = _dir.Name.Substring(0, maxSize);
                            }
                            else
                            {
                                charCol[_x] = _dir.Name.Length;
                            }
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
                            if(_file.Name.Length > maxSize && maxSize > 0)
                            {
                                charCol[_x] = maxSize;
                                colValues[_x, _y] = _file.Name.Substring(0, maxSize - 4) + "[..]";
                            }
                            else
                            {
                                charCol[_x] = _file.Name.Length;
                            }
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
                        Console.Write(TextTool.AddBlankRight(colValues[x, y], charCol[x]) + " ");
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
            
            int x = 0;
            foreach (var item in _Command.baseValues)
            {
                if(x > 0)
                    path += item + " ";
                x++;
            }

            path = path.Replace("\"", "").Replace("/","\\");
            string pathCombined = path[0] != '\\' ? pathToGo + '\\' + path : pathToGo + path;
            if (Directory.Exists(pathCombined))
            {
                shellConfig.actualDir = Path.GetFullPath(pathCombined);
            }
            else if(Directory.Exists(path))
            {
                shellConfig.actualDir = Path.GetFullPath(path);
            }
            else
            {
                Console.WriteLine(path);
                Console.WriteLine("Chemin d'accès non reconnu.");
                return 1;
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
            bool end = false;
            foreach (var item in _Command.arguments)
            {
                if (item == "-add")
                {
                    Console.WriteLine("Creation of the data connection.");
                    Console.Write("Host : ");
                    string hs = Console.ReadLine();
                    Console.Write("Database : ");
                    string db = Console.ReadLine();
                    Console.Write("Username : ");
                    string user = Console.ReadLine();
                    Console.Write("Password : ");
                    string pswd = Console.ReadLine();


                    sqlConfig.Init(hs, db, user, pswd);
                    end = true;
                }
                else if (item == "-load")
                {
                    sqlConfig.Load();
                    end = true;
                }
                else if (item == "-test")
                {
                    sqlConfig.TestConnection();
                    end = true;
                }
                else if (item == "-info")
                {
                    sqlConfig.Infos();
                    end = true;
                }
                else if (item == "-select")
                {
                    string request = " ";

                    while (request.Replace(" ", "").Length == 0 || (request[0] == '\"' && request[request.Length - 1] != '\"'))
                    {
                        request += " ";
                        Console.Write("> ");
                        request += Console.ReadLine();
                        request = request.Trim();
                    }
                    sqlConfig.Select(request);
                    end = true;
                }
                else
                {
                    Console.WriteLine("Argument non reconnu :");
                    Console.WriteLine("\"" + item + "\"");
                }
            }

            if (!end)
            {
                Console.WriteLine("'sql' a besoin d'un argument pour fonctionner.");
            }

            return 0;
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
