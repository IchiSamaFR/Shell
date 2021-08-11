using Shell.Class.Config;
using Shell.Class.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell.Class.Functions
{
    class DsFunction
    {
        public static Command command;
        public static ShellConfig shellConfig;

        static string pathSource;
        static int index;

        public static int Ds()
        {
            command = Main.Command;
            shellConfig = Main.shellConfig;
            pathSource = "";
            index = 1;

            double divider = 0;

            if (command.IsCommandLike(index, "-k $end") || command.IsCommandLike(index, "-ko $end"))
            {
                divider = 1024;
            }
            else if (command.IsCommandLike(index, "-m $end") || command.IsCommandLike(index, "-mo $end"))
            {
                divider = 1048576;
            }
            else if (command.IsCommandLike(index, "-g $end") || command.IsCommandLike(index, "-go $end"))
            {
                divider = 1073741824;
            }
            else if (command.IsCommandLike(index, "-t $end") || command.IsCommandLike(index, "-to $end"))
            {
                divider = 1099511627776;
            }
            else if (command.IsCommandLike(index, "$end"))
            {
                divider = 1;
            }


            Console.WriteLine(TextTool.AddBlankRight("", 5) + " "
                + TextTool.AddBlankLeft("Disponible", 16) + " "
                + TextTool.AddBlankLeft("Total", 16) + " "
                + TextTool.AddBlankLeft("Disponible(%)", 13));

            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                {
                    string freeSpacePerc = (drive.AvailableFreeSpace / (float)drive.TotalSize * 100).ToString("### ### ### ### ##0.#") + "%";
                    string freeSpace = (drive.AvailableFreeSpace / divider).ToString("### ### ### ### ##0.#");
                    string totalSpace = (drive.TotalSize / divider).ToString("### ### ### ### ##0.#");


                    Console.WriteLine(TextTool.AddBlankRight(drive.Name, 5) + " "
                        + TextTool.AddBlankLeft(freeSpace, 16) + " "
                        + TextTool.AddBlankLeft(totalSpace, 16) + " "
                        + TextTool.AddBlankLeft(freeSpacePerc, 13));
                }
            }
            return 1;
        }
    }
}
