using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Shell.Class
{
    public class Command
    {
        public string function = "";
        public List<string> baseValues = new List<string>();
        public List<string> values = new List<string>();
        public List<string> arguments = new List<string>();
        public Command(string str)
        {
            List<string> toReturn = new List<string>();
            List<string> newCommand = new List<string>();

            newCommand = str.Split('\"').ToList();
            for (int i = 0; i < newCommand.Count; i++)
            {
                if(i == 0)
                {
                    function = baseValues[i].Replace(" ", "");
                    continue;
                }

                if (i % 2 == 1)
                {
                    baseValues.Add("\"" + newCommand[i].Substring(0) + "\"");
                }
                else
                {
                    str = Regex.Replace(str, " {2,}", " ");
                    baseValues.AddRange(newCommand[i].Split(' '));
                }
            }

            foreach (var item in baseValues)
            {
                if(item[0] == '-')
                {
                    arguments.Add(item);
                }
                else
                {
                    values.Add(item);
                }
            }
        }
    }
}
