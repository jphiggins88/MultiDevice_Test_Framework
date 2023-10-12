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
using System.IO;

namespace Server_GUI
{
    // State object for reading client data asynchronously  
    public class StateObject
    {
        // Size of receive buffer.  
        public const int BufferSize = 1024;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
        // Client socket.
        public Socket workSocket = null;
        public int uniqueSocketID = 0;
    }

    public class AsynchronousSocketListener
    {
        public const string TIME_MS = "HH:mm:ss.ffffff";

        public const string TAG_PROGRAM_NAME = "<pn>";
        public const string TAG_TESTGROUP_NUM = "<gn>";
        public const string TAG_COMPUTER_NUM = "<cn>";
        public const string TAG_SLOT_NUM = "<slt>";
        public const string TAG_TESTAPP_VERSION = "<tv>";

        public const string TAG_TEST_TYPE = "<tt>";
        public const string TAG_TEST_NAME = "<tn>";

        public const string TAG_SERIAL_NUM = "<sn>";
        public const string TAG_STATUS = "<sts>";
        public const string TAG_PERCENT = "<pct>";
        public const string TAG_CYCLE_COUNT = "<cyc>";
        public const string TAG_DESCRIPTION = "<des>";
        public const string TAG_PATH_TO_ERROR_LOG = "<pth>";

        public const string TAG_END_OF_FILE = "<EOF>";
        public const string TAG_ACK_SPECIFIER = "<ACK>";
        public const string TAG_HEARTBEAT_SPECIFIER = "<HB>";
        public const string TAG_CLIENTINFO_SPECIFIER = "<CLIENTINFO>";
        public const string TAG_UPDATE_SPECIFIER = "<UPDATE>";
        public const string TAG_STATUS_SPECIFIER = "<STATUS>";
        public const string TAG_LOCATION_SPECIFIER = "<LOCATION>";
        public const string TAG_FILETRANSFER_SPECIFIER = "<FILETRANSFER>";
        public const string TAG_FILETRANSFER_OW_SPECIFIER = "<FILETRANSFER_OW>";

        public static string logFileName = "";

        public static string txtFileWithType2SharedFolderAddress = Application.StartupPath + @"\addressOfEssdSharedFolder.txt";
        public static string addressOfType2SharedFolder = GetAddressFromTextFile(txtFileWithType2SharedFolderAddress);

        public static string txtFileWithOneDriveSharedFolderAddress = Application.StartupPath + @"\addressOfOneDriveSharedFolder.txt";
        public static string addressOfOneDriveSharedFolder = GetAddressFromTextFile(txtFileWithOneDriveSharedFolderAddress);

        // Used to make  a new log file every week. Counts to 7, then will be reset to 1
        public int sevenDayCounter = 1;
        protected DateTime CurrentDay = DateTime.Today; ////initialize current day
        protected DateTime DayOfLastCycle = DateTime.Today; //initialize Day of last cycle day

        // Thread signal.  
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        private static ManualResetEvent sendDone = new ManualResetEvent(false);

        // object of the server GUI used to change the contents of the listBox
        public Server_GUI mMainForm;
        public Email_Sender emailSender = new Email_Sender();
        public ClientController mainClientController = new ClientController();
        Client lastClientConnected;
        Client targetClient;

        // Acknowledge timer
        // This timer is used to allow a specified amount of time to pass to allw the clent to send an acknowledgement message
        // Once this time expires, the Server will act on the lack of acknowledgement
        //public static double ackTimeLimit = 2000; // in milliseconds
        public static double ackTimeLimit = 60000000; // in milliseconds
        public System.Timers.Timer ackTimer = new System.Timers.Timer(ackTimeLimit);

        // Heart Beat timer
        // This timer is started when the 1'st client is added. If all clients are deleted, then it will be started again when the next "ist" client is added.
        // When the Timer elapses the server will loop through the client list and look for any client's who have not sent a heart beat
        //public static double heartBeatTimeLimit = 120000; // in milliseconds, 2min
        public static double heartBeatTimeLimit = 300000000; // in milliseconds, 5min
        public System.Timers.Timer heartBeatTimer_serverSide = new System.Timers.Timer(heartBeatTimeLimit);

        // Sets the initial time at which the daily status email will be sent. This can be changed in the GUI.
        static int hour = 15;
        static int minute = 36;

        public static DailyTrigger trigger = new DailyTrigger(hour, minute);


        // Socket Listener Constructor
        public AsynchronousSocketListener(Server_GUI mMainForm)
        {
            this.mMainForm = mMainForm;
            this.mMainForm.textBoxDailyUpdate_time.Text = hour.ToString() + ":" + minute.ToString();

            trigger.OnTimeTriggered += () =>
            {
                //MessageBox.Show("The Timer Has Triggered"); // for debug
                VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tOnTimeTriggered\tHeartbeat Timer was triggered\r\n");

                // Make sure the send email check box is checked and there is at least 1 client connected.
                if (this.mMainForm.sendEmailOnAlert_checkBox.Checked && mainClientController.Clients.Count > 0)
                {
                    string[] type2_emailBodyAndSubject = CreateSummaryOfAllTests_type2Devices();
                    string[] allDevices_emailBodyAndSubject = CreateSummaryOfAllTests_allTypes();

                    emailSender.SendDailyUpdateEmailForAllDevicesOfType2(type2_emailBodyAndSubject[0], type2_emailBodyAndSubject[1] );

                    emailSender.SendDailyUpdateEmailForAllDevices_AllTypes(allDevices_emailBodyAndSubject[0], allDevices_emailBodyAndSubject[1]);

                    VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tOnTimeTriggered\tEmail Sent : Daily Update/s sent\r\n");
                }
            };
        }


        #region delegates and event handlers

        public event EventHandler<CustomEventArgs> ReadFromClient;
        public event EventHandler<string> UpdateConnectedClientsList;
        public event EventHandler<CustomEventArgs2> UpdateConnectedClientsGridView;
        public event EventHandler<CustomEventArgs2_withTargetClient> UpdateConnectedClientsGridView_updateClient;
        public event EventHandler<CustomEventArgs3_withTargetClient> UpdateConnectedClients_BigPicture_updateClient;
        public event EventHandler<CustomEventArgs4_statusUpdates> UpdateConnectedClients_BigPicture_StatusOnly_updateClient;
        public event EventHandler<CustomEventArgs3> UpdateConnectedClientsGridView_status;
        public event EventHandler<string> UpdateConnectedClientsGridView_deleteClient;
        public event EventHandler<string> UpdateSerialNumber;

        protected virtual void OnReadFromClient(CustomEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            ReadFromClient?.Invoke(this, e);
        }

        protected virtual void OnUpdateConnectedClientsList(string e)
        {
            UpdateConnectedClientsList?.Invoke(this, e);
        }

        //(string id, string serialNum, string dateTime, string testGroupNum, string compNum, string slotNum, string programName, string testAppVersion, string status, string percent
        protected virtual void OnUpdateConnectedClientsGridView(CustomEventArgs2 e)
        {
            UpdateConnectedClientsGridView?.Invoke(this, e);
        }

        // (string id, string serialNum, string dateTime, string testGroupNum, string compNum, string slotNum, string programName, string testAppVersion, string status, string percent, int targetClient)
        protected virtual void OnUpdateConnectedClientsGridView_updateClient(CustomEventArgs2_withTargetClient e)
        {
            UpdateConnectedClientsGridView_updateClient?.Invoke(this, e);
        }

