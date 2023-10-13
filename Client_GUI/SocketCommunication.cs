using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;


namespace Client_GUI
{
    public class ThisClientInfo
    {
        //standard socket End Of File tag. This must be present for the server to accept the message
        public string TAG_EOF = "<EOF>";
        public string TAG_ACK = "<ACK>";
        public string TAG_CLIENT_INFO = "<CLIENTINFO>";
        public string TAG_CLIENT_LOCATION = "<LOCATION>";
        public string TAG_STATUS = "<STATUS>";
        public string TAG_UPDATE_INITIAL_INFO = "<UPDATE>";

        // GUI ID will be assigned serverside and changed here
        public string GUI_ID { get; set; }
        public string thisIPaddress { get; set; }
        public string dateAndTimeOfConnection { get; set; }

        public string programName { get; set; }
        public string pcGroupNumber { get; set; }
        public string compNumber { get; set; }
        public string slotNumber { get; set; }
        public string testAppVersion { get; set; }
        public string serialNumber { get; set; }
        public string statusOfTest { get; set; }
        public double global_PercentComplete { get; set; }


        public string descriptionOfCurrentState { get; set; }

        public string testType { get; set; }
        public string modelNumber { get; set; }

        public string testGroupName { get; set; }
        public string globalErrorToSendToSocket { get; set; }
        public string testTypeAbbreviation { get; set; }
        public int totalCycles { get; set; }
        


        public ThisClientInfo()
        {
            GUI_ID = "unknown ID";
            thisIPaddress = "Ip_handledServerSide";
            dateAndTimeOfConnection = "time_handledServerSide";


            pcGroupNumber = "???";
            compNumber = "???";
            slotNumber = "???";

            programName = "???";
            testAppVersion = "???";
            serialNumber = "???";

            statusOfTest = "Not Started";
            global_PercentComplete = 0.0;




            descriptionOfCurrentState = "???";

            testType = "???";
            modelNumber = "???";

            testGroupName = "???";
            globalErrorToSendToSocket = "???";
            testTypeAbbreviation = "???";
            totalCycles = 0;
    }


        public string ConcatenateInitialClientInfo()
        {
            string concatenatedClientInfo = TAG_CLIENT_INFO
                                            + GUI_ID + ";; "
                                            + thisIPaddress + ";; "
                                            + dateAndTimeOfConnection + ";; "
                                            + "<pn>" + programName + ";; "
                                            + "<gn>" + pcGroupNumber + ";; "
                                            + "<cn>" + compNumber + ";; "
                                            + "<slt>" + slotNumber + ";; "
                                            + "<tv>" + testAppVersion + ";; "
                                            + TAG_EOF;

            return concatenatedClientInfo;
        }

        public string ConcatenateUpdatedInitialClientInfo()
        {
            string concatenatedClientLocationInfo = TAG_UPDATE_INITIAL_INFO
                                            + GUI_ID + ";; "
                                            + "<pn>" + programName + ";; "
                                            + "<sn>" + serialNumber + ";; "
                                            + "<sts>" + statusOfTest + ";; "
                                            + "<pct>" + global_PercentComplete + ";; "
                                            + TAG_EOF;

            return concatenatedClientLocationInfo;
        }

        public string ConcatenateClientLocation()
        {
            string concatenatedClientLocationInfo = TAG_CLIENT_LOCATION
                                            + GUI_ID + ";; "
                                            + thisIPaddress + ";; "
                                            + dateAndTimeOfConnection + ";; "
                                            + "<gn>" + pcGroupNumber + ";; "
                                            + "<cn>" + compNumber + ";; "
                                            + "<slt>" + slotNumber + ";; "
                                            + TAG_EOF;

            return concatenatedClientLocationInfo;
        }

        public string ConcatenateClientStatus()
        {
            string concatenatedClientStatusInfo = TAG_STATUS
                                            + GUI_ID + ";; "
                                            + "<sts>" + statusOfTest + ";; "
                                            + "<pct>" + global_PercentComplete + ";; "
                                            + TAG_EOF;

            return concatenatedClientStatusInfo;
        }

