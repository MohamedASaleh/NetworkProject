using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetworkProject
{
    public partial class GameSettingScreen : Form
    {
        Socket currentSocket;
        List<Client> clients = new List<Client>();
        Dictionary<Point, int> snakes = new Dictionary<Point, int>();
        Dictionary<Point, int> ladders = new Dictionary<Point, int>();

        int numberOfPlayers, port;
        bool isserver;
        IPAddress serverIP;
        Socket newClient;

        Socket tempServer;
        IPAddress[] a;
        IPEndPoint GEp;


        //Form Constructor
        public GameSettingScreen(bool isServer, IPAddress IP)
        {
          
            serverIP = IP;
            isserver = isServer;
            numberOfPlayers = 0;
            port = 15000;
            InitializeComponent();
           

            if (!isServer)
            {
                //joined as client (initialize the TCP client socket, connect to Servers IP and wait for the server to start the game
                btnStartGame.Enabled = false;
                //to be added in a thread to not hault the program
                Thread t = new Thread(new ParameterizedThreadStart(JoinServer));
                t.Start(IP);
                
            }
            else
            {
                tempServer = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                a = Dns.GetHostAddresses(Dns.GetHostName());
                GEp = new IPEndPoint(a[a.Length - 1], port);
                tempServer.Bind(GEp);
                tempServer.EnableBroadcast = true;

                //joined as server (start the TCP server socket, start UDP server socket to broadcasting the servers IP and finally accept client sockets using the TCP socket
                InitializeServer();
                tmrBroadCastIP.Start();
                Thread newthread = new Thread(new ThreadStart(AcceptPlayers));
                newthread.Start();
            }
        }




        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////CLIENT///////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //Client Function implementation
        void JoinServer(object obj)
        {
            IPAddress IP = (IPAddress)obj;
            //write code to initialize currentSocket to be client socket and connect to server IP
            currentSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iep = new IPEndPoint(IP, port);
            currentSocket.Connect(iep);
            byte[] byteArr;
            IPAddress[] a = Dns.GetHostAddresses(Dns.GetHostName());
            byteArr = Encoding.ASCII.GetBytes(a[a.Length - 1].ToString());
            currentSocket.Send(byteArr);

            IPEndPoint tempiep = new IPEndPoint(IPAddress.Any, 11000);
            tempServer = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            tempServer.Bind(tempiep);
            EndPoint ep = (EndPoint)tempiep;

            while (true)
            {
                byteArr = new byte[1024];
                               
                int recv = tempServer.ReceiveFrom(byteArr, ref ep);
                if (recv < 4)
                    break;
                string temp = Encoding.ASCII.GetString(byteArr, 0, recv);
                Client client = new Client(temp, numberOfPlayers);
                if (!clients.Contains(client))
                {
                    numberOfPlayers++;
                    clients.Add(client);
                    if (listBox1.InvokeRequired)
                        listBox1.Invoke(new MethodInvoker(delegate
                        {
                            listBox1.Items.Add("Player " + numberOfPlayers + ": " + client.IP);
                        }));
                    else
                        listBox1.Items.Add("Player " + numberOfPlayers + ": " + client.IP);
                }
            }

            Thread t = new Thread(new ParameterizedThreadStart(RecieveDataFromServer));
            t.Start(int.Parse(Encoding.ASCII.GetString(byteArr)));
        }

        void RecieveDataFromServer(object obj)
        {
            //this function in different thread as it will halt the application during recieving from server
            int numberOfPlayers = (int)obj;

            
            GenerateSnakesAndLadders();
            char[,] board = GenerateBoard(snakes, ladders);
            GamePlayingScreen gpc = new GamePlayingScreen(board, snakes, ladders, clients, numberOfPlayers, currentSocket, false);
            if (this.InvokeRequired)
                this.Invoke(new MethodInvoker(delegate
                {
                    this.Visible = false;

                }));
            else
                this.Visible = false;
            gpc.ShowDialog();
            
        }







        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////SERVER///////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //Server Functions implementation
        void InitializeServer()
        {
            //write code to initialize currentSocket to be server socket
            //add the server to clientList
            //set the servers rank = 0;
            currentSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress[] IPaddresses = Dns.GetHostAddresses(Dns.GetHostName());
            IPEndPoint serverEP = new IPEndPoint(IPaddresses[IPaddresses.Length - 1], port);
            currentSocket.Bind(serverEP);
            currentSocket.Listen(8);
            Client serverClient = new Client(IPaddresses[IPaddresses.Length - 1].ToString(), numberOfPlayers);
            clients.Add(serverClient);
            numberOfPlayers++;
            listBox1.Items.Add("Player " + numberOfPlayers + ": " + IPaddresses[IPaddresses.Length - 1].ToString());
        }

        void BroadCastIP()
        {
            byte[] bytearr = Encoding.ASCII.GetBytes(GEp.ToString());
            tempServer.SendTo(bytearr, new IPEndPoint(IPAddress.Broadcast, port));
        }
        void AcceptPlayers()
        {
            //write the code of server socket to accept incoming players
            //create an object from class Client and fill in its information
            //assign a rank for this created object (which is the client index in list) and add to list of clients
            //where Client is a class contains all information about client (mentioned in the project document)
            newClient = currentSocket.Accept();
            byte[] byteArr = new byte[1024];
            int num = newClient.Receive(byteArr);
            string temp = Encoding.ASCII.GetString(byteArr, 0, num);
            Client newclient = new Client(temp, numberOfPlayers);
            clients.Add(newclient);
            numberOfPlayers++;

            //send players IP to all clients
            foreach (Client client in clients)
            {
                byteArr = Encoding.ASCII.GetBytes(client.IP);
                tempServer.SendTo(byteArr, new IPEndPoint(IPAddress.Broadcast, 11000));
            }

            //to access control from a thread 
            if (listBox1.InvokeRequired)
                listBox1.Invoke(new MethodInvoker(delegate
                {
                    listBox1.Items.Add("Player " + numberOfPlayers + ": " + temp);
                }));
            else
                listBox1.Items.Add("Player " + numberOfPlayers + ": " + temp);

        }

        private void btnStartGame_Click(object sender, EventArgs e)
        {
            //stop broadcasting the IP
            tmrBroadCastIP.Stop();

            byte[] arr = Encoding.ASCII.GetBytes(clients.Count.ToString());
            tempServer.SendTo(arr, new IPEndPoint(IPAddress.Broadcast, 11000));
            

            //generate board

            GenerateSnakesAndLadders();
            char[,] board = GenerateBoard(snakes, ladders);
            GamePlayingScreen gpc = new GamePlayingScreen(board, snakes, ladders, clients, clients.Count, currentSocket, true);
            if (this.InvokeRequired)
                this.Invoke(new MethodInvoker(delegate
                {
                    this.Visible = false;

                }));
            else
                this.Visible = false;
            gpc.ShowDialog();
            
        }
        Random r;

        private void tmrBroadCastIP_Tick(object sender, EventArgs e)
        {
            BroadCastIP();
        }





        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////COMMON FUNCTION USED BY BOTH///////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //Common function
        void GenerateSnakesAndLadders()
        {
            snakes.Add(new Point(6, 1), 1);
            snakes.Add(new Point(2, 2), 2);
            snakes.Add(new Point(9, 4), 1);
            snakes.Add(new Point(1, 6), 3);
            snakes.Add(new Point(2, 6), 2);
            snakes.Add(new Point(4, 6), 1);
            snakes.Add(new Point(1, 8), 1);
            snakes.Add(new Point(2, 9), 2);
            snakes.Add(new Point(8, 9), 1);
            snakes.Add(new Point(0, 7), 1);
            ladders.Add(new Point(1, 0), 1);
            ladders.Add(new Point(3, 0), 2);
            ladders.Add(new Point(8, 1), 1);
            ladders.Add(new Point(6, 2), 1);
            ladders.Add(new Point(0, 2), 2);
            ladders.Add(new Point(6, 5), 1);
            ladders.Add(new Point(3, 6), 2);
            ladders.Add(new Point(8, 4), 3);
            ladders.Add(new Point(4, 8), 1);
            ladders.Add(new Point(0, 8), 1);
        }
        char[,] GenerateBoard(Dictionary<Point, int> snakes, Dictionary<Point, int> ladders)
        {
            char[,] board = new char[10, 10];
            foreach (var snake in snakes)
            {
                board[snake.Key.Y, snake.Key.X] = 'S';
            }
            foreach (var ladder in ladders)
            {
                board[ladder.Key.Y, ladder.Key.X] = 'L';
            }
            return board;
        }
        private void GameSettingScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }


    }
}
