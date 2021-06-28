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

            functions.Add("fcolor", new Function("fcolor", "interface", new Func<int>(ColorFunctions.ChangeForeColor)));
            functions.Add("fg", new Function("fcolor", "interface", new Func<int>(ColorFunctions.ChangeForeColor)));
            functions.Add("colors", new Function("color", "interface", new Func<int>(ColorFunctions.ShowColors)));
            functions.Add("color", new Function("color", "interface", new Func<int>(ColorFunctions.ShowColors)));

            functions.Add("ls", new Function("ls", "directory", new Func<int>(LsFunction.LsFolders)));
            functions.Add("cd", new Function("cd", "directory", new Func<int>(DirFunction.CurrentDir)));
            functions.Add("find", new Function("find", "directory", new Func<int>(FindFunction.Find)));
            functions.Add("cat", new Function("cat", "directory", new Func<int>(CatFunction.Cat)));

            functions.Add("echo", new Function("echo", "tool", new Func<int>(SystemFunction.Echo)));
            functions.Add("exit", new Function("exit", "tool", new Func<int>(SystemFunction.Exit)));
            functions.Add("clear", new Function("clear", "tool", new Func<int>(SystemFunction.Clear)));
            functions.Add("help", new Function("help", "tool", new Func<int>(SystemFunction.Help)));
            functions.Add("sql", new Function("sql", "tool", new Func<int>(SQLFunction.SQL)));
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
    }
}
