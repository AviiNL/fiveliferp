using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.NUI
{
    class ResourceGenerator
    {
        public string resource_manifest_version { get; set; }
        public string ui_page { get; set; }

        public Dictionary<string, string> data_files = new Dictionary<string, string>();
        public List<string> client_scripts = new List<string>();
        public List<string> server_scripts = new List<string>();
        public List<string> files = new List<string>();


        public override string ToString()
        {
            var output = $"resource_manifest_version '{resource_manifest_version}'\n\n";

            output += $"ui_page '{ui_page}'\n\n";

            foreach (var data_file in data_files)
            {
                output += $"datafile '{data_file.Key.ToUpper()}' '{data_file.Value}'\n";
            }
            output += "\n";

            output += "client_script {\n";
            foreach (var script in client_scripts)
            {
                output += $"\t'{script}',\n";
            }
            output += "}\n\n";

            output += "server_script {\n";
            foreach (var script in server_scripts)
            {
                output += $"\t'{script}',\n";
            }
            output += "}\n\n";

            output += "files {\n";
            foreach (var data_file in data_files)
            {
                output += $"\t'{data_file.Value}',\n";
            }
            foreach (var script in files)
            {
                output += $"\t'{script}',\n";
            }
            output += "}\n\n";

            return output;
        }

    }
}
