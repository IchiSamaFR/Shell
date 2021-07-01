using Shell.Class.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell.Class.Functions
{
    public class SystemFunction
    {
        public static int Echo()
        {
            int x = 0;
            string ret = "";
            foreach (var item in Main.Command.baseValues)
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
        public static int Exit()
        {
            Environment.Exit(0);
            return 1;
        }
        public static int Clear()
        {
            Console.Clear();
            return 1;
        }
        public static int Help()
        {
            string lastGroup = "";
            foreach (var item in Main.functions)
            {
                if (item.Value.Group != lastGroup)
                {
                    lastGroup = item.Value.Group;
                    Console.WriteLine("");
                    Console.WriteLine(lastGroup);
                }
                if (item.Key != "help")
                {
                    Console.WriteLine(TextTool.StringToWidth("  " + item.Key, 14) + item.Value.Description);
                }
            }
            return 1;
        }
    }
}
