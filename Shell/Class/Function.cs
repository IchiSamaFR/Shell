using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell.Class
{
    public class Function
    {
        public string Name;
        public string Group;
        public Func<int> ToCall;

        public Function(string name, Func<int> toCall)
        {
            this.Name = name;
            this.ToCall = toCall;
        }

        public Function(string name, string group, Func<int> toCall)
            : this (name, toCall)
        {
            this.Group = group;
        }
    }
}
