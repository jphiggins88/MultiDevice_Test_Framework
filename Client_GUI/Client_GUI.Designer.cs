using System;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;
//using DataLogging;
using System.Net.Sockets;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Timers;
using System.Text;

namespace Client_GUI
{
    partial class Client_GUI
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button_socketConnect = new System.Windows.Forms.Button();
            this.lBox_recentReceivedMessages = new System.Windows.Forms.ListBox();
            this.button_send = new System.Windows.Forms.Button();
            this.text_sendBox = new System.Windows.Forms.TextBox();
            this.text_lastMessageReceived = new System.Windows.Forms.TextBox();
            this.textBox_clientID = new System.Windows.Forms.TextBox();
            this.lbl_clientId = new System.Windows.Forms.Label();
            this.lbl_lastMessageReceived = new System.Windows.Forms.Label();
            this.lbl_recentReceivedMessages = new System.Windows.Forms.Label();
            this.text_testGroupIdentifier = new System.Windows.Forms.TextBox();
            this.text_statusOfTest = new System.Windows.Forms.TextBox();
            this.text_progress = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lbl_programName = new System.Windows.Forms.Label();
            this.lbl_computerName = new System.Windows.Forms.Label();
            this.lbl_slotNumber = new System.Windows.Forms.Label();
            this.lbl_testAppVersion = new System.Windows.Forms.Label();
            this.lbl_testGroupIdentifier = new System.Windows.Forms.Label();
            this.lbl_satusOfTest = new System.Windows.Forms.Label();
            this.lbl_progress = new System.Windows.Forms.Label();
            this.lbl_serverIp = new System.Windows.Forms.Label();
            this.cBox_programName = new System.Windows.Forms.ComboBox();
            this.cBox_slotNumber = new System.Windows.Forms.ComboBox();
            this.cBox_testDuration = new System.Windows.Forms.ComboBox();
            this.lbl_testDuration = new System.Windows.Forms.Label();
            this.lbl_pcGroup = new System.Windows.Forms.Label();
            this.lbl_ECD = new System.Windows.Forms.Label();
            this.text_ECD = new System.Windows.Forms.TextBox();
            this.text_startDate = new System.Windows.Forms.TextBox();
            this.lbl_startDate = new System.Windows.Forms.Label();
            this.text_totalFailures = new System.Windows.Forms.TextBox();
            this.lbl_totalFailures = new System.Windows.Forms.Label();
            this.rbtn_VC = new System.Windows.Forms.RadioButton();
            this.rbtn_BT = new System.Windows.Forms.RadioButton();
            this.rbtn_PT = new System.Windows.Forms.RadioButton();
            this.lbl_selectTest = new System.Windows.Forms.Label();
            this.text_currentTest = new System.Windows.Forms.TextBox();
            this.lbl_currentTest = new System.Windows.Forms.Label();
            this.rText_statusOfTest = new System.Windows.Forms.RichTextBox();
            this.lbl_richTextStatusOfTests = new System.Windows.Forms.Label();
            this.cBox_serverIp = new System.Windows.Forms.ComboBox();
            this.lbl_completedCycles = new System.Windows.Forms.Label();
            this.text_completedCycles = new System.Windows.Forms.TextBox();
            this.button_startTest = new System.Windows.Forms.Button();
            this.lbl_testNotes = new System.Windows.Forms.Label();
            this.rText_testNotes = new System.Windows.Forms.RichTextBox();
            this.checkBox_forceFailureOnNextCycle = new System.Windows.Forms.CheckBox();
            this.text_dateCompleted = new System.Windows.Forms.TextBox();
            this.lbl_dateCompleted = new System.Windows.Forms.Label();
            this.text_hoursPer1kCycles = new System.Windows.Forms.TextBox();
            this.lbl_hoursPer1kCycles = new System.Windows.Forms.Label();
            this.lbl_testAppVersionNum = new System.Windows.Forms.Label();
            this.checkBox_stopOnFailure = new System.Windows.Forms.CheckBox();
            this.gBox_socketCommunication = new System.Windows.Forms.GroupBox();
            this.lbl_messageToSend = new System.Windows.Forms.Label();
            this.gBox_testStatus = new System.Windows.Forms.GroupBox();
            this.gBox_testDeviceInfo = new System.Windows.Forms.GroupBox();
            this.text_serialNumber = new System.Windows.Forms.TextBox();
            this.lbl_serialNumber = new System.Windows.Forms.Label();
            this.text_modelNumber = new System.Windows.Forms.TextBox();
            this.lbl_modelNumber = new System.Windows.Forms.Label();
            this.gBox_testSetup = new System.Windows.Forms.GroupBox();
            this.gBox_testDeviceLocation = new System.Windows.Forms.GroupBox();
            this.text_pcGroup = new System.Windows.Forms.TextBox();
            this.cBox_computerName = new System.Windows.Forms.ComboBox();
            this.gBox_socketConnectionInfo = new System.Windows.Forms.GroupBox();
            this.gBox_timeMetrics = new System.Windows.Forms.GroupBox();
            this.gBox_socketCommunication.SuspendLayout();
            this.gBox_testStatus.SuspendLayout();
            this.gBox_testDeviceInfo.SuspendLayout();
            this.gBox_testSetup.SuspendLayout();
            this.gBox_testDeviceLocation.SuspendLayout();
            this.gBox_socketConnectionInfo.SuspendLayout();
            this.gBox_timeMetrics.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_socketConnect
            // 
            this.button_socketConnect.Location = new System.Drawing.Point(6, 65);
            this.button_socketConnect.Name = "button_socketConnect";
            this.button_socketConnect.Size = new System.Drawing.Size(112, 34);
            this.button_socketConnect.TabIndex = 0;
            this.button_socketConnect.Text = "Connect";
            this.button_socketConnect.UseVisualStyleBackColor = true;
            this.button_socketConnect.Click += new System.EventHandler(this.button_socket_connect_click);
            // 
            // lBox_recentReceivedMessages
            // 
            this.lBox_recentReceivedMessages.BackColor = System.Drawing.Color.Gainsboro;
            this.lBox_recentReceivedMessages.FormattingEnabled = true;
            this.lBox_recentReceivedMessages.ItemHeight = 25;
            this.lBox_recentReceivedMessages.Location = new System.Drawing.Point(6, 135);
            this.lBox_recentReceivedMessages.Name = "lBox_recentReceivedMessages";
            this.lBox_recentReceivedMessages.Size = new System.Drawing.Size(696, 129);
            this.lBox_recentReceivedMessages.TabIndex = 2;
            // 
            // button_send
            // 
            this.button_send.Location = new System.Drawing.Point(6, 366);
            this.button_send.Name = "button_send";
            this.button_send.Size = new System.Drawing.Size(112, 34);
            this.button_send.TabIndex = 0;
            this.button_send.Text = "Send";
            this.button_send.UseVisualStyleBackColor = true;
            this.button_send.Click += new System.EventHandler(this.button_send_Click);
            // 
            // text_sendBox
            // 
            this.text_sendBox.BackColor = System.Drawing.Color.Gainsboro;
            this.text_sendBox.Location = new System.Drawing.Point(6, 329);
            this.text_sendBox.Name = "text_sendBox";
            this.text_sendBox.Size = new System.Drawing.Size(696, 31);
            this.text_sendBox.TabIndex = 1;
            // 
            // text_lastMessageReceived
            // 
            this.text_lastMessageReceived.BackColor = System.Drawing.Color.Gainsboro;
            this.text_lastMessageReceived.Location = new System.Drawing.Point(6, 65);
            this.text_lastMessageReceived.Multiline = true;
            this.text_lastMessageReceived.Name = "text_lastMessageReceived";
            this.text_lastMessageReceived.Size = new System.Drawing.Size(696, 34);
            this.text_lastMessageReceived.TabIndex = 3;
            // 
            // textBox_clientID
            // 
            this.textBox_clientID.BackColor = System.Drawing.Color.White;
            this.textBox_clientID.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.textBox_clientID.Location = new System.Drawing.Point(139, 111);
            this.textBox_clientID.Name = "textBox_clientID";
            this.textBox_clientID.ReadOnly = true;
            this.textBox_clientID.Size = new System.Drawing.Size(101, 31);
            this.textBox_clientID.TabIndex = 5;
            this.textBox_clientID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbl_clientId
            // 
            this.lbl_clientId.AutoSize = true;
            this.lbl_clientId.Location = new System.Drawing.Point(6, 114);
            this.lbl_clientId.Name = "lbl_clientId";
            this.lbl_clientId.Size = new System.Drawing.Size(127, 25);
            this.lbl_clientId.TabIndex = 6;
            this.lbl_clientId.Text = "This Client\'s ID";
            // 
            // lbl_lastMessageReceived
            // 
            this.lbl_lastMessageReceived.AutoSize = true;
            this.lbl_lastMessageReceived.Location = new System.Drawing.Point(6, 37);
            this.lbl_lastMessageReceived.Name = "lbl_lastMessageReceived";
            this.lbl_lastMessageReceived.Size = new System.Drawing.Size(188, 25);
            this.lbl_lastMessageReceived.TabIndex = 6;
            this.lbl_lastMessageReceived.Text = "Last message received";
            // 
            // lbl_recentReceivedMessages
            // 
            this.lbl_recentReceivedMessages.AutoSize = true;
            this.lbl_recentReceivedMessages.Location = new System.Drawing.Point(6, 109);
            this.lbl_recentReceivedMessages.Name = "lbl_recentReceivedMessages";
            this.lbl_recentReceivedMessages.Size = new System.Drawing.Size(223, 25);
            this.lbl_recentReceivedMessages.TabIndex = 6;
            this.lbl_recentReceivedMessages.Text = "Recent communication log";
            // 
            // text_testGroupIdentifier
            // 
            this.text_testGroupIdentifier.BackColor = System.Drawing.SystemColors.Window;
            this.text_testGroupIdentifier.Location = new System.Drawing.Point(132, 24);
            this.text_testGroupIdentifier.Name = "text_testGroupIdentifier";
            this.text_testGroupIdentifier.Size = new System.Drawing.Size(183, 31);
            this.text_testGroupIdentifier.TabIndex = 11;
            this.text_testGroupIdentifier.Text = "GenericTests";
            // 
            // text_statusOfTest
            // 
            this.text_statusOfTest.BackColor = System.Drawing.Color.Gainsboro;
            this.text_statusOfTest.Location = new System.Drawing.Point(112, 167);
            this.text_statusOfTest.Name = "text_statusOfTest";
            this.text_statusOfTest.Size = new System.Drawing.Size(237, 31);
            this.text_statusOfTest.TabIndex = 13;
            // 
            // text_progress
            // 
            this.text_progress.BackColor = System.Drawing.Color.Gainsboro;
            this.text_progress.Location = new System.Drawing.Point(27, 64);
            this.text_progress.Name = "text_progress";
            this.text_progress.Size = new System.Drawing.Size(56, 31);
            this.text_progress.TabIndex = 14;
            // 
            // progressBar1
            // 
            this.progressBar1.ForeColor = System.Drawing.Color.LimeGreen;
            this.progressBar1.Location = new System.Drawing.Point(89, 61);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(260, 34);
            this.progressBar1.TabIndex = 15;
            // 
            // lbl_programName
            // 
            this.lbl_programName.AutoSize = true;
            this.lbl_programName.Location = new System.Drawing.Point(6, 36);
            this.lbl_programName.Name = "lbl_programName";
            this.lbl_programName.Size = new System.Drawing.Size(133, 25);
            this.lbl_programName.TabIndex = 17;
            this.lbl_programName.Text = "Program Name";
            // 
            // lbl_computerName
            // 
            this.lbl_computerName.AutoSize = true;
            this.lbl_computerName.Location = new System.Drawing.Point(41, 103);
            this.lbl_computerName.Name = "lbl_computerName";
            this.lbl_computerName.Size = new System.Drawing.Size(85, 25);
            this.lbl_computerName.TabIndex = 18;
            this.lbl_computerName.Text = "PC Name";
            // 
            // lbl_slotNumber
            // 
            this.lbl_slotNumber.AutoSize = true;
            this.lbl_slotNumber.Location = new System.Drawing.Point(13, 64);
            this.lbl_slotNumber.Name = "lbl_slotNumber";
            this.lbl_slotNumber.Size = new System.Drawing.Size(113, 25);
            this.lbl_slotNumber.TabIndex = 19;
            this.lbl_slotNumber.Text = "Slot Number";
            // 
            // lbl_testAppVersion
            // 
            this.lbl_testAppVersion.AutoSize = true;
            this.lbl_testAppVersion.Location = new System.Drawing.Point(12, 12);
            this.lbl_testAppVersion.Name = "lbl_testAppVersion";
            this.lbl_testAppVersion.Size = new System.Drawing.Size(179, 25);
            this.lbl_testAppVersion.TabIndex = 20;
            this.lbl_testAppVersion.Text = "Test Program Version";
            // 
            // lbl_testGroupIdentifier
            // 
            this.lbl_testGroupIdentifier.AutoSize = true;
            this.lbl_testGroupIdentifier.Location = new System.Drawing.Point(6, 27);
            this.lbl_testGroupIdentifier.Name = "lbl_testGroupIdentifier";
            this.lbl_testGroupIdentifier.Size = new System.Drawing.Size(120, 25);
            this.lbl_testGroupIdentifier.TabIndex = 21;
            this.lbl_testGroupIdentifier.Text = "Test Group ID";
            // 
            // lbl_satusOfTest
            // 
            this.lbl_satusOfTest.AutoSize = true;
            this.lbl_satusOfTest.Location = new System.Drawing.Point(46, 170);
            this.lbl_satusOfTest.Name = "lbl_satusOfTest";
            this.lbl_satusOfTest.Size = new System.Drawing.Size(60, 25);
            this.lbl_satusOfTest.TabIndex = 23;
            this.lbl_satusOfTest.Text = "Status";
            // 
            // lbl_progress
            // 
            this.lbl_progress.AutoSize = true;
            this.lbl_progress.Location = new System.Drawing.Point(1, 70);
            this.lbl_progress.Name = "lbl_progress";
            this.lbl_progress.Size = new System.Drawing.Size(27, 25);
            this.lbl_progress.TabIndex = 25;
            this.lbl_progress.Text = "%";
            // 
            // lbl_serverIp
            // 
            this.lbl_serverIp.AutoSize = true;
            this.lbl_serverIp.Location = new System.Drawing.Point(6, 30);
            this.lbl_serverIp.Name = "lbl_serverIp";
            this.lbl_serverIp.Size = new System.Drawing.Size(81, 25);
            this.lbl_serverIp.TabIndex = 26;
            this.lbl_serverIp.Text = "Server IP";
            // 
            // cBox_programName
            // 
            this.cBox_programName.BackColor = System.Drawing.Color.Gainsboro;
            this.cBox_programName.FormattingEnabled = true;
            this.cBox_programName.Location = new System.Drawing.Point(145, 33);
            this.cBox_programName.Name = "cBox_programName";
            this.cBox_programName.Size = new System.Drawing.Size(204, 33);
            this.cBox_programName.TabIndex = 27;
            // 
            // cBox_slotNumber
            // 
            this.cBox_slotNumber.BackColor = System.Drawing.SystemColors.Window;
            this.cBox_slotNumber.FormattingEnabled = true;
            this.cBox_slotNumber.Location = new System.Drawing.Point(132, 61);
            this.cBox_slotNumber.Name = "cBox_slotNumber";
            this.cBox_slotNumber.Size = new System.Drawing.Size(182, 33);
            this.cBox_slotNumber.TabIndex = 29;
            // 
            // cBox_testDuration
            // 
            this.cBox_testDuration.BackColor = System.Drawing.SystemColors.Window;
            this.cBox_testDuration.FormattingEnabled = true;
            this.cBox_testDuration.Location = new System.Drawing.Point(128, 163);
            this.cBox_testDuration.Name = "cBox_testDuration";
            this.cBox_testDuration.Size = new System.Drawing.Size(169, 33);
            this.cBox_testDuration.TabIndex = 30;
            // 
            // lbl_testDuration
            // 
            this.lbl_testDuration.AutoSize = true;
            this.lbl_testDuration.Location = new System.Drawing.Point(6, 166);
            this.lbl_testDuration.Name = "lbl_testDuration";
            this.lbl_testDuration.Size = new System.Drawing.Size(116, 25);
            this.lbl_testDuration.TabIndex = 31;
            this.lbl_testDuration.Text = "Test Duration";
            // 
            // lbl_pcGroup
            // 
            this.lbl_pcGroup.AutoSize = true;
            this.lbl_pcGroup.Location = new System.Drawing.Point(41, 142);
            this.lbl_pcGroup.Name = "lbl_pcGroup";
            this.lbl_pcGroup.Size = new System.Drawing.Size(87, 25);
            this.lbl_pcGroup.TabIndex = 32;
            this.lbl_pcGroup.Text = "PC group";
            // 
            // lbl_ECD
            // 
            this.lbl_ECD.AutoSize = true;
            this.lbl_ECD.Location = new System.Drawing.Point(23, 77);
            this.lbl_ECD.Name = "lbl_ECD";
            this.lbl_ECD.Size = new System.Drawing.Size(45, 25);
            this.lbl_ECD.TabIndex = 33;
            this.lbl_ECD.Text = "ECD";
            // 
            // text_ECD
            // 
            this.text_ECD.BackColor = System.Drawing.Color.Gainsboro;
            this.text_ECD.Location = new System.Drawing.Point(71, 74);
            this.text_ECD.Name = "text_ECD";
            this.text_ECD.Size = new System.Drawing.Size(263, 31);
            this.text_ECD.TabIndex = 34;
            this.text_ECD.Text = "wait 10 cycles";
            // 
            // text_startDate
            // 
            this.text_startDate.BackColor = System.Drawing.Color.Gainsboro;
            this.text_startDate.Location = new System.Drawing.Point(71, 37);
            this.text_startDate.Name = "text_startDate";
            this.text_startDate.Size = new System.Drawing.Size(263, 31);
            this.text_startDate.TabIndex = 36;
            // 
            // lbl_startDate
            // 
            this.lbl_startDate.AutoSize = true;
            this.lbl_startDate.Location = new System.Drawing.Point(20, 40);
            this.lbl_startDate.Name = "lbl_startDate";
            this.lbl_startDate.Size = new System.Drawing.Size(48, 25);
            this.lbl_startDate.TabIndex = 35;
            this.lbl_startDate.Text = "Start";
            // 
            // text_totalFailures
            // 
            this.text_totalFailures.BackColor = System.Drawing.Color.Gainsboro;
            this.text_totalFailures.Location = new System.Drawing.Point(246, 24);
            this.text_totalFailures.Name = "text_totalFailures";
            this.text_totalFailures.Size = new System.Drawing.Size(103, 31);
            this.text_totalFailures.TabIndex = 38;
            this.text_totalFailures.Text = "0";
            // 
            // lbl_totalFailures
            // 
            this.lbl_totalFailures.AutoSize = true;
            this.lbl_totalFailures.Location = new System.Drawing.Point(170, 27);
            this.lbl_totalFailures.Name = "lbl_totalFailures";
            this.lbl_totalFailures.Size = new System.Drawing.Size(70, 25);
            this.lbl_totalFailures.TabIndex = 37;
            this.lbl_totalFailures.Text = "Failures";
            // 
            // rbtn_VC
            // 
            this.rbtn_VC.AutoSize = true;
            this.rbtn_VC.Checked = true;
            this.rbtn_VC.Location = new System.Drawing.Point(172, 56);
            this.rbtn_VC.Name = "rbtn_VC";
            this.rbtn_VC.Size = new System.Drawing.Size(149, 29);
            this.rbtn_VC.TabIndex = 43;
            this.rbtn_VC.TabStop = true;
            this.rbtn_VC.Text = "Voltage Check";
            this.rbtn_VC.UseVisualStyleBackColor = true;
            // 
            // rbtn_BT
            // 
            this.rbtn_BT.AutoSize = true;
            this.rbtn_BT.Location = new System.Drawing.Point(172, 91);
            this.rbtn_BT.Name = "rbtn_BT";
            this.rbtn_BT.Size = new System.Drawing.Size(110, 29);
            this.rbtn_BT.TabIndex = 44;
            this.rbtn_BT.Text = "Boot Test";
            this.rbtn_BT.UseVisualStyleBackColor = true;
            // 
            // rbtn_PT
            // 
            this.rbtn_PT.AutoSize = true;
            this.rbtn_PT.Location = new System.Drawing.Point(172, 126);
            this.rbtn_PT.Name = "rbtn_PT";
            this.rbtn_PT.Size = new System.Drawing.Size(120, 29);
            this.rbtn_PT.TabIndex = 45;
            this.rbtn_PT.Text = "Power Test";
            this.rbtn_PT.UseVisualStyleBackColor = true;
            // 
            // lbl_selectTest
            // 
            this.lbl_selectTest.AutoSize = true;
            this.lbl_selectTest.Location = new System.Drawing.Point(172, 28);
            this.lbl_selectTest.Name = "lbl_selectTest";
            this.lbl_selectTest.Size = new System.Drawing.Size(93, 25);
            this.lbl_selectTest.TabIndex = 46;
            this.lbl_selectTest.Text = "Select Test";
            // 
            // text_currentTest
            // 
            this.text_currentTest.BackColor = System.Drawing.Color.Gainsboro;
            this.text_currentTest.Location = new System.Drawing.Point(112, 130);
            this.text_currentTest.Name = "text_currentTest";
            this.text_currentTest.Size = new System.Drawing.Size(237, 31);
            this.text_currentTest.TabIndex = 48;
            // 
            // lbl_currentTest
            // 
            this.lbl_currentTest.AutoSize = true;
            this.lbl_currentTest.Location = new System.Drawing.Point(1, 133);
            this.lbl_currentTest.Name = "lbl_currentTest";
            this.lbl_currentTest.Size = new System.Drawing.Size(105, 25);
            this.lbl_currentTest.TabIndex = 47;
            this.lbl_currentTest.Text = "Current Test";
            // 
            // rText_statusOfTest
            // 
            this.rText_statusOfTest.BackColor = System.Drawing.Color.Gainsboro;
            this.rText_statusOfTest.Location = new System.Drawing.Point(6, 239);
            this.rText_statusOfTest.Name = "rText_statusOfTest";
            this.rText_statusOfTest.Size = new System.Drawing.Size(343, 231);
            this.rText_statusOfTest.TabIndex = 49;
            this.rText_statusOfTest.Text = "";
            // 
            // lbl_richTextStatusOfTests
            // 
            this.lbl_richTextStatusOfTests.AutoSize = true;
            this.lbl_richTextStatusOfTests.Location = new System.Drawing.Point(6, 211);
            this.lbl_richTextStatusOfTests.Name = "lbl_richTextStatusOfTests";
            this.lbl_richTextStatusOfTests.Size = new System.Drawing.Size(182, 25);
            this.lbl_richTextStatusOfTests.TabIndex = 50;
            this.lbl_richTextStatusOfTests.Text = "Status of running test";
            // 
            // cBox_serverIp
            // 
            this.cBox_serverIp.BackColor = System.Drawing.SystemColors.Window;
            this.cBox_serverIp.FormattingEnabled = true;
            this.cBox_serverIp.Location = new System.Drawing.Point(139, 27);
            this.cBox_serverIp.Name = "cBox_serverIp";
            this.cBox_serverIp.Size = new System.Drawing.Size(210, 33);
            this.cBox_serverIp.TabIndex = 51;
            // 
            // lbl_completedCycles
            // 
            this.lbl_completedCycles.AutoSize = true;
            this.lbl_completedCycles.Location = new System.Drawing.Point(6, 27);
            this.lbl_completedCycles.Name = "lbl_completedCycles";
            this.lbl_completedCycles.Size = new System.Drawing.Size(61, 25);
            this.lbl_completedCycles.TabIndex = 53;
            this.lbl_completedCycles.Text = "Cycles";
            // 
            // text_completedCycles
            // 
            this.text_completedCycles.BackColor = System.Drawing.Color.Gainsboro;
            this.text_completedCycles.Location = new System.Drawing.Point(73, 24);
            this.text_completedCycles.Name = "text_completedCycles";
            this.text_completedCycles.Size = new System.Drawing.Size(83, 31);
            this.text_completedCycles.TabIndex = 52;
            this.text_completedCycles.Text = "0";
            // 
            // button_startTest
            // 
            this.button_startTest.Location = new System.Drawing.Point(6, 30);
            this.button_startTest.Name = "button_startTest";
            this.button_startTest.Size = new System.Drawing.Size(159, 123);
            this.button_startTest.TabIndex = 54;
            this.button_startTest.Text = "Start Test";
            this.button_startTest.UseVisualStyleBackColor = true;
            this.button_startTest.Click += new System.EventHandler(this.button_startTest_Click);
            // 
            // lbl_testNotes
            // 
            this.lbl_testNotes.AutoSize = true;
            this.lbl_testNotes.Location = new System.Drawing.Point(11, 483);
            this.lbl_testNotes.Name = "lbl_testNotes";
            this.lbl_testNotes.Size = new System.Drawing.Size(189, 25);
            this.lbl_testNotes.TabIndex = 56;
            this.lbl_testNotes.Text = "Test Notes for logging";
            // 
            // rText_testNotes
            // 
            this.rText_testNotes.BackColor = System.Drawing.SystemColors.Window;
            this.rText_testNotes.Location = new System.Drawing.Point(6, 511);
            this.rText_testNotes.Name = "rText_testNotes";
            this.rText_testNotes.Size = new System.Drawing.Size(328, 115);
            this.rText_testNotes.TabIndex = 55;
            this.rText_testNotes.Text = "";
            // 
            // checkBox_forceFailureOnNextCycle
            // 
            this.checkBox_forceFailureOnNextCycle.AutoSize = true;
            this.checkBox_forceFailureOnNextCycle.Location = new System.Drawing.Point(6, 202);
            this.checkBox_forceFailureOnNextCycle.Name = "checkBox_forceFailureOnNextCycle";
            this.checkBox_forceFailureOnNextCycle.Size = new System.Drawing.Size(241, 29);
            this.checkBox_forceFailureOnNextCycle.TabIndex = 57;
            this.checkBox_forceFailureOnNextCycle.Text = "Force failure on next cycle";
            this.checkBox_forceFailureOnNextCycle.UseVisualStyleBackColor = true;
            // 
            // text_dateCompleted
            // 
            this.text_dateCompleted.BackColor = System.Drawing.Color.Gainsboro;
            this.text_dateCompleted.Location = new System.Drawing.Point(71, 111);
            this.text_dateCompleted.Name = "text_dateCompleted";
            this.text_dateCompleted.Size = new System.Drawing.Size(263, 31);
            this.text_dateCompleted.TabIndex = 59;
            // 
            // lbl_dateCompleted
            // 
            this.lbl_dateCompleted.AutoSize = true;
            this.lbl_dateCompleted.Location = new System.Drawing.Point(11, 114);
            this.lbl_dateCompleted.Name = "lbl_dateCompleted";
            this.lbl_dateCompleted.Size = new System.Drawing.Size(57, 25);
            this.lbl_dateCompleted.TabIndex = 58;
            this.lbl_dateCompleted.Text = "Finish";
            // 
            // text_hoursPer1kCycles
            // 
            this.text_hoursPer1kCycles.BackColor = System.Drawing.Color.Gainsboro;
            this.text_hoursPer1kCycles.Location = new System.Drawing.Point(150, 148);
            this.text_hoursPer1kCycles.Name = "text_hoursPer1kCycles";
            this.text_hoursPer1kCycles.Size = new System.Drawing.Size(132, 31);
            this.text_hoursPer1kCycles.TabIndex = 65;
            this.text_hoursPer1kCycles.Text = "wait 10 cycles";
            // 
            // lbl_hoursPer1kCycles
            // 
            this.lbl_hoursPer1kCycles.AutoSize = true;
            this.lbl_hoursPer1kCycles.Location = new System.Drawing.Point(6, 151);
            this.lbl_hoursPer1kCycles.Name = "lbl_hoursPer1kCycles";
            this.lbl_hoursPer1kCycles.Size = new System.Drawing.Size(137, 25);
            this.lbl_hoursPer1kCycles.TabIndex = 64;
            this.lbl_hoursPer1kCycles.Text = "Hours/1k cycles";
            // 
            // lbl_testAppVersionNum
            // 
            this.lbl_testAppVersionNum.AutoSize = true;
            this.lbl_testAppVersionNum.Location = new System.Drawing.Point(197, 12);
            this.lbl_testAppVersionNum.Name = "lbl_testAppVersionNum";
            this.lbl_testAppVersionNum.Size = new System.Drawing.Size(19, 25);
            this.lbl_testAppVersionNum.TabIndex = 66;
            this.lbl_testAppVersionNum.Text = "-";
            // 
            // checkBox_stopOnFailure
            // 
            this.checkBox_stopOnFailure.AutoSize = true;
            this.checkBox_stopOnFailure.Checked = true;
            this.checkBox_stopOnFailure.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_stopOnFailure.Location = new System.Drawing.Point(6, 226);
            this.checkBox_stopOnFailure.Name = "checkBox_stopOnFailure";
            this.checkBox_stopOnFailure.Size = new System.Drawing.Size(188, 29);
            this.checkBox_stopOnFailure.TabIndex = 67;
            this.checkBox_stopOnFailure.Text = "Stop test on failure";
            this.checkBox_stopOnFailure.UseVisualStyleBackColor = true;
            // 
            // gBox_socketCommunication
            // 
            this.gBox_socketCommunication.BackColor = System.Drawing.Color.Silver;
            this.gBox_socketCommunication.Controls.Add(this.lbl_messageToSend);
            this.gBox_socketCommunication.Controls.Add(this.lBox_recentReceivedMessages);
            this.gBox_socketCommunication.Controls.Add(this.button_send);
            this.gBox_socketCommunication.Controls.Add(this.text_sendBox);
            this.gBox_socketCommunication.Controls.Add(this.text_lastMessageReceived);
            this.gBox_socketCommunication.Controls.Add(this.lbl_lastMessageReceived);
            this.gBox_socketCommunication.Controls.Add(this.lbl_recentReceivedMessages);
            this.gBox_socketCommunication.Location = new System.Drawing.Point(12, 886);
            this.gBox_socketCommunication.Name = "gBox_socketCommunication";
            this.gBox_socketCommunication.Size = new System.Drawing.Size(708, 407);
            this.gBox_socketCommunication.TabIndex = 68;
            this.gBox_socketCommunication.TabStop = false;
            this.gBox_socketCommunication.Text = "Socket Communication";
            // 
            // lbl_messageToSend
            // 
            this.lbl_messageToSend.AutoSize = true;
            this.lbl_messageToSend.Location = new System.Drawing.Point(6, 301);
            this.lbl_messageToSend.Name = "lbl_messageToSend";
            this.lbl_messageToSend.Size = new System.Drawing.Size(221, 25);
            this.lbl_messageToSend.TabIndex = 7;
            this.lbl_messageToSend.Text = "Message to send to server";
            // 
            // gBox_testStatus
            // 
            this.gBox_testStatus.BackColor = System.Drawing.Color.Silver;
            this.gBox_testStatus.Controls.Add(this.lbl_richTextStatusOfTests);
            this.gBox_testStatus.Controls.Add(this.rText_statusOfTest);
            this.gBox_testStatus.Controls.Add(this.progressBar1);
            this.gBox_testStatus.Controls.Add(this.text_statusOfTest);
            this.gBox_testStatus.Controls.Add(this.text_progress);
            this.gBox_testStatus.Controls.Add(this.lbl_satusOfTest);
            this.gBox_testStatus.Controls.Add(this.lbl_progress);
            this.gBox_testStatus.Controls.Add(this.lbl_totalFailures);
            this.gBox_testStatus.Controls.Add(this.text_totalFailures);
            this.gBox_testStatus.Controls.Add(this.lbl_currentTest);
            this.gBox_testStatus.Controls.Add(this.text_currentTest);
            this.gBox_testStatus.Controls.Add(this.text_completedCycles);
            this.gBox_testStatus.Controls.Add(this.lbl_completedCycles);
            this.gBox_testStatus.Location = new System.Drawing.Point(365, 199);
            this.gBox_testStatus.Name = "gBox_testStatus";
            this.gBox_testStatus.Size = new System.Drawing.Size(355, 476);
            this.gBox_testStatus.TabIndex = 69;
            this.gBox_testStatus.TabStop = false;
            this.gBox_testStatus.Text = "Test Staus";
            // 
            // gBox_testDeviceInfo
            // 
            this.gBox_testDeviceInfo.BackColor = System.Drawing.Color.Silver;
            this.gBox_testDeviceInfo.Controls.Add(this.text_serialNumber);
            this.gBox_testDeviceInfo.Controls.Add(this.lbl_serialNumber);
            this.gBox_testDeviceInfo.Controls.Add(this.text_modelNumber);
            this.gBox_testDeviceInfo.Controls.Add(this.lbl_modelNumber);
            this.gBox_testDeviceInfo.Controls.Add(this.lbl_programName);
            this.gBox_testDeviceInfo.Controls.Add(this.cBox_programName);
            this.gBox_testDeviceInfo.Location = new System.Drawing.Point(365, 681);
            this.gBox_testDeviceInfo.Name = "gBox_testDeviceInfo";
            this.gBox_testDeviceInfo.Size = new System.Drawing.Size(355, 199);
            this.gBox_testDeviceInfo.TabIndex = 70;
            this.gBox_testDeviceInfo.TabStop = false;
            this.gBox_testDeviceInfo.Text = "Test Device Info";
            // 
            // text_serialNumber
            // 
            this.text_serialNumber.BackColor = System.Drawing.Color.Gainsboro;
            this.text_serialNumber.Location = new System.Drawing.Point(145, 109);
            this.text_serialNumber.Name = "text_serialNumber";
            this.text_serialNumber.Size = new System.Drawing.Size(204, 31);
            this.text_serialNumber.TabIndex = 30;
            // 
            // lbl_serialNumber
            // 
            this.lbl_serialNumber.AutoSize = true;
            this.lbl_serialNumber.Location = new System.Drawing.Point(33, 112);
            this.lbl_serialNumber.Name = "lbl_serialNumber";
            this.lbl_serialNumber.Size = new System.Drawing.Size(35, 25);
            this.lbl_serialNumber.TabIndex = 31;
            this.lbl_serialNumber.Text = "SN";
            // 
            // text_modelNumber
            // 
            this.text_modelNumber.BackColor = System.Drawing.Color.Gainsboro;
            this.text_modelNumber.Location = new System.Drawing.Point(145, 72);
            this.text_modelNumber.Name = "text_modelNumber";
            this.text_modelNumber.Size = new System.Drawing.Size(204, 31);
            this.text_modelNumber.TabIndex = 28;
            // 
            // lbl_modelNumber
            // 
            this.lbl_modelNumber.AutoSize = true;
            this.lbl_modelNumber.Location = new System.Drawing.Point(33, 75);
            this.lbl_modelNumber.Name = "lbl_modelNumber";
            this.lbl_modelNumber.Size = new System.Drawing.Size(63, 25);
            this.lbl_modelNumber.TabIndex = 29;
            this.lbl_modelNumber.Text = "Model";
            // 
            // gBox_testSetup
            // 
            this.gBox_testSetup.BackColor = System.Drawing.Color.Silver;
            this.gBox_testSetup.Controls.Add(this.gBox_testDeviceLocation);
            this.gBox_testSetup.Controls.Add(this.button_startTest);
            this.gBox_testSetup.Controls.Add(this.cBox_testDuration);
            this.gBox_testSetup.Controls.Add(this.lbl_testDuration);
            this.gBox_testSetup.Controls.Add(this.rbtn_VC);
            this.gBox_testSetup.Controls.Add(this.checkBox_stopOnFailure);
            this.gBox_testSetup.Controls.Add(this.rbtn_BT);
            this.gBox_testSetup.Controls.Add(this.rbtn_PT);
            this.gBox_testSetup.Controls.Add(this.lbl_selectTest);
            this.gBox_testSetup.Controls.Add(this.checkBox_forceFailureOnNextCycle);
            this.gBox_testSetup.Controls.Add(this.rText_testNotes);
            this.gBox_testSetup.Controls.Add(this.lbl_testNotes);
            this.gBox_testSetup.Location = new System.Drawing.Point(12, 43);
            this.gBox_testSetup.Name = "gBox_testSetup";
            this.gBox_testSetup.Size = new System.Drawing.Size(347, 632);
            this.gBox_testSetup.TabIndex = 71;
            this.gBox_testSetup.TabStop = false;
            this.gBox_testSetup.Text = "Test Setup";
            // 
            // gBox_testDeviceLocation
            // 
            this.gBox_testDeviceLocation.Controls.Add(this.text_pcGroup);
            this.gBox_testDeviceLocation.Controls.Add(this.cBox_computerName);
            this.gBox_testDeviceLocation.Controls.Add(this.lbl_testGroupIdentifier);
            this.gBox_testDeviceLocation.Controls.Add(this.text_testGroupIdentifier);
            this.gBox_testDeviceLocation.Controls.Add(this.lbl_computerName);
            this.gBox_testDeviceLocation.Controls.Add(this.lbl_slotNumber);
            this.gBox_testDeviceLocation.Controls.Add(this.cBox_slotNumber);
            this.gBox_testDeviceLocation.Controls.Add(this.lbl_pcGroup);
            this.gBox_testDeviceLocation.Location = new System.Drawing.Point(6, 286);
            this.gBox_testDeviceLocation.Name = "gBox_testDeviceLocation";
            this.gBox_testDeviceLocation.Size = new System.Drawing.Size(328, 180);
            this.gBox_testDeviceLocation.TabIndex = 72;
            this.gBox_testDeviceLocation.TabStop = false;
            this.gBox_testDeviceLocation.Text = "Test Device Location";
            // 
            // text_pcGroup
            // 
            this.text_pcGroup.BackColor = System.Drawing.Color.Gainsboro;
            this.text_pcGroup.Location = new System.Drawing.Point(132, 139);
            this.text_pcGroup.Name = "text_pcGroup";
            this.text_pcGroup.Size = new System.Drawing.Size(183, 31);
            this.text_pcGroup.TabIndex = 34;
            this.text_pcGroup.Text = "Determined by PC";
            // 
            // cBox_computerName
            // 
            this.cBox_computerName.BackColor = System.Drawing.SystemColors.Window;
            this.cBox_computerName.FormattingEnabled = true;
            this.cBox_computerName.Location = new System.Drawing.Point(132, 100);
            this.cBox_computerName.Name = "cBox_computerName";
            this.cBox_computerName.Size = new System.Drawing.Size(182, 33);
            this.cBox_computerName.TabIndex = 33;
            // 
            // gBox_socketConnectionInfo
            // 
            this.gBox_socketConnectionInfo.BackColor = System.Drawing.Color.Silver;
            this.gBox_socketConnectionInfo.Controls.Add(this.button_socketConnect);
            this.gBox_socketConnectionInfo.Controls.Add(this.textBox_clientID);
            this.gBox_socketConnectionInfo.Controls.Add(this.lbl_clientId);
            this.gBox_socketConnectionInfo.Controls.Add(this.lbl_serverIp);
            this.gBox_socketConnectionInfo.Controls.Add(this.cBox_serverIp);
            this.gBox_socketConnectionInfo.Location = new System.Drawing.Point(365, 43);
            this.gBox_socketConnectionInfo.Name = "gBox_socketConnectionInfo";
            this.gBox_socketConnectionInfo.Size = new System.Drawing.Size(355, 150);
            this.gBox_socketConnectionInfo.TabIndex = 72;
            this.gBox_socketConnectionInfo.TabStop = false;
            this.gBox_socketConnectionInfo.Text = "Socket Connection Info";
            // 
            // gBox_timeMetrics
            // 
            this.gBox_timeMetrics.BackColor = System.Drawing.Color.Silver;
            this.gBox_timeMetrics.Controls.Add(this.lbl_startDate);
            this.gBox_timeMetrics.Controls.Add(this.lbl_ECD);
            this.gBox_timeMetrics.Controls.Add(this.text_ECD);
            this.gBox_timeMetrics.Controls.Add(this.text_startDate);
            this.gBox_timeMetrics.Controls.Add(this.text_hoursPer1kCycles);
            this.gBox_timeMetrics.Controls.Add(this.lbl_hoursPer1kCycles);
            this.gBox_timeMetrics.Controls.Add(this.lbl_dateCompleted);
            this.gBox_timeMetrics.Controls.Add(this.text_dateCompleted);
            this.gBox_timeMetrics.Location = new System.Drawing.Point(12, 681);
            this.gBox_timeMetrics.Name = "gBox_timeMetrics";
            this.gBox_timeMetrics.Size = new System.Drawing.Size(347, 199);
            this.gBox_timeMetrics.TabIndex = 73;
            this.gBox_timeMetrics.TabStop = false;
            this.gBox_timeMetrics.Text = "Time Metrics";
            // 
            // Client_GUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gray;
            this.ClientSize = new System.Drawing.Size(729, 1316);
            this.Controls.Add(this.gBox_timeMetrics);
            this.Controls.Add(this.gBox_socketConnectionInfo);
            this.Controls.Add(this.gBox_testSetup);
            this.Controls.Add(this.gBox_testDeviceInfo);
            this.Controls.Add(this.gBox_testStatus);
            this.Controls.Add(this.gBox_socketCommunication);
            this.Controls.Add(this.lbl_testAppVersionNum);
            this.Controls.Add(this.lbl_testAppVersion);
            this.Name = "Client_GUI";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.gBox_socketCommunication.ResumeLayout(false);
            this.gBox_socketCommunication.PerformLayout();
            this.gBox_testStatus.ResumeLayout(false);
            this.gBox_testStatus.PerformLayout();
            this.gBox_testDeviceInfo.ResumeLayout(false);
            this.gBox_testDeviceInfo.PerformLayout();
            this.gBox_testSetup.ResumeLayout(false);
            this.gBox_testSetup.PerformLayout();
            this.gBox_testDeviceLocation.ResumeLayout(false);
            this.gBox_testDeviceLocation.PerformLayout();
            this.gBox_socketConnectionInfo.ResumeLayout(false);
            this.gBox_socketConnectionInfo.PerformLayout();
            this.gBox_timeMetrics.ResumeLayout(false);
            this.gBox_timeMetrics.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Button button_socketConnect;
        public System.Windows.Forms.ListBox lBox_recentReceivedMessages;
        public System.Windows.Forms.Button button_send;
        public System.Windows.Forms.TextBox text_sendBox;
        public System.Windows.Forms.TextBox text_lastMessageReceived;
        public System.Windows.Forms.TextBox textBox_clientID;
        public System.Windows.Forms.Label lbl_clientId;
        public System.Windows.Forms.Label lbl_lastMessageReceived;
        public System.Windows.Forms.Label lbl_recentReceivedMessages;
        public System.Windows.Forms.TextBox text_testGroupIdentifier;
        public System.Windows.Forms.TextBox text_statusOfTest;
        public System.Windows.Forms.TextBox text_progress;
        public System.Windows.Forms.ProgressBar progressBar1;
        public System.Windows.Forms.Label lbl_programName;
        public System.Windows.Forms.Label lbl_computerName;
        public System.Windows.Forms.Label lbl_slotNumber;
        public System.Windows.Forms.Label lbl_testAppVersion;
        public System.Windows.Forms.Label lbl_testGroupIdentifier;
        public System.Windows.Forms.Label lbl_satusOfTest;
        public System.Windows.Forms.Label lbl_progress;
        public System.Windows.Forms.Label lbl_serverIp;
        public ComboBox cBox_programName;
        public ComboBox cBox_slotNumber;
        public ComboBox cBox_testDuration;
        public Label lbl_testDuration;
        public Label lbl_pcGroup;
        public Label lbl_ECD;
        public TextBox text_ECD;
        public TextBox text_startDate;
        public Label lbl_startDate;
        public TextBox text_totalFailures;
        public Label lbl_totalFailures;
        public RadioButton rbtn_VC;
        public RadioButton rbtn_BT;
        public RadioButton rbtn_PT;
        public Label lbl_selectTest;
        public TextBox text_currentTest;
        public Label lbl_currentTest;
        public RichTextBox rText_statusOfTest;
        public Label lbl_richTextStatusOfTests;
        public ComboBox cBox_serverIp;
        public Label lbl_completedCycles;
        public TextBox text_completedCycles;
        public Button button_startTest;
        public Label lbl_testNotes;
        public RichTextBox rText_testNotes;
        public CheckBox checkBox_forceFailureOnNextCycle;
        public TextBox text_dateCompleted;
        public Label lbl_dateCompleted;
        public TextBox text_hoursPer1kCycles;
        public Label lbl_hoursPer1kCycles;
        public Label lbl_testAppVersionNum;
        public CheckBox checkBox_stopOnFailure;
        private GroupBox gBox_socketCommunication;
        private GroupBox gBox_testStatus;
        private GroupBox gBox_testDeviceInfo;
        private GroupBox gBox_testSetup;
        private GroupBox gBox_testDeviceLocation;
        private GroupBox gBox_socketConnectionInfo;
        private GroupBox gBox_timeMetrics;
        public TextBox text_serialNumber;
        public Label lbl_serialNumber;
        public TextBox text_modelNumber;
        public Label lbl_modelNumber;
        public Label lbl_messageToSend;
        public ComboBox cBox_computerName;
        public TextBox text_pcGroup;
    }
}

