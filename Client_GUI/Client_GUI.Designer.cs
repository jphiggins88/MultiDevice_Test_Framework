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
            this.text_computerName = new System.Windows.Forms.TextBox();
            this.textBox_testAppVersion = new System.Windows.Forms.TextBox();
            this.text_testGroupIdentifier = new System.Windows.Forms.TextBox();
            this.text_deviceType = new System.Windows.Forms.TextBox();
            this.text_statusOfTest = new System.Windows.Forms.TextBox();
            this.text_progress = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.text_testType = new System.Windows.Forms.TextBox();
            this.lbl_programName = new System.Windows.Forms.Label();
            this.lbl_computerName = new System.Windows.Forms.Label();
            this.lbl_slotNumber = new System.Windows.Forms.Label();
            this.lbl_testAppVersion = new System.Windows.Forms.Label();
            this.lbl_testGroupIdentifier = new System.Windows.Forms.Label();
            this.lbl_deviceType = new System.Windows.Forms.Label();
            this.lbl_satusOfTest = new System.Windows.Forms.Label();
            this.lbl_testType = new System.Windows.Forms.Label();
            this.lbl_progress = new System.Windows.Forms.Label();
            this.lbl_serverIp = new System.Windows.Forms.Label();
            this.cBox_programName = new System.Windows.Forms.ComboBox();
            this.cBox_pcGroup = new System.Windows.Forms.ComboBox();
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
            this.text_completedDays = new System.Windows.Forms.TextBox();
            this.lbl_completedDays = new System.Windows.Forms.Label();
            this.text_remainingDays = new System.Windows.Forms.TextBox();
            this.lbl_remainingDays = new System.Windows.Forms.Label();
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
            this.text_remainingHours = new System.Windows.Forms.TextBox();
            this.text_completedHours = new System.Windows.Forms.TextBox();
            this.lbl_remainingHours = new System.Windows.Forms.Label();
            this.lbl_completedHours = new System.Windows.Forms.Label();
            this.text_hoursPer1kCycles = new System.Windows.Forms.TextBox();
            this.lbl_hoursPer1kCycles = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button_socketConnect
            // 
            this.button_socketConnect.Location = new System.Drawing.Point(35, 80);
            this.button_socketConnect.Name = "button_socketConnect";
            this.button_socketConnect.Size = new System.Drawing.Size(112, 34);
            this.button_socketConnect.TabIndex = 0;
            this.button_socketConnect.Text = "Connect";
            this.button_socketConnect.UseVisualStyleBackColor = true;
            this.button_socketConnect.Click += new System.EventHandler(this.button_socket_connect_click);
            // 
            // lBox_recentReceivedMessages
            // 
            this.lBox_recentReceivedMessages.FormattingEnabled = true;
            this.lBox_recentReceivedMessages.ItemHeight = 25;
            this.lBox_recentReceivedMessages.Location = new System.Drawing.Point(12, 1004);
            this.lBox_recentReceivedMessages.Name = "lBox_recentReceivedMessages";
            this.lBox_recentReceivedMessages.Size = new System.Drawing.Size(322, 204);
            this.lBox_recentReceivedMessages.TabIndex = 2;
            // 
            // button_send
            // 
            this.button_send.Location = new System.Drawing.Point(307, 1215);
            this.button_send.Name = "button_send";
            this.button_send.Size = new System.Drawing.Size(112, 34);
            this.button_send.TabIndex = 0;
            this.button_send.Text = "Send";
            this.button_send.UseVisualStyleBackColor = true;
            this.button_send.Click += new System.EventHandler(this.button_send_Click);
            // 
            // text_sendBox
            // 
            this.text_sendBox.Location = new System.Drawing.Point(17, 1215);
            this.text_sendBox.Name = "text_sendBox";
            this.text_sendBox.Size = new System.Drawing.Size(269, 31);
            this.text_sendBox.TabIndex = 1;
            // 
            // text_lastMessageReceived
            // 
            this.text_lastMessageReceived.Location = new System.Drawing.Point(365, 1004);
            this.text_lastMessageReceived.Multiline = true;
            this.text_lastMessageReceived.Name = "text_lastMessageReceived";
            this.text_lastMessageReceived.Size = new System.Drawing.Size(404, 41);
            this.text_lastMessageReceived.TabIndex = 3;
            // 
            // textBox_clientID
            // 
            this.textBox_clientID.BackColor = System.Drawing.Color.White;
            this.textBox_clientID.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.textBox_clientID.Location = new System.Drawing.Point(703, 12);
            this.textBox_clientID.Name = "textBox_clientID";
            this.textBox_clientID.ReadOnly = true;
            this.textBox_clientID.Size = new System.Drawing.Size(101, 31);
            this.textBox_clientID.TabIndex = 5;
            this.textBox_clientID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbl_clientId
            // 
            this.lbl_clientId.AutoSize = true;
            this.lbl_clientId.Location = new System.Drawing.Point(570, 15);
            this.lbl_clientId.Name = "lbl_clientId";
            this.lbl_clientId.Size = new System.Drawing.Size(127, 25);
            this.lbl_clientId.TabIndex = 6;
            this.lbl_clientId.Text = "This Client\'s ID";
            // 
            // lbl_lastMessageReceived
            // 
            this.lbl_lastMessageReceived.AutoSize = true;
            this.lbl_lastMessageReceived.Location = new System.Drawing.Point(365, 976);
            this.lbl_lastMessageReceived.Name = "lbl_lastMessageReceived";
            this.lbl_lastMessageReceived.Size = new System.Drawing.Size(188, 25);
            this.lbl_lastMessageReceived.TabIndex = 6;
            this.lbl_lastMessageReceived.Text = "Last message received";
            // 
            // lbl_recentReceivedMessages
            // 
            this.lbl_recentReceivedMessages.AutoSize = true;
            this.lbl_recentReceivedMessages.Location = new System.Drawing.Point(12, 976);
            this.lbl_recentReceivedMessages.Name = "lbl_recentReceivedMessages";
            this.lbl_recentReceivedMessages.Size = new System.Drawing.Size(223, 25);
            this.lbl_recentReceivedMessages.TabIndex = 6;
            this.lbl_recentReceivedMessages.Text = "Recent communication log";
            // 
            // text_computerName
            // 
            this.text_computerName.Location = new System.Drawing.Point(197, 589);
            this.text_computerName.Name = "text_computerName";
            this.text_computerName.Size = new System.Drawing.Size(269, 31);
            this.text_computerName.TabIndex = 8;
            // 
            // textBox_testAppVersion
            // 
            this.textBox_testAppVersion.Location = new System.Drawing.Point(217, 12);
            this.textBox_testAppVersion.Name = "textBox_testAppVersion";
            this.textBox_testAppVersion.Size = new System.Drawing.Size(269, 31);
            this.textBox_testAppVersion.TabIndex = 10;
            // 
            // text_testGroupIdentifier
            // 
            this.text_testGroupIdentifier.Location = new System.Drawing.Point(197, 663);
            this.text_testGroupIdentifier.Name = "text_testGroupIdentifier";
            this.text_testGroupIdentifier.Size = new System.Drawing.Size(269, 31);
            this.text_testGroupIdentifier.TabIndex = 11;
            // 
            // text_deviceType
            // 
            this.text_deviceType.Location = new System.Drawing.Point(197, 700);
            this.text_deviceType.Name = "text_deviceType";
            this.text_deviceType.Size = new System.Drawing.Size(269, 31);
            this.text_deviceType.TabIndex = 12;
            // 
            // text_statusOfTest
            // 
            this.text_statusOfTest.Location = new System.Drawing.Point(197, 737);
            this.text_statusOfTest.Name = "text_statusOfTest";
            this.text_statusOfTest.Size = new System.Drawing.Size(269, 31);
            this.text_statusOfTest.TabIndex = 13;
            // 
            // text_progress
            // 
            this.text_progress.Location = new System.Drawing.Point(111, 401);
            this.text_progress.Name = "text_progress";
            this.text_progress.Size = new System.Drawing.Size(84, 31);
            this.text_progress.TabIndex = 14;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(24, 435);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(655, 34);
            this.progressBar1.TabIndex = 15;
            // 
            // text_testType
            // 
            this.text_testType.Location = new System.Drawing.Point(197, 774);
            this.text_testType.Name = "text_testType";
            this.text_testType.Size = new System.Drawing.Size(269, 31);
            this.text_testType.TabIndex = 16;
            // 
            // lbl_programName
            // 
            this.lbl_programName.AutoSize = true;
            this.lbl_programName.Location = new System.Drawing.Point(33, 555);
            this.lbl_programName.Name = "lbl_programName";
            this.lbl_programName.Size = new System.Drawing.Size(133, 25);
            this.lbl_programName.TabIndex = 17;
            this.lbl_programName.Text = "Program Name";
            // 
            // lbl_computerName
            // 
            this.lbl_computerName.AutoSize = true;
            this.lbl_computerName.Location = new System.Drawing.Point(33, 592);
            this.lbl_computerName.Name = "lbl_computerName";
            this.lbl_computerName.Size = new System.Drawing.Size(144, 25);
            this.lbl_computerName.TabIndex = 18;
            this.lbl_computerName.Text = "Computer Name";
            // 
            // lbl_slotNumber
            // 
            this.lbl_slotNumber.AutoSize = true;
            this.lbl_slotNumber.Location = new System.Drawing.Point(33, 629);
            this.lbl_slotNumber.Name = "lbl_slotNumber";
            this.lbl_slotNumber.Size = new System.Drawing.Size(113, 25);
            this.lbl_slotNumber.TabIndex = 19;
            this.lbl_slotNumber.Text = "Slot Number";
            // 
            // lbl_testAppVersion
            // 
            this.lbl_testAppVersion.AutoSize = true;
            this.lbl_testAppVersion.Location = new System.Drawing.Point(32, 15);
            this.lbl_testAppVersion.Name = "lbl_testAppVersion";
            this.lbl_testAppVersion.Size = new System.Drawing.Size(179, 25);
            this.lbl_testAppVersion.TabIndex = 20;
            this.lbl_testAppVersion.Text = "Test Program Version";
            // 
            // lbl_testGroupIdentifier
            // 
            this.lbl_testGroupIdentifier.AutoSize = true;
            this.lbl_testGroupIdentifier.Location = new System.Drawing.Point(33, 666);
            this.lbl_testGroupIdentifier.Name = "lbl_testGroupIdentifier";
            this.lbl_testGroupIdentifier.Size = new System.Drawing.Size(120, 25);
            this.lbl_testGroupIdentifier.TabIndex = 21;
            this.lbl_testGroupIdentifier.Text = "Test Group ID";
            // 
            // lbl_deviceType
            // 
            this.lbl_deviceType.AutoSize = true;
            this.lbl_deviceType.Location = new System.Drawing.Point(33, 703);
            this.lbl_deviceType.Name = "lbl_deviceType";
            this.lbl_deviceType.Size = new System.Drawing.Size(106, 25);
            this.lbl_deviceType.TabIndex = 22;
            this.lbl_deviceType.Text = "Device Type";
            // 
            // lbl_satusOfTest
            // 
            this.lbl_satusOfTest.AutoSize = true;
            this.lbl_satusOfTest.Location = new System.Drawing.Point(33, 740);
            this.lbl_satusOfTest.Name = "lbl_satusOfTest";
            this.lbl_satusOfTest.Size = new System.Drawing.Size(60, 25);
            this.lbl_satusOfTest.TabIndex = 23;
            this.lbl_satusOfTest.Text = "Status";
            // 
            // lbl_testType
            // 
            this.lbl_testType.AutoSize = true;
            this.lbl_testType.Location = new System.Drawing.Point(33, 777);
            this.lbl_testType.Name = "lbl_testType";
            this.lbl_testType.Size = new System.Drawing.Size(84, 25);
            this.lbl_testType.TabIndex = 24;
            this.lbl_testType.Text = "Test Type";
            // 
            // lbl_progress
            // 
            this.lbl_progress.AutoSize = true;
            this.lbl_progress.Location = new System.Drawing.Point(24, 404);
            this.lbl_progress.Name = "lbl_progress";
            this.lbl_progress.Size = new System.Drawing.Size(81, 25);
            this.lbl_progress.TabIndex = 25;
            this.lbl_progress.Text = "Progress";
            // 
            // lbl_serverIp
            // 
            this.lbl_serverIp.AutoSize = true;
            this.lbl_serverIp.Location = new System.Drawing.Point(153, 83);
            this.lbl_serverIp.Name = "lbl_serverIp";
            this.lbl_serverIp.Size = new System.Drawing.Size(81, 25);
            this.lbl_serverIp.TabIndex = 26;
            this.lbl_serverIp.Text = "Server IP";
            // 
            // cBox_programName
            // 
            this.cBox_programName.FormattingEnabled = true;
            this.cBox_programName.Location = new System.Drawing.Point(197, 550);
            this.cBox_programName.Name = "cBox_programName";
            this.cBox_programName.Size = new System.Drawing.Size(182, 33);
            this.cBox_programName.TabIndex = 27;
            // 
            // cBox_pcGroup
            // 
            this.cBox_pcGroup.FormattingEnabled = true;
            this.cBox_pcGroup.Location = new System.Drawing.Point(197, 512);
            this.cBox_pcGroup.Name = "cBox_pcGroup";
            this.cBox_pcGroup.Size = new System.Drawing.Size(182, 33);
            this.cBox_pcGroup.TabIndex = 28;
            // 
            // cBox_slotNumber
            // 
            this.cBox_slotNumber.FormattingEnabled = true;
            this.cBox_slotNumber.Location = new System.Drawing.Point(197, 629);
            this.cBox_slotNumber.Name = "cBox_slotNumber";
            this.cBox_slotNumber.Size = new System.Drawing.Size(182, 33);
            this.cBox_slotNumber.TabIndex = 29;
            // 
            // cBox_testDuration
            // 
            this.cBox_testDuration.FormattingEnabled = true;
            this.cBox_testDuration.Location = new System.Drawing.Point(211, 308);
            this.cBox_testDuration.Name = "cBox_testDuration";
            this.cBox_testDuration.Size = new System.Drawing.Size(182, 33);
            this.cBox_testDuration.TabIndex = 30;
            // 
            // lbl_testDuration
            // 
            this.lbl_testDuration.AutoSize = true;
            this.lbl_testDuration.Location = new System.Drawing.Point(24, 311);
            this.lbl_testDuration.Name = "lbl_testDuration";
            this.lbl_testDuration.Size = new System.Drawing.Size(181, 25);
            this.lbl_testDuration.TabIndex = 31;
            this.lbl_testDuration.Text = "Desired Test Duration";
            // 
            // lbl_pcGroup
            // 
            this.lbl_pcGroup.AutoSize = true;
            this.lbl_pcGroup.Location = new System.Drawing.Point(104, 517);
            this.lbl_pcGroup.Name = "lbl_pcGroup";
            this.lbl_pcGroup.Size = new System.Drawing.Size(87, 25);
            this.lbl_pcGroup.TabIndex = 32;
            this.lbl_pcGroup.Text = "PC group";
            // 
            // lbl_ECD
            // 
            this.lbl_ECD.AutoSize = true;
            this.lbl_ECD.Location = new System.Drawing.Point(1166, 849);
            this.lbl_ECD.Name = "lbl_ECD";
            this.lbl_ECD.Size = new System.Drawing.Size(188, 25);
            this.lbl_ECD.TabIndex = 33;
            this.lbl_ECD.Text = "Estimated Completion";
            // 
            // text_ECD
            // 
            this.text_ECD.Location = new System.Drawing.Point(1353, 849);
            this.text_ECD.Name = "text_ECD";
            this.text_ECD.Size = new System.Drawing.Size(269, 31);
            this.text_ECD.TabIndex = 34;
            // 
            // text_startDate
            // 
            this.text_startDate.Location = new System.Drawing.Point(1353, 800);
            this.text_startDate.Name = "text_startDate";
            this.text_startDate.Size = new System.Drawing.Size(269, 31);
            this.text_startDate.TabIndex = 36;
            // 
            // lbl_startDate
            // 
            this.lbl_startDate.AutoSize = true;
            this.lbl_startDate.Location = new System.Drawing.Point(1257, 800);
            this.lbl_startDate.Name = "lbl_startDate";
            this.lbl_startDate.Size = new System.Drawing.Size(90, 25);
            this.lbl_startDate.TabIndex = 35;
            this.lbl_startDate.Text = "Start Date";
            // 
            // text_totalFailures
            // 
            this.text_totalFailures.Location = new System.Drawing.Point(218, 848);
            this.text_totalFailures.Name = "text_totalFailures";
            this.text_totalFailures.Size = new System.Drawing.Size(269, 31);
            this.text_totalFailures.TabIndex = 38;
            // 
            // lbl_totalFailures
            // 
            this.lbl_totalFailures.AutoSize = true;
            this.lbl_totalFailures.Location = new System.Drawing.Point(24, 851);
            this.lbl_totalFailures.Name = "lbl_totalFailures";
            this.lbl_totalFailures.Size = new System.Drawing.Size(112, 25);
            this.lbl_totalFailures.TabIndex = 37;
            this.lbl_totalFailures.Text = "Total Failures";
            // 
            // text_completedDays
            // 
            this.text_completedDays.Location = new System.Drawing.Point(1353, 897);
            this.text_completedDays.Name = "text_completedDays";
            this.text_completedDays.Size = new System.Drawing.Size(114, 31);
            this.text_completedDays.TabIndex = 40;
            // 
            // lbl_completedDays
            // 
            this.lbl_completedDays.AutoSize = true;
            this.lbl_completedDays.Location = new System.Drawing.Point(1203, 897);
            this.lbl_completedDays.Name = "lbl_completedDays";
            this.lbl_completedDays.Size = new System.Drawing.Size(144, 25);
            this.lbl_completedDays.TabIndex = 39;
            this.lbl_completedDays.Text = "Completed Days";
            // 
            // text_remainingDays
            // 
            this.text_remainingDays.Location = new System.Drawing.Point(1353, 950);
            this.text_remainingDays.Name = "text_remainingDays";
            this.text_remainingDays.Size = new System.Drawing.Size(114, 31);
            this.text_remainingDays.TabIndex = 42;
            // 
            // lbl_remainingDays
            // 
            this.lbl_remainingDays.AutoSize = true;
            this.lbl_remainingDays.Location = new System.Drawing.Point(1208, 953);
            this.lbl_remainingDays.Name = "lbl_remainingDays";
            this.lbl_remainingDays.Size = new System.Drawing.Size(139, 25);
            this.lbl_remainingDays.TabIndex = 41;
            this.lbl_remainingDays.Text = "Remaining Days";
            // 
            // rbtn_VC
            // 
            this.rbtn_VC.AutoSize = true;
            this.rbtn_VC.Checked = true;
            this.rbtn_VC.Location = new System.Drawing.Point(224, 202);
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
            this.rbtn_BT.Location = new System.Drawing.Point(224, 237);
            this.rbtn_BT.Name = "rbtn_BT";
            this.rbtn_BT.Size = new System.Drawing.Size(110, 29);
            this.rbtn_BT.TabIndex = 44;
            this.rbtn_BT.Text = "Boot Test";
            this.rbtn_BT.UseVisualStyleBackColor = true;
            // 
            // rbtn_PT
            // 
            this.rbtn_PT.AutoSize = true;
            this.rbtn_PT.Location = new System.Drawing.Point(224, 272);
            this.rbtn_PT.Name = "rbtn_PT";
            this.rbtn_PT.Size = new System.Drawing.Size(120, 29);
            this.rbtn_PT.TabIndex = 45;
            this.rbtn_PT.Text = "Power Test";
            this.rbtn_PT.UseVisualStyleBackColor = true;
            // 
            // lbl_selectTest
            // 
            this.lbl_selectTest.AutoSize = true;
            this.lbl_selectTest.Location = new System.Drawing.Point(224, 174);
            this.lbl_selectTest.Name = "lbl_selectTest";
            this.lbl_selectTest.Size = new System.Drawing.Size(93, 25);
            this.lbl_selectTest.TabIndex = 46;
            this.lbl_selectTest.Text = "Select Test";
            // 
            // text_currentTest
            // 
            this.text_currentTest.Location = new System.Drawing.Point(218, 811);
            this.text_currentTest.Name = "text_currentTest";
            this.text_currentTest.Size = new System.Drawing.Size(269, 31);
            this.text_currentTest.TabIndex = 48;
            // 
            // lbl_currentTest
            // 
            this.lbl_currentTest.AutoSize = true;
            this.lbl_currentTest.Location = new System.Drawing.Point(24, 814);
            this.lbl_currentTest.Name = "lbl_currentTest";
            this.lbl_currentTest.Size = new System.Drawing.Size(105, 25);
            this.lbl_currentTest.TabIndex = 47;
            this.lbl_currentTest.Text = "Current Test";
            // 
            // rText_statusOfTest
            // 
            this.rText_statusOfTest.Location = new System.Drawing.Point(1173, 1073);
            this.rText_statusOfTest.Name = "rText_statusOfTest";
            this.rText_statusOfTest.Size = new System.Drawing.Size(449, 217);
            this.rText_statusOfTest.TabIndex = 49;
            this.rText_statusOfTest.Text = "";
            // 
            // lbl_richTextStatusOfTests
            // 
            this.lbl_richTextStatusOfTests.AutoSize = true;
            this.lbl_richTextStatusOfTests.Location = new System.Drawing.Point(1173, 1038);
            this.lbl_richTextStatusOfTests.Name = "lbl_richTextStatusOfTests";
            this.lbl_richTextStatusOfTests.Size = new System.Drawing.Size(182, 25);
            this.lbl_richTextStatusOfTests.TabIndex = 50;
            this.lbl_richTextStatusOfTests.Text = "Status of running test";
            // 
            // cBox_serverIp
            // 
            this.cBox_serverIp.FormattingEnabled = true;
            this.cBox_serverIp.Location = new System.Drawing.Point(237, 80);
            this.cBox_serverIp.Name = "cBox_serverIp";
            this.cBox_serverIp.Size = new System.Drawing.Size(182, 33);
            this.cBox_serverIp.TabIndex = 51;
            // 
            // lbl_completedCycles
            // 
            this.lbl_completedCycles.AutoSize = true;
            this.lbl_completedCycles.Location = new System.Drawing.Point(33, 478);
            this.lbl_completedCycles.Name = "lbl_completedCycles";
            this.lbl_completedCycles.Size = new System.Drawing.Size(154, 25);
            this.lbl_completedCycles.TabIndex = 53;
            this.lbl_completedCycles.Text = "Completed Cycles";
            // 
            // text_completedCycles
            // 
            this.text_completedCycles.Location = new System.Drawing.Point(197, 475);
            this.text_completedCycles.Name = "text_completedCycles";
            this.text_completedCycles.Size = new System.Drawing.Size(269, 31);
            this.text_completedCycles.TabIndex = 52;
            this.text_completedCycles.Text = "0";
            // 
            // button_startTest
            // 
            this.button_startTest.Location = new System.Drawing.Point(32, 182);
            this.button_startTest.Name = "button_startTest";
            this.button_startTest.Size = new System.Drawing.Size(112, 34);
            this.button_startTest.TabIndex = 54;
            this.button_startTest.Text = "Start Test";
            this.button_startTest.UseVisualStyleBackColor = true;
            // 
            // lbl_testNotes
            // 
            this.lbl_testNotes.AutoSize = true;
            this.lbl_testNotes.Location = new System.Drawing.Point(702, 1063);
            this.lbl_testNotes.Name = "lbl_testNotes";
            this.lbl_testNotes.Size = new System.Drawing.Size(326, 25);
            this.lbl_testNotes.TabIndex = 56;
            this.lbl_testNotes.Text = "Test Notes: Enter notes here for logging";
            // 
            // rText_testNotes
            // 
            this.rText_testNotes.Location = new System.Drawing.Point(702, 1091);
            this.rText_testNotes.Name = "rText_testNotes";
            this.rText_testNotes.Size = new System.Drawing.Size(449, 113);
            this.rText_testNotes.TabIndex = 55;
            this.rText_testNotes.Text = "";
            // 
            // checkBox_forceFailureOnNextCycle
            // 
            this.checkBox_forceFailureOnNextCycle.AutoSize = true;
            this.checkBox_forceFailureOnNextCycle.Location = new System.Drawing.Point(408, 174);
            this.checkBox_forceFailureOnNextCycle.Name = "checkBox_forceFailureOnNextCycle";
            this.checkBox_forceFailureOnNextCycle.Size = new System.Drawing.Size(241, 29);
            this.checkBox_forceFailureOnNextCycle.TabIndex = 57;
            this.checkBox_forceFailureOnNextCycle.Text = "Force failure on next cycle";
            this.checkBox_forceFailureOnNextCycle.UseVisualStyleBackColor = true;
            // 
            // text_dateCompleted
            // 
            this.text_dateCompleted.Location = new System.Drawing.Point(1353, 1004);
            this.text_dateCompleted.Name = "text_dateCompleted";
            this.text_dateCompleted.Size = new System.Drawing.Size(269, 31);
            this.text_dateCompleted.TabIndex = 59;
            // 
            // lbl_dateCompleted
            // 
            this.lbl_dateCompleted.AutoSize = true;
            this.lbl_dateCompleted.Location = new System.Drawing.Point(1166, 1004);
            this.lbl_dateCompleted.Name = "lbl_dateCompleted";
            this.lbl_dateCompleted.Size = new System.Drawing.Size(142, 25);
            this.lbl_dateCompleted.TabIndex = 58;
            this.lbl_dateCompleted.Text = "Date Completed";
            // 
            // text_remainingHours
            // 
            this.text_remainingHours.Location = new System.Drawing.Point(1549, 953);
            this.text_remainingHours.Name = "text_remainingHours";
            this.text_remainingHours.Size = new System.Drawing.Size(90, 31);
            this.text_remainingHours.TabIndex = 61;
            // 
            // text_completedHours
            // 
            this.text_completedHours.Location = new System.Drawing.Point(1544, 900);
            this.text_completedHours.Name = "text_completedHours";
            this.text_completedHours.Size = new System.Drawing.Size(90, 31);
            this.text_completedHours.TabIndex = 60;
            // 
            // lbl_remainingHours
            // 
            this.lbl_remainingHours.AutoSize = true;
            this.lbl_remainingHours.Location = new System.Drawing.Point(1483, 956);
            this.lbl_remainingHours.Name = "lbl_remainingHours";
            this.lbl_remainingHours.Size = new System.Drawing.Size(60, 25);
            this.lbl_remainingHours.TabIndex = 63;
            this.lbl_remainingHours.Text = "Hours";
            // 
            // lbl_completedHours
            // 
            this.lbl_completedHours.AutoSize = true;
            this.lbl_completedHours.Location = new System.Drawing.Point(1478, 900);
            this.lbl_completedHours.Name = "lbl_completedHours";
            this.lbl_completedHours.Size = new System.Drawing.Size(60, 25);
            this.lbl_completedHours.TabIndex = 62;
            this.lbl_completedHours.Text = "Hours";
            // 
            // text_hoursPer1kCycles
            // 
            this.text_hoursPer1kCycles.Location = new System.Drawing.Point(1420, 666);
            this.text_hoursPer1kCycles.Name = "text_hoursPer1kCycles";
            this.text_hoursPer1kCycles.Size = new System.Drawing.Size(114, 31);
            this.text_hoursPer1kCycles.TabIndex = 65;
            // 
            // lbl_hoursPer1kCycles
            // 
            this.lbl_hoursPer1kCycles.AutoSize = true;
            this.lbl_hoursPer1kCycles.Location = new System.Drawing.Point(1248, 666);
            this.lbl_hoursPer1kCycles.Name = "lbl_hoursPer1kCycles";
            this.lbl_hoursPer1kCycles.Size = new System.Drawing.Size(166, 25);
            this.lbl_hoursPer1kCycles.TabIndex = 64;
            this.lbl_hoursPer1kCycles.Text = "Hours per 1k cycles";
            // 
            // Client_GUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1784, 1316);
            this.Controls.Add(this.text_hoursPer1kCycles);
            this.Controls.Add(this.lbl_hoursPer1kCycles);
            this.Controls.Add(this.lbl_remainingHours);
            this.Controls.Add(this.lbl_completedHours);
            this.Controls.Add(this.text_remainingHours);
            this.Controls.Add(this.text_completedHours);
            this.Controls.Add(this.text_dateCompleted);
            this.Controls.Add(this.lbl_dateCompleted);
            this.Controls.Add(this.checkBox_forceFailureOnNextCycle);
            this.Controls.Add(this.lbl_testNotes);
            this.Controls.Add(this.rText_testNotes);
            this.Controls.Add(this.button_startTest);
            this.Controls.Add(this.lbl_completedCycles);
            this.Controls.Add(this.text_completedCycles);
            this.Controls.Add(this.cBox_serverIp);
            this.Controls.Add(this.lbl_richTextStatusOfTests);
            this.Controls.Add(this.rText_statusOfTest);
            this.Controls.Add(this.text_currentTest);
            this.Controls.Add(this.lbl_currentTest);
            this.Controls.Add(this.lbl_selectTest);
            this.Controls.Add(this.rbtn_PT);
            this.Controls.Add(this.rbtn_BT);
            this.Controls.Add(this.rbtn_VC);
            this.Controls.Add(this.text_remainingDays);
            this.Controls.Add(this.lbl_remainingDays);
            this.Controls.Add(this.text_completedDays);
            this.Controls.Add(this.lbl_completedDays);
            this.Controls.Add(this.text_totalFailures);
            this.Controls.Add(this.lbl_totalFailures);
            this.Controls.Add(this.text_startDate);
            this.Controls.Add(this.lbl_startDate);
            this.Controls.Add(this.text_ECD);
            this.Controls.Add(this.lbl_ECD);
            this.Controls.Add(this.lbl_pcGroup);
            this.Controls.Add(this.lbl_testDuration);
            this.Controls.Add(this.cBox_testDuration);
            this.Controls.Add(this.cBox_slotNumber);
            this.Controls.Add(this.cBox_pcGroup);
            this.Controls.Add(this.cBox_programName);
            this.Controls.Add(this.lbl_serverIp);
            this.Controls.Add(this.lbl_progress);
            this.Controls.Add(this.lbl_testType);
            this.Controls.Add(this.lbl_satusOfTest);
            this.Controls.Add(this.lbl_deviceType);
            this.Controls.Add(this.lbl_testGroupIdentifier);
            this.Controls.Add(this.lbl_testAppVersion);
            this.Controls.Add(this.lbl_slotNumber);
            this.Controls.Add(this.lbl_computerName);
            this.Controls.Add(this.lbl_programName);
            this.Controls.Add(this.text_testType);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.text_progress);
            this.Controls.Add(this.text_statusOfTest);
            this.Controls.Add(this.text_deviceType);
            this.Controls.Add(this.text_testGroupIdentifier);
            this.Controls.Add(this.textBox_testAppVersion);
            this.Controls.Add(this.text_computerName);
            this.Controls.Add(this.lbl_recentReceivedMessages);
            this.Controls.Add(this.lbl_lastMessageReceived);
            this.Controls.Add(this.lbl_clientId);
            this.Controls.Add(this.textBox_clientID);
            this.Controls.Add(this.text_lastMessageReceived);
            this.Controls.Add(this.lBox_recentReceivedMessages);
            this.Controls.Add(this.text_sendBox);
            this.Controls.Add(this.button_send);
            this.Controls.Add(this.button_socketConnect);
            this.Name = "Client_GUI";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
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
        public System.Windows.Forms.TextBox text_computerName;
        public System.Windows.Forms.TextBox textBox_testAppVersion;
        public System.Windows.Forms.TextBox text_testGroupIdentifier;
        public System.Windows.Forms.TextBox text_deviceType;
        public System.Windows.Forms.TextBox text_statusOfTest;
        public System.Windows.Forms.TextBox text_progress;
        public System.Windows.Forms.ProgressBar progressBar1;
        public System.Windows.Forms.TextBox text_testType;
        public System.Windows.Forms.Label lbl_programName;
        public System.Windows.Forms.Label lbl_computerName;
        public System.Windows.Forms.Label lbl_slotNumber;
        public System.Windows.Forms.Label lbl_testAppVersion;
        public System.Windows.Forms.Label lbl_testGroupIdentifier;
        public System.Windows.Forms.Label lbl_deviceType;
        public System.Windows.Forms.Label lbl_satusOfTest;
        public System.Windows.Forms.Label lbl_testType;
        public System.Windows.Forms.Label lbl_progress;
        public System.Windows.Forms.Label lbl_serverIp;
        public ComboBox cBox_programName;
        public ComboBox cBox_pcGroup;
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
        public TextBox text_completedDays;
        public Label lbl_completedDays;
        public TextBox text_remainingDays;
        public Label lbl_remainingDays;
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
        public TextBox text_remainingHours;
        public TextBox text_completedHours;
        public Label lbl_remainingHours;
        public Label lbl_completedHours;
        public TextBox text_hoursPer1kCycles;
        public Label lbl_hoursPer1kCycles;
    }
}

