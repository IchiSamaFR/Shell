using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Shell.Tools
{
    public static class TextTool
    {
        public static string StringToWidth(string str, int length)
        {
            if(str.Length >= length)
            {
                return str.Substring(0, length);
            }
            else
            {
                return str.PadRight(length);
            }
        }
        public static List<string> CommandToArgs(string str)
        {
            str = Regex.Replace(str, " {2,}", " ");
            List<string> toReturn = new List<string>();
            List<string> baseValues = new List<string>();

            baseValues = str.Split('\"').ToList();
            for (int i = 0; i < baseValues.Count; i++)
            {
                if (i % 2 == 1)
                {
                    toReturn.Add("\"" + baseValues[i].Substring(0) + "\"");
                }
                else
                {
                    toReturn.AddRange(baseValues[i].Split(' '));
                }
            }

            return toReturn;
        }
        public static List<string> GetArgs(List<string> args)
        {
            List<string> _args = new List<string>();
            foreach (var item in args)
            {
                if (item.Length > 0 && item[0] == '-')
                {
                    _args.Add(item);
                }
            }

            return _args;
        }
        public static List<string> GetValues(List<string> args)
        {
            List<string> _args = new List<string>();
            foreach (var item in args)
            {
                if (item.Length > 0 && item[0] != '-')
                {
                    if (item[0] == '\"' && item[item.Length - 1] == '\"')
                    {
                        _args.Add(item.Substring(1, item.Length - 2));
                    }
                    else
                    {
                        _args.Add(item);
                    }
                }
            }
            return _args;
        }
    }
}
