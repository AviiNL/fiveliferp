using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FiveLife.Server.Remote
{
    public class TextBoxStreamWriter : TextWriter
    {
        TextBox _output = null;

        public TextBoxStreamWriter(TextBox output)
        {
            _output = output;
            _output.Enabled = true;
        }

        public override void Write(char value)
        {
            if(_output.InvokeRequired)
            {
                _output.Invoke(new Action<char>(Write), new object[] { value });
                return;
            }

            base.Write(value);
            _output.AppendText(value.ToString());
        }

        public override Encoding Encoding {
            get { return Encoding.UTF8; }
        }
    }
}
