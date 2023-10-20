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
using System.Security.Cryptography.X509Certificates;

namespace Server_GUI
{
    public partial class Server_GUI : Form
    {
        //Delegates used for writing to text boxes and list boxes
        public delegate void ProcessDelegate(object sender, CustomEventArgs e);
        public delegate void WriteToConnectedListBoxDelegate(object sender, string e);
        public delegate void WriteToGridViewBoxDelegate(object sender, CustomEventArgs3_withTargetClient e);
        public delegate void WriteToGridViewBoxDelegate_updateClient(object sender, CustomEventArgs3_withTargetClient e);
        public delegate void WriteTo_BigPictureDelegate_UpdateClient(object sender, CustomEventArgs3_withTargetClient e);
        public delegate void WriteTo_BigPictureDelegate_StatusOnly_UpdateClient(object sender, CustomEventArgs3_withTargetClient e);
        public delegate void WriteToGridViewBoxDelegate_status(object sender, CustomEventArgs3_withTargetClient e);
        public delegate void WriteToGridViewBoxDelegate_deleteClient(object sender, string e);
        public delegate void UpdateSerialNumberDelegate(object sender, string e);

        public AsynchronousSocketListener mAsyncListener;
        public Email_Sender emailSender;
        List<Control> ControlList = new List<Control>();

        public const string SERVER_VERSION = "1.07e";

        public Server_GUI()
        {
            InitializeComponent();

            mAsyncListener = new AsynchronousSocketListener(this);
            emailSender = new Email_Sender();

            bool ipv4AddressFound = false;
            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    this.Text = "Server Version: " + SERVER_VERSION + " - IP: " + ip.ToString();
                    ipv4AddressFound = true;
                }
            }
            if (!ipv4AddressFound)
            { 
                throw new Exception("No network adapters with an IPv4 address in the system!");
            }

            // Subscribe to all Events 
            mAsyncListener.ReadFromClient += WriteToIncomingMessagesBox;
            mAsyncListener.UpdateConnectedClientsList += WriteToConnectedClientHistoryBox;
            mAsyncListener.UpdateConnectedClientsGridView += WriteToGridViewClientQueueBox;
            mAsyncListener.UpdateConnectedClientsGridView_updateClient += UpdateSpecificClientInGridViewClientQueueBox;

            mAsyncListener.UpdateConnectedClients_BigPicture_updateClient += UpdateInfoInGraphicalOverview;
            //mAsyncListener.UpdateConnectedClients_BigPicture_StatusOnly_updateClient += WriteTo_BigPicture_StatusOnly_UpdateClient;

