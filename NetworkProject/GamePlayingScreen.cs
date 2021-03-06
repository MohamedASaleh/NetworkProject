﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;

namespace NetworkProject
{
    public partial class GamePlayingScreen : Form
    {
        char[,] gameBoard;
        Dictionary<Point, int> Snakes;
        Dictionary<Point, int> Ladders;
        List<Client> Clients;
        List<Point> PlayersLocation;
        int myIndex;
        Socket currentPlayer;
        bool IsServer;
        Bitmap Board;
        bool gameStillOn;


        public int getCurrentPlayerIndex()
        {
            IPAddress[] addresses = Dns.GetHostAddresses(Dns.GetHostName());
            for (int i = 0; i < Clients.Count; i++)
            {
                if (Clients[i].IP.Equals(addresses[addresses.Length - 1].ToString()))
                    return i;
            }
            return -1;
        }


        public GamePlayingScreen(char[,] board, Dictionary<Point, int> snakes, Dictionary<Point, int> ladders, List<Client> clients, int numberOfPlayers, Socket me, bool Server)
        {
            InitializeComponent();
            Clients = clients;
            gameBoard = board;
            Snakes = snakes;
            Ladders = ladders;
            currentPlayer = me;
            gameStillOn = true;
            PlayersLocation = new List<Point>();
            for (int i = 0; i < numberOfPlayers; i++)
                PlayersLocation.Add(new Point(0, 0));
            GeneratePlayerList(numberOfPlayers);
            IsServer = Server;
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            DrawBoard();
            myIndex = getCurrentPlayerIndex();
            if (IsServer)
            {
                btnRollTheDice.Enabled = true;
                myIndex = 0;
                ////////////////////////////////
                clients[myIndex].CurrentPlayer = true;
                for (int i = 1; i < clients.Count; i++)
                {
                    Thread t = new Thread(new ParameterizedThreadStart(RecieveFromClients));
                    t.Start(clients[i]);
                }
            }
            else
            {
                Thread t = new Thread(RecieveFromServer);
                t.Start();
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////DRAWING FUNCTIONS/////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////YOU DON'T NEED TO WRITE ANY CODE HERE///////////////////////////////////////////////////////////////////////////////////
        void GeneratePlayerList(int numberOfPlayers)
        {
            //maximum number of players is 8
            numberOfPlayers = numberOfPlayers > 8 ? 8 : numberOfPlayers;
            for (int i = 0; i < numberOfPlayers; i++)
            {
                Label label = new Label();
                label.AutoSize = true;
                label.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                label.Location = new System.Drawing.Point(85, 65 + i * 50);
                label.Name = "label2";
                label.Size = new System.Drawing.Size(76, 19);
                label.TabIndex = 0;
                label.Text = "Player " + (i + 1);
                this.groupBox1.Controls.Add(label);
                PictureBox pictureBox = new PictureBox();
                pictureBox.Location = new System.Drawing.Point(30, 55 + i * 50);
                pictureBox.Name = "pictureBox2";
                pictureBox.Size = new System.Drawing.Size(48, 40);
                pictureBox.TabIndex = 0;
                pictureBox.TabStop = false;
                GeneratePlayerColor(i + 1);
                Image bmp = new Bitmap(pictureBox.Width, pictureBox.Height);
                Graphics g = Graphics.FromImage(bmp);
                g.FillEllipse(new SolidBrush(PlayerColors[i]), 0, 0, 48, 40);
                g.Flush();
                pictureBox.BackgroundImage = bmp;
                this.groupBox1.Controls.Add(pictureBox);

            }
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
        }
        List<Color> PlayerColors = new List<Color>();
        void GeneratePlayerColor(int index)
        {
            PlayerColors.Add(Color.FromArgb(index * 200 % 255, index * 300 % 255, index * 400 % 255));
        }
        void DrawBoard()
        {
            Bitmap bmp = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);
            Graphics g = Graphics.FromImage(bmp);
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (i % 2 == 0)
                    {
                        if (j % 2 == 0)
                            g.FillRectangle(Brushes.White, new Rectangle(j * 50, i * 50, 50, 50));
                        else
                            g.FillRectangle(Brushes.Gray, new Rectangle(j * 50, i * 50, 50, 50));
                    }
                    else
                    {
                        if (j % 2 == 0)
                            g.FillRectangle(Brushes.Gray, new Rectangle(j * 50, i * 50, 50, 50));
                        else
                            g.FillRectangle(Brushes.White, new Rectangle(j * 50, i * 50, 50, 50));
                    }
                }
            }
            int counter = 100;
            for (int j = 0; j < 10; j++)
            {
                if (j % 2 == 0)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        g.DrawString(counter-- + "", SystemFonts.DefaultFont, Brushes.Black, new PointF(i * 50, j * 50));
                    }
                }
                else
                {
                    for (int i = 9; i >= 0; i--)
                    {
                        g.DrawString(counter-- + "", SystemFonts.DefaultFont, Brushes.Black, new PointF(i * 50, j * 50));
                    }
                }

            }

            
            


            for (int i = 0; i < 11; i++)
            {
                g.DrawLine(Pens.Black, new Point(0, i * 50), new Point(500, i * 50));
                g.DrawLine(Pens.Black, new Point(i * 50, 0), new Point(i * 50, 500));
            }
            g.FillRectangle(Brushes.LightPink, new Rectangle(0, 0, 50, 50));
            g.FillRectangle(Brushes.LightPink, new Rectangle(0, 450, 50, 50));
            Bitmap snakeImg = new Bitmap("snake.png");
            foreach (var snake in Snakes)
            {
                g.DrawImage(snakeImg, snake.Key.X * 50, (9 - snake.Key.Y) * 50, 50, (snake.Value + 1) * 50);
            }
            Bitmap ladderImg = new Bitmap("ladder.png");
            foreach (var ladder in Ladders)
            {
                g.DrawImage(ladderImg, ladder.Key.X * 50, (9 - ladder.Key.Y - ladder.Value) * 50 + 25, 50, ladder.Value * 50 + 10);
            }
            g.DrawString("START", SystemFonts.DefaultFont, Brushes.Red, new PointF(5, 470));
            g.DrawString("END", SystemFonts.DefaultFont, Brushes.Red, new PointF(10, 20));
            Board = bmp;
            pictureBox1.BackgroundImage = bmp;
        }
        private void GamePlayingScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }

        void DrawAllPlayers()
        {
            Bitmap bmp = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);
            Graphics g = Graphics.FromImage(bmp);
            for (int i = 0; i < PlayersLocation.Count; i++)
            {
                g.FillEllipse(new SolidBrush(PlayerColors[i]), new Rectangle(PlayersLocation[i].X * 50, (9 - PlayersLocation[i].Y) * 50, 50 - i, 50 - i));
            }
            pictureBox1.Image = bmp;
        }

        private void GamePlayingScreen_Paint(object sender, PaintEventArgs e)
        {
            DrawAllPlayers();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////YOUR CODE HERE///////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void btnRollTheDice_Click(object sender, EventArgs e)
        {
            //write the button code here:
            //1- disable "RollTheDice" button
            //2- generate random number and write it in textbox
            //3- after 3 sec move the player coin
            //4- check if new location is ladder or snake using gameBoard array and modify the new location based on the value of gameBoard[y,x] = 'S' or = 'L'
            //6- update the location of currentPlayer (to be modified in drawing)
            btnRollTheDice.Enabled = false;
            Random diceNumber = new Random();
            int horray = diceNumber.Next(1, 7);
            textBox1.Text = horray.ToString();
            textBox1.Update();
            btnRollTheDice.Update();
            Thread.Sleep(3000);
            Point location = calcnewPositions(PlayersLocation[myIndex].X, PlayersLocation[myIndex].Y, horray);
            Point winningLocation = new Point(0, 9);
            
            if (IsServer)
            {
                //call BroadCastLocation(0) as the server index is always 0 in the client list
                //call BroadCastWhoseTurn(0) to see which player will play after server
                if (location.X == winningLocation.X && location.Y == winningLocation.Y)
                {
                    SendTheWinnerIsMeToServer();
                }
                BroadCastLocation(0);
                DrawAllPlayers();
                BroadCastWhoseTurn(0);
            }

            else
            {
                //if final location is the winning location then call the function SendTheWinnerIsMeToServer()
                //else send the final location to server by calling SendLocationToServer()
                if(location.X==winningLocation.X&&location.Y==winningLocation.Y)
                    SendTheWinnerIsMeToServer();

                else
                    SendLocationToServer();
            }
        }
        private Point calcnewPositions(int x, int y, int dice)
        {
            int index = getCurrentPlayerIndex();
            if (y % 2 == 0)
            {
                x += dice;
                if (x >= 10)
                {
                    x = 9 - (x - 10);
                    y += 1;
                }
                char next_state = gameBoard[y, x];
                if (next_state == 'S')
                {
                    int num_rows = Snakes[new Point(x, y)];
                    y -= num_rows;
                    PlayersLocation[index] = new Point(x, y);
                    draw_new_positions(x, y);
                }
                else if (next_state == 'L')
                {

                    int num_rows = Ladders[new Point(x, y)];
                    y += num_rows;
                    PlayersLocation[index] = new Point(x, y);
                    draw_new_positions(x, y);
                }
                else
                {
                    PlayersLocation[index] = new Point(x, y);
                    draw_new_positions(x, y);
                }
            }
            else if (y % 2 == 1)
            {
                x -= dice;
                if (x < 0)
                {
                    x *= -1;
                    x -= 1;
                    y += 1;
                }
                if ((x <= 0 && y >= 9) || (y > 9 && x >= 0))
                {
                    x = 0;
                    y = 9;
                }
                char next_state = gameBoard[y, x];
                if (next_state == 'S')
                {
                    int num_rows = Snakes[new Point(x, y)];
                    y -= num_rows;
                    PlayersLocation[index] = new Point(x, y);
                    draw_new_positions(x, y);
                }
                else if (next_state == 'L')
                {

                    int num_rows = Ladders[new Point(x, y)];
                    y += num_rows;
                    PlayersLocation[index] = new Point(x, y);
                    draw_new_positions(x, y);
                }
                else
                {
                    PlayersLocation[index] = new Point(x, y);
                    draw_new_positions(x, y);
                }
            }
            Point location = new Point(x, y);
            return location;
        }
        private void draw_new_positions(int x, int y)
        {
            int index = getCurrentPlayerIndex();

            Bitmap bmp = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);
            Graphics g = Graphics.FromImage(bmp);
            g.FillEllipse(new SolidBrush(PlayerColors[index]), new Rectangle(x * 50, (9 - y) * 50, 50, 50));
            pictureBox1.Image = bmp;
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////CLIENT///////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void RecieveFromServer()
        {

            bool recieve = true;
            while (recieve)
            {
                byte[] byteArr = new byte[1024];
                int recv=currentPlayer.Receive(byteArr);
                
                //use the currentPlayer socket to recieve from the server

                //parse the recieved message

                string message = Encoding.ASCII.GetString(byteArr, 0, recv);

                string[] arr;
                arr = message.Split('#');


                //if turn message check if the IP matched with my IP
                //then check if currentPlayer boolean = true
                //enable "RollTheDice" button and play
                //else keep it disabled
                if (!message.Contains("#"))
                {
                    if (Clients[myIndex].Rank==int.Parse(message))
                    {
                        if (btnRollTheDice.InvokeRequired)
                            btnRollTheDice.Invoke(new MethodInvoker(delegate
                            {
                                btnRollTheDice.Enabled = true;

                            }));
                        else
                            btnRollTheDice.Enabled = true;
                    }
                }
                //if location message then update the location of player n
                //update client n location
                else if (arr.Length == 3)
                {
                    string[] ar = arr[0].Split(',');
                    PlayersLocation[int.Parse(arr[2])] = new Point(int.Parse(ar[0]), int.Parse(ar[1]));
                    Clients[int.Parse(arr[2])].location = PlayersLocation[int.Parse(arr[2])];
                    Thread.Sleep(3000);
                    DrawAllPlayers();
                }

                //if winning message
                //go to WinningForm with the playerNumber
                else if (arr.Length == 2)
                {
                    WinningForm win = new WinningForm(int.Parse(arr[1]));
                    if (this.InvokeRequired)
                        this.Invoke(new MethodInvoker(delegate
                        {
                            this.Visible = false;

                        }));
                    else
                        this.Visible = false;
                    recieve = false;
                    win.ShowDialog();

                }
            }
        }

        void SendLocationToServer()
        {
            //use the currentPlayer socket to send to server "PlayersLocation[myIndex]"
            //message should look like this:
            //IP#PlayersLocation[myIndex]#
            byte[] arr = Encoding.ASCII.GetBytes("l" + "#" + Clients[myIndex].Rank.ToString() + "#" + PlayersLocation[myIndex].X + "," + PlayersLocation[myIndex].Y + "#");

            currentPlayer.Send(arr);
        }
        void SendTheWinnerIsMeToServer()
        {
            //use the currentPlayer socket to send to server the winner message
            //message should look like this:
            //IP
            if (myIndex != 0)
            {
                byte[] arr = Encoding.ASCII.GetBytes(Clients[myIndex].Rank.ToString());
                currentPlayer.Send(arr);
            }
            else
            {
                BroadCastTheWinnerIs(myIndex);
                gameStillOn = false;
                WinningForm win = new WinningForm(myIndex);
                if (this.InvokeRequired)
                    this.Invoke(new MethodInvoker(delegate
                    {
                        this.Visible = false;

                    }));
                else
                    this.Visible = false;
                win.ShowDialog();

            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////SERVER///////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        void RecieveFromClients(Object client)
        {
            Client c = (Client)client;
            while(gameStillOn)
            { 
            //recieve message and parse it
            byte[] bytearr = new byte[1024];

            int recv = c.player.Receive(bytearr);
            string message = Encoding.ASCII.GetString(bytearr, 0, recv);

            //if Winning Message
            //call BroadCastTheWinnerIs(playerNumber)
            //go to WinningForm

            string[] arr = message.Split('#');
                if (!arr[0].Equals("l"))
                {
                    BroadCastTheWinnerIs(int.Parse(arr[0]));
                    gameStillOn = false;
                    WinningForm win = new WinningForm(int.Parse(arr[0]));
                    if (this.InvokeRequired)
                        this.Invoke(new MethodInvoker(delegate
                        {
                            this.Visible = false;

                        }));
                    else
                        this.Visible = false;
                    win.ShowDialog();

                }
                else
                {
                    //if LocationMessage
                    //call BraodCastLocation(player number)
                    //call BroadCastWhoseTurn(player number)
                    string[] loc = arr[2].Split(',');
                    PlayersLocation[int.Parse(arr[1])] = new Point(int.Parse(loc[0]), int.Parse(loc[1]));
                    BroadCastLocation(int.Parse(arr[1]));
                    BroadCastWhoseTurn(int.Parse(arr[1]));


                }
            }
        }
        void BroadCastLocation(int playerNumber)
        {
            //here send the mssage to all clients, containing the location of PlayersLocation[playerNumber] and attach its IP and playerNumber
            for (int i = 1; i < Clients.Count; i++)
            {
                byte[] bytearr = Encoding.ASCII.GetBytes(PlayersLocation[playerNumber].X + "," + PlayersLocation[playerNumber].Y + "#" + Clients[playerNumber].Rank.ToString() + "#" + playerNumber);
                Clients[i].player.Send(bytearr);
            }



        }
        void BroadCastWhoseTurn(int playerNumber)
        {
            //see in the client list which 1 has the turn to play after playerNumber
            //here send the message to all clients, containing the IP only

            int x;
            if (playerNumber == Clients.Count - 1)
                x = 0;
            else
                x = playerNumber + 1;
            if (x == 0)
            {
                if (btnRollTheDice.InvokeRequired)
                    btnRollTheDice.Invoke(new MethodInvoker(delegate
                    {
                        btnRollTheDice.Enabled = true;

                    }));
                else
                    btnRollTheDice.Enabled = true;
            }
            else
            {
                for (int i = 1; i < Clients.Count; i++)
                {
                    byte[] bytearr = Encoding.ASCII.GetBytes(Clients[x].Rank.ToString());
                    Clients[i].player.Send(bytearr);
                }
            }

        }
        void BroadCastTheWinnerIs(int playerNumber)
        {
            //send to all clients message, containing IP,playerNumber
            for (int i = 1; i < Clients.Count; i++)
            {
                byte[] bytearr = Encoding.ASCII.GetBytes(Clients[playerNumber].IP + "#" + playerNumber.ToString());
                Clients[i].player.Send(bytearr);
            }
        }
    }
}