        // (string id, string serialNum, string dateTime,string testGroupNum, string compNum, string slotNum, string programName, string testAppVersion, string status, string percent, string serialNumberLastFour, string testType)
        protected virtual void OnUpdateConnectedClients_BigPicture_updateClient(CustomEventArgs3_withTargetClient e)
        {
            UpdateConnectedClients_BigPicture_updateClient?.Invoke(this, e);
        }

        // (string id, string testGroupNum, string compNum, string slotNum, string status, string percent, string cycleCount, string descriptionOfState, bool wasManuallyClosed)
        protected virtual void OnUpdateConnectedClients_BigPicture_StatusOnly_updateClient(CustomEventArgs4_statusUpdates e)
        {
            UpdateConnectedClients_BigPicture_StatusOnly_updateClient?.Invoke(this, e);
        }

        // (string clientID, string status, string percent)
        protected virtual void OnUpdateConnectedClientsGridView_status(CustomEventArgs3 e)
        {
            UpdateConnectedClientsGridView_status?.Invoke(this, e);
        }

        protected virtual void OnUpdateConnectedClientsGridView_deleteClient(string e)
        {
            UpdateConnectedClientsGridView_deleteClient?.Invoke(this, e);
        }

        protected virtual void OnUpdateSerialNumber(string e)
        {
            UpdateSerialNumber?.Invoke(this, e);
        }

        #endregion delegates and event handlers



        #region socket functions

