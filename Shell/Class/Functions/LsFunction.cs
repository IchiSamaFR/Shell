using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Shell.Class.Tools;

namespace Shell.Class.Functions
{
    public class LsFunction
    {
        public static Command command;
        static CultureInfo culture = new CultureInfo("fr-FR");

        static string pathSource;
        static bool list;
        static int index;
        static IFormatProvider frenchFormatProvider =
            new CultureInfo("fr-FR");

        public static int LsFolders()
        {
            command = Main.Command;
            pathSource = Main.shellConfig.actualDir;
            list = false;
            index = 1;


            if (command.IsCommandLike(index, "-l"))
            {
                list = true;
                index += 1;
                if (command.IsCommandLike(index, "$value $end"))
                {
                    pathSource = DirectoryTool.SetPath(command.GetBaseValue(index).Replace(" ", ""));
                    if (File.Exists(pathSource))
                    {
                        LsFile(pathSource);
                        return 0;
                    }
                    else if (!Directory.Exists(pathSource))
                    {
                        return 0;
                    }
                }
                else if (!command.IsCommandLike(index, "$end"))
                {
                    return 0;
                }
            }
            else if (command.IsCommandLike(index, "$value"))
            {
                pathSource = DirectoryTool.SetPath(command.GetBaseValue(index).Replace(" ", ""));
                index += 1;
                if (command.IsCommandLike(index, "-l $end"))
                {
                    list = true;
                }
                else if (!command.IsCommandLike(index, "$end"))
                {
                    return 0;
                }
            }
            else if (!command.IsCommandLike(index, "$end"))
            {
                return 0;
            }

            if (list)
            {
                Console.Write(TextTool.AddBlankLeft(" ", 9) +
                              "  " + TextTool.AddBlankRight("<DIR>", 20));
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("  " + "..");
                Console.ForegroundColor = Main.shellConfig.textColor;
                Console.WriteLine("");

                long size = 0L;
                int files = 0, folders = 0;

                foreach (var item in Directory.GetDirectories(pathSource))
                {
                    LsDir(item);
                    folders++;
                }
                foreach (var item in Directory.GetFiles(pathSource))
                {
                    long val = 0L;
                    LsFile(item, out val);
                    size += val;
                    files++;
                }
                Console.WriteLine(TextTool.AddBlankLeft(files.ToString(), 12) 
                        + TextTool.AddBlankRight(" fichier(s)", 12) 
                        + TextTool.AddBlankLeft(size.ToString("#,##0", frenchFormatProvider), 20) + " octets");
                Console.WriteLine(TextTool.AddBlankLeft(folders.ToString(), 12)
                        + TextTool.AddBlankRight(" dossier(s)", 12));

            }
            else
            {
                var tuple = GetFiles();
                string[,] colValues = tuple.Item1;
                int[] charCol = tuple.Item2;
                int countDir = tuple.Item3;

                if (countDir == 0)
                {
                    return 0;
                }

                for (int y = 0; y < colValues.GetLength(1); y++)
                {
                    for (int x = 0; x < colValues.GetLength(0); x++)
                    {
                        if (countDir > 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            countDir--;
                        }
                        else
                        {
                            Console.ForegroundColor = Main.shellConfig.textColor;
                        }
                        Console.Write(TextTool.AddBlankRight(colValues[x, y], charCol[x]) + " ");
                    }
                    Console.WriteLine("");
                }
            }
            return 1;
        }

        static Tuple<string[,], int[], int> GetFiles()
        {
            int _x = 0;
            int _y = 0;
            int maxSize = 30;
            int _countDir = 0;
            int col = 4;

            if (!Directory.Exists(pathSource))
            {
                return Tuple.Create(new string[0,0], new int[0], 0);
            }

            int amount = Directory.GetDirectories(pathSource).Length + Directory.GetFiles(pathSource).Length + 1;
            string[,] colValues = new string[col, (int)Math.Ceiling((float)amount / (float)col)];
            int[] charCol = new int[col];

            colValues[_x, _y] = "..";
            _countDir++;
            _x++;
            foreach (var item in Directory.GetDirectories(pathSource))
            {
                DirectoryInfo _dir = new DirectoryInfo(item);
                colValues[_x, _y] = _dir.Name;
                if (charCol[_x] < _dir.Name.Length)
                {
                    if (_dir.Name.Length > maxSize && maxSize > 0)
                    {
                        charCol[_x] = maxSize;
                        colValues[_x, _y] = _dir.Name.Substring(0, maxSize);
                    }
                    else
                    {
                        charCol[_x] = _dir.Name.Length;
                    }
                }

                _x++;
                if (_x == col)
                {
                    _y++;
                    _x = 0;
                }
                _countDir++;
            }
            foreach (var item in Directory.GetFiles(pathSource))
            {
                FileInfo _file = new FileInfo(item);
                colValues[_x, _y] = _file.Name;
                if (charCol[_x] < _file.Name.Length)
                {
                    if (_file.Name.Length > maxSize && maxSize > 0)
                    {
                        charCol[_x] = maxSize;
                        colValues[_x, _y] = _file.Name.Substring(0, maxSize - 4) + "[..]";
                    }
                    else
                    {
                        charCol[_x] = _file.Name.Length;
                    }
                }

                _x++;
                if (_x == col)
                {
                    _y++;
                    _x = 0;
                }
            }
            return Tuple.Create(colValues, charCol, _countDir);
        }
        
        static void LsFile(string path)
        {
            FileInfo _file = new FileInfo(path);
            Console.WriteLine(_file.CreationTime.ToString("dd/MM/yyyy") +
                             "  " + TextTool.AddBlankLeft(_file.Length.ToString("#,##0", frenchFormatProvider), 20) +
                             "  " + _file.Name + "");
        }
        static void LsFile(string path, out long size)
        {
            size = 0L;
            FileInfo _file = new FileInfo(path);
            long fileLength = _file.Length;
            Console.WriteLine(_file.CreationTime.ToString("dd/MM/yyyy") +
                             "  " + TextTool.AddBlankLeft(fileLength.ToString("#,##0", frenchFormatProvider), 20) +
                             "  " + _file.Name + "");
            size += fileLength;
        }
        static void LsDir(string path)
        {
            DirectoryInfo _dir = new DirectoryInfo(path);
            Console.Write(_dir.CreationTime.ToString("dd/mm/yyyy") +
                             "  " + TextTool.AddBlankRight("<DIR>", 20));

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("  " + _dir.Name + "\\");
            Console.ForegroundColor = Main.shellConfig.textColor;
            Console.WriteLine("");
        }
    }
}
