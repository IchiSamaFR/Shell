using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell.Class.Config
{
    public class ShellConfig
    {
        public string actualDir;
        public ConsoleColor pathColor;
        public ConsoleColor textColor;

        public ShellConfig(string directory)
        {
            actualDir = directory;
        }
        public ShellConfig(string directory, ConsoleColor path, ConsoleColor text)
            : this(directory)
        {
            pathColor = path;
            textColor = text;
        }
    }
}
