﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PostBuild
{
    class Program
    {
        const string RESOURCE_NAME = "fivelife";
        
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        static void Main(string[] args)
        {
            CopyFiles(@"C:\FiveM\Source\FiveLife\FiveLife.Database\bin\Debug", "Entity*.dll");
            CopyFiles(@"C:\FiveM\Source\FiveLife\FiveLife.Database\bin\Debug", "MySql*.dll");
            CopyFiles(@"C:\FiveM\Source\FiveLife\FiveLife.Database\bin\Debug", "System*.dll");
            CopyFiles(@"C:\FiveM\Source\FiveLife\FiveLife.Database\bin\Debug", "I18*.dll");
            CopyFiles(@"C:\FiveM\Source\FiveLife\FiveLife.Database\bin\Debug", "Nequeo*.dll");

            CopyFiles(@"C:\FiveM\Source\FiveLife\FiveLife.Client\bin\Debug", "*.dll");
            CopyFiles(@"C:\FiveM\Source\FiveLife\FiveLife.Server\bin\Debug", "*.dll");
            CopyFiles(@"C:\FiveM\Source\FiveLife\PostBuild\bin\Debug\Extra", "*");

            Restart();
        }

        static void CopyFiles(string path, string pattern)
        {
            var clientFiles = Directory.GetFiles(path, pattern, SearchOption.AllDirectories);

            foreach (var file in clientFiles)
            {
                var dest = file.Replace(path, @"C:\FiveM\Server\server-data\resources\[FiveLife]\fivelife");
                var destDir = Directory.GetParent(dest);

                Directory.CreateDirectory(destDir.FullName);

                File.Copy(file, dest, true);
            }

            if(File.Exists(@"C:\FiveM\Server\server-data\resources\[FiveLife]\fivelife\\CitizenFX.Core.dll"))
                File.Delete(@"C:\FiveM\Server\server-data\resources\[FiveLife]\fivelife\\CitizenFX.Core.dll");
        }

        static void Restart()
        {
            IntPtr hwnd = FindWindow(null, "Start Server");

            if (hwnd != IntPtr.Zero)
            {
                if (SetForegroundWindow(hwnd))
                {
                    System.Windows.Forms.SendKeys.SendWait($"restart {RESOURCE_NAME}\n");
                }
                else
                {
                    Console.WriteLine("Unable to foreground window");
                    Console.ReadKey();
                }
            }
            else
            {
                Console.WriteLine("Window not found");
            }
        }

    }
}
