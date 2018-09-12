using FiveLife.Server.Remote.Connection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FiveLife.Server.Remote
{
    public partial class Form1 : Form
    {
        TextWriter _writer;
        ConsoleClient client;

        public Form1()
        {
            InitializeComponent();

            _writer = new TextBoxStreamWriter(txtLogBox);
            Console.SetOut(_writer);

            client = new ConsoleClient();
            client.OnConnect += Client_OnConnect;
            client.OnDisconnect += Client_OnDisconnect;

            client.OnDataReceived += Client_OnDataReceived;
            client.Start();
        }

        private void Client_OnDisconnect()
        {
            if(menuStrip1.InvokeRequired)
            {
                menuStrip1.Invoke(new Action(Client_OnDisconnect));
                return;
            }

            mnuItmConnect.Text = "Connect";
        }

        private void Client_OnConnect()
        {
            if (menuStrip1.InvokeRequired)
            {
                menuStrip1.Invoke(new Action(Client_OnConnect));
                return;
            }

            mnuItmConnect.Text = "Disconnect";
        }

        private void Client_OnDataReceived(string data)
        {
            Console.WriteLine($"< {data}");
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            client.Close();
            Application.Exit();
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(client.isConnected)
            {
                client.Close();
                return;
            }
            client.Start();
        }
    }
}
