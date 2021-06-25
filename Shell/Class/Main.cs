using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Shell.Class.Config;
using Shell.Class.Functions;
using Shell.Class.Tools;
using System.Security.AccessControl;
using System.Globalization;

namespace Shell.Class
{
    public class Main
    {
        public static ShellConfig shellConfig;
        public static SQLConfig sqlConfig;
        public static Command Command;
        public static bool Logged = false;
        public static Dictionary<string, ConsoleColor> colors = new Dictionary<string, ConsoleColor>();
        public static Dictionary<string, Function> functions = new Dictionary<string, Function>();

        public Main()
        {
            Init();
            Update();
        }

        public void Init()
        {
            Console.CancelKeyPress += delegate {
                Clear();
                WaitCommand();
            };
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

            functions.Add("fcolor", new Function("fcolor", "interface", new Func<int>(ChangeForeColor)));
            functions.Add("fg", new Function("fcolor", "interface", new Func<int>(ChangeForeColor)));
            functions.Add("colors", new Function("colors", "interface", new Func<int>(ShowColors)));
            functions.Add("color", new Function("color", "interface", new Func<int>(ShowColors)));

            functions.Add("ls", new Function("ls", "directory", new Func<int>(LsFolders)));
            functions.Add("cd", new Function("cd", "directory", new Func<int>(CurrentDir)));
            functions.Add("find", new Function("find", "directory", new Func<int>(FindFunction.Find)));
            functions.Add("cat", new Function("cat", "directory", new Func<int>(CatFunction.Cat)));

            functions.Add("echo", new Function("echo", "tool", new Func<int>(Echo)));
            functions.Add("exit", new Function("exit", "tool", new Func<int>(Exit)));
            functions.Add("clear", new Function("clear", "tool", new Func<int>(Clear)));
            functions.Add("help", new Function("help", "tool", new Func<int>(Help)));
            functions.Add("sql", new Function("sql", "tool", new Func<int>(SQL)));
        }


        void Update()
        {
            while (true)
            {
                WaitCommand();
            }
        }
        public void WaitCommand()
        {
            Console.ForegroundColor = shellConfig.pathColor;
            Console.WriteLine(shellConfig.actualDir + ">");
            if (Logged)
            {
                Console.Write("logged");
            }
            Console.Write("~ ");
            Console.ForegroundColor = shellConfig.textColor;

            CommandCall(Console.ReadLine());
        }

        public void CommandCall(string command)
        {
            Command = new Command(command);
            bool find = false;

            foreach (var item in functions)
            {
                if (item.Key == Command.function)
                {
                    item.Value.ToCall.Invoke();
                    find = true;
                    break;
                }
            }
            if (Command.function.Replace(" ", "") != "" && !find)
            {
                Console.WriteLine("Commande non reconnu.");
            }
            Console.WriteLine("");
        }

        private int ChangeForeColor()
        {
            bool find = false;
            if (Command.values.Count == 0)
            {
                Console.WriteLine("'fcolor' a besoin d'une valeur pour fonctionner.");
                Console.WriteLine("Exemple : 'fcolor red'.");
                return 0;
            }
            else if (Command.values.Count > 1)
            {
                Console.WriteLine("'fcolor' a besoin d'une seule valeur pour fonctionner.");
                Console.WriteLine("Exemple : 'fcolor red'.");
                return 0;
            }

            foreach (var item in colors)
            {
                if (item.Key == Command.values[0])
                {
                    shellConfig.textColor = item.Value;
                    find = true;
                    break;
                }
            }
            if (!find)
            {
                Console.WriteLine("Couleur non trouvé.");
                return 0;
            }
            return 1;
        }
        private int ShowColors()
        {
            foreach (var item in colors)
            {
                Console.WriteLine(item.Key);
            }
            return 1;
        }
        private int Echo()
        {
            int x = 0;
            string ret = "";
            foreach (var item in Command.baseValues)
            {
                if (x > 0)
                {
                    ret += item + " ";
                }
                x++;
            }
            Console.WriteLine(ret);
            return 1;
        }
        private int LsFolders()
        {
            bool list = false;

            string path = shellConfig.actualDir;

            if (Command.values.Count > 0)
            {
                path = "";
                foreach (var item in Command.values)
                {
                    path += item + " ";
                }
            }

            foreach (var item in Command.arguments)
            {
                if (item.ToLower() == "-l" || item.ToLower() == "-list")
                {
                    list = true;
                }
                else
                {
                    Console.WriteLine("Argument non reconnu :");
                    Console.WriteLine("\"" + item + "\"");
                    return 0;
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
                return 0;
            }
            if (!CanRead(path))
            {
                Console.WriteLine("Vous n'avez pas l'autorisation de lire ce dossier.");
                return 0;
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
                            if (_file.Name.Length > maxSize && maxSize > 0)
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
                        if (_countDir > 0)
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
            return 1;
        }
        private int CurrentDir()
        {
            string pathToGo = shellConfig.actualDir;
            string path = "";

            int x = 0;
            foreach (var item in Command.baseValues)
            {
                if (x > 0)
                    path += item + " ";
                x++;
            }

            path = path.Replace("\"", "").Replace("/", "\\");
            string pathCombined = path[0] != '\\' ? pathToGo + '\\' + path : pathToGo + path;
            if (Directory.Exists(pathCombined))
            {
                shellConfig.actualDir = Path.GetFullPath(pathCombined);
            }
            else if (Directory.Exists(path))
            {
                shellConfig.actualDir = Path.GetFullPath(path);
            }
            else
            {
                Console.WriteLine(path);
                Console.WriteLine("Chemin d'accès non reconnu.");
                return 0;
            }

            return 1;
        }

        private int Exit()
        {
            Environment.Exit(0);
            return 1;
        }
        private int Clear()
        {
            Console.Clear();
            return 1;
        }
        private int Help()
        {
            string lastGroup = "";
            foreach (var item in functions)
            {
                if (item.Value.Group != lastGroup)
                {
                    lastGroup = item.Value.Group;
                    Console.WriteLine("");
                    Console.WriteLine(lastGroup);
                }
                if (item.Key != "help")
                {
                    Console.WriteLine(TextTool.StringToWidth("  " + item.Key, 10) + "e");
                }
            }
            return 1;
        }
        private int SQL()
        {
            bool end = false;
            foreach (var item in Command.arguments)
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
                    string request = "";

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

            return 1;
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