            mAsyncListener.UpdateConnectedClientsGridView_deleteClient += WriteToGridViewBox_deleteClient;
            mAsyncListener.UpdateSerialNumber += UpdateSerialNumber;
            mAsyncListener.UpdateConnectedClientsGridView_status += WriteToGridViewBox_status;

        }


        #region Event handlers and helper functions

        private void WriteToIncomingMessagesBox(object sender, CustomEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new ProcessDelegate(WriteToIncomingMessagesBox), new object[] { sender, e });
            }
            else
            {   
                this.lbox_incomingMessages.Items.Add(e.lastReceived);

                // If more than 10 messages have been logged, start removing the oldest when a new message is added.
                if (lbox_incomingMessages.Items.Count > 20)
                {
                    this.lbox_incomingMessages.Items.RemoveAt(0);
                }

                this.text_lastMessageReceived.Text = e.lastReceived;
            }

        }

        // update the gui list box with the information received/parsed from the client
        private void WriteToConnectedClientHistoryBox(object sender, string e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new WriteToConnectedListBoxDelegate(WriteToConnectedClientHistoryBox), new object[] { sender, e });
            }
            else
            {
                this.lbox_connectedClientHistory.Items.Add(e);
            }
        }

        // update the gui dataGrid with the information received/parsed from the client
        private void WriteToGridViewClientQueueBox(object sender, CustomEventArgs3_withTargetClient e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new WriteToGridViewBoxDelegate(WriteToGridViewClientQueueBox), new object[] { sender, e });
            }
            else 
            {
                this.gridView_clientQueue.Rows.Add(e.client_id, 
                                                e.client_serialNum, 
                                                e.client_dateTime, 
                                                e.client_testGroupNum, 
                                                e.client_compNum, 
                                                e.client_slotNum, 
                                                e.client_programName, 
                                                e.client_testAppVersion, 
                                                e.client_status, 
                                                e.client_percent);
            }
        }

        // update the gui dataGrid with the new information received/parsed from the client.
        // This function will overwrite the existing entries if the client ID's match.
        private void UpdateSpecificClientInGridViewClientQueueBox(object sender, CustomEventArgs3_withTargetClient e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new WriteToGridViewBoxDelegate_updateClient(UpdateSpecificClientInGridViewClientQueueBox), new object[] { sender, e });
            }
            else
            {
                int rowToEdit = e.gridView_targetClient;
                this.gridView_clientQueue.Rows[rowToEdit].Cells[1].Value = e.client_serialNum;
                this.gridView_clientQueue.Rows[rowToEdit].Cells[3].Value = e.client_testGroupNum;
                this.gridView_clientQueue.Rows[rowToEdit].Cells[4].Value = e.client_compNum;
                this.gridView_clientQueue.Rows[rowToEdit].Cells[5].Value = e.client_slotNum;
                this.gridView_clientQueue.Rows[rowToEdit].Cells[6].Value = e.client_programName;
                this.gridView_clientQueue.Rows[rowToEdit].Cells[7].Value = e.client_testAppVersion;
                this.gridView_clientQueue.Rows[rowToEdit].Cells[8].Value = e.client_status;
                this.gridView_clientQueue.Rows[rowToEdit].Cells[9].Value = e.client_percent;
            }
        }

        private TableLayoutPanel SetTargetPcLayoutPanel (CustomEventArgs3_withTargetClient e)
        {
            // Set the correct computer grid to modify.
            // Set the targetCOmputerLayoutPanel to the appropriate layoutPanel in the GUI that needs modifying with client info.
            TableLayoutPanel targetComputerLayoutPanel = null;
            try
            {
                if (e.client_testGroupNum == "01")
                {
                    if (e.client_compNum == "PC-01")
                    {
                        targetComputerLayoutPanel = this.tableLayoutPanel_PC01;
                    }
                    else if (e.client_compNum == "PC-02")
                    {
                        targetComputerLayoutPanel = this.tableLayoutPanel_PC02;
                    }
                    else if (e.client_compNum == "PC-03")
                    {
                        targetComputerLayoutPanel = this.tableLayoutPanel_PC03;
                    }
                }
                else if (e.client_testGroupNum == "02")
                {
                    if (e.client_compNum == "PC-04")
                    {
                        targetComputerLayoutPanel = this.tableLayoutPanel_PC04;
                    }
                    else if (e.client_compNum == "PC-05")
                    {
                        targetComputerLayoutPanel = this.tableLayoutPanel_PC05;
                    }
                    else if (e.client_compNum == "PC-06")
                    {
                        targetComputerLayoutPanel = this.tableLayoutPanel_PC06;
                    }
                }
                else if (e.client_testGroupNum == "03")
                {
                    if (e.client_compNum == "PC-07")
                    {
                        targetComputerLayoutPanel = this.tableLayoutPanel_PC07;
                    }
                    else if (e.client_compNum == "PC-08")
                    {
                        targetComputerLayoutPanel = this.tableLayoutPanel_PC08;
                    }
                    else if (e.client_compNum == "PC-09")
                    {
                        targetComputerLayoutPanel = this.tableLayoutPanel_PC09;
                    }
                }
                else if (e.client_testGroupNum == "04")
                {
                    if (e.client_compNum == "PC-10")
                    {
                        targetComputerLayoutPanel = this.tableLayoutPanel_PC10;
                    }
                    else if ((e.client_compNum == "PC-11"))
                    {
                        targetComputerLayoutPanel = this.tableLayoutPanel_PC11;
                    }
                    else if (e.client_compNum == "PC-12")
                    {
                        targetComputerLayoutPanel = this.tableLayoutPanel_PC12;
                    }
                }
                else
                {
                    targetComputerLayoutPanel = null;
                    throw new Exception("Server received an unrecognizable testGroupNumber number or PC name.\r\n" +
                        "Values received:: Chamber: " + e.client_testGroupNum.ToString() + "   PC: " + e.client_compNum.ToString());
                }
            }
            catch (Exception except)
            {
                mAsyncListener.VerboseLog(DateTime.Now.ToString(AsynchronousSocketListener.TIME_MS) + "\tEventHandler - WriteTo_BigPicture_UpdateClient - Exception: " + except.Message + "\r\n");
                MessageBox.Show("Exception: " + except.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return targetComputerLayoutPanel;
        }

        private void UpdateColumnInfoInAppropriateSlot(CustomEventArgs3_withTargetClient e, TableLayoutPanel targetComputerLayoutPanel)
        {
            // Parse the slot number passed in from the client GUI.
            int slotNum_toInt = 0;
            Int32.TryParse(e.client_slotNum, out slotNum_toInt);

            // Handle the client specifying a slot number greater than 10. The Gui can only handle up to 10 slots.
            if (slotNum_toInt > 10)
            {
                string message = "The server received a slot number greater than 10.\r\n" +
                    "Received from from client: " + e.client_id.ToString() + " on PC: " + e.client_compNum.ToString() +
                    "\r\nSlot number received was: " + e.client_slotNum.ToString();
                Debug.WriteLine("Exception:\r\n" + message);
                mAsyncListener.VerboseLog(message);
                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Subtract 1 from the slotNum since the grid array is zero indexed. (slot 1 is column 0, slot 8 is column 7)
            slotNum_toInt = slotNum_toInt - 1;

            // Using the target layoutPanel and the target slot number, modify the appropriate text boxes according to the client information passed in.
            if (targetComputerLayoutPanel != null && slotNum_toInt < 10)
            {
                targetComputerLayoutPanel.GetControlFromPosition(slotNum_toInt, 1).Text = e.client_programName;
                targetComputerLayoutPanel.GetControlFromPosition(slotNum_toInt, 2).Text = e.client_serialNumLastFour;
                targetComputerLayoutPanel.GetControlFromPosition(slotNum_toInt, 3).Text = e.client_testType;
                targetComputerLayoutPanel.GetControlFromPosition(slotNum_toInt, 4).Text = e.client_percent;

                // Change color based on test type
                if (e.client_testType == "VC")
                {
                    targetComputerLayoutPanel.GetControlFromPosition(slotNum_toInt, 3).BackColor = Color.MediumBlue;
                }
                else if (e.client_testType == "BT")
                {
                    targetComputerLayoutPanel.GetControlFromPosition(slotNum_toInt, 3).BackColor = Color.Indigo;
                }
                else if (e.client_testType == "PT")
                {
                    targetComputerLayoutPanel.GetControlFromPosition(slotNum_toInt, 3).BackColor = Color.Orange;
                }
                else
                {
                    targetComputerLayoutPanel.GetControlFromPosition(slotNum_toInt, 3).BackColor = Color.LightGray;
                }

                // Change color based on status. 
                if (e.client_status == "Starting")
                {
                    targetComputerLayoutPanel.GetControlFromPosition(slotNum_toInt, 0).BackColor = Color.Yellow;
                }
                else if (e.client_status == "Running")
                {
                    targetComputerLayoutPanel.GetControlFromPosition(slotNum_toInt, 0).BackColor = Color.Green;
                }
                else if (e.client_status == "Failed")
                {
                    targetComputerLayoutPanel.GetControlFromPosition(slotNum_toInt, 0).BackColor = Color.Red;
                }
                else if (e.client_status == "Stopped")
                {
                    targetComputerLayoutPanel.GetControlFromPosition(slotNum_toInt, 0).BackColor = Color.Tomato;
                }
                else if (e.client_status == "Complete")
                {
                    targetComputerLayoutPanel.GetControlFromPosition(slotNum_toInt, 0).BackColor = Color.Lime;
                }
                else if (e.client_status == "Unknown" && e.client_wasManuallyClosed == false)
                {
                    targetComputerLayoutPanel.GetControlFromPosition(slotNum_toInt, 0).BackColor = Color.Purple;
                    // Keep track of all slots that have been designated as unknown/possibly disconnected and add them to a list of text boxes to monitor for clicks.
                    // If a box is purple and the user clicks it, it's contents will be cleared. The contents do not auto clear if a disconnection is suspected. 
                    // We want to see which clients may have an issue by designating them with the color purple instead of just erasing them.

                    // If a client has been disconnected or its state is unknown, then add it to a list of text boxes to monitor for clicks. A click will be needed to clear the disconnected status.
                    ControlList.Add(targetComputerLayoutPanel.GetControlFromPosition(slotNum_toInt, 0));
                    // Call a function to dynamically create a click event for the added textBox.
                    trackDisconnectedClientTextBoxes();
                }
                else if (e.client_status == "manualClose")
                {
                    targetComputerLayoutPanel.GetControlFromPosition(slotNum_toInt, 0).BackColor = Color.DarkGray;

                    // If a client has been manually closed, then add it to a list of text boxes to monitor for clicks. A click will be needed to clear the manually closed status.
                    ControlList.Add(targetComputerLayoutPanel.GetControlFromPosition(slotNum_toInt, 0));
                    // Call a function to dynamically create a click event for the added textBox.
                    trackDisconnectedClientTextBoxes();
                }
            }
        }

        // Update the GUI with info once more information has been received from the client
        private void UpdateInfoInGraphicalOverview(object sender, CustomEventArgs3_withTargetClient e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new WriteTo_BigPictureDelegate_UpdateClient(UpdateInfoInGraphicalOverview), new object[] { sender, e });
            }
            else
            {
                TableLayoutPanel targetComputerLayoutPanel = SetTargetPcLayoutPanel(e);
                UpdateColumnInfoInAppropriateSlot(e, targetComputerLayoutPanel);
            }
        }

        private void WriteTo_BigPicture_StatusOnly_UpdateClient(object sender, CustomEventArgs3_withTargetClient e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new WriteTo_BigPictureDelegate_StatusOnly_UpdateClient(WriteTo_BigPicture_StatusOnly_UpdateClient), new object[] { sender, e });
            }
            else
            {
                TableLayoutPanel targetComputerLayoutPanel = SetTargetPcLayoutPanel(e);
                UpdateColumnInfoInAppropriateSlot(e, targetComputerLayoutPanel);

            }
        }


        private void WriteToGridViewBox_deleteClient(object sender, string e)
        {
            mAsyncListener.VerboseLog(DateTime.Now.ToString(AsynchronousSocketListener.TIME_MS) + "\tEventHandler - WriteToGridViewBox_deleteClient - accepting a string: " + e);

            if (this.InvokeRequired)
            {
                this.Invoke(new WriteToGridViewBoxDelegate_deleteClient(WriteToGridViewBox_deleteClient), new object[] { sender, e });
            }
            else
            {
                // The unique id of the dead client is passed in
                string deadClientID = e;

                // the Column that contains the unique ID for each row/client is elemnt 0 in the array
                int uniqueIDColumn = 0;

                gridView_clientQueue.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                // Loop through the rows looking at a specific column, searching for the uniqueID of the dead Client you want to remove.
                foreach (DataGridViewRow row in gridView_clientQueue.Rows)
                {
                    try
                    {
                        // Null check was implemented because an unknonw client kept connecting, it was added to the client list, but it never updated it's values since it was not a recognized/accepted testDevice.
                        // The common IP of this "mystery client" was ###,###,###,###:48936. I'm assuming this is another device on my network doing some kind of port scanning.
                        if (row.Cells[uniqueIDColumn].Value != null)
                        {
                            if (row.Cells[uniqueIDColumn].Value.ToString().Equals(deadClientID))
                            {
                                gridView_clientQueue.Rows.RemoveAt(row.Index);
                                break;
                            }
                        }
                    }
                    catch (Exception error)
                    {
                        MessageBox.Show("Tried to remove a client, but client does not exist. \r\n\r\n" + error);
                    } 
                }
            }
        }

        // TODO where is this used? remove it?
        private void UpdateSerialNumber(object sender, string e)
        {
            mAsyncListener.VerboseLog(DateTime.Now.ToString(AsynchronousSocketListener.TIME_MS) + "\tEventHandler - UpdateSerialNumber - accepting a string: " + e);

            if (this.InvokeRequired)
            {
                this.Invoke(new UpdateSerialNumberDelegate(UpdateSerialNumber), new object[] { sender, e });
            }
            else
            {
                this.gridView_clientQueue.Rows.Add("test", e, "TEST");
            }
        }


        private void WriteToGridViewBox_status(object sender, CustomEventArgs3_withTargetClient e)
        {
            mAsyncListener.VerboseLog(DateTime.Now.ToString(AsynchronousSocketListener.TIME_MS) + "\tEventHandler - WriteToGridViewBox_status - accepting CustomEventArgs3");
            if (this.InvokeRequired)
            {
                this.Invoke(new WriteToGridViewBoxDelegate_status(WriteToGridViewBox_status), new object[] { sender, e });
            }
            else
            {
                // The columns that contain the status and the percent are 7 and 8 respectively
                int uniqueIDColumn = 0;
                int statusColumn = 7;
                int percentColumn = 8;

                // Loop thorugh each client in the data grid
                foreach (DataGridViewRow row in gridView_clientQueue.Rows)
                {
                    // look in the client ID column, if it matches the target ID,
                    // then overwrite the status and percent values, and change the background color of the cell
                    if (row.Cells[uniqueIDColumn].Value.ToString().Equals(e.client_id))
                    {
                        row.Cells[statusColumn].Value = e.client_status;
                        row.Cells[percentColumn].Value = e.client_percent;

                        if(e.client_status == "Running")
                        {
                            row.Cells[statusColumn].Style.BackColor = Color.Green;
                        }
                        else if (e.client_status == "Complete")
                        {
                            row.Cells[statusColumn].Style.BackColor = Color.Lime;

                        }
                        else if (e.client_status == "Failed")
                        {
                            row.Cells[statusColumn].Style.BackColor = Color.Red;
                        }
                        break;
                    }
                }

            }
        }

        #endregion Event handlers and helper functions


        private void button_ListenForConnections_Click(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                mAsyncListener.StartListening();
            })
            { IsBackground = true }.Start();
            
            //change the button to disabled if connected
            new Thread(delegate()
            {
                try
                {
                    Invoke((MethodInvoker)delegate
                    {
                        button_ListenForConnections.Enabled = false;
                    });
                }
                catch
                {
                    MessageBox.Show("failed to listen");
                }  
            }).Start();
        }


        // Button 2 = send to all Button
        private void button_SendToALL_Click(object sender, EventArgs e)
        {
            string dataFromTextbox = text_sendToAll.Text + AsynchronousSocketListener.TAG_END_OF_FILE;
            try
            {
                new Thread(() =>
                {
                    // send the data in the GUI text box to the client that was just connected to the server
                    mAsyncListener.SendToAll(dataFromTextbox);
                }).Start();
            }
            catch
            {
                MessageBox.Show("Send Failed");
            }
        }


        private void button_TestEmailSend_Click(object sender, EventArgs e)
         {
            try
            {
                new Thread(() =>
                {
                    Email_Sender.emailToAddress = this.EmailAddress_textBox.Text;
                    emailSender.SendTest("sample string");
                }).Start();
            }
            catch
            {
                MessageBox.Show("Send Failed");
            }
        }


        private void button_UpdateEmailLists_Click(object sender, EventArgs e)
        {
            try
            {
                new Thread(() =>
                {
                    // Open the email list text file and set the send-to list. (addresses are delimited by commas.
                    Email_Sender.emailToList_deviceOwners = Email_Sender.ParseEmailRecipientSingleString(Email_Sender.pathToEmailRecipientList_deviceOwners); //Receiver Email Address
                    Email_Sender.emailToList_managers = Email_Sender.ParseEmailRecipientSingleString(Email_Sender.pathToEmailRecipientList_managers); //Receiver Email Address
                    MessageBox.Show("Email lists have been updated with the new contents of the text files.\r\n" +
                        "testDevice owners: " + Email_Sender.emailToList_deviceOwners +
                        "\r\nmanagers: " + Email_Sender.emailToList_managers);
                }).Start();
            }
            catch
            {
                MessageBox.Show("Failed to update list.");
            }
        }


        private void button_testTableLayout_Click(object sender, EventArgs e)
        {
            this.tableLayoutPanel_PC01.GetControlFromPosition(1, 1).Text = "testing";
        }


        private void buttonChangeUpdateTime_Click(object sender, EventArgs e)
        {

            string newUpdateTime = textBoxDailyUpdate_time.Text;
            string[] splitTime = newUpdateTime.Split(':');
            try
            {
                int hour = Int32.Parse(splitTime[0]);
                int minute = Int32.Parse(splitTime[1]);

                if(hour >= 0 && hour < 24)
                {
                    if(minute >= 0 && hour < 60)
                    {
                        
                        // Cancel the last task so you don't have multiple triggers going.
                        AsynchronousSocketListener.trigger.CancellationToken.Cancel();
                        Thread.Sleep(30);
                        AsynchronousSocketListener.trigger.Dispose();
                        AsynchronousSocketListener.trigger = new DailyTrigger(hour, minute);
                        AsynchronousSocketListener.trigger.OnTimeTriggered += () =>
                        {
                            // Make sure the send email check box is checked and there is at least 1 client connected.
                            if (sendEmailOnAlert_checkBox.Checked && mAsyncListener.mainClientController.Clients.Count > 0)
                            {
                                string[] type2_emailBodyAndSubject = mAsyncListener.CreateSummaryOfAllTests_type2Devices();
                                string[] allDevices_emailBodyAndSubject = mAsyncListener.CreateSummaryOfAllTests_allTypes();
                                emailSender.SendDailyUpdateEmailForAllDevicesOfType2(type2_emailBodyAndSubject[0], type2_emailBodyAndSubject[1]);
                                emailSender.SendDailyUpdateEmailForAllDevices_AllTypes(allDevices_emailBodyAndSubject[0], allDevices_emailBodyAndSubject[1]);
                                mAsyncListener.VerboseLog("Email Sent : Daily Update sent\r\n");
                            }
                        };
                    }
                    else
                    {
                        MessageBox.Show("Minutes out of range. You entered " + minute + ".\r\nMinutes must be 0-59.");
                    }
                }
                else
                {
                    MessageBox.Show("Hours out of range. You entered " + hour + ".\r\nHours must be 0-23.");
                }
            }
            catch (FormatException err)
            {
                MessageBox.Show("Could not parse time. " +
                    "\r\nHour and Minutes must be integers." +
                    "\r\nhh:mm is an acceptable format." +
                    "\r\nRange from 00:00 to 23:59" +
                    "\r\nRemove spaces and separate with a \":\"." +
                    "\r\n" + err);
            }
        }

        /// <summary>
        /// Add the slot number text box of the client that has been marked as potentially disconnected or manually closed. 
        /// Start listening for a click and then clear the contents of the entire column if the slotNum textbox is clicked.
        /// </summary>
        private void trackDisconnectedClientTextBoxes()
        {
            foreach (Control slotNumTextBox in ControlList)
            {
                // If a client is disconnected/unknown it's slot number box will be Purple.
                // If a client is manually closed it's slot number box will be Dark Gray.
                if (slotNumTextBox.BackColor.Equals(Color.Purple) || slotNumTextBox.BackColor.Equals(Color.DarkGray))
                {
                    ((TextBox)slotNumTextBox).Click += (sender, e) =>
                    {
                        TextBox b = (TextBox)sender;
                        b.BackColor = SystemColors.Control;
                        TableLayoutPanel targetLayoutPanel = (TableLayoutPanel)b.Parent;
                        int column = targetLayoutPanel.GetColumn(b);
                        targetLayoutPanel.GetControlFromPosition(column, 1).Text = "";
                        targetLayoutPanel.GetControlFromPosition(column, 2).Text = "";
                        targetLayoutPanel.GetControlFromPosition(column, 3).Text = "";
                        targetLayoutPanel.GetControlFromPosition(column, 3).BackColor = SystemColors.Window;
                        targetLayoutPanel.GetControlFromPosition(column, 4).Text = "";
                    };
                }
            }
        }
    }








    

  




    











    






}

