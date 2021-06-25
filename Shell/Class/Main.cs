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

            functions.Add("ls", new Function("ls", "directory", new Func<int>(LsFunction.LsFolders)));
            functions.Add("cd", new Function("cd", "directory", new Func<int>(DirFunction.CurrentDir)));
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
