using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NetworkProject
{
    public class Client
    {
        //client information mentioned in the project documentation
        public Socket player;
        public int Rank = -1;
        public string IP;

        public Client(string ip, int rank)
        {
            IP = ip;
            Rank = rank;
        }
    }
}
