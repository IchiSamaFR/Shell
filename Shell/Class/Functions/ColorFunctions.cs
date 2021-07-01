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

        static int index;
        static string value;

        public static int ChangeForeColor()
        {
            command = Main.Command;
            shellConfig = Main.shellConfig;
            index = 1;
            bool find = false;

            if(command.IsCommandLike(index, "$value $end"))
            {
                value = command.GetBaseValue(index);
            }
            else
            {
                return 0;
            }

            foreach (var item in Main.colors)
            {
                if (item.Key == value)
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
