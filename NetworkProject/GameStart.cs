using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;


namespace NetworkProject
{
    public partial class GameStart : Form
    {
        IPAddress ip;
        bool serverCreated;
        System.Windows.Forms.Timer timer;
        public GameStart()
        {
            InitializeComponent();

            timer = new System.Windows.Forms.Timer();
            timer.Interval = (1 * 100); // 100 msecs
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();

            Thread newthread = new Thread(new ThreadStart(Listen));
            newthread.Start();
        }

        private void btnStartAsServer_Click(object sender, EventArgs e)
        {
            try
            {
                timer.Stop();
                GameSettingScreen gsc = new GameSettingScreen(true, null);
                gsc.Show();
                this.Visible = false;
            }
            catch(Exception)
            {
                MessageBox.Show("Please close WiFi and make Ethernet connection with at least one another device");
            }
        }

        private void btnJoinAsClient_Click(object sender, EventArgs e)
        {
            GameSettingScreen gsc = new GameSettingScreen(false, ip);
            gsc.Show();
            this.Visible = false;
        }
        private void Listen()
        {
            ip = IPAddress.Any;
            //recieve the servers IP using UDP and place it in the variable "ip"
            byte[] arr = new byte[1024];
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, 15000);
            client.Bind(iep);
            EndPoint ep = (EndPoint)iep;
            int recv = client.ReceiveFrom(arr, ref ep);
            string stringData = Encoding.ASCII.GetString(arr, 0, recv);
            string[] temp = stringData.Split(':');
            ip = IPAddress.Parse(temp[0]);
            serverCreated = true;
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            if (serverCreated)
            {
                btnStartAsServer.Enabled = false;
                btnJoinAsClient.Enabled = true;
            }
        }

        private void GameStart_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }
    }
}
