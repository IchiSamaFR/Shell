using Shell.Class.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell.Class.Functions
{
    public class ColorFunctions
    {
        public static Command command;
        public static ShellConfig shellConfig;

        public static int ChangeForeColor()
        {
            command = Main.Command;
            shellConfig = Main.shellConfig;

            bool find = false;
            if (command.values.Count == 0)
            {
                Console.WriteLine("'fcolor' a besoin d'une valeur pour fonctionner.");
                Console.WriteLine("Exemple : 'fcolor red'.");
                return 0;
            }
            else if (command.values.Count > 1)
            {
                Console.WriteLine("'fcolor' a besoin d'une seule valeur pour fonctionner.");
                Console.WriteLine("Exemple : 'fcolor red'.");
                return 0;
            }

            foreach (var item in Main.colors)
            {
                if (item.Key == command.values[0])
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
        public static int ShowColors()
        {
            foreach (var item in Main.colors)
            {
                Console.WriteLine(item.Key);
            }
            return 1;
        }
    }
}
