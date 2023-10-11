using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;

using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace Client_GUI
{
    public partial class Client_GUI : Form
    {

        private static string DoubleQuote = "\"";
        public static string rootDirectory = Application.StartupPath;
        public static string testFiles = rootDirectory + "\\TestFiles";

        private static string IP_LIST = testFiles + "\\Addresses\\validIPAddresses.txt";
        //private static string DEVICE_PROGRAM = "DeviceProgram";

        private static string SLOT_LIST = testFiles + "\\TestInfo\\SlotNumbers.txt";
        private static string PC_GROUP_FILE = testFiles + "\\TestInfo\\PcGroup.txt";
        private static string DEVICE_PROGRAM_FILE = testFiles + "\\TestInfo\\ProgramInfo.txt";
        private static string TEST_CYCLES_FILE = testFiles + "\\TestInfo\\TestCycles.txt";

        //private static string PC_GROUP = DEVICE_INFORMATION + "\\PcGroup.txt";
        //private static string DEVICE_INFORMATION = testFiles + "\\DeviceInformation";
        //private static string TEST_CYCLES = DEVICE_INFORMATION + "\\TestCycles.txt";


        public Client_GUI()
        {
            InitializeComponent();
            mAsyncClient = new SocketCommunication.AsynchronousClient();

            //client.ReadFromServer += WriteToTextBox;
            mAsyncClient.ReadFromServer += WriteToTextBox;
            mAsyncClient.DisableButton1 += button_socket_connect_Disable;
            mAsyncClient.ChangeTextBoxToDisconnected += change_textBox_Disconnected;
            mAsyncClient.UpdateIDBox += IDbox_update;
        }

        delegate void SetTextCallback(string message);
        delegate void SetTextCallback2(TextBox ctrl, string message);
        delegate void SetComboText(ComboBox combo, string message);
        delegate void SetRichTextBox(RichTextBox richText, string message);
        delegate void SetRichTextBoxMultiLine(RichTextBox richText, string[,] message);
        delegate string GetTextCallBack(TextBox ctrl);
        delegate string GetLabelCallBack(Label ctrl);
        delegate void SetProgressBar(int value);
        delegate void SetComboIndex(ComboBox combo, int value);
        delegate int GetComboIndex(ComboBox combo);
        delegate void SetCheckBox(CheckBox checkBox, bool value);
        delegate string GetComboCallBack(ComboBox cmb);
        delegate string GetRichCallBack(RichTextBox rtxtbox);
        delegate string GetGUICaption();
        //delegate void SetButtonColor(Button button, Color color);
        delegate void SetButtonColor(Color color);
        delegate Color GetColor();
        delegate void SetTextBoxColorDelegate(TextBox tBox, Color color);
        delegate void SetCboxColorDelegate(ComboBox cBox, Color color);
        //delegate Color GetColor();

        // Delegates for socket communication
        public Socket client;
        public SocketCommunication.AsynchronousClient mAsyncClient;
        public delegate void ReadFromServerDelegate(object sender, string cli);
        public delegate void IDboxUpdateDelegate(object sender, string cli);
        public delegate void button_socket_connect_DisableDelegate(object sender, EventArgs e);
        public delegate void change_textBox_DisconnectedDelegate(object sender, EventArgs e);

        DeviceTester deviceTest;
        protected DataLog dataLog = new DataLog();

        public const string TESTAPP_VERSION = "1.0.0";

        public bool radioButtonJustChanged = false;

        private Thread workerThread = null;

        protected int loopCount = 0;
        protected string CycleLimit = "NO";
        protected string Error = "";

        // Boolean flag used to stop the test
        public bool stopProcess = false;

        public bool stopButtonPressed = false;

        public static string testNotes;

        public int originalGuiHeight;


        private void Form1_Load(object sender, EventArgs e)
        {
            originalGuiHeight = this.Height;

            this.lbl_testAppVersionNum.Text = TESTAPP_VERSION;
            //this.textBox_testAppVersion.Text = TESTAPP_VERSION;
            // populate combo box with available information
            FillComboBox(cBox_programName, DEVICE_PROGRAM_FILE);
            FillPcGroupBox(cBox_pcGroup, PC_GROUP_FILE);
            FillComboBox(cBox_testDuration, TEST_CYCLES_FILE);
            FillComboBox(cBox_serverIp, IP_LIST);
            FillComboBox(cBox_slotNumber, SLOT_LIST);


            // AUto populate the Drive infomration with generic info and system details
            cBox_programName.Text = "NA"; // TODO populate with first index in the file containg list of all possible programs
            cBox_pcGroup.Text = "Select One";
            text_computerName.Text = System.Environment.MachineName;
            //cBox_slotNumber.Text = "1";
            cBox_testDuration.Text = "50000";
            rText_testNotes.Text = "Generic Testing";
            rText_statusOfTest.Text = string.Empty;
            cBox_serverIp.SelectedIndex = 0;
            cBox_slotNumber.SelectedIndex = 0;
        }

        public void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            string dataFromTextbox = text_sendBox.Text;
            try
            {
                new Thread(() =>
                {
                    if (this.cBox_serverIp.BackColor == Color.Green)
                    {
                        mAsyncClient.SendManualCloseMessageToServer();
                    }

                }).Start();
            }
            catch
            {
                MessageBox.Show("Failed to send \"manual close\" message to server.");
            }
        }

        /// <summary>
        /// Updates the rich text box to display the detailed status of the running test
        /// </summary>
        /// <param name="message"></param>
        public void UpdateStatus(string message)
        {
            if (this.rText_statusOfTest.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(UpdateStatus);
                this.Invoke(d, new object[] { message });
            }
            else
            {
                if (message == "") // clear status text box
                {
                    this.rText_statusOfTest.Text = message;
                }
                else // add text to status box
                {
                    this.rText_statusOfTest.Text += message;
                }
            }
        }

        public void SetStartButton(string caption)
        {
            if (this.button_startTest.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetStartButton);
                this.Invoke(d, new object[] { caption });
            }
            else
            {
                this.button_startTest.Text = caption;
            }
        }

        public string GetComboText(ComboBox combo)
        {
            if (combo.InvokeRequired)
            {
                GetComboCallBack d = new GetComboCallBack(GetComboText);
                string controlText = (string)this.Invoke(d, new object[] { combo });
                return (controlText);
            }
            else
            {
                return combo.Text;
            }
        }

        public string GetText(TextBox textBox)
        {
            if (textBox.InvokeRequired)
            {
                GetTextCallBack d = new GetTextCallBack(GetText);
                string controlText = (string)this.Invoke(d, new object[] { textBox });
                return (controlText);
            }
            else
            {
                return textBox.Text;
            }
        }

        public string GetLabelText(Label label)
        {
            if (label.InvokeRequired)
            {
                GetLabelCallBack d = new GetLabelCallBack(GetLabelText);
                string controlText = (string)this.Invoke(d, new object[] { label });
                return (controlText);
            }
            else
            {
                return label.Text;
            }
        }

        public string GetRichText(RichTextBox richTextBox)
        {
            if (richTextBox.InvokeRequired)
            {
                GetRichCallBack d = new GetRichCallBack(GetRichText);
                string controlText = (string)this.Invoke(d, new object[] { richTextBox });
                return (controlText);
            }
            else
            {
                return richTextBox.Text;
            }
        }

        public int GetComboBoxIndex(ComboBox combo)
        {
            if (combo.InvokeRequired)
            {
                GetComboIndex d = new GetComboIndex(GetComboBoxIndex);
                int index = (int)this.Invoke(d, new object[] { combo });
                return (index);
            }
            else
            {
                return combo.SelectedIndex;
            }
        }

        public int GetComboMaxIndex(ComboBox combo)
        {
            if (combo.InvokeRequired)
            {
                GetComboIndex d = new GetComboIndex(GetComboMaxIndex);
                int index = (int)this.Invoke(d, new object[] { combo });
                return (index);
            }
            else
            {
                return combo.Items.Count;
            }
        }

        public void SetRichText(RichTextBox richTextBox, string text)
        {
            if (richTextBox.InvokeRequired)
            {
                SetRichTextBox d = new SetRichTextBox(SetRichText);
                this.Invoke(d, new object[] { richTextBox, text });
            }
            else
            {
                richTextBox.Text = text;
            }
        }

        public void SetRichTextMultiLine(RichTextBox richTextBox, string[,] text)
        {
            if (richTextBox.InvokeRequired)
            {
                SetRichTextBoxMultiLine d = new SetRichTextBoxMultiLine(SetRichTextMultiLine);
                this.Invoke(d, new object[] { richTextBox, text });
            }
            else
            {
                // clear the text box first
                richTextBox.Clear();

                //richTextBox.Text = text;
                foreach (string line in text)
                    richTextBox.AppendText(line + "\r\n");
            }
        }

        public void SetText(TextBox textBox, string text)
        {
            if (textBox.InvokeRequired)
            {
                SetTextCallback2 d = new SetTextCallback2(SetText);
                this.Invoke(d, new object[] { textBox, text });
            }
            else
            {
                textBox.Text = text;
            }
        }

        public void SetCombo(ComboBox combo, string text)
        {
            if (combo.InvokeRequired)
            {
                SetComboText d = new SetComboText(SetCombo);
                this.Invoke(d, new object[] { combo, text });
            }
            else
            {
                combo.Text = text;
            }
        }

        public void SetTestBox(string test)
        {
            if (this.text_currentTest.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetTestBox);
                this.Invoke(d, new object[] { test });
            }
            else
            {
                this.text_currentTest.Text = test;

                // Set back color based on test
                switch (test)
                {
                    case "Voltage Check Test":
                        this.text_currentTest.BackColor = Color.LightSeaGreen;
                        this.text_currentTest.Text = "Voltage Check Test";
                        break;

                    case "Boot Test":
                        this.text_currentTest.BackColor = Color.LightGreen;
                        this.text_currentTest.Text = "Boot Test";
                        break;

                    case "Power Test":
                        this.text_currentTest.BackColor = Color.Orange;
                        this.text_currentTest.Text = "Power Test";
                        break;

                }

            }
        }

        public void SetCaption(string caption)
        {
            if (this.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetCaption);
                this.Invoke(d, new object[] { caption });
            }
            else
            {
                this.Text = caption;
            }
        }

        public string GetCaption()
        {
            if (this.InvokeRequired)
            {
                GetGUICaption d = new GetGUICaption(GetCaption);
                string controlText = (string)this.Invoke(d);
                return (controlText);
            }
            else
            {
                return this.Text;
            }
        }
        
        public string GetProgress()
        {
            if (this.InvokeRequired)
            {
                GetGUICaption d = new GetGUICaption(GetProgress);
                string controlText = (string)this.Invoke(d);
                return (controlText);
            }
            else
            {
                return this.progressBar1.Value.ToString();
            }
        }

        public void SetProgress(int value)
        {
            if (this.InvokeRequired)
            {
                SetProgressBar d = new SetProgressBar(SetProgress);
                this.Invoke(d, new object[] { value });
            }
            else
            {
                this.progressBar1.Value = value;
            }
        }

        public void SetComboBoxIndex(ComboBox combo, int value)
        {
            if (this.InvokeRequired)
            {
                SetComboIndex d = new SetComboIndex(SetComboBoxIndex);
                this.Invoke(d, new object[] { combo, value });
            }
            else
            {
                combo.SelectedIndex = value;
            }
        }

        public void SetCheckBoxValue(CheckBox checkBox, bool value)
        {
            if (this.InvokeRequired)
            {
                SetCheckBox d = new SetCheckBox(SetCheckBoxValue);
                this.Invoke(d, new object[] { checkBox, value });
            }
            else
            {
                checkBox.Checked = value;
            }
        }

        private void txtRevision_TextChanged(object sender, EventArgs e)
        {
            if (this.Height == originalGuiHeight)
            {
                this.Height = 390;
            }
            else if (this.Height == 390)
            {
                this.Height = 250;
            }
            else
            {
                this.Height = originalGuiHeight;
            }
        }

        private void tabMainMenu_Click(object sender, EventArgs e)
        {
            this.Height = 240;
        }

        private void QuickResize(object sender, EventArgs e)
        {
            this.Height = originalGuiHeight;
        }

        private void FillComboBox(ComboBox combo, string filePath)
        {
            FileStream fileStream;
            StreamReader streamReader;

            if (System.IO.File.Exists(filePath))
            {
                fileStream = new FileStream(filePath, FileMode.Open);
                streamReader = new StreamReader(fileStream);
                string line;

                while ((line = streamReader.ReadLine()) != null)
                {
                    line = line.Replace(DoubleQuote, "");
                    combo.Items.Add(line);
                }

                streamReader.Close();
            }

            else
            {
                MessageBox.Show("File does not exist.\r\n\r\nLooking for :\t" + filePath);
            }
        }

        private void FillPcGroupBox(ComboBox combo, string filePath)
        {
            FileStream fileStream;
            StreamReader streamReader;

            if (System.IO.File.Exists(filePath))
            {
                fileStream = new FileStream(filePath, FileMode.Open);
                streamReader = new StreamReader(fileStream);
                string line;
                string[] temp;

                while ((line = streamReader.ReadLine()) != null)
                {
                    line = line.Replace(DoubleQuote, "");
                    temp = line.Split('\t');
                    combo.Items.Add(temp[0]);
                }

                streamReader.Close();
            }
        }

        // TODO use this function to get a list of possible pcGroups
        private void cBox_pcGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[,] BuildArray = new string[100, 5];
            FileStream fs;
            StreamReader sr;
            int Row = 0;
            cBox_slotNumber.Items.Clear();

            try
            {
                if (System.IO.File.Exists(PC_GROUP_FILE))
                {
                    fs = new FileStream(PC_GROUP_FILE, FileMode.Open);
                    sr = new StreamReader(fs);
                    string line;

                    //reading each line from text file
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] temp = line.Split('\t');
                        BuildArray[Row, 0] = temp[0];
                        BuildArray[Row, 1] = temp[1];
                        BuildArray[Row, 2] = temp[2];
                        Row++;
                    }
                    sr.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error pcGroup.txt not formatted correctly. " +
                                ex.Message);
            }

            //*****************************************************************
            //        Set Slot number  (Set Starting and Number of Slots)
            //*****************************************************************
            int StartSlot, MaxSlot, i;
            Row = cBox_pcGroup.SelectedIndex;

            StartSlot = Convert.ToInt32(BuildArray[Row, 1]);
            MaxSlot = Convert.ToInt32(BuildArray[Row, 2]) + (StartSlot - 1);

            for (i = StartSlot; i <= MaxSlot; i++)
            {
                cBox_slotNumber.Items.Add(i);
            }
        }

        // Socket Communication Event Handlers

        public void WriteToTextBox(object sender, string e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new ReadFromServerDelegate(WriteToTextBox), new object[] { sender, e });
            }
            else
            {
                this.lBox_recentReceivedMessages.Items.Add(e);
                // If more than 7 messages have been logged, start removing the oldest when a new message is added.
                if (lBox_recentReceivedMessages.Items.Count > 7)
                {
                    this.lBox_recentReceivedMessages.Items.RemoveAt(0);
                }
                this.text_lastMessageReceived.Text = e;
            }
        }

        public void IDbox_update(object sender, string e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new IDboxUpdateDelegate(IDbox_update), new object[] { sender, e });
            }
            else
            {
                this.textBox_clientID.Text = e;
            }
        }

        private void button_Send_Click_2(object sender, EventArgs e)
        {
            string dataFromTextbox = text_sendBox.Text;

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

        public void change_textBox_Disconnected(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                //this.BeginInvoke(new ProcessDelegate(WriteToTextBox), new object[] { sender, e });
                this.Invoke(new change_textBox_DisconnectedDelegate(change_textBox_Disconnected), new object[] { sender, e });
            }
            else
            {
                this.button_socketConnect.Enabled = true;
                //this.textBox_serverIP.BackColor = Color.Red;
                //this.cboIpList.BackColor = Color.Red;
                this.cBox_serverIp.BackColor = Color.LightSalmon;
                // Set the flag to false so that the client will resend all necessary info if the client is reconnected to the server.
                deviceTest.isInitialTestInfoSentToServer = false;
            }
        }








        public void SetStartButtonColor(Color color)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new SetButtonColor(SetStartButtonColor), new object[] { color });
            }
            else
            {
                button_startTest.BackColor = color;
            }
        }

        public void SetComboBoxColor(ComboBox cBox, Color color)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new SetCboxColorDelegate(SetComboBoxColor), new object[] { cBox, color });
            }
            else
            {
                cBox.BackColor = color;
            }
        }

        public void SetTextBoxColor(TextBox tBox, Color color)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new SetTextBoxColorDelegate(SetTextBoxColor), new object[] { tBox, color });
            }
            else
            {
                tBox.BackColor = color;
            }
        }

        public Color GetStartButtonColor()
        {
            if (this.InvokeRequired) // or this.button_startTest.InvokeRequired
            {
                return (Color)this.Invoke(new GetColor(() => GetStartButtonColor()));
            }

            return this.button_startTest.BackColor;
        }

        public Color GetTextBoxColor(TextBox tBox)
        {
            if (this.InvokeRequired)
            {
                return (Color)this.Invoke(new GetColor(() => GetTextBoxColor(tBox)));
            }

            return tBox.BackColor;
        }

        public Color GetComboBoxColor(ComboBox cBox)
        {
            if (this.InvokeRequired)
            {
                return (Color)this.Invoke(new GetColor(() => GetComboBoxColor(cBox)));
            }

            return cBox.BackColor;
        }










        // Connect Button
        private void button_socket_connect_click(object sender, EventArgs e)
        {
            string ipFromTextbox = GetComboText(cBox_serverIp);

            try
            {
                new Thread(() =>
                {
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

        public void button_socket_connect_Disable(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                //this.BeginInvoke(new ProcessDelegate(WriteToTextBox), new object[] { sender, e });
                this.Invoke(new button_socket_connect_DisableDelegate(button_socket_connect_Disable), new object[] { sender, e });
            }
            else
            {
                this.button_socketConnect.Enabled = false;
                //this.textBox_serverIP.BackColor = Color.Green;
                //this.cboIpList.Enabled = false;
                this.cBox_serverIp.BackColor = Color.Green;
            }
        }

        // Send Button
        private void button_send_Click(object sender, EventArgs e)
        {
            string dataFromTextbox = text_sendBox.Text;

            try
            {
                new Thread(() =>
                {
                    mAsyncClient.Send(client, dataFromTextbox + "<EOF>");
                }).Start();
            }
            catch
            {
                MessageBox.Show("Send Failed");
            }
        }

        private void button_startTest_Click(object sender, EventArgs e)
        {
            if (this.button_startTest.Text == "Start Test")
            {
                if (button_startTest.BackColor == Color.Red)
                {
                    button_startTest.BackColor = Control.DefaultBackColor;
                    return;
                }
                stopButtonPressed = false;

                deviceTest = new DeviceTester(this);

                this.workerThread = new Thread(new ThreadStart(deviceTest.MainLoop));
                this.workerThread.IsBackground = true;
                this.workerThread.Start();
            }
            else if (this.button_startTest.Text == "Stop")
            {
                stopProcess = true;
                stopButtonPressed = true;

                this.button_startTest.Text = "Start";
                this.rText_statusOfTest.Text = "Test Manually Stopped";
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


    //TODO move to SoocketCommunication class
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

    //TODO move to SoocketCommunication class
    /// <summary>
    /// A class to temporatily hold the socket to give it visibility to other functions
    /// </summary>
    public static class TempSocketHolder
    {
        public static Socket TempSocket;
    }

}
