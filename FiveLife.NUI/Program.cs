using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

// C:\FiveM\Source\FiveLife\FiveLife.NUI\bin\Debug\FiveLife.NUI.exe

namespace FiveLife.NUI
{
    class Program
    {
        private static readonly string path = @"C:\FiveM\Source\FiveLife\FiveLife.NUI\html\build\";
        private static readonly string dest = @"C:\FiveM\Server\server-data\resources\[FiveLife]\fivelife\html\";
        private static readonly string resource = @"C:\FiveM\Server\server-data\resources\[FiveLife]\fivelife\__resource.lua";

        static void Main(string[] args)
        {
            YarnBuild();

            var Resource = new ResourceGenerator();

            Resource.resource_manifest_version = "05cfa83c-a124-4cfa-a768-c24a5811d8f9";
            Resource.ui_page = "html/index.html";

            Resource.handling_files.Add("handling.meta");

            Resource.client_scripts.Add("fivelife.client.net.dll");

            Resource.server_scripts.Add("fivelife.database.net.dll");
            Resource.server_scripts.Add("fivelife.server.net.dll");

            Resource.files.Add("fivelife.shared.net.dll");
            Resource.files.Add("fivelife.nativeui.net.dll");
            Resource.files.Add("Newtonsoft.Json.dll");

            if (!Directory.Exists(dest))
                Directory.CreateDirectory(dest);
            
            foreach(var file in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories))
            {
                
                var relativePath = file.Replace(path, "");
                relativePath = Regex.Replace(relativePath, @"\.[a-f0-9]{8}\.", ".", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                var destination = dest + relativePath;
                var destDir = destination.Replace(Path.GetFileName(destination), "");

                if (!Directory.Exists(destDir))
                    Directory.CreateDirectory(destDir);

                var content = File.ReadAllText(file);
                content = Regex.Replace(content, @"\.[a-f0-9]{8}\.", ".", RegexOptions.Multiline | RegexOptions.IgnoreCase);

                Resource.files.Add(@"html/" + relativePath.Replace(@"\", "/"));

                File.WriteAllText(destination, content);
                
            }

            File.WriteAllText(resource, Resource.ToString());
        }

        private static void YarnBuild()
        {
            var startInfo = new ProcessStartInfo();
            startInfo.WorkingDirectory = @"C:\FiveM\Source\FiveLife\FiveLife.NUI\html\";
            startInfo.FileName = @"c:\Users\Yme-Jan\AppData\Roaming\npm\yarn";
            startInfo.Arguments = "run build";

            Process proc = Process.Start(startInfo);

            proc.WaitForExit();
        }

    }
}
