using Shell.Class.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell.Class.Functions
{
    public class SQLFunction
    {
        public static Command command;
        public static SQLConfig sqlConfig;

        public static int SQL()
        {
            command = Main.Command;
            sqlConfig = Main.sqlConfig;

            bool end = false;
            foreach (var item in command.arguments)
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
    }
}