        public void StartListening()
        {
            VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tStartListening\r\n");
            // Subscribe to the acknowledge timer so an interrupt will be called on timer elapse
            ackTimer.Elapsed += OnAckTimerElapsed;
            // Subscribe to the heart beat timer so an interrupt will be called on timer elapse
            heartBeatTimer_serverSide.Elapsed += OnHeartBeatTimerElapsed_ServerSide;
            
            // IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());

            // Listen for connections from any IP address
            IPAddress ipAddress = IPAddress.Any;
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true)
                {
                    allDone.Reset(); 
                    VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tStartListening: BeginAccept\r\n");
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener); 
                    allDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                VerboseLog(DateTime.Now.ToString(TIME_MS) + "\ttStartListening: EXCEPTION\tFailed to start listening with the following error:\r\n" + e);
            }
        }


        public void AcceptCallback(IAsyncResult ar)
        {
            allDone.Set(); 
            Socket listener = (Socket)ar.AsyncState;
            Socket newSocket = listener.EndAccept(ar);

            VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tAcceptCallback: Connection accepted.\tCreating a new socket to handle the new connection\r\n");
            StateObject state = new StateObject();
            state.workSocket = newSocket;
            VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tAcceptCallback: handler.BeginReceive\tMake the new socket listen for incoming messages\r\n");
            newSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }


        public void ReadCallback(IAsyncResult ar)
        {
            VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tReadCallBack\r\n");
            String content = String.Empty;
            String result = "--test--"; 
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            try
             {
                VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tReadCallBack: handler.EndReceive\tgetting payload\r\n");
                int bytesRead = handler.EndReceive(ar);
                VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tReadCallBack: ParseBytesRead\tparsing payload\r\n");
                ParseBytesRead(bytesRead, handler, state, content, result);
            }
            catch (Exception e)
            {
                VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tException encountered when trying to receive message from client - " + e.ToString() + "\tClosing the socket\r\n");
                VerboseLog(DateTime.Now.ToString(TIME_MS) + "\thandler.Close()\tClosing socket and releasing resources\r\n");
                handler.Close();

                int unresponsiveClientID = state.uniqueSocketID;
                VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tReadCallback: Unresponsive client ID :  " + unresponsiveClientID + "\r\n");

                string deadClientID;
                VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tSorting through client list for the unresponsive client.\t There are a total of " + mainClientController.Clients.Count + " clients in the list.");

                for (int i = 0; i < mainClientController.Clients.Count; i++)
                {
                    if (mainClientController.Clients[i]._unique_Id == unresponsiveClientID)
                    {
                        mainClientController.Clients[i]._isDead = true;

                        if (mainClientController.Clients[i]._status != "manualClose")
                        {
                            VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tmanualClose status detected\tMarking client as dead in big picture\r\n");
                            MarkClientAsDeadInBigPicture(i);
                            mainClientController.Clients[i]._status = "Unknown";
                            VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tUnknown status detected\tsending email All Info - " + mainClientController.Clients[i]._status + "\r\n");
                            SendAlertEmail_AllInfo(mainClientController.Clients[i], "");
                        }
                        VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tremoving client" + mainClientController.Clients[i]._unique_Id + "\r\n");
                        deadClientID = RemoveDeadClient(i); //TODO : is this handled elsewhere???
                    }
                } 
            }
        }


        private void SendAlertEmail_AllInfo(Client clientInfo, string pathToErrorLog)
        {
            if (this.mMainForm.sendEmailOnAlert_checkBox.Checked)
            {
                emailSender.SendAlert_AllInfo(clientInfo, pathToErrorLog);
                VerboseLog(DateTime.Now.ToString(TIME_MS) + "Email Sent regarding client : " + clientInfo._unique_Id + "\r\n");
            }
        }


        public void SendToAll(String data)
        {
            VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tSendToAll\r\n");
            foreach (Client client in mainClientController.Clients)
            {
                Send(client._socket, data);
            }
        }


        private void Send(Socket handler, String data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tSend --> sending::: " + data + "\r\n");
            handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);

            // Start the acknowledgment timer when the data is being sent. Don't start the timer if we are only sending an <ACK> to a previously received message.
            if (!data.StartsWith(TAG_ACK_SPECIFIER))
            {
                VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tSend prvious message is not replying to an <ACK>\t therefore starting ACK timer.\r\n");
                ackTimer.Enabled = true;
                ackTimer.Start();
            }
        }


        private void SendCallback(IAsyncResult ar)
        {
            VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tSendCallback\r\n");
            try
            {
                Socket handler = (Socket)ar.AsyncState;
                int bytesSent = handler.EndSend(ar);
                VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tSendCallback: handler.EndSend - Sent " + bytesSent + " bytes to client\r\n");  
                sendDone.Set();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Server:: " + e.ToString());
                VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tSendCallback: Exception: " + e.ToString() + "\r\n");
            }
        }


        private void OnAckTimerElapsed(Object source, System.Timers.ElapsedEventArgs e)
        {
            ackTimer.Stop();
            VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tOnAckTimerElapsed\tClient did not acknowledge the last message sent. Timer has been stopped\r\n");
        }


        private void OnHeartBeatTimerElapsed_ServerSide(Object source, System.Timers.ElapsedEventArgs e)
        {
            // If it remains false, the 2nd for loop will not be started
            bool deadClientsDetected = false;

            // loop through client list and make sure every connected client has checked in recently with a heart beat message
            for (int i = 0; i < mainClientController.Clients.Count; i++)
            {
                // If the client does NOT have a heart beat
                if (!mainClientController.Clients[i]._hasHeartBeat)
                {
                    // then see if it is a new client that hasn't had time to send a heartbeat. If so, don't add it to the dead list. Change it's "new" flag to false so
                    // it will be added to the dead list the next time the server side heart beat timeer elapses if it does not have a heart beat.
                    if (mainClientController.Clients[i]._isNewClient_ignoreHeartBeatOneTime)
                    {
                        VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tOnHeartBeatTimerElapsed_ServerSide, ignore ONE time for new client. Setting ignore once flag to false\r\n");
                        mainClientController.Clients[i]._isNewClient_ignoreHeartBeatOneTime = false;
                    }
                    else
                    {
                        deadClientsDetected = true;
                        VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tOnHeartBeatTimerElapsed_ServerSide, marking client as dead\r\n");
                        // add the client to the alert list by designating it as _isDead. One alert will be sent out including a list of all the unreachable clients
                        mainClientController.Clients[i]._isDead = true;
                    }
                }
            }

            if (deadClientsDetected == true)
            {
                Debug.WriteLine("Server:: areAnyClientsDead == true. Finding the ID of the dead client.\r\n\tThere are currently " + mainClientController.Clients.Count + " clients connected.");

                // log the missed heartbeat
                for (int j = 0; j < mainClientController.Clients.Count; j++)
                {
                    if (mainClientController.Clients[j]._isDead)
                    {
                        if (mainClientController.Clients[j]._status != "manualClose")
                        {
                            VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tOnHeartBeatTimerElapsed_ServerSide, MarkClientAsDeadInBigPicture\r\n");
                            MarkClientAsDeadInBigPicture(j);

                            mainClientController.Clients[j]._status = "Unknown";
                            if (this.mMainForm.sendEmailOnAlert_checkBox.Checked)
                            {
                                VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tOnHeartBeatTimerElapsed_ServerSide, SendAlertEmail_AllInfo\r\n");
                                SendAlertEmail_AllInfo(mainClientController.Clients[j], "");

                            }
                        }
                        Debug.WriteLine("Server:: Calling RemoveDeadClient() on client " + mainClientController.Clients[j]._unique_Id);

                        // remove client from the list and change the status in the GUI to unknown/disconnected
                        VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tOnHeartBeatTimerElapsed_ServerSide, RemoveDeadCLient():" + mainClientController.Clients[j]._unique_Id + "\r\n");
                        string deadClientID = RemoveDeadClient(j);

                        // Debug log the removal/disconnection
                        VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tServer:: Client " + deadClientID + " missed a heartbeat\r\n");
                    }
                }
            }
        }
        
        #endregion socket functions


        /// <summary>
        /// Removes the dead client from the client list and the dataGridView in the GUI.
        /// Obtains the dead client unique ID based off the client index passed in.
        /// </summary>
        /// <param name="clientIndex"></param>
        /// <returns>the unique client ID of the dead client.</returns>
        public string RemoveDeadClient(int clientIndex)
        {
            string deadClientID = mainClientController.Clients[clientIndex]._unique_Id.ToString();

            // Remove the client from the griddView in the GUI
            VerboseLog(DateTime.Now.ToString(TIME_MS) + "RemoveDeadClient -> calling OnUpdateConnectedClientsGridView_deleteClient delete form gridview" + deadClientID + "\r\n");
            OnUpdateConnectedClientsGridView_deleteClient(deadClientID.ToString());
            Debug.WriteLine("Server:: Attempting to remove:  " + deadClientID);

            // Remove the client from the client list at the index that contains the matching uniqueID
            VerboseLog(DateTime.Now.ToString(TIME_MS) + "RemoveDeadClient: Removing " + deadClientID + "\tcurrent client count:  " + mainClientController.Clients.Count);
            mainClientController.Clients.RemoveAt(clientIndex);
            VerboseLog(DateTime.Now.ToString(TIME_MS) + "RemoveDeadClient: Client Removed\tcurrent client count: " + mainClientController.Clients.Count);

            // After removing the client above, if there are no clients left in the list, then stop the heartbeat monitor
            if (mainClientController.Clients.Count == 0)
            {
                heartBeatTimer_serverSide.Stop();
                VerboseLog(DateTime.Now.ToString(TIME_MS) + "RemoveDeadClient: client count = 0\tstopping heartbeat timer");
            }

            return deadClientID;
        }


        public void MarkClientAsDeadInBigPicture(int i)
        {
            targetClient = mainClientController.Clients[i];
            targetClient._descriptionOfState = "Client is disconnected or unreachable";
            targetClient._status = "Unknown";
            VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tMarkClientAsDeadInBigPicture: Calling OnUpdateConnectedClients_BigPicture_StatusOnly_updateClient with eventArgs: CustomEventArgs4_statusUpdates\r\n");
            OnUpdateConnectedClients_BigPicture_StatusOnly_updateClient(new CustomEventArgs4_statusUpdates(
                                                       targetClient._unique_Id.ToString(),
                                                       targetClient._testGroupNumber,
                                                       targetClient._compNumber,
                                                       targetClient._slotNumber,
                                                       targetClient._status,
                                                       targetClient._percent,
                                                       targetClient._cycleCount,
                                                       targetClient._descriptionOfState,
                                                       targetClient._manuallyClosed
                                                       ));
        }


        #region logging and email sending

        protected string DateToFilename(string dateTodayString)
        {
            dateTodayString = dateTodayString.Replace("/", "-");
            dateTodayString = dateTodayString.Replace("\\", "-");
            dateTodayString = dateTodayString.Replace(" 12:00:00 AM", "");
            return dateTodayString;
        }

        /// <summary>
        /// Logs the data passed into the fucntion. Will change the log name every 7 days to keep the log files from becoming too large.
        /// </summary>
        /// <param name="data"></param>
        public void VerboseLog(string data)
        {
            // Get today's day
            this.CurrentDay = DateTime.Today;

            // If the day has changed
            if (this.CurrentDay != this.DayOfLastCycle)
            {
                // Incriment the counter
                sevenDayCounter++;

                if (sevenDayCounter > 7)
                {
                    // Reset the 7 day counter
                    sevenDayCounter = 1;
                    // change the file name to reflect the 7 day reset date
                    logFileName = DateToFilename(this.CurrentDay.ToString());
                }

                // Update the dayOfLastCycle so the if statement will be entered on the next day change 
                this.DayOfLastCycle = this.CurrentDay;
            }
            // reads the newLogFileNumber and incriments the log whenever there is a calendar day change 
            string filePath = Application.StartupPath + "\\logs" + "\\ServerLog_" + logFileName + ".log";
            LogData(filePath, data);
        }

        public void LogData(string path, string data)
        {
            FileStream fileStream;
            StreamWriter streamWriter;
            int retryNumber = 5;
            int retryDelay = 500;

            for (int i = 1; i <= retryNumber; i++)
            {
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));

                    if (System.IO.File.Exists(path))
                    {
                        fileStream = new FileStream(path, FileMode.Append, FileAccess.Write);
                    }
                    else
                    {
                        fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
                    }

                    streamWriter = new StreamWriter(fileStream);
                    streamWriter.WriteLine(data);
                    streamWriter.Close();

                    break;
                }
                catch (Exception ex) when (i <= retryNumber)
                {
                    Thread.Sleep(retryDelay);

                    if (i == retryNumber)
                    {
                        MessageBox.Show("Cannot Open File " + path + ex);
                    }

                    return;
                }
            }
        }


        /// <summary>
        /// Copy the appropriate log file/s from the LAN shared folder to the OneDrive shared folder.
        /// The files to be copied will be specified in the content of the sockets message and passed into this function.
        /// </summary>
        public void CopyLogFilesToOneDriveSharedFolder(string contents, bool overwriteFileIfExists)
        {
            string[] contentSplit = contents.Split(";; ");
            string clientID = contentSplit[1];

            for (int j = 0; j < contentSplit.Length; j++)
            {
                if (contentSplit[j].StartsWith("<sp>"))
                {
                    // Filter out the "<sp>", and the souce path text will begin immediately after.
                    string sourcePath = contentSplit[j].Remove(0, 4);
                    int indexOfDotLog = sourcePath.IndexOf(".log");

                    // Remove everythign after ".log" if it exists.
                    if (sourcePath.Length > (indexOfDotLog + 4))
                    {
                        sourcePath = sourcePath.Remove(indexOfDotLog + 4);
                    }
                    // Modify the base folder of the sourcePath and set it as the destinationPath. This will keep the general folder structure of the LAN shared drive.
                    string destinationPath = sourcePath.Replace(addressOfType2SharedFolder, addressOfOneDriveSharedFolder);
                    try
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));

                        VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tCopyLogFilesToOneDriveSharedFolder: Attempting to Copy File: " + sourcePath + " to: " + destinationPath + "\r\n");
                        File.Copy(sourcePath, destinationPath, overwriteFileIfExists);
                        VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tCopyLogFilesToOneDriveSharedFolder: Copy successful\r\n");

                    }
                    catch (IOException iox)
                    {
                        VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tCopyLogFilesToOneDriveSharedFolder: Copy Unsuccessful\r\n");
                        MessageBox.Show("There was a problem transferring the log file.\r\n\r\n" + iox);
                    }
                }
            }
        }


        public string[] CreateSummaryOfAllTests_type2Devices()
        {
            string bodyOfSummary = "";

            //emailSubject must not contain any return "\r" characters.
            // This will be overwritten if any errors were detected in any tests.
            // It will be overwritten with an Error prefix, followed by the serial numbers of all devices experiencing errors.
            string emailSubject = "All Devices Good, No Problems Detected.";

            bool allDevicesGood = true;
            bool bodyQuickDescriptionPrefixHasBeenAdded = false;

            for (int i = 0; i < mainClientController.Clients.Count; i++)
            {
                // Only parse type2 tests. Determined by test type.
                if (mainClientController.Clients[i]._testType == "VC" || mainClientController.Clients[i]._testType == "BT" || mainClientController.Clients[i]._testType == "PT")
                {

                    // If device is marked as dead, add it to the quick description
                    if (mainClientController.Clients[i]._isDead == true || mainClientController.Clients[i]._status == "Failed" || mainClientController.Clients[i]._status == "Unknown")
                    {
                        // If one or more devices are marked dead, this flag will be set to false.
                        allDevicesGood = false;

                        // Only add a "Problems Detected" prefix once.
                        if (bodyQuickDescriptionPrefixHasBeenAdded == false)
                        {
                            emailSubject = "Problems Detected with: ";
                            bodyQuickDescriptionPrefixHasBeenAdded = true;
                        }

                        emailSubject += mainClientController.Clients[i]._serialNumber.ToString() + ", ";
                    }

                    // This is else doesn't make sense. This should never be reached unless mainClientController.Clients[i] actually exists.
                    // Decide what happens if mainClientController.Clients[i] exists, but its details are still null.
                    if (mainClientController.Clients[i] != null)
                    {
                        string serialNumber = "NA";
                        string programName = "NA";
                        string testType = "NA";
                        string cycleCount = "NA";
                        string status = "NA";


                        if (mainClientController.Clients[i]._serialNumber != null)
                        {
                            serialNumber = mainClientController.Clients[i]._serialNumber.ToString();
                        }
                        if (mainClientController.Clients[i]._programName != null)
                        {
                            programName = mainClientController.Clients[i]._programName.ToString();
                        }
                        if (mainClientController.Clients[i]._testType != null)
                        {
                            testType = mainClientController.Clients[i]._testType.ToString();
                        }
                        if (mainClientController.Clients[i]._cycleCount != null)
                        {
                            cycleCount = mainClientController.Clients[i]._cycleCount.ToString();
                        }
                        if (mainClientController.Clients[i]._status != null)
                        {
                            status = mainClientController.Clients[i]._status.ToString();
                        }

                        bodyOfSummary += "\r\n" + serialNumber +
                            "\r\nProgram Name: " + programName +
                            ";\tTest Type: " + testType +
                            ";\tCycles: " + cycleCount +
                            "\tStatus: " + status;

                    }
                    else
                    {
                        bodyOfSummary += "\r\nNo type2 devices running.";
                    }

                }
            }
            string[] completeSummary = { bodyOfSummary, emailSubject };

            return completeSummary;
        }


        public string[] CreateSummaryOfAllTests_allTypes()
        {
            string bodyOfSummary = "";

            // emailSubject must not contain any return "\r" characters.
            // This will be overwritten if any errors were detected in any devices.
            // It will be overwritten with an Error prefix, followed by the serial numbers of all devices experiencing errors.
            string emailSubject = "All Devices Good, No Problems Detected.";

            bool allDevicesGood = true;
            bool bodyQuickDescriptionPrefixHasBeenAdded = false;
            string deviceType;

            for (int i = 0; i < mainClientController.Clients.Count; i++)
            {
                // Designate device type by the test it's running.
                if (mainClientController.Clients[i]._testType == "VC" || mainClientController.Clients[i]._testType == "BT" || mainClientController.Clients[i]._testType == "PT")
                {
                    deviceType = "type2";
                }
                else
                {
                    deviceType = "Standard";
                }

                // If device is marked as dead, add it to the quick description
                if (mainClientController.Clients[i]._isDead == true || mainClientController.Clients[i]._status == "Failed" || mainClientController.Clients[i]._status == "Unknown")
                {
                    // If one or more devices are marked dead, this flag will be set to false.
                    allDevicesGood = false;

                    // Only add a "Problems Detected" prefix once.
                    if (bodyQuickDescriptionPrefixHasBeenAdded == false)
                    {
                        emailSubject = "Problems Detected with: ";
                        bodyQuickDescriptionPrefixHasBeenAdded = true;
                    }

                    emailSubject += mainClientController.Clients[i]._serialNumber.ToString() + ", ";

                }

                // This is else doesn't make sense. This should never be reached unless mainClientController.Clients[i] actually exists.
                // Decide what happens if mainClientController.Clients[i] exists, but its details are still null.
                if (mainClientController.Clients[i] != null)
                {
                    string serialNumber = "NA";
                    string programName = "NA";
                    string testType = "NA";
                    string cycleCount = "NA";
                    string status = "NA";


                    if (mainClientController.Clients[i]._serialNumber != null)
                    {
                        serialNumber = mainClientController.Clients[i]._serialNumber.ToString();
                    }
                    if (mainClientController.Clients[i]._programName != null)
                    {
                        programName = mainClientController.Clients[i]._programName.ToString();
                    }
                    if (mainClientController.Clients[i]._testType != null)
                    {
                        testType = mainClientController.Clients[i]._testType.ToString();
                    }
                    if (mainClientController.Clients[i]._cycleCount != null)
                    {
                        cycleCount = mainClientController.Clients[i]._cycleCount.ToString();
                    }
                    if (mainClientController.Clients[i]._status != null)
                    {
                        status = mainClientController.Clients[i]._status.ToString();
                    }

                    bodyOfSummary += "\r\n" + deviceType + "\t\t" + serialNumber +
                        "\r\nProgram Name: " + programName +
                        ";\tTest Type: " + testType +
                        ";\tCycles: " + cycleCount +
                        "\tStatus: " + status;
                }
                else
                {
                    bodyOfSummary += "\r\nNo devices running.";
                }

            }
            string[] completeSummary = { bodyOfSummary, emailSubject };

            return completeSummary;
        }


        public static string GetAddressFromTextFile(string path)
        {
            try
            {
                StreamReader addressReader = File.OpenText(path);
                return addressReader.ReadLine();
            }
            catch
            {
                MessageBox.Show("filepath:" + path + " Not Present");
                return "fail";
            }
        }

        #endregion logging and email sending



        #region message parsing

        public static bool LookForTagInMessageAndSetClientData(Client client ,string stringSlice, string tag)
        {
            bool needToSendEmail = false;
            if (stringSlice.StartsWith(tag))
            {
                string newString = stringSlice.Remove(0, tag.Length);

                switch (tag)
                {
                    case (TAG_TESTGROUP_NUM):
                        client._testGroupNumber = newString;
                        break;
                    case (TAG_COMPUTER_NUM):
                        client._compNumber = newString;
                        break;
                    case (TAG_SLOT_NUM):
                        client._slotNumber = newString;
                        break;
                    case (TAG_TESTAPP_VERSION):
                        client._testAppVersion = newString;
                        break;
                    case (TAG_TEST_TYPE):
                        client._testType = newString;
                        break;
                    case (TAG_SERIAL_NUM):
                        client._serialNumber = newString;
                        client._serialNumberLastFour = client._serialNumber.Substring(client._serialNumber.Length - 4);
                        break;
                    case (TAG_STATUS):
                        client._status = newString;
                        if ((newString == "Failed") || (newString == "Unknown"))
                        {
                            needToSendEmail = true;
                        }
                        else if (newString == "manualClose")
                        {
                            client._manuallyClosed = true;
                        }
                        break;
                    case (TAG_PERCENT):
                        client._percent = newString;
                        break;
                    case (TAG_CYCLE_COUNT):
                        client._cycleCount = newString;
                        break;
                    case (TAG_DESCRIPTION):
                        client._descriptionOfState = newString;
                        break;
                    case (TAG_PATH_TO_ERROR_LOG):
                        client._ = newString;
                        break;
                    case (TAG_ACK_SPECIFIER):
                        client. = newString;
                        break;
                    case (TAG_UPDATE_SPECIFIER):
                        client. = newString;
                        break;
                    case (TAG_STATUS_SPECIFIER):
                        client. = newString;
                        break;
                    case (TAG_FILETRANSFER_SPECIFIER):
                        client. = newString;
                        break;
                    case (TAG_FILETRANSFER_OW_SPECIFIER):
                        client. = newString;
                        break;
                }
            }
            return needToSendEmail;
        }


        public void ParseBytesRead(int bytesRead, Socket handler, StateObject state, string content, string result)
        {
            VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tParseBytesRead\r\n");

            if (bytesRead > 0)
            {
                // There may be more data, so store the data received so far.  
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read more data.  
                content = state.sb.ToString();
                VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tParseBytesRead:  Read " + content.Length + " bytes from socket.\r\n");
                VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tParseBytesRead:  Data <-- " + content + "\r\n");

                // If it's the FIRST message after connection, then the "<CLIENTINFO>" tag will be present.
                // we need to read this info and extract information about the testApp version, computer#, testGroup#, testProgram
                if (content.StartsWith(TAG_CLIENTINFO_SPECIFIER))
                {
                    // add the socket object that is now connected to the new client to the list of clients.
                    // Set the StateObjects uniqueID to the same uniqueID given to the Client object.
                    // This ID will be used for identifying and disconnecting/removing sockets/clients.
                    state.uniqueSocketID = mainClientController.AddClient(handler);
                    VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tParseBytesRead: <CLIENTINFO> detected\tAdded this socket (" + state.uniqueSocketID + ") to connected client list\r\n");

                    // If the client count is exactly 1 after just adding the client using the line above, then it means
                    // the client count is not empty so the heartbeat timer should be started. It also means this is not 
                    // the 2nd, 3rd,....or 100th client, in which case, the timer would already be started.
                    // Start the heart beat timer
                    VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tParseBytesRead: <CLIENTINFO> detected\tChecking if this is the 1st and only socket/client connected\r\n");
                    if (mainClientController.Clients.Count == 1)
                    {
                        heartBeatTimer_serverSide.Enabled = true;
                        heartBeatTimer_serverSide.AutoReset = true;
                        VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tParseBytesRead: <CLIENTINFO> detected\t This is the only client detected. Starting Server-Heartbeat timer\r\n");
                        heartBeatTimer_serverSide.Start();
                    }

                    // send the newly added client the unique ID assigned by the server.
                    VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tParseBytesRead: <CLIENTINFO> detected\tsending this client its Server generated ID (" + state.uniqueSocketID + ")\r\n");
                    Send(handler, "<ID>" + state.uniqueSocketID.ToString() + TAG_END_OF_FILE);


                    // The client info message will arrive in this format:
                    //<CLIENTINFO>;; thisIPaddress;; dateAndTimeOfConnection;; programName;; compNumber;; testGroupNumber;; testAppVersion;; <EOF>;
                    //Parse out the program name, computer number, testGroup number, and testApp version
                    // The device Serial Number and test type (VC-voltagecheck, BT-boottest, PT-powertest...) will be sent later once the individual testApp GUI retrievs it from the device

                    string[] contentSplit = content.Split(";; ");

                    // loop through all the clients in the list and look for the last client connected
                    for (int i = 0; i < mainClientController.Clients.Count; i++)
                    {
                        // if the unique id of the client matches the last client connected...
                        if (mainClientController.Clients[i]._unique_Id == mainClientController.lastClientConnectedID)
                        {
                            // designate that Client as the last client connected
                            lastClientConnected = mainClientController.Clients[i];

                            //loop through the string array and look for specific tags to parse information
                            for (int j = 0; j < contentSplit.Length; j++)
                            {
                                LookForTagInMessageAndSetClientData(lastClientConnected, contentSplit[j], TAG_PROGRAM_NAME);
                                LookForTagInMessageAndSetClientData(lastClientConnected, contentSplit[j], TAG_TESTGROUP_NUM);
                                LookForTagInMessageAndSetClientData(lastClientConnected, contentSplit[j], TAG_COMPUTER_NUM);
                                LookForTagInMessageAndSetClientData(lastClientConnected, contentSplit[j], TAG_SLOT_NUM);
                                LookForTagInMessageAndSetClientData(lastClientConnected, contentSplit[j], TAG_TESTAPP_VERSION);
                                LookForTagInMessageAndSetClientData(lastClientConnected, contentSplit[j], TAG_SERIAL_NUM);
                                LookForTagInMessageAndSetClientData(lastClientConnected, contentSplit[j], TAG_STATUS);
                                LookForTagInMessageAndSetClientData(lastClientConnected, contentSplit[j], TAG_PERCENT);

                                {
                                    /*Old Method
                                    // if the <pn> tag is detected, then delete that tag and set the Clients programName
                                    // <pn> stands for program name
                                    if (contentSplit[j].StartsWith("<pn>"))
                                    {
                                        //contentSplit[j].Replace("<pn>", "");
                                        string newString = contentSplit[j].Remove(0, 4);
                                        lastClientConnected._programName = newString;
                                    }
                                    // <gn> stands for testGroupNumber number
                                    else if (contentSplit[j].StartsWith("<gn>"))
                                    {
                                        //contentSplit[j].Replace("<gn>", "");
                                        string newString = contentSplit[j].Remove(0, 4);
                                        lastClientConnected._testGroupNum = newString;
                                    }

                                    // <cn> stands for computer number
                                    else if (contentSplit[j].StartsWith("<cn>"))
                                    {
                                        //contentSplit[j].Replace("<cn>", "");
                                        string newString = contentSplit[j].Remove(0, 4);
                                        lastClientConnected._compNumber = newString;
                                    }
                                    // <slt> stands for Slot Number
                                    else if (contentSplit[j].StartsWith("<slt>"))
                                    {
                                        //contentSplit[j].Replace("<tv>", "");
                                        string newString = contentSplit[j].Remove(0, 5);
                                        lastClientConnected._slotNumber = newString;
                                    }
                                    // <tv> stands for Torture stand version
                                    else if (contentSplit[j].StartsWith("<tv>"))
                                    {
                                        //contentSplit[j].Replace("<tv>", "");
                                        string newString = contentSplit[j].Remove(0, 4);
                                        lastClientConnected._testAppVersion = newString;
                                    }
                                    // <sn> stands for serial number
                                    else if (contentSplit[j].StartsWith("<sn>"))
                                    {
                                        //contentSplit[j].Replace("<tv>", "");
                                        string newString = contentSplit[j].Remove(0, 4);
                                        lastClientConnected._serialNumber = newString;
                                    }
                                    // <sts> stands for status
                                    else if (contentSplit[j].StartsWith("<sts>"))
                                    {
                                        //contentSplit[j].Replace("<tv>", "");
                                        string newString = contentSplit[j].Remove(0, 5);
                                        lastClientConnected._status = newString;
                                    }
                                    // <pct> stands for percent
                                    else if (contentSplit[j].StartsWith("<pct>"))
                                    {
                                        //contentSplit[j].Replace("<tv>", "");
                                        string newString = contentSplit[j].Remove(0, 5);
                                        lastClientConnected._percent = newString;
                                    }
                                     */
                                }
                            }

                            // TODO make a member funtion of Client to concatenate all information within the object itself and return it as shown below.
                            string connectedClientInformation = "ClientID:: " + lastClientConnected._unique_Id.ToString() + " :: " +
                                                            lastClientConnected._thisIPaddress.ToString() + " :: " +
                                                            lastClientConnected._dateAndTimeOfConnection.ToString() + " :: " +
                                                            lastClientConnected._programName + " :: " +
                                                            lastClientConnected._testGroupNumber + " :: " +
                                                            lastClientConnected._compNumber + " :: " +
                                                            lastClientConnected._slotNumber + " :: " +
                                                            lastClientConnected._testAppVersion + " :: " +
                                                            lastClientConnected._serialNumber + " :: " +
                                                            lastClientConnected._status + " :: " +
                                                            lastClientConnected._percent;

                            VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tParseBytesRead: <CLIENTINFO> detected\tParsed connected Client Info from last client payload: " + connectedClientInformation + "\r\n");

                            // update the connectedClient Listbox in the GUI
                            VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tParseBytesRead: <CLIENTINFO> detected, calling OnUpdateConnectedClientsList, passing in connected Client Information Above\tUpdating connected client List in GUI\r\n");
                            OnUpdateConnectedClientsList(connectedClientInformation);

                            VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tParseBytesRead: <CLIENTINFO> detected, calling OnUpdateConnectedClientsGridView, passing in CustomEventArgs2\tUpdating connected client GridView in GUI\r\n");
                            OnUpdateConnectedClientsGridView(new CustomEventArgs2(
                                                            lastClientConnected._unique_Id.ToString(), 
                                                            lastClientConnected._serialNumber,
                                                            lastClientConnected._dateAndTimeOfConnection.ToString(),
                                                            lastClientConnected._testGroupNumber,
                                                            lastClientConnected._compNumber,
                                                            lastClientConnected._slotNumber,
                                                            lastClientConnected._programName,
                                                            lastClientConnected._testAppVersion,
                                                            lastClientConnected._status,
                                                            lastClientConnected._percent
                                                            ));
                            break;
                        }
                    }
                }

                // If the End of File tag is detected, then read the message payload
                if (content.IndexOf(TAG_END_OF_FILE) > -1)
                {
                    VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tParseBytesRead: (<EOF> > -1) detected\tpayload not empty\r\n");
                    VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tParseBytesRead:  Read " + content.Length + " bytes from socket.\r\n");
                    VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tParseBytesRead:  Data <-- " + content + "\r\n");

                    // Acknowledge the message by sending back <ACK> to the server. If the message itself is an <ACK> response, then do not send <ACK> back
                    if (!content.StartsWith(TAG_ACK_SPECIFIER))
                    {
                        // Acknowledge the message by sending back <ACK> to the client
                        VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tParseBytesRead: (<EOF> > -1) detected: No <ACK> detected\tNot an ACK from client, so Sending <ACK> to client\r\n");
                        Send(handler, TAG_ACK_SPECIFIER + TAG_END_OF_FILE);
                    }

                    VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tParseBytesRead: (<EOF> > -1) detected\tReading message payload\r\n");
                    OnReadFromClient(new CustomEventArgs(handler.Handle.ToString(), result, "", content));  // Why is this called again. isn't everything read

                    if(content.StartsWith(TAG_SERIAL_NUM))
                    {
                        string newString = content.Remove(0, 4);
                        VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tParseBytesRead: (<EOF> > -1) detected: <sn> detected\tUpdating SN\r\n");
                        OnUpdateSerialNumber(newString);
                    }
                    if (content.StartsWith(TAG_ACK_SPECIFIER))
                    {
                        // The last message sent from the server to the client was acknowledged by the the client
                        // Stop the Acknowledgement timer so the timer elapsed interrupt will not be called
                        VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tParseBytesRead: (<EOF> > -1) detected: <ACK> detected\tstopping ACK timer\r\n");
                        ackTimer.Stop();

                        // log that the last message sent was acknowledged
                        Debug.WriteLine("Server::  <ACK> received from client");
                    }
                    if (content.StartsWith(TAG_UPDATE_SPECIFIER))
                    {
                        VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tParseBytesRead: (<EOF> > -1) detected: <UPDATE> detected\tParsing Initial Update\r\n");

                        ParseInitialUpdateOfTest(content);
                    }
                    if (content.StartsWith(TAG_STATUS_SPECIFIER))
                    {
                        VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tParseBytesRead: (<EOF> > -1) detected: <STATUS> detected\tParsing Status Update\r\n");
                        ParseStatusOfTest(content);
                    }
                    if (content.StartsWith(TAG_FILETRANSFER_SPECIFIER) && this.mMainForm.transferLogsDaily_checkBox.Checked)
                    {
                        VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tParseBytesRead: (<EOF> > -1) detected: <FILETRANSFER> detected\tCopying logs to shared folder\r\n");

                        CopyLogFilesToOneDriveSharedFolder(content, false);
                    }
                    if (content.StartsWith(TAG_FILETRANSFER_OW_SPECIFIER) && this.mMainForm.transferLogsDaily_checkBox.Checked) 
                    {
                        VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tParseBytesRead: (<EOF> > -1) detected: <FILETRANSFER_OW> detected\tCopying/overwriting logs to shared folder\r\n");

                        CopyLogFilesToOneDriveSharedFolder(content, true);
                    }

                    // If the client is sending a PC/Slot update, then parse the data and update the GUI.
                    if (content.StartsWith(TAG_LOCATION_SPECIFIER))
                    {
                        VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tParseBytesRead: (<EOF> > -1) detected: <LOCATION> detected\tParsing Location\r\n");

                        ParseLocationOfTests(content);
                    }

                    // If the received message is a <HB> for heart beat
                    if (content.StartsWith(TAG_HEARTBEAT_SPECIFIER))
                    {
                        VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tParseBytesRead: (<EOF> > -1) detected: <HB> detected\tSetting this client's _hasHearBeat to True\r\n");
                        string newString = content.Remove(0, 4);
                        char[] trimChars = { ' ', ':', ':', ' ', '<', 'E', 'O', 'F', '>' };
                        string idOfCLient = newString.TrimEnd(trimChars);

                        // Loop through the client list to find the correct client and set it's heart beat to true
                        for (int i = 0; i < mainClientController.Clients.Count; i++)
                        {
                            if (mainClientController.Clients[i]._unique_Id.ToString() == idOfCLient)
                            {
                                mainClientController.Clients[i]._hasHeartBeat = true;
                                VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tParseBytesRead: (<EOF> > -1) detected: <HB> detected\tThis client's _hasHearBeat set to True\r\n");

                                break;
                            }
                        }
                    }
                    //clear the last message so only the new message is recorded
                    state.sb.Clear();

                    //keep listening for incoming messages
                    VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tParseBytesRead: (<EOF> > -1) detected: BeginReceive\tKeep listening for incoming messages\r\n");
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
                }
                else
                {
                    // Not all data received. Get more.  
                    VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tParseBytesRead: <EOF> NOT detected: BeginReceive\tPartial payload received. Keep listening for the rest of the incoming messages\r\n");
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
                }
            }
        } 
        
        
        /// <summary>
        /// Receives an update of a test device from a client once the client has completed a cycle and 
        /// can send the relevant information that it did not have upon connection. 
        /// </summary>
        public void ParseInitialUpdateOfTest(string status)
        {
            VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tParseInitialUpdateOfTest - " + status + "\r\n");
            string[] contentSplit = status.Split(";; ");
            string clientID = contentSplit[1];
            
            for (int i = 0; i < mainClientController.Clients.Count; i++)
            {
                if (mainClientController.Clients[i]._unique_Id.ToString() == clientID)
                {
                    targetClient = mainClientController.Clients[i];

                    //loop through the string array and look for specific tags to parse information
                    for (int j = 0; j < contentSplit.Length; j++)
                    {
                        LookForTagInMessageAndSetClientData(targetClient, contentSplit[j], TAG_PROGRAM_NAME);
                        LookForTagInMessageAndSetClientData(targetClient, contentSplit[j], TAG_COMPUTER_NUM);
                        LookForTagInMessageAndSetClientData(targetClient, contentSplit[j], TAG_TESTGROUP_NUM);
                        LookForTagInMessageAndSetClientData(targetClient, contentSplit[j], TAG_TESTAPP_VERSION);
                        LookForTagInMessageAndSetClientData(targetClient, contentSplit[j], TAG_TEST_NAME);
                        LookForTagInMessageAndSetClientData(targetClient, contentSplit[j], TAG_SERIAL_NUM);
                        LookForTagInMessageAndSetClientData(targetClient, contentSplit[j], TAG_STATUS);
                        LookForTagInMessageAndSetClientData(targetClient, contentSplit[j], TAG_PERCENT);
                        LookForTagInMessageAndSetClientData(targetClient, contentSplit[j], TAG_CYCLE_COUNT);
                        LookForTagInMessageAndSetClientData(targetClient, contentSplit[j], TAG_SLOT_NUM);
                        LookForTagInMessageAndSetClientData(targetClient, contentSplit[j], TAG_TEST_TYPE);

                        {
                            /* Old Method
                            // if the <pn> tag is detected, then delete that tag and set the Clients programName
                            // <pn> stands for program name
                            if (contentSplit[j].StartsWith("<pn>"))
                            {
                                //contentSplit[j].Replace("<pn>", "");
                                string newString = contentSplit[j].Remove(0, 4);
                                targetClient._programName = newString;
                            }
                            // <cn> stands for computer number
                            else if (contentSplit[j].StartsWith("<cn>"))
                            {
                                //contentSplit[j].Replace("<cn>", "");
                                string newString = contentSplit[j].Remove(0, 4);
                                targetClient._compNumber = newString;
                            }
                            // <gn> stands for testGroupNum
                            else if (contentSplit[j].StartsWith("<gn>"))
                            {
                                //contentSplit[j].Replace("<gn>", "");
                                string newString = contentSplit[j].Remove(0, 4);
                                targetClient._testGroupNum = newString;
                            }
                            // <tv> stands for Torture stand version
                            else if (contentSplit[j].StartsWith("<tv>"))
                            {
                                //contentSplit[j].Replace("<tv>", "");
                                string newString = contentSplit[j].Remove(0, 4);
                                targetClient._testAppVersion = newString;
                            }
                            // <tn> stands for Test Name (ex: VC, BT, PT...)
                            else if (contentSplit[j].StartsWith("<tn>"))
                            {
                                //contentSplit[j].Replace("<tv>", "");
                                string newString = contentSplit[j].Remove(0, 4);
                                targetClient._testName = newString;
                            }
                            // <sn> stands for serial number
                            else if (contentSplit[j].StartsWith("<sn>"))
                            {
                                string newString = contentSplit[j].Remove(0, 4);
                                targetClient._serialNumber = newString;
                                // Extract the last 4 digits of the SN from the whole SN and save it as a property of the client.
                                targetClient._serialNumberLastFour = targetClient._serialNumber.Substring(targetClient._serialNumber.Length - 4);
                            }
                            // <sts> stands for status
                            else if (contentSplit[j].StartsWith("<sts>"))
                            {
                                string newString = contentSplit[j].Remove(0, 5);
                                targetClient._status = newString;
                            }
                            // <pct> stands for percent
                            else if (contentSplit[j].StartsWith("<pct>"))
                            {
                                string newString = contentSplit[j].Remove(0, 5);
                                targetClient._percent = newString;
                            }
                            // <cyc> stands for cycle count
                            else if (contentSplit[j].StartsWith("<cyc>"))
                            {
                                string newString = contentSplit[j].Remove(0, 5);
                                targetClient._cycleCount = newString;
                            }
                            // <slt> stands for Slot Number
                            else if (contentSplit[j].StartsWith("<slt>"))
                            {
                                string newString = contentSplit[j].Remove(0, 5);
                                targetClient._slotNumber = newString;
                            }
                            // <slt> stands for Test Type (VC, BT, PT...)
                            else if (contentSplit[j].StartsWith("<tt>"))
                            {
                                string newString = contentSplit[j].Remove(0, 4);
                                targetClient._testType = newString;
                            }
                            */
                        }
                    }

                    string connectedClientInformation = "ClientID:: " + targetClient._unique_Id.ToString() + " :: " +
                                                        targetClient._thisIPaddress.ToString() + " :: " +
                                                        targetClient._dateAndTimeOfConnection.ToString() + " :: " +
                                                        targetClient._programName + " :: " +
                                                        targetClient._testGroupNumber + " :: " +
                                                        targetClient._compNumber + " :: " +
                                                        targetClient._slotNumber + " :: " +
                                                        targetClient._testAppVersion + " :: " +
                                                        targetClient._serialNumber + " :: " +
                                                        targetClient._status + " :: " +
                                                        targetClient._percent;


                    VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tParseInitialUpdateOfTest: connectedClientInformation string created: " + connectedClientInformation + "\r\n");
                    VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tParseInitialUpdateOfTest: calling OnUpdateConnectedClientsList, passing in connected Client Information Above\tUpdating connected client List in GUI\r\n");

                    OnUpdateConnectedClientsList(connectedClientInformation);

                    int rows = this.mMainForm.gridView_clientQueue.Rows.Count;
                    string clientIdToLookFor = targetClient._unique_Id.ToString();
                    int rowToReplace;

                    for (int j = 0; j < (rows-1); j++)  // rows - 1 because the last row will always be empty.
                    {
                        try
                        {
                            string cellInGridViewWithClientId = this.mMainForm.gridView_clientQueue.Rows[j].Cells[0].Value.ToString();
                            
                            if (cellInGridViewWithClientId == clientIdToLookFor)
                            {
                                rowToReplace = j;

                                VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tParseInitialUpdateOfTest: calling OnUpdateConnectedClientsGridView_updateClient, passing in CustomEventArgs2_withTargetClient\r\n");

                                OnUpdateConnectedClientsGridView_updateClient(new CustomEventArgs2_withTargetClient(
                                                        targetClient._unique_Id.ToString(),
                                                        targetClient._serialNumber,
                                                        targetClient._dateAndTimeOfConnection.ToString(),
                                                        targetClient._testGroupNumber,
                                                        targetClient._compNumber,
                                                        targetClient._slotNumber,
                                                        targetClient._programName,
                                                        targetClient._testAppVersion,
                                                        targetClient._status,
                                                        targetClient._percent,
                                                        //include the row to replace in the GUI
                                                        rowToReplace
                                                        ));

                                VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tParseInitialUpdateOfTest: calling OnUpdateConnectedClients_BigPicture_updateClient, passing in CustomEventArgs3_withTargetClient\r\n");

                                OnUpdateConnectedClients_BigPicture_updateClient(new CustomEventArgs3_withTargetClient(
                                                       targetClient._unique_Id.ToString(),
                                                       targetClient._serialNumber,
                                                       targetClient._dateAndTimeOfConnection.ToString(),
                                                       targetClient._testGroupNumber,
                                                       targetClient._compNumber,
                                                       targetClient._slotNumber,
                                                       targetClient._programName,
                                                       targetClient._testAppVersion,
                                                       targetClient._status,
                                                       targetClient._percent,

                                                        targetClient._serialNumberLastFour,
                                                        targetClient._testType
                                                       ));

                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tParseInitialUpdateOfTest: Exception: Could not get value in specified row/column of the gridView\r\n");
                            MessageBox.Show("Could not get value in specified row/column of the gridView\r\n" + ex);
                        }
                    }
                    break;
                }
            }
        }



        /// <summary>
        /// Receives a status of a device test from a client. 
        /// Looks for "percentUpdate:###", "complete", "running", or "failed", and acts accordingly.
        /// </summary>
        public void ParseStatusOfTest(string status)
        {
            VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tParseStatusOfTest: status: " + status + "\r\n");
            string[] contentSplit = status.Split(";; ");
            string clientID = contentSplit[1];
            bool needToSendEmail = false;
            string pathToErrorLog = "";

            for (int i = 0; i < mainClientController.Clients.Count; i++)
            {
                if (mainClientController.Clients[i]._unique_Id.ToString() == clientID)
                {
                    targetClient = mainClientController.Clients[i];

                    for (int j = 0; j < contentSplit.Length; j++)
                    {
                        /*
                        LookForTagInMessageAndSetClientData(targetClient, contentSplit[j], TAG_COMPUTER_NUM);
                        LookForTagInMessageAndSetClientData(targetClient, contentSplit[j], TAG_TESTGROUP_NUM);

                        needToSendEmail = LookForTagInMessageAndSetClientData(targetClient, contentSplit[j], TAG_STATUS); // need to handle conditional

                        LookForTagInMessageAndSetClientData(targetClient, contentSplit[j], TAG_PERCENT);
                        LookForTagInMessageAndSetClientData(targetClient, contentSplit[j], TAG_CYCLE_COUNT);
                        LookForTagInMessageAndSetClientData(targetClient, contentSplit[j], TAG_SLOT_NUM);
                        LookForTagInMessageAndSetClientData(targetClient, contentSplit[j], TAG_DESCRIPTION);
                        LookForTagInMessageAndSetClientData(targetClient, contentSplit[j], TAG_TRANSFER_PATH);
                         */
                        if (contentSplit[j].StartsWith("<cn>"))
                        {
                            string newString = contentSplit[j].Remove(0, 4);
                            targetClient._compNumber = newString;
                        }
                        else if (contentSplit[j].StartsWith("<gn>"))
                        {
                            string newString = contentSplit[j].Remove(0, 4);
                            targetClient._testGroupNumber = newString;
                        }
                        else if (contentSplit[j].StartsWith("<sts>"))
                        {
                            string newString = contentSplit[j].Remove(0, 5);
                            targetClient._status = newString;

                            if((newString == "Failed") || (newString == "Unknown"))
                            {
                                needToSendEmail = true;
                            }
                            else if (newString == "manualClose")
                            {
                                targetClient._manuallyClosed = true;
                            }
                        }
                        else if (contentSplit[j].StartsWith("<pct>"))
                        {
                            string newString = contentSplit[j].Remove(0, 5);
                            targetClient._percent = newString;
                        }
                        else if (contentSplit[j].StartsWith("<cyc>"))
                        {
                            string newString = contentSplit[j].Remove(0, 5);
                            targetClient._cycleCount = newString;
                        }
                        else if (contentSplit[j].StartsWith("<slt>"))
                        {
                            string newString = contentSplit[j].Remove(0, 5);
                            targetClient._slotNumber = newString;
                        }
                        else if (contentSplit[j].StartsWith("<des>"))
                        {
                            string newString = contentSplit[j].Remove(0, 5);
                            targetClient._descriptionOfState = newString;
                        }
                        else if (contentSplit[j].StartsWith("<pth>"))
                        {
                            pathToErrorLog = contentSplit[j].Remove(0, 5);
                        }
                    }

                    VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tParseStatusOfTest: calling OnUpdateConnectedClients_BigPicture_StatusOnly_updateClient, passing in CustomEventArgs4_statusUpdates\r\n");

                    OnUpdateConnectedClients_BigPicture_StatusOnly_updateClient(new CustomEventArgs4_statusUpdates(
                                                       targetClient._unique_Id.ToString(),
                                                       targetClient._testGroupNumber,
                                                       targetClient._compNumber,
                                                       targetClient._slotNumber,
                                                       targetClient._status,
                                                       targetClient._percent,
                                                       targetClient._cycleCount,
                                                       targetClient._descriptionOfState,
                                                       targetClient._manuallyClosed
                                                       ));

                    if(needToSendEmail == true && this.mMainForm.sendEmailOnAlert_checkBox.Checked)
                     {
                        SendAlertEmail_AllInfo(targetClient, pathToErrorLog);
                        needToSendEmail = false;
                    }

                    break;
                }
            }
        }


        /// <summary>
        /// Receives an Update containing the PC name and slot number of a given client,
        /// then updates the GUI accordingly.
        /// </summary>
        public void ParseLocationOfTests(string status)
        {
            VerboseLog(DateTime.Now.ToString(TIME_MS) + "\tParseLocationOfTests: status: " + status  + "\r\n");
            string[] contentSplit = status.Split(";; ");
            string clientID = contentSplit[1];

            for (int i = 0; i < mainClientController.Clients.Count; i++)
            {
                if (mainClientController.Clients[i]._unique_Id.ToString() == clientID)
                {
                    targetClient = mainClientController.Clients[i];

                    for (int j = 0; j < contentSplit.Length; j++)
                    {
                        if (contentSplit[j].StartsWith("<cn>"))
                        {
                            string newString = contentSplit[j].Remove(0, 4);
                            targetClient._compNumber = newString;
                        }
                        else if (contentSplit[j].StartsWith("<slt>"))
                        {
                            string newString = contentSplit[j].Remove(0, 5);
                            targetClient._slotNumber = newString;
                        }
                    }
                    break;
                }
            }
        }

        #endregion message parsing






    }

}
 