        public void UpdateDateAndTime()
        {
            DateTime timeRightNow = DateTime.Now;

            dateAndTimeOfConnection = timeRightNow.ToString();
        }
    }

    public class SocketCommunication
    {
        


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

            public const string TIME_MS = "HH:mm:ss.ffffff";

            

            ThisClientInfo thisClient;

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


            //public event EventHandler<string> UpdateIDBox;    // This method only works up to .NET 4.5. The system.EventHandler<T> delegate was changed after 4.5. Use Action instead.
            public event Action<object, string> UpdateIDBox;
            //public event EventHandler<string> ReadFromServer; // This method only works up to .NET 4.5. The system.EventHandler<T> delegate was changed after 4.5. Use Action instead.
            public event Action<object, string> ReadFromServer;

            public event EventHandler DisableButton1;
            public event EventHandler ChangeTextBoxToDisconnected;



            // The response from the remote device.  
            private static String response = String.Empty;

            //public ThisClientInfo thisClient = new ThisClientInfo();

            public bool isConnectedToServer = false;

            // Acknowledge timer
            // This timer is used to allow a specified amount of time to pass to allw the clent to send an acknowledgement message
            // Once this time expires, the Server will act on the lack of acknowledgement
            //public static double ackTimeLimit = 2000; // in milliseconds
            public static double ackTimeLimit = 200000000; // in milliseconds
            public System.Timers.Timer ackTimer = new System.Timers.Timer(ackTimeLimit);

            //public static double heartBeatTimeLimit = 60000; // in milliseconds, 1 min
            //public static double heartBeatTimeLimit = 40000; // in milliseconds,
            public static double heartBeatTimeLimit = 1800000000; // in milliseconds,
            public System.Timers.Timer heartBeatTimer = new System.Timers.Timer(heartBeatTimeLimit);



            public AsynchronousClient(ThisClientInfo thisClient)
            {
                this.thisClient = thisClient;
            }


            public virtual void OnReadFromServer(string client)
            {
                ReadFromServer?.Invoke(this, client);
            }

            public virtual void OnDisableButton1()
            {
                DisableButton1?.Invoke(this, EventArgs.Empty);

            }

            public virtual void OnChangeTextBoxToDisconnected()
            {
                ChangeTextBoxToDisconnected?.Invoke(this, EventArgs.Empty);

            }

            protected virtual void OnUpdateIDBox(string e)
            {
                UpdateIDBox?.Invoke(this, e);
            }

            //public Socket void StartClient(string ipAddressFromGUI)
            public Socket StartClient(string ipAddressFromGUI)
            {
                Console.WriteLine(DateTime.Now.ToString(TIME_MS) + "\tStartClient\r\n");
                // Subscribe to the acknowledge timer so an interrupt will be called on timer elapse
                ackTimer.Elapsed += OnAckTimerElapsed;

                // Subscribe to the heart beat timer so an interrupt will be called on timer elapse
                heartBeatTimer.Elapsed += OnHeartBeatTimerElapsed;

                string specifiedIPAddress = ipAddressFromGUI;
                IPAddress ipAddress = IPAddress.Parse(specifiedIPAddress);


                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP socket.  
                Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);



