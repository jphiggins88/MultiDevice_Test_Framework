using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using System.Timers;

namespace Client_GUI
{
    public partial class Client_GUI : Form
    {

        public Socket client;


        public delegate void ReadFromServerDelegate(object sender, string cli);

        public delegate void IDboxUpdateDelegate(object sender, string cli);

        public delegate void button1_DisableDelegate(object sender, EventArgs e);


        public AsynchronousClient mAsyncClient;

        public Client_GUI()
        {
            InitializeComponent();
            mAsyncClient = new AsynchronousClient();
             
            //client.ReadFromServer += WriteToTextBox;
            mAsyncClient.ReadFromServer += WriteToTextBox;
            mAsyncClient.DisableButton1 += button1_Disable;
            mAsyncClient.UpdateIDBox += IDbox_update;
        }


        private void button1_Disable(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                //this.BeginInvoke(new ProcessDelegate(WriteToTextBox), new object[] { sender, e });
                this.Invoke(new button1_DisableDelegate(button1_Disable), new object[] { sender, e });
            }
            else
            {
                this.button1.Enabled = false;

            }
        }

        private void WriteToTextBox(object sender, string e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new ReadFromServerDelegate(WriteToTextBox), new object[] { sender, e });
            }
            else
            {
                this.listBox1.Items.Add(e);

                // If more than 10 messages have been logged, start removing the oldest when a new message is added.
                if(listBox1.Items.Count > 7)
                {
                    this.listBox1.Items.RemoveAt(0);
                }

                this.textBox3.Text = e;
            }
        }

        private void IDbox_update(object sender, string e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new IDboxUpdateDelegate(IDbox_update), new object[] { sender, e });
            }
            else
            {
                this.textBox_ClientID.Text = e;
            }
        }

        // Connect Button
        private void button1_Click(object sender, EventArgs e)
        {
            string ipFromTextbox = textBox1.Text;
            
            try
            {
                //sock.Connect(new IPEndPoint(IPAddress.Parse(textBox1.Text), 3));
                new Thread(() =>
                {
                    //the StartClient() function will initiate the connection and aloos return the client object it created
                    //pass in the ip address in the GUI text box
                    //client = AsynchronousClient.StartClient(ipFromTextbox);

                    // Start a client in the new AsyncClient object and return the client that is made
                    client = mAsyncClient.StartClient(ipFromTextbox);
                })
                { IsBackground = true }.Start();
            }
            catch
            {
                MessageBox.Show("Connection Failed");
            }
        }

        // Send Button
        private void button2_Click(object sender, EventArgs e)
        {
            string dataFromTextbox = textBox2.Text;

            try
            {
                //sock.Connect(new IPEndPoint(IPAddress.Parse(textBox1.Text), 3));
                new Thread(() =>
                {
                    // send the data in the GUI text box to the client that was just connected to the server
                    // DO NOT CHANGE THE TAGS or colon signatures or the server side parsing will not work correctly

                    //mAsyncClient.Send(client, mAsyncClient.thisClient.GUI_ID + " :: " + dataFromTextbox + "<EOF>");

                    // The line below is used for testing. It should be commented out for release.
                    mAsyncClient.Send(client, dataFromTextbox + "<EOF>");
                }).Start();
            }
            catch
            {
                MessageBox.Show("Send Failed");
            }
        }

        private void sendSN_test_Click(object sender, EventArgs e)
        {
            string dataFromTextbox = textBox4.Text;

            try
            {
                //sock.Connect(new IPEndPoint(IPAddress.Parse(textBox1.Text), 3));
                new Thread(() =>
                {
                    // send the data in the GUI text box to the client that was just connected to the server
                    // DO NOT CHANGE THE TAGS or colon signatures or the server side parsing will not work correctly
                    mAsyncClient.Send(client, "<sn>" + mAsyncClient.thisClient.GUI_ID + " :: " + dataFromTextbox + "<EOF>");

                    //read();
                }).Start();

            }
            catch
            {
                MessageBox.Show("Send Failed");
            }
        }
    }


    //class containing information about this client
    //coputer number, test group number, TestApplication version, ipaddress,...
    ////Date and Time connected, TestApp-GUI unique ID (may need to be assigned by Server).
    public class ThisClientInfo
    {
        // GUI ID will be assigned serverside and changed here
        public string GUI_ID = "unknown ID";
        public string thisIPaddress = "Ip_handledServerSide";
        public string dateAndTimeOfConnection = "time_handledServerSide";
        public string programName = "program_name";
        public string compNumber = "comp_xx";
        public string testGroupNumber = "testGroupNumber_xx";
        public string testAppVersion = "testAppVersion_x.x.xx";
        public string serialNumber = "ABC123456";
        public string status = "NA";
        public string percent = "NA";

        //standard socket End Of File tag. This must be present for the server to accept the message
        public string socketEOFtag = "<EOF>";

        public string ackTag = "<ACK>";

        //Tag to specify client info
        public string clientINFOtag = "<CLIENTINFO>";


        public string AllCLientInfo()
        {
            string concatenatedClientInfo = clientINFOtag
                                            + GUI_ID + ";; "
                                            + thisIPaddress + ";; "
                                            + dateAndTimeOfConnection + ";; "
                                            + "<pn>" + programName + ";; "
                                            + "<cn>" + compNumber + ";; " 
                                            + "<gn>" + testGroupNumber + ";; " 
                                            + "<tv>" + testAppVersion + ";; "
                                            + "<sn>" + serialNumber + ";; "
                                            + "<sts>" + status + ";; "
                                            + "<pct>" + percent + ";; "
                                            + socketEOFtag;

            return concatenatedClientInfo;
        }

        public void UpdateDateAndTime()
        {
            DateTime timeRightNow = DateTime.Now;

            dateAndTimeOfConnection = timeRightNow.ToString();
        }
    }


    // State object for receiving data from remote device.  
    public class StateObject
    {
        // Client socket.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 256;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();
    }

   /// <summary>
   /// A class to temporatily hold the socket to give it visibility to other functions
   /// </summary>
    public static class TempSocketHolder
    {
        public static Socket TempSocket;
    }

    public class AsynchronousClient
    {
        
        
        // The IP address for the remote device.
        //private const string specifiedIPAddress = "127.0.0.1";
        
        // The port number for the remote device.  
        private const int port = 11000;

        // Flag to denote the connection staus of this client. Is used to prevent dconnecting more than once
        //private bool thisClientIsConnected = false;

        // ManualResetEvent instances signal completion.  
        private static ManualResetEvent connectDone = new ManualResetEvent(false);
        private static ManualResetEvent sendDone = new ManualResetEvent(false);
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);

        public event EventHandler<string> ReadFromServer;
        public event EventHandler DisableButton1;
        public event EventHandler<string> UpdateIDBox;

        // The response from the remote device.  
        private static String response = String.Empty;

        public ThisClientInfo thisClient = new ThisClientInfo();

        // Acknowledge timer
        // This timer is used to allow a specified amount of time to pass to allw the clent to send an acknowledgement message
        // Once this time expires, the Server will act on the lack of acknowledgement
        public static double ackTimeLimit = 2000; // in milliseconds
        public System.Timers.Timer ackTimer = new System.Timers.Timer(ackTimeLimit);

        public static double heartBeatTimeLimit = 40000; // in milliseconds, 10sec
        //public static double heartBeatTimeLimit = 180000; // in milliseconds, 3min
        public System.Timers.Timer heartBeatTimer = new System.Timers.Timer(heartBeatTimeLimit);


        public virtual void OnReadFromServer(string client)
        {
            ReadFromServer?.Invoke(this, client);
        }

        public virtual void OnDisableButton1()
        {
            DisableButton1?.Invoke(this, EventArgs.Empty);
            
        }

        protected virtual void OnUpdateIDBox(string e)
        {
            UpdateIDBox?.Invoke(this, e);
        }

        //public Socket void StartClient(string ipAddressFromGUI)
        public Socket StartClient(string ipAddressFromGUI)
        {
            // Subscribe to the acknowledge timer so an interrupt will be called on timer elapse
            ackTimer.Elapsed += OnAckTimerElapsed;

            // Subscribe to the heart beat timer so an interrupt will be called on timer elapse
            heartBeatTimer.Elapsed += OnHeartBeatTimerElapsed;

            // Connect to a remote device.
            // Establish the remote endpoint for the socket.  
            //IPAddress ipAddress = ipHostInfo.AddressList[0];

            string specifiedIPAddress = ipAddressFromGUI;
            IPAddress ipAddress = IPAddress.Parse(specifiedIPAddress);


            IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

            // Create a TCP/IP socket.  
            Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            

            try
            {
                // Connect to the remote endpoint.  
                client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();

                // Release the socket.  
                //client.Shutdown(SocketShutdown.Both);
                //client.Close();

                //retun client so it can be used/passed into other fucntions from the Client_GUI form
                return client;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Client:: " + e.ToString());
                return client;
            }
        }

        public void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.  
                client.EndConnect(ar);
                // Once the client is connected, Disable the connect button in the GUI
                OnDisableButton1();
                //thisClientIsConnected = true;

                Debug.WriteLine("Client:: Socket connected to {0}", client.RemoteEndPoint.ToString());
                //MessageBox.Show("Client::\r\n\r\nSocket connected to {0}" + client.RemoteEndPoint.ToString());
                // Signal that the connection has been made.  
                connectDone.Set();

                // Create the state object.  
                StateObject state = new StateObject();
                state.workSocket = client;

                // Send info about this client to Server
                //thisClient.UpdateDateAndTime();

                connectDone.WaitOne();


                // Copy the current client socket to the TempSocketHolder so if can be referenced from the Hearbeat function
                TempSocketHolder.TempSocket = client;

                //as soon as the client connects to the server, send information about the client
                Send(client, thisClient.AllCLientInfo());
                sendDone.WaitOne();

                // Begin receiving the data from the remote device.  
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);

                // Start the heart beat timer. Allow a specified amount of time to recieve an acknowledge message from the server.
                heartBeatTimer.Enabled = true;
                heartBeatTimer.AutoReset = true;
                heartBeatTimer.Start();

            }
            catch (Exception e)
            {
                Debug.WriteLine("Client:: " + e.ToString());
            }
        }

        public void ReceiveCallback(IAsyncResult ar)
        {
            String content = String.Empty;
            // Retrieve the state object and the client socket
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket client = state.workSocket;

            try
            {
                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);

                ParseBytesRead(bytesRead, client, state, content);
            }
            catch (Exception e)
            {
                //
                //This is enetered into upon initial disconnection of the server (ex: the server randomly closes)
                //

                Debug.WriteLine("Client:: " + e.ToString());

                // Close the socket and release all system resources.
                client.Close();

                MessageBox.Show("Connection to Server was lost: \r\n\r\n" + e.ToString());
            }
        }

        public void Send(Socket client, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            try
            {
                // Begin sending the data to the remote device.  
                client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), client);

                // Start the acknowledgment timer when the data is being sent.
                // Don't start the timer if we are only sending an <ACK> to a previously received message.
                // We don't require and acknowledgement to the client's acknowledgement of the message that was just received from the Server
                if (!data.StartsWith("<ACK>"))
                {
                    // Start a timer. Allow a specified amount of time to recieve an acknowledge message from the client.
                    ackTimer.Enabled = true;
                    ackTimer.Start();
                }
            }
            catch (Exception e)
            {
                //This is enetered into if the client tries to send something AFTER the server is already closed

                Debug.WriteLine("Client:: " + e.ToString());
                MessageBox.Show("Connection to Server was lost: \r\n\r\n" + e.ToString());
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);
                Debug.WriteLine("Client::  Sent {0} bytes to server.", bytesSent);
                //MessageBox.Show("Client::\r\n\r\nSent {0} bytes to server." + bytesSent);

                // Signal that all bytes have been sent.  
                sendDone.Set();

            }
            catch (Exception e)
            {
                Debug.WriteLine("Client:: " + e.ToString());
                MessageBox.Show("Connection to Server was lost: \r\n\r\n" + e.ToString());
            }
        }


        public void ParseBytesRead(int bytesRead, Socket client, StateObject state, string content)
        {
            if (bytesRead > 0)
            {
                // There might be more data, so store the data received so far.  
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                content = state.sb.ToString();

                if (content.IndexOf("<EOF>") > -1)
                {
                    // All the data has been read from the server by the client. Display it on the console.  
                    Debug.WriteLine("Client:: " + "Read {0} bytes from Server socket. \n Data : {1}", content.Length, content);
                    //MessageBox.Show("Server::\r\n\r\nRead {0} bytes from socket. \n Data : {1}" + content.Length + content);

                    // If the content of the message contains the client's ID number designated by the server
                    if(content.StartsWith("<ID>"))
                    {
                        // remove the <ID> tag from the received message
                        string newString = content.Remove(0, 4);

                        // remove the <EOF> tag from the end
                        char[] trimChars = { '<', 'E', 'O', 'F', '>' };
                        newString = newString.TrimEnd(trimChars);
                        thisClient.GUI_ID = newString;

                        // Update GUI with client ID
                        OnUpdateIDBox(newString);
                    }

                    if (content.StartsWith("<ACK>"))
                    {
                        // The last message sent from the client to the server was acknowledged by the the server
                        // Stop the Acknowledgement timer so the timer elapsed interrupt will not be called
                        ackTimer.Stop();

                        // log that the last message sent was acknowledged
                        Debug.WriteLine("Client::  <ACK> received from Server");
                    }

                    // Acknowledge the message by sending back <ACK> to the server
                    // If the message itself is an <ACK> response, then do not send <ACK> back
                    if (!content.StartsWith("<ACK>"))
                    {
                        string message = "<ACK>" + thisClient.GUI_ID + "<EOF>";

                        Send(client, message);

                        Debug.WriteLine("Client:: " + message + " Sent to Server"); 
                    }
                    // Update the GUI with the received message
                    OnReadFromServer(content);

                    //clear the last message so only the new message is recorded
                    state.sb.Clear();

                }

                //keep listening for incoming messages
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);

            }
            else
            {
                // All the data has arrived; put it in response.  
                if (state.sb.Length > 1)
                {
                    //response = state.sb.ToString();
                    content = state.sb.ToString();
                }
                // Signal that all bytes have been received.  
                //receiveDone.Set();
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }


        }

        private void OnAckTimerElapsed(Object source, System.Timers.ElapsedEventArgs e)
        {
            // Stop the ackTimer
            ackTimer.Stop();

            // Send Alert email

            // Update GUI and mark unresponsive client as potantially dead...?

            // log "no acknowledgement received" message
            Debug.WriteLine("Client::  server did not <ACK> the last message");

            // pop up message saying no ack received...?
        }

        private void OnHeartBeatTimerElapsed(Object source, System.Timers.ElapsedEventArgs e)
        {
            // Whenever the heart beat timer elapses, send an "OK" message to the Server
            // DO NOT CHANGE THE TAGS or colon signatures or the server side parsing will not work correctly
            Send(TempSocketHolder.TempSocket, "<HB>" + thisClient.GUI_ID + " :: " + "<EOF>");
            
        }

        /// <summary>
        /// Receives a status from the test application's running test and sends the test status to the remote Server GUI. 
        /// Looks for (camelcase/no spaces) "percentUpdate:###", "complete", "running", or "failed", and acts accordingly.
        /// </summary>
        private void TestStatusUpdate(string status)
        {
            // The message received by the Server GUI will be split on every ";; "
            string message = "<STATUS>;; " + status + ";; " + thisClient.GUI_ID + ";; <EOF>";

            Send(TempSocketHolder.TempSocket, message);

        }

    }






}
