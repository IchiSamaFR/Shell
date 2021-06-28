using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Shell.Class.Tools
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
        public static string AddBlankRight(string item, int length)
        {
            if (item == null) return item;

            for (int i = item.Length - 1; i < length; i++)
            {
                item += " ";
            }
            return item;
        }
        public static string ConcatSize(string item, int length)
        {
            if (item == null || length <= 0) return "";

            if (length > item.Length)
            {
                return item.Substring(0, length);
            }
            return item;
        }
        public static string AddBlankLeft(string item, int length)
        {
            if (item == null) return item;

            for (int i = item.Length - 1; i < length; i++)
            {
                item = " " + item;
            }
            return item;
        }
        public static string ClearBlank(string item)
        {
            while (true)
            {
                int x = item.LastIndexOf(" ");
                if (x == item.Length - 1)
                {
                    item = item.Substring(0, x - 1);
                }
                else
                {
                    break;
                }
            }
            return item;
        }

        public static bool IsDateTime(string date)
        {
            try
            {
                CultureInfo culture = new CultureInfo("fr-FR");
                DateTime tempDate = Convert.ToDateTime(date, culture);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool CancelableReadLine(out string value)
        {
            value = string.Empty;
            var buffer = new StringBuilder();
            var key = Console.ReadKey(true);
            while (key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.Escape)
            {
                if (key.Key == ConsoleKey.Backspace && Console.CursorLeft > 0)
                {
                    var cli = --Console.CursorLeft;
                    buffer.Remove(cli, 1);
                    Console.CursorLeft = 0;
                    Console.Write(new String(Enumerable.Range(0, buffer.Length + 1).Select(o => ' ').ToArray()));
                    Console.CursorLeft = 0;
                    Console.Write(buffer.ToString());
                    Console.CursorLeft = cli;
                    key = Console.ReadKey(true);
                }
                else if (Char.IsLetterOrDigit(key.KeyChar) || Char.IsWhiteSpace(key.KeyChar))
                {
                    var cli = Console.CursorLeft;
                    buffer.Insert(cli, key.KeyChar);
                    Console.CursorLeft = 0;
                    Console.Write(buffer.ToString());
                    Console.CursorLeft = cli + 1;
                    key = Console.ReadKey(true);
                }
                else if (key.Key == ConsoleKey.LeftArrow && Console.CursorLeft > 0)
                {
                    Console.CursorLeft--;
                    key = Console.ReadKey(true);
                }
                else if (key.Key == ConsoleKey.RightArrow && Console.CursorLeft < buffer.Length)
                {
                    Console.CursorLeft++;
                    key = Console.ReadKey(true);
                }
                else
                {
                    key = Console.ReadKey(true);
                }
            }

            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                value = buffer.ToString();
                return true;
            }
            return false;
        }
    }
}