                try
                {
                    Console.WriteLine(DateTime.Now.ToString(TIME_MS) + "\tStartClient: BeginConnect\r\n");

                    // Connect to the remote endpoint.  
                    client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
                    connectDone.WaitOne();

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
                    Console.WriteLine(DateTime.Now.ToString(TIME_MS) + "\tConnectCallback\r\n");

                    // Retrieve the socket from the state object.  
                    Socket client = (Socket)ar.AsyncState;

                    Console.WriteLine(DateTime.Now.ToString(TIME_MS) + "\tConnectCallback: EndConnect\t complete the connection\r\n");
                    // Complete the connection.
                    client.EndConnect(ar);
                    // Once the client is connected, Disable the connect button in the GUI
                    OnDisableButton1();

                    Debug.WriteLine("Client:: Socket connected to {0}", client.RemoteEndPoint.ToString());

                    // Signal that the connection has been made.  
                    connectDone.Set();

                    isConnectedToServer = true;

                    // Create the state object.  
                    StateObject state = new StateObject();
                    state.workSocket = client;

                    // Send info about this client to Server
                    //thisClient.UpdateDateAndTime();

                    connectDone.WaitOne();

                    // Copy the current client socket to the TempSocketHolder so if can be referenced from the Hearbeat function
                    TempSocketHolder.TempSocket = client;

                    //as soon as the client connects to the server, send information about the client
                    Console.WriteLine(DateTime.Now.ToString(TIME_MS) + "\tConnectCallback: Send\tSend available client info\r\n");
                    Send(client, thisClient.ConcatenateInitialClientInfo());
                    sendDone.WaitOne();

                    // Begin receiving the data from the remote device.
                    Console.WriteLine(DateTime.Now.ToString(TIME_MS) + "\tConnectCallback: BeginReceive\tBegin receiving payload from server\r\n");
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
                Console.WriteLine(DateTime.Now.ToString(TIME_MS) + "\tReceiveCallback\r\n");

                String content = String.Empty;
                // Retrieve the state object and the client socket
                // from the asynchronous state object.  
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                try
                {
                    // Read data from the remote device.
                    Console.WriteLine(DateTime.Now.ToString(TIME_MS) + "\tReceiveCallback: EndReceive\tRead payload from server\r\n");

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

                    MessageBox.Show("Failed to Receive Callback.\r\nThe Connection to the Server was lost: \r\n\r\n" + e.ToString());

                    isConnectedToServer = false;

                    OnChangeTextBoxToDisconnected();
                }
            }


