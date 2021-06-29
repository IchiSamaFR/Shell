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
        public Command()
        {

        }
        public Command(string str)
        {
            Init(str);
        }
        public void Init(string str)
        {
            List<string> tempValues = new List<string>();
            List<string> newCommand = new List<string>();

            newCommand = str.Split('\"').ToList();
            for (int i = 0; i < newCommand.Count; i++)
            {
                if (i % 2 == 1)
                {
                    tempValues.Add("" + newCommand[i] + "");
                    baseValues.Add("\"" + newCommand[i] + "\"");
                }
                else
                {
                    str = Regex.Replace(str, " {2,}", " ");
                    foreach (var item in newCommand[i].Split(' '))
                    {
                        if(item.Replace(" ", "") != "")
                        {
                            tempValues.Add(item);
                            baseValues.Add(item);
                        }
                    }
                }
            }

            int x = -1;
            foreach (var item in tempValues)
            {
                if (item.Length > 0)
                {
                    x++;
                    if (x == 0)
                    {
                        function = item;
                        continue;
                    }

                    if (item[0] == '-' || item[0] == '>' || item[0] == '<')
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

        public string GetBaseValue(int ind)
        {
            if(baseValues.Count > ind)
            {
                return baseValues[ind];
            }
            return "";
        }
    }
}