            public void Send(Socket client, String data)
            {
                if (isConnectedToServer == true)
                {
                    // Convert the string data to byte data using ASCII encoding.  
                    byte[] byteData = Encoding.ASCII.GetBytes(data);

                    try
                    {
                        // Begin sending the data to the remote device.
                        Console.WriteLine(DateTime.Now.ToString(TIME_MS) + "\tSend: BeginSend\tstart sending\r\n");
                        client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), client);

                        // Start the acknowledgment timer when the data is being sent.
                        // Don't start the timer if we are only sending an <ACK> to a previously received message.
                        // We don't require and acknowledgement to the client's acknowledgement of the message that was just received from the Server
                        if (!data.StartsWith("<ACK>"))
                        {
                            Console.WriteLine(DateTime.Now.ToString(TIME_MS) + "\tSend: not sending an <ACK> to a previous message\tTherefore, start the <ACK> timer\r\n");

                            // Start a timer. Allow a specified amount of time to recieve an acknowledge message from the client.
                            ackTimer.Enabled = true;
                            ackTimer.Start();
                        }
                    }
                    catch (Exception e)
                    {
                        //This is enetered into if the client tries to send something AFTER the server is already closed

                        Debug.WriteLine("Client:: " + e.ToString());
                        MessageBox.Show("Failed to Send.\r\nConnection to Server was lost: \r\n\r\n" + e.ToString());

                        isConnectedToServer = false;
                        OnChangeTextBoxToDisconnected();
                    }
                }
                else
                {
                    OnChangeTextBoxToDisconnected();
                }
            }


            private void SendCallback(IAsyncResult ar)
            {
                try
                {
                    Console.WriteLine(DateTime.Now.ToString(TIME_MS) + "\tSendCallback\r\n");

                    // Retrieve the socket from the state object.  
                    Socket client = (Socket)ar.AsyncState;

                    // Complete sending the data to the remote device.
                    Console.WriteLine(DateTime.Now.ToString(TIME_MS) + "\tSendCallback: EndSend\r\n");
                    int bytesSent = client.EndSend(ar);
                    Debug.WriteLine("Client::  Sent {0} bytes to server.", bytesSent);

                    // Signal that all bytes have been sent.  
                    sendDone.Set();

                }
                catch (Exception e)
                {
                    Debug.WriteLine("Client:: " + e.ToString());
                    MessageBox.Show("Send Callback.\r\nConnection to Server was lost: \r\n\r\n" + e.ToString());
                }
            }


            public void ParseBytesRead(int bytesRead, Socket client, StateObject state, string content)
            {
                Console.WriteLine(DateTime.Now.ToString(TIME_MS) + "\tParseBytesRead\r\n");

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
                        if (content.StartsWith("<ID>"))
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
                            Console.WriteLine(DateTime.Now.ToString(TIME_MS) + "\tParseBytesRead: <ACK> detected\tstopping ACK timer\r\n");

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
                            Console.WriteLine(DateTime.Now.ToString(TIME_MS) + "\tParseBytesRead: <ACK> NOT detected\tSince this isn't an ACK from server, we need to send an ACK. Sending ACK\r\n");

                            string message = "<ACK>" + thisClient.GUI_ID + "<EOF>";

                            Send(client, message);

                            Debug.WriteLine("Client:: " + message + " Sent to Server");
                        }
                        // Update the GUI with the received message
                        OnReadFromServer(content);

                        //clear the last message so only the new message is recorded
                        state.sb.Clear();

                    }

                    Console.WriteLine(DateTime.Now.ToString(TIME_MS) + "\tParseBytesRead: BeginReceive\r\n");

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
                    Console.WriteLine(DateTime.Now.ToString(TIME_MS) + "\tParseBytesRead: BeginReceive\r\n");
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                }
            }


            private void OnAckTimerElapsed(Object source, System.Timers.ElapsedEventArgs e)
            {
                Console.WriteLine(DateTime.Now.ToString(TIME_MS) + "\tOnAckTimerElapsed\tServer did not ACK last message. Stopping ACK timer\r\n");
                // Stop the ackTimer
                ackTimer.Stop();

                // log "no acknowledgement received" message
                Debug.WriteLine("Client::  server did not <ACK> the last message");

                // pop up message saying no ack received...?
            }


            private void OnHeartBeatTimerElapsed(Object source, System.Timers.ElapsedEventArgs e)
            {
                if (isConnectedToServer == true)
                {
                    Console.WriteLine(DateTime.Now.ToString(TIME_MS) + "\tOnHeartBeatTimerElapsed\tSending OK checkin message to server\r\n");

                    // Whenever the heart beat timer elapses, send an "OK" message to the Server
                    // DO NOT CHANGE THE TAGS or colon signatures or the server side parsing will not work correctly
                    Send(TempSocketHolder.TempSocket, "<HB>" + thisClient.GUI_ID + " :: " + "<EOF>");
                }
                else
                {
                    Debug.WriteLine("Heartbeat timer elapsed:\r\nIt looks like the connection to Server was lost..");
                }
            }

            /// <summary>
            /// Receives a status from the test and sends the test status to the remote Server GUI. 
            /// Looks for (camelcase/no spaces) "percentUpdate:###", "complete", "running", or "failed", and acts accordingly.
            /// </summary>
            private void TestStatusUpdate(string status)
            {
                // The message received by the Server GUI will be split on every ";; "
                string message = "<STATUS>;; " + status + ";; " + thisClient.GUI_ID + ";; <EOF>";

                Send(TempSocketHolder.TempSocket, message);

            }

            public void UpdateInitialSocketInfo(CustomEventArgs4 testInformation)
            {

                // Message must begin with "<UPDATE>" and be seperated with ";;" delimiters
                // The server will handle filtering values based on <##> tags. The message does not have to be sent in this order.
                // Only the location of the client ID matters. In this case, it is element [1] in the string array once it is split.
                string message = "<UPDATE>;; " + thisClient.GUI_ID + ";; " +
                    "<pn>" + testInformation.client_programName + ";; " +
                    "<sn>" + testInformation.client_serialNum + ";; " +
                    "<pg>" + testInformation.client_pcGroupNumber + ";; " +
                    "<cn>" + testInformation.client_compNum + ";; " +
                    "<slt>" + testInformation.client_slotNum + ";; " +
                    "<tv>" + testInformation.client_testAppVersion + ";; " +
                    "<tn>" + testInformation.client_testName + ";; " +
                    "<sts>" + testInformation.client_status + ";; " +
                    "<pct>" + testInformation.client_percent + ";; " +
                    "<cyc>" + testInformation.client_cycleCount + ";; " +
                    "<tt>" + testInformation.client_testType + ";; <EOF>";

                Console.WriteLine(DateTime.Now.ToString(TIME_MS) + "\tUpdateInitialSocketInfo\tsending inital info\r\n");

                Send(TempSocketHolder.TempSocket, message);
                // need to grab a specific socket to send this with
            }

            public void SendTestUpdateToServer(CustomEventArgs5_StatusOnly testInformation)
            {

                // Message must begin with "<STATUS>" and be seperated with ";;" delimiters
                // The server will handle filtering values based on <##> tags. The message does not have to be sent in this order.
                // Only the location of the client ID matters. In this case, it is element [1] in the string array once it is split.
                string message = "<STATUS>;; " + thisClient.GUI_ID + ";; " +
                    "<pg>" + testInformation.client_pcGroupNumber + ";; " +
                    "<cn>" + testInformation.client_compNum + ";; " +
                    "<slt>" + testInformation.client_slotNum + ";; " +
                    "<sts>" + testInformation.client_status + ";; " +
                    "<pct>" + testInformation.client_percent + ";; " +
                    "<cyc>" + testInformation.client_cycleCount + ";; " +
                    "<des>" + testInformation.client_descriptionOfState + ";; " +
                    "<pth>" + testInformation.client_pathToErrorLog + ";; <EOF>";

                Console.WriteLine(DateTime.Now.ToString(TIME_MS) + "\tSendTestUpdateToServer\tsending info to server\r\n");

                Send(TempSocketHolder.TempSocket, message);
                // need to grab a specific socket to send this with
            }

            public void SendManualCloseMessageToServer()
            {

                // Message must begin with "<STATUS>" and be seperated with ";;" delimiters
                // The server will handle filtering values based on <##> tags. The message does not have to be sent in this order.
                // Only the location of the client ID matters. In this case, it is element [1] in the string array once it is split.
                string message = "<STATUS>;; " + thisClient.GUI_ID + ";; " + "<sts>" + "manualClose" + ";; " + ";; <EOF>";

                Console.WriteLine(DateTime.Now.ToString(TIME_MS) + "\tSendManualCloseMessageToServer\tsending info to server\r\n");

                Send(TempSocketHolder.TempSocket, message);
            }

            /// <summary>
            /// Sends a command to the Server telling the server to copy the file in the "sourcePath" to the destination path.
            /// <br/>The "destinationPath" is specified by the Server and cannot be set by the GUI.
            /// <br/>Assuming no code changes on the Server end, the "destinationPath" is the address to the shared OneDrive Folder.
            /// </summary>
            /// <param name="sourcePath"></param>
            /// <param name="overWriteFlag"></param>
            public void SendFileTransferCommandToServer(string sourcePath, bool overWriteFlag)
            {

                // Message must begin with "<FILETRANSFER>" and be seperated with ";;" delimiters
                // The server will handle filtering values based on <##> tags. The message does not have to be sent in this order.
                // Only the location of the client ID matters. In this case, it is element [1] in the string array once it is split.

                string fileTransferFlag;

                if (overWriteFlag)
                {
                    Console.WriteLine(DateTime.Now.ToString(TIME_MS) + "\tSendFileTransferCommandToServer: <FILETRANSFER_OW>\tsending info to server\r\n");

                    fileTransferFlag = "<FILETRANSFER_OW>";
                }
                else
                {
                    Console.WriteLine(DateTime.Now.ToString(TIME_MS) + "\tSendFileTransferCommandToServer: <FILETRANSFER>\tsending info to server\r\n");

                    fileTransferFlag = "<FILETRANSFER>";
                }

                string message = fileTransferFlag + ";; " + thisClient.GUI_ID + ";; " + "<sp>" + sourcePath + ";; <EOF>";
                try
                {
                    Send(TempSocketHolder.TempSocket, message);
                }
                catch (Exception e)
                {
                    MessageBox.Show("There was a problem sending the file transfer command to Server.\r\n\r\n" + e);
                }
            }
        }
    }
}
