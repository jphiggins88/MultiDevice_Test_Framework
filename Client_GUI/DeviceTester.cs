using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Client_GUI
{
    public class DeviceTester
    {
        public Client_GUI clientForm;
        protected DataLog dataLog = new DataLog();
        protected TextBox ecdEndCycle;
        //protected TextBox testCyclesBox;
        static StreamReader addressReader;

        public CustomEventArgs4 initialTestUpdateInfo;
        public CustomEventArgs5_StatusOnly testUpdateInfo;

        // addresss of the files that contains the lists of things to be tested for.
        public static string rootDirectory = Client_GUI.rootDirectory;
        protected static string IndividualFilePath = rootDirectory + "\\COM.log";
        protected static string MasterLogPath = rootDirectory + "\\Main Log.log";
        public static string logFileName = "";
        public static string testFiles = Client_GUI.testFiles;
        public static string txtFileWithLocalSharedFolderAddress = testFiles + @"\Addresses\addressOfLocalSharedFolder.txt";
        public static string addressOfLocalSharedFolder = CheckIfFileExists_LocalSharedFolder();
        protected static string globalTestInfoLogPath = rootDirectory + "\\" + "unknownSN" + "\\TestInfo.log";
        public string VerboseLogPath;
        public string copyToPath = "copyToPathNotDefined";

        protected DateTime EcdIntervalTime;
        protected DateTime HoursPer1000CyclesTime;
        protected DateTime StartTime;
        protected DateTime CurrentDay = DateTime.Today;
        protected DateTime DayOfLastCycle = DateTime.Today;
        protected const int HOURS_PER_DAY = 24;

        protected const string LINE_SEP = "---------------------------------------------";

        protected const string CYCLE_COUNT_SEPARATOR = LINE_SEP + LINE_SEP + LINE_SEP + "\r\n";

        protected const int ECD_INTERVAL = 100;
        protected const int HOURS_PER_1000_CYCLES_INTERVAL = 50;
        protected const int DEFAULT_PROGRESSBAR_MAX = 100;

        protected int targetCycleCount = 0;
        protected Boolean isFirstCycle = true;

        protected int startCycle = 0;
        protected int endCycle;

        private string status;

        protected string Error = "";
        protected const string SUCCESS = "SUCCESS";
        protected const string FAIL = "FAIL";

        protected string VOLTAGE_CHECK = "Voltage Check Test";
        protected string BOOT_TEST = "Boot Test";
        protected string POWER_TEST = "Power Test";

        protected bool genericInfoHasBeenLogged = false;
        public bool FileTransferCommandHasBeenSent = false;
        public bool isInitialTestInfoSentToServer = false;

        static Random randomGenerator = new Random();

        // Test device info
        protected string TsCodeRev;
        protected string driveType; // TODO implement this
        protected string testType;
        protected string serialNumber = "Not Found Yet";
        public string modelNumber = "undefined"; // TODO implement
        public string programName = "undefined";

        protected int totalCycles = 0;


        // Test Info for Socket Messages
        public string testTypeAbbreviation = "not set";
        public string slotNum = "0";
        public int comPort = 0;
        public string compNumber = "empty";
        public string pcGroupNumber = "PC group not selected"; //TODO implement this
        public double global_PercentComplete = 0.0;
        public string descriptionOfCurrentState = "Nothing to Report";
        public string statusOfTest = "No Status defined";
        public static string testGroupName = "undefined";
        public string globalErrorToSendToSocket = "undefined";




        public DeviceTester(Client_GUI form)
        {
            this.clientForm = form;
            this.clientForm.stopProcess = false;
            FileTransferCommandHasBeenSent = false;
            this.ecdEndCycle = this.clientForm.text_completedCycles;
            this.StartTime = DateTime.Now;
            this.EcdIntervalTime = DateTime.Now;
            this.testType = "undefined";
            this.TsCodeRev = this.clientForm.lbl_testAppVersionNum.Text;

            // Set test type according to radio buttons in the GUI
            if (this.clientForm.rbtn_VC.Checked)
            {
                this.testType = VOLTAGE_CHECK;
            }
            else if (this.clientForm.rbtn_BT.Checked)
            {
                this.testType = BOOT_TEST;
            }
            else if (this.clientForm.rbtn_PT.Checked)
            {
                this.testType = POWER_TEST;
            }

            // Change color of the GUI according to test type
            this.clientForm.SetTestBox(this.testType);
        }


        protected void RunTest()
        {
            // Clear the status box
            this.clientForm.UpdateStatus("");

            this.clientForm.UpdateStatus("Test Running...\n");
            totalCycles = Convert.ToInt32(this.clientForm.GetText(this.clientForm.text_completedCycles));

            status = "SUCCESS";

            // TODO resolve how I want to simulate com ports and slots
            // The slot number is retrieved from the GUI
            //comPort = GetComPortNumber();
            slotNum = this.clientForm.GetComboText(this.clientForm.cBox_slotNumber);

            do
            {
                CheckIfInitialTestInfoNeedsToBeSent();

                SendTestInfoLogCopyCommandToServerIfNotAlreadySent(globalTestInfoLogPath, true);

                //VerboseLog("\r\n" + "Socket Tester: " + CYCLE_COUNT_SEPARATOR + "Socket Tester: " + DateTime.Now.ToString() + " Begin Test Cycle: " + totalTestCycles.ToString());
                VerboseLog("\r\n" + "Socket Tester: " + CYCLE_COUNT_SEPARATOR + "Socket Tester: " + DateTime.Now.ToString() + " Begin Test Cycle: " + totalCycles.ToString());


                status = RunFakeTests();

                // Induce a manual failure by checking box in the GUI
                if (this.clientForm.checkBox_forceFailureOnNextCycle.Checked)
                {
                    status = "Manual Failure";
                    reportErrors(status);
                }


                // Returning here will prevent all the below code including Check status and Update Server. Is this handled in mainloop
                if (status != SUCCESS)
                {
                    return;
                }


                VerboseLog("\r\nSocket Tester: Cycle Complete");

                totalCycles = Convert.ToInt32(this.clientForm.text_completedCycles.Text);
                totalCycles++;
                this.clientForm.SetText(this.clientForm.text_completedCycles, totalCycles.ToString());

                VerboseLog("\r\nSocket Tester: Checking Status\r\nSocket Tester: Waiting for device to be ready");

                CalculateEcdCycles();

                CheckTestStatusAndUpdateServer();

                // Clear the status box
                this.clientForm.UpdateStatus("");

                this.isFirstCycle = false;
                Thread.Sleep(1000);

            } while ((this.clientForm.stopProcess == false) && (status == SUCCESS));

        }

        private string RunFakeTests()
        {
            status = FakeTestSioCheck();
            if (status != SUCCESS)
            {
                reportErrors(status);
                if (this.clientForm.checkBox_stopOnFailure.Checked)
                {
                    return status;
                }
            }
            status = FakeTestVoltagCheck();
            if (status != SUCCESS)
            {
                reportErrors(status);
                if (this.clientForm.checkBox_stopOnFailure.Checked)
                {
                    return status;
                }
            }
            status = FakeTestDeviceCheck();
            if (status != SUCCESS)
            {
                reportErrors(status);
                if (this.clientForm.checkBox_stopOnFailure.Checked)
                {
                    return status;
                }
            }

            // status should equal SUCCESS if this code is reached
            return status;
        }

        private string FakeTestSioCheck()
        {
            // Random Num should yield a 1 in 100 chance of test failing
            int randNum = randomGenerator.Next(1, 101);

            this.clientForm.UpdateStatus("Simulating SIO communication test\n");
            VerboseLog("simulating SIO communication");
            Thread.Sleep(1000);
            if (randNum == 1)
            {
                return status = "Fail during SIO Check";
            }

            VerboseLog("SIO check: good\r\n");
            this.clientForm.UpdateStatus("Result: GOOD\n");

            return status = SUCCESS;
        }

        private string FakeTestVoltagCheck()
        {
            // Random Num should yield a 1 in 100 chance of test failing
            int randNum = randomGenerator.Next(1, 101);

            this.clientForm.UpdateStatus("Simulating voltage within bounds check\n");
            VerboseLog("simulated voltage = " + randomGenerator.Next(4000, 6000) + "mV");
            VerboseLog("simulated current = " + randomGenerator.Next(100, 900) + "mA");
            Thread.Sleep(2000);
            if (randNum == 1)
            {
                return status = "Fail during Voltage Check";
            }

            VerboseLog("All components good\r\n");
            this.clientForm.UpdateStatus("Result: GOOD\n");

            return status = SUCCESS;
        }
        
        private string FakeTestDeviceCheck()
        {
            // Random Num should yield a 1 in 100 chance of test failing
            int randNum = randomGenerator.Next(1, 101);

            this.clientForm.UpdateStatus("Simulating device specific functionality\n");
            VerboseLog("device specific function 1");
            Thread.Sleep(1000);
            VerboseLog("device specific fucntion 2");
            Thread.Sleep(2000);
            if (randNum == 1)
            {
                return status = "Fail during Device Check";
            }

            VerboseLog("device is functioning properly\r\n");
            this.clientForm.UpdateStatus("Result: GOOD\n");

            return status = SUCCESS;
        }

        protected int GetComPortNumber()
        {
            return randomGenerator.Next(1, (5 + 1));
        }

        private void reportErrors(string failureType)
        {
            totalCycles = Convert.ToInt32(this.clientForm.text_completedCycles.Text);

            VerboseLog("Socket Tester: ERROR - FAILED CYCLE: " + totalCycles);

            ErrorReport(failureType + " : during test cycle " + totalCycles);
        }

        protected void ErrorReport(string message)
        {
            int failures = Convert.ToInt32(this.clientForm.GetText(this.clientForm.text_totalFailures));
            failures++;
            this.clientForm.SetText(this.clientForm.text_totalFailures, failures.ToString());
            this.clientForm.text_totalFailures.BackColor = Color.LightCoral;

            Error = message;
            string errorLogPath = rootDirectory + "\\Error_Log.log";
            string errorData = DateTime.Now.ToString() + " " + message + "    ";
            string errorSeperator = "\r\n\r\n";

            string cycleData = "Serial Number: " + serialNumber +
                                " Cycles = " + totalCycles.ToString() +
                                " Failures = " + failures.ToString() +
                                " Status = " + this.clientForm.GetRichText(this.clientForm.rText_statusOfTest);

            dataLog.LogData(IndividualFilePath, errorData + cycleData + errorSeperator);

            dataLog.LogData(MasterLogPath, errorData + "Station " +
                                           this.clientForm.GetComboText(this.clientForm.cBox_pcGroup) +
                                           cycleData);
                 
            dataLog.LogData(errorLogPath, errorData + cycleData + errorSeperator);

            if(this.clientForm.checkBox_stopOnFailure.Checked)
            {
                this.clientForm.button_startTest.BackColor = Color.Red;
                this.clientForm.SetStartButton("Start");
            }


            // for UPDATING the Socket Server
            globalErrorToSendToSocket = message;
            statusOfTest = "Failed";
            this.clientForm.stopProcess = true;

        }

        public void GetDeviceInfoFromDevice()
        {
            int randSerialNumber = randomGenerator.Next(100000000, 299999990);
            serialNumber = randSerialNumber.ToString();

            int randProgramSelector = randomGenerator.Next(0, 4);           //TODO make the max value large enough to include all values in the programName file that is parsed on initialization. <that value + 1> to include the last element.
            //this.clientForm.cBox_programName.SelectedIndex = randProgramSelector;   // Change the combo box selection based on the random number...or do I want to just have the device create a completely random name instead of selecting from the GUI combo box options.
            //this.clientForm.SetComboBoxIndex(randProgramSelector);
            //programName = this.clientForm.cBox_programName.Text;
            if (randProgramSelector == 0) { programName = "Titan"; }
            else if (randProgramSelector == 1) { programName = "Europa"; }
            else if (randProgramSelector == 2) { programName = "Callisto"; }
            else if (randProgramSelector == 3) { programName = "Ganymede"; }

            int randModelNumber = randomGenerator.Next(100, 499);
            modelNumber = "PL" + randModelNumber.ToString() + "A";

        }

        public void PopulateGuiFieldsWithDeviceInfo()
        {
            this.clientForm.SetText(this.clientForm.text_serialNumber, serialNumber);
            this.clientForm.SetText(this.clientForm.text_modelNumber, modelNumber);
            this.clientForm.SetCombo(this.clientForm.cBox_programName, programName);
        }

        /// <summary>
        /// Get information about test location from GUI and set Validation.cs global variables to the correct values. 
        /// </summary>
        public void GetTestLocationAndProgramInformation()
        {

            // Create a random slot number from 1 - 6
            //TODO
            //slotNum = comPort;


            if (this.clientForm.GetComboText(this.clientForm.cBox_programName) != null)
            {
                programName = this.clientForm.GetComboText(this.clientForm.cBox_programName);
            }

            if (this.clientForm.GetComboText(this.clientForm.cBox_pcGroup) != null)
            {
                compNumber = this.clientForm.GetComboText(this.clientForm.cBox_pcGroup);

                if ((compNumber == "PC-01") || (compNumber == "PC-02") || (compNumber == "PC-02"))
                {
                    pcGroupNumber = "01";
                }
                else if ((compNumber == "PC-04") || (compNumber == "PC-05") || (compNumber == "PC-06"))
                {
                    pcGroupNumber = "02";
                }

                else if ((compNumber == "PC-07") || (compNumber == "PC-08") || (compNumber == "PC-09"))
                {
                    pcGroupNumber = "03";
                }
                else if ((compNumber == "PC-10") || (compNumber == "PC-11") || (compNumber == "PC-12"))
                {
                    pcGroupNumber = "04";
                }
                else
                {
                    pcGroupNumber = "unknown pc group"; // TODO implement checking this system's name and handle it accordingly
                }

            }
        }

        public static string CheckIfFileExists_LocalSharedFolder()
        {
            try
            {
                if (File.Exists(txtFileWithLocalSharedFolderAddress))
                {
                    addressReader = File.OpenText(txtFileWithLocalSharedFolderAddress);
                    return addressReader.ReadLine();
                }

                return "fail";
            }
            catch
            {
                MessageBox.Show("filepath:" + txtFileWithLocalSharedFolderAddress + " Not Present");
                return "fail";
            }
        }

        public void PopulateTestInfoLogWithDriveInfo()
        {
            string genericInfoLogPath = rootDirectory + "\\" + testGroupName + "\\" + serialNumber + "\\TestInfo.log";

            string copyToPath = addressOfLocalSharedFolder + "\\" + testGroupName + "\\" + serialNumber + "\\TestInfo.log";

            // Set the global address to the destination path of the file copied to the shared folder.
            globalTestInfoLogPath = copyToPath;

            int slotNum = comPort;


            //log: whitelist parameters, blacklist parameters (expected/unexpected)
            string genericLogData = " \r\n#########################################################\r\n" +
                                    "Time: " + DateTime.Now.ToString() + " \r\n" +
                                    "TestApp version: " + Client_GUI.TESTAPP_VERSION + " \r\n" +
                                    "COM ports: " + comPort + " \r\n" +
                                    "PC Group and PC: " + this.clientForm.GetComboText(this.clientForm.cBox_pcGroup) + " \r\n" +
                                    "Slot Number: " + slotNum + "\r\n" +
                                    "---------------------------------------------------------\r\n" +
                                    "ITR: " + this.clientForm.GetText(this.clientForm.text_testGroupIdentifier) + " \r\n" +
                                    "Current Test: " + this.clientForm.text_currentTest.Text + " \r\n" +
                                    "Program Name: " + this.clientForm.GetComboText(this.clientForm.cBox_programName) + " \r\n" +
                                    "Model Number: " + modelNumber + " \r\n" +
                                    "Serial Number: " + serialNumber + " \r\n" +
                                    "Notes: " + this.clientForm.GetRichText(this.clientForm.rText_testNotes) + " \r\n" +
                                    "---------------------------------------------------------\r\n";


            dataLog.LogData(genericInfoLogPath, genericLogData);

            dataLog.LogData(genericInfoLogPath, " \r\n\r\n#########################################################\r\n\r\n\r\n\r\n\r\n");

            // Set true to keep from logging every cycle count. This should only log once every test start.
            genericInfoHasBeenLogged = true;

            TransferLogToLocalSharedFolder(genericInfoLogPath, copyToPath, true, "testInfo");
        }


        /// <summary>
        /// Calculates ECD based on cycle counts
        /// </summary>
        protected void CalculateEcdCycles()
        {
            TimeSpan timeElapsed;
            DateTime ecdDate = DateTime.Now;

            // Get the number of test cycles desired from the GUI
            string testDuration = this.clientForm.GetComboText(this.clientForm.cBox_testDuration);

            if (testDuration == "")
            {
                targetCycleCount = 0;
            }
            else
            {
                targetCycleCount = Convert.ToInt32(testDuration);
            }

            // Get current cycleCount from GUI
            int currentCycleCount = Convert.ToInt32(this.clientForm.GetText(this.clientForm.text_completedCycles));

            int remainingCycles;
            double remainingSeconds;

            // Calculate ECD or set actual completion date if already complete
            if (currentCycleCount <= targetCycleCount)
            {

                // Calculate a new Estimated Completion Date every x cycles, where x is provided by the ECD_INTERVAL value
                if (startCycle == 0)
                {
                    startCycle = currentCycleCount;
                    // Record the start date/time of the interval
                    this.EcdIntervalTime = DateTime.Now;
                    endCycle = startCycle + ECD_INTERVAL;
                }
                // Interval point is reached, calculate ECD
                else if (currentCycleCount == endCycle)
                {
                    // Calculate how long 1 interval took
                    timeElapsed = DateTime.Now - EcdIntervalTime;
                    remainingCycles = targetCycleCount - endCycle;

                    // calculate ECD
                    double timePerCycle = timeElapsed.TotalSeconds / ECD_INTERVAL;
                    remainingSeconds = (remainingCycles * timePerCycle);
                    //remainingDays = ((remainingCycles * timeElapsed.TotalDays) / ECD_INTERVAL);
                    double remainingDays = remainingSeconds / (60 * 60 * 24);
                    ecdDate = ecdDate.AddDays(remainingDays);
                    startCycle = 0;
                    this.clientForm.SetText(this.clientForm.text_ECD, ecdDate.ToString());
                }

            }

            else // CPRT gate reached. Set ACD
            {
                this.clientForm.SetText(this.clientForm.text_dateCompleted, DateTime.Now.ToString());
                this.clientForm.button_startTest.BackColor = Color.LimeGreen;

                this.clientForm.SetProgress(DEFAULT_PROGRESSBAR_MAX);
                this.clientForm.SetText(this.clientForm.text_progress, DEFAULT_PROGRESSBAR_MAX.ToString());
            }

            // Set progress based on CPRT
            if (currentCycleCount > targetCycleCount)
            {
                global_PercentComplete = 100;
                this.clientForm.button_startTest.BackColor = Color.LimeGreen;

                this.clientForm.SetProgress(DEFAULT_PROGRESSBAR_MAX);
                this.clientForm.SetText(this.clientForm.text_progress, DEFAULT_PROGRESSBAR_MAX.ToString());
            }

            else
            {
                global_PercentComplete = ((double)currentCycleCount / (double)targetCycleCount) * DEFAULT_PROGRESSBAR_MAX;

                this.clientForm.SetProgress((int)global_PercentComplete);
                this.clientForm.SetText(this.clientForm.text_progress, global_PercentComplete.ToString());
            }
        }

        /// <summary>
        /// Calculates the time needed for drive to complete 1000 cycles
        /// </summary>
        public void CalcHoursPer1000Cycles()
        {
            TimeSpan timeElapsed;
            int hoursPer1000Cycles;

            // only calculate if interval is reached
            //if (loopCount % HOURS_PER_1000_CYCLES_INTERVAL == 0)
            if (totalCycles % HOURS_PER_1000_CYCLES_INTERVAL == 0)
            {
                timeElapsed = (DateTime.Now - HoursPer1000CyclesTime);
                hoursPer1000Cycles = ((timeElapsed.Minutes * 20) / 60);
                this.clientForm.SetText(this.clientForm.text_hoursPer1kCycles, hoursPer1000Cycles.ToString());

                HoursPer1000CyclesTime = DateTime.Now;
            }
        }

        /// <summary>
        /// Checks string value if each character is numeric
        /// </summary>
        /// <param name="value">string to check for numerical value</param>
        /// <returns>true/false</returns>
        protected bool IsNumber(string value) //TODO implement this if needed
        {
            int i = 0;

            // loop through all characters
            for (i = 0; i < value.Length; i++)
            {
                // test if character is numeric
                if (char.IsNumber(value, i) == false)
                {
                    return false;
                }
            }
            return true;
        }

        protected string DateToFilename(string dateTodayString)
        {
            dateTodayString = dateTodayString.Replace("/", "-");    //replace the slash with a dash so you can use it as a filename
            dateTodayString = dateTodayString.Replace("\\", "-");
            dateTodayString = dateTodayString.Replace(" 12:00:00 AM", "");  //strip the end off the returned Current Day string
            return dateTodayString;
        }

        protected void VerboseLog(string data)
        {
            this.CurrentDay = DateTime.Today;

            VerboseLogPath = rootDirectory + "\\" + testGroupName + "\\" + "SN_Unknown" + "\\Verbose_" + logFileName + ".log";

            if ((serialNumber != null) && (copyToPath == null))
            {
                // We need to initialize the copyToPath to something right away.
                // This global variable will be used for sending error messages to the server.
                copyToPath = addressOfLocalSharedFolder + "\\" + testGroupName + "\\" + serialNumber + "\\Verbose_" + logFileName + ".log";

            }

            // If the drive SN has been read and logging is not being done in the Application Startup Path.
            // Check if the date has changed to determine if the log file name should be changed.
            if ((serialNumber != null) && (this.CurrentDay != this.DayOfLastCycle))
            {
                ChangeLogNameOnDayChange();
            }

            bool debugVar = false;
            if (debugVar)
            {
                ChangeLogNameOnDayChange();
            }

            dataLog.LogData(VerboseLogPath, data);
        }

        /// <summary>
        /// Change the log destination name at midnight.
        /// The new log name will reflect the new date. 
        /// The completed log will be copied to the shared folder.
        /// </summary>
        protected void ChangeLogNameOnDayChange()
        {
            copyToPath = addressOfLocalSharedFolder + "\\" + testGroupName + "\\" + serialNumber + "\\Verbose_" + logFileName + ".log";

            // Transfer  logs to the shared folder.
            // Designate the now-completed log as the log that needs to be transferred to the LAN shared folder.
            // This is done BEFORE the filename change
            TransferLogToLocalSharedFolder(VerboseLogPath, copyToPath, false, "dailyLog");

            // Update new filename to reflect the new date.
            logFileName = DateToFilename(this.CurrentDay.ToString());

            // Change the path to reflect the new filename.
            VerboseLogPath = rootDirectory + "\\" + testGroupName + "\\" + serialNumber + "\\Verbose_" + logFileName + ".log";

            // update the dayOfLastCycle for future comparisons. If It's a new day, make this the new standard to test against.
            this.DayOfLastCycle = this.CurrentDay;
        }

        /// <summary>
        /// Transfers a file from sourcePath to destinationPath with the option to overwrite a file of the same name if it exists at the destinationPath.
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destinationPath"></param>
        /// <param name="sendOverwriteCommand"></param>
        public void TransferLogToLocalSharedFolder(string sourcePath, string destinationPath, bool sendOverwriteCommand, string dailyLogOrTestInfo)
        {
            try
            {
                // If directory tree does not exist, make it.
                Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));

                // Copy the file to the LAN shared folder. This is not the WAN Onedrive folder.
                // Pass in false to prevent overwriting existing files. True will overwrite existing files. "sendOverwriteCommand" is a bool that contains the intention. 
                File.Copy(sourcePath, destinationPath, sendOverwriteCommand);
            }
            catch (IOException iox)
            {
                MessageBox.Show("There was a problem transferring the log file.\r\n\r\n" + iox);
            }


            if (dailyLogOrTestInfo == "testInfo")
            {
                SendTestInfoLogCopyCommandToServerIfNotAlreadySent(destinationPath, sendOverwriteCommand);
            }
            // else daily Log
            else if (dailyLogOrTestInfo == "dailyLog")
            {
                SendDailyLogCopyCommandToServer(destinationPath, sendOverwriteCommand);
            }

        }


        /// <summary>
        /// Sends a command to the server to copy the testInfo.log log to the Onedrive folder.
        /// If the client GUI is connected to the server AND the FileTransferCommand has NOT been sent to the Server yet,
        /// <br/>then the File Transfer command will be sent to the server and the sent flag will be marked as true.
        /// </summary>
        /// <param name="destPath"></param>
        /// <param name="sndOverwriteCmd"></param>
        public void SendTestInfoLogCopyCommandToServerIfNotAlreadySent(string destPath, bool sndOverwriteCmd)
        {
            if (clientForm.mAsyncClient.isConnectedToServer == true && FileTransferCommandHasBeenSent == false)
            {
                // Send a message to the server telling it to copy the file that was just copied to the shared folder on the LAN and copy it to the shared OneDrive folder.
                // Note the destinationPath becomes the sourcePath for the 2nd file transfer that takes place between the Server and the shared OneDrive folder.
                // The "destinationPath" argument being passed in becomes the "sourcePath" listed in the function's definition.
                clientForm.mAsyncClient.SendFileTransferCommandToServer(destPath, sndOverwriteCmd);
                FileTransferCommandHasBeenSent = true;
            }
        }

        /// <summary>
        /// Sends a command to the server to copy the previous days log to the Onedrive folder.
        /// If the client GUI is connected to the server AND the FileTransferCommand has NOT been sent to the Server yet,
        /// <br/>then the File Transfer command will be sent to the server and the sent flag will be marked as true.
        /// </summary>
        /// <param name="destPath"></param>
        /// <param name="sndOverwriteCmd"></param>
        public void SendDailyLogCopyCommandToServer(string destPath, bool sndOverwriteCmd)
        {
            if (clientForm.mAsyncClient.isConnectedToServer == true)
            {
                // Send a message to the server telling it to copy the file that was just copied to the shared folder on the LAN and copy it to the shared OneDrive folder.
                // Note the destinationPath becomes the sourcePath for the 2nd file transfer that takes place between the Server and the shared OneDrive folder.
                // The "destinationPath" argument being passed in becomes the "sourcePath" listed in the function's definition.
                clientForm.mAsyncClient.SendFileTransferCommandToServer(destPath, sndOverwriteCmd);
            }
        }

        public void TransferErrorLogToSharedFolder(string sourcePath, string destinationPath, bool sendOverwriteCommand)
        {
            try
            {
                // If directory tree does not exist, make it.
                Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));

                // Copy the file to the LAN shared folder (not the WAN Onedrive folder).
                // Pass in false to prevent overwriting existing files. True will overwrite existing files. "sendOverwriteCommand" is a bool that contains the intention. 
                File.Copy(sourcePath, destinationPath, sendOverwriteCommand);
            }
            catch (IOException iox)
            {
                MessageBox.Show("There was a problem transferring the error log file.\r\n\r\n" + iox);
            }

        }

        /// <summary>
        /// Send a FileTransfer command to the Server telling it to copy the error log from the LAN Shared folder to the WAN Onedrive folder.
        /// </summary>
        /// <param name="destPath"></param>
        /// <param name="sndOverwriteCmd"></param>
        public void SendErrorLogCopyCommandToServerIfNotAlreadySent(string destPath, bool sndOverwriteCmd)
        {
            if (clientForm.mAsyncClient.isConnectedToServer == true)
            {
                // Note the destinationPath becomes the sourcePath for the 2nd file transfer that takes place between the Server and the shared OneDrive folder.
                // The "destinationPath" argument being passed in becomes the "sourcePath" listed in the function's definition.
                clientForm.mAsyncClient.SendFileTransferCommandToServer(destPath, sndOverwriteCmd);
            }
        }

        /// <summary>
        /// Sends the initial test info if it has not been sent yet.
        /// <br/> Sends program name, serial number, pcGroup, PC, slot, testApp version, current test, status.
        /// <br/> Example use: Send test/drive specific info if a test is connected to the server after it has been running for a while.
        /// </summary>
        public void CheckIfInitialTestInfoNeedsToBeSent()
        {
            // If the client is connected to the server AND the initial test info has NOT been sent, we need to send it.
            // This may happen if the client is connected to the sever after the test has been running for a while and already passed the Testdrive()->CommandSequence() call.
            if ((clientForm.cBox_serverIp.BackColor == Color.Green) && (isInitialTestInfoSentToServer == false))
            {
                SendInitialTestInfo();
            }
        }

        public void CheckTestStatusAndUpdateServer()
        {
            //if (statusOfTest == "Failed" && copyToPath != null)
            if (statusOfTest == "Failed")
            {
                SendErrorToServer(globalErrorToSendToSocket);
            }
            else
            {
                SetStatusOfTest();

                // If no error, don't pass in a path to the error log.
                SendTestUpdateToSocketServer("");
            }
        }

        /// <summary>
        /// If statusOfTest variable is not marked as "Failed", then update it accordingly to:
        /// <br/>Stopped, Complete, Running
        /// </summary>
        public void SetStatusOfTest()
        {
            if (statusOfTest != "Failed")
            {
                // Check to see if the user has manually stopped the test by pressing the stop button.
                // If the stop button has not been pressed, test to see if the test is complete or still running.
                if (this.clientForm.stopButtonPressed == true)
                {
                    statusOfTest = "Stopped";
                }
                else
                {
                    if (global_PercentComplete >= 100)
                    {
                        statusOfTest = "Complete";
                    }
                    else
                    {
                        statusOfTest = "Running";
                    }
                }

            }

        }

        private void GetCycleInformation()
        {
            // get Cycle information
            if (this.clientForm.GetText(this.clientForm.text_completedCycles) != null)
            {
                //loopCount = Convert.ToInt32(this.clientForm.GetText(this.clientForm.text_completedCycles));
                totalCycles = Convert.ToInt32(this.clientForm.GetText(this.clientForm.text_completedCycles));
            }
            else
            {
                MessageBox.Show("Cycle count text box must not be empty. Add a value and click OK");
            }

            //totalTestCycles = Convert.ToInt32(this.clientForm.GetText(this.testCyclesBox));
            //totalCycles = Convert.ToInt32(this.clientForm.text_completedCycles);
        }

        /// <summary>
        /// Update Socket Server and send Test info:
        ///     program name, serial number, pcGroupNumber, PC, slot, testAppVersion, test type, status, cycle count, percent, test type abreviation.
        /// Set a flag to indicat that initial test info has been sent to Server.
        /// </summary>
        public void SendInitialTestInfo()
        {
            // If the socket client is connected to the Server (GUI button Green), send the appropriate information to the Server
            if (clientForm.cBox_serverIp.BackColor == Color.Green)
            {
                // UPDATE Socket Server - Send generic info
                initialTestUpdateInfo = new CustomEventArgs4(
                                                        programName,
                                                        serialNumber,
                                                        pcGroupNumber,
                                                        compNumber,
                                                        slotNum,
                                                        "Version" + Client_GUI.TESTAPP_VERSION,
                                                        this.clientForm.text_currentTest.Text,
                                                        "Starting",
                                                        "??",
                                                        "??",
                                                        testTypeAbbreviation);

                // Send the drive/test information to the Server;
                clientForm.mAsyncClient.UpdateInitialSocketInfo(initialTestUpdateInfo);

                // change the flag to sent.
                isInitialTestInfoSentToServer = true;
            }

        }

        public void GetTestTypeAbbreviation(string typeOfTest)
        {
            if (typeOfTest == VOLTAGE_CHECK)
            {
                testTypeAbbreviation = "VC";

            }
            else if (typeOfTest == BOOT_TEST)
            {
                testTypeAbbreviation = "BT";

            }
            else if (typeOfTest == POWER_TEST)
            {
                testTypeAbbreviation = "PT";

            }
            else
            {
                testTypeAbbreviation = "unknown";

            }
        }

        public void SendTestUpdateToSocketServer(string pathToErrorLog)
        {
            //if (clientForm.textBox_serverIP.BackColor == Color.Green)
            if (clientForm.cBox_serverIp.BackColor == Color.Green)
            {

                testUpdateInfo = new CustomEventArgs5_StatusOnly(pcGroupNumber,
                                                                   compNumber,
                                                                   slotNum,
                                                                   statusOfTest,
                                                                   global_PercentComplete.ToString(),
                                                                   this.clientForm.text_completedCycles.Text,
                                                                   descriptionOfCurrentState,
                                                                   pathToErrorLog);

                clientForm.mAsyncClient.SendTestUpdateToServer(testUpdateInfo);
            }
        }

        public void SendErrorToServer(string message)
        {
            descriptionOfCurrentState = message;

            string newFinalPath;

            try
            {
                // Append "fail" to the end of the filename.
                newFinalPath = copyToPath.Replace(".log", "--fail.log");
            }
            catch (Exception e)
            {
                MessageBox.Show("The copyToPath is null. Cannot send info to Server. No error log will be copied to shared folder. No email will be sent.\r\n\r\n" + e);

                string tempCopyToPath = addressOfLocalSharedFolder + "\\" + testGroupName + "\\" +
                serialNumber + "\\Verbose_" + logFileName + ".log";

                newFinalPath = tempCopyToPath.Replace(".log", "--fail.log");

            }


            TransferErrorLogToSharedFolder(VerboseLogPath, newFinalPath, true);

            // Test logs will be copied from the LAN shared folder to the Onedrive shared folder.
            SendErrorLogCopyCommandToServerIfNotAlreadySent(newFinalPath, true);

            SendTestUpdateToSocketServer(newFinalPath);
        }

        /// <summary>
        /// Confirms device is alive and working properly. For simulation purposes, this will have a 1 in 10 chance of returning FAIL.
        /// </summary>
        /// <returns></returns>
        protected string CheckIfDeviceIsFunctional()
        {
            // TODO set a random serial number simulating getting the SN from the drive itself

            if (randomGenerator.Next(1, 11) == 1)
            {
                return "Device not functional";
            }
            return SUCCESS;
        }

        /// <summary>
        /// Checks if the drive is fucntional.
        /// <br/>
        /// <br/>If device SN is known (or randomly generated):
        /// <br/>populate TestInfo.log file with test/drive info,
        /// <br/>retrieve and set info pertaining to test location, program name, and test type,
        /// <br/>and send initial test info to the Server if connected.
        /// </summary>
        /// <param name="sequencePosition"></param>
        /// <returns>SUCCESS, or FAIL </returns>
        protected string ReadDetailsFromDevice(string sequencePosition)
        {

            // If the SN has been detected (should only take 1 cycle) then output
            // generic info once in the SN folder. Will output again if test is restarted.
            if ((serialNumber != null) && (serialNumber != "") && (genericInfoHasBeenLogged == false))
            {
                GetTestLocationAndProgramInformation();
                GetTestTypeAbbreviation(this.testType);
                PopulateTestInfoLogWithDriveInfo();
                SendInitialTestInfo();
            }


            // TODO make a random number generator generate a failure and give it a random failure name.

            return SUCCESS;
        }

        public void MainLoop()
        {
            this.clientForm.stopProcess = false;
            FileTransferCommandHasBeenSent = false;

            string testStatus = "";

            this.clientForm.UpdateStatus("Starting Test\n");

            //TODO set test ITR number to whatever is displayed in the text box
            testGroupName = this.clientForm.GetText(this.clientForm.text_testGroupIdentifier);

            GetDeviceInfoFromDevice();
            PopulateGuiFieldsWithDeviceInfo();

            string testDriveStatus = CheckIfDeviceIsFunctional();

            if (testDriveStatus != SUCCESS)
            {
                this.clientForm.SetStartButton("Start");
                this.clientForm.button_startTest.BackColor = Color.Red;

                // TODO Need to send error message to server saying "failed in initial test setup"

                return;
            }

            VerboseLog("\r\n\r\nDevice check is good. Device will be powered off.");

            // Handle turning off whatever device you are testing

            VerboseLog("-------------------Device is now OFF-----------------------");

            this.clientForm.SetTestBox(this.testType);
            HoursPer1000CyclesTime = DateTime.Now;
            this.clientForm.SetStartButton("Stop");

            while (this.clientForm.stopProcess == false)
            {
                this.clientForm.UpdateStatus("Looping\r\n");
                testStatus = "";

                GetCycleInformation();

                //loopCount++;
                //totalTestCycles++;
                totalCycles++;

                CalcHoursPer1000Cycles();   // Figure out why I wrote this TODO

                // set cycle information
                //this.clientForm.SetText(this.clientForm.text_completedCycles, loopCount.ToString());
                this.clientForm.SetText(this.clientForm.text_completedCycles, totalCycles.ToString());
                //this.clientForm.SetText(this.testCyclesBox, totalTestCycles.ToString());

                CalculateEcdCycles();

                VerboseLog("Device powering on\r\n");
                this.clientForm.UpdateStatus("Powering on Device");
                Thread.Sleep(1000);
                VerboseLog("Device was powered on\r\n");
                this.clientForm.UpdateStatus("Device on");

                //CheckTestStatusAndUpdateServer();

                string commandSequenceStatus = this.ReadDetailsFromDevice("MAINLOOP");

                if (commandSequenceStatus == FAIL)
                {
                    ErrorReport("CommandSequence Fail");
                    CheckTestStatusAndUpdateServer();
                    return;
                }

                RunTest();

                // Start Button Background Color will be set to red if "ErrorReport()" has been called in any tests.
                // If any test encountered a failure then close ports, set start button red, and return to waiting for the start button to be clicked.
                if (this.clientForm.button_startTest.BackColor == Color.Red || this.clientForm.stopProcess == true)
                {
                    this.clientForm.UpdateStatus(testStatus += "Test stopped\n");
                    // this should already be done if this branch is entered into. 
                    this.clientForm.SetStartButton("Start");
                    // Reset this flag for the next time the start button is pressed.
                    this.clientForm.stopProcess = false;

                    CheckTestStatusAndUpdateServer();
                    return;
                }
                this.isFirstCycle = false;

                CheckTestStatusAndUpdateServer();
            }

            this.clientForm.UpdateStatus(testStatus = "Test stopped\n");

            this.clientForm.SetStartButton("Start");

            if (this.clientForm.stopButtonPressed == true)
            {
                CheckTestStatusAndUpdateServer();
                this.clientForm.stopButtonPressed = false;
            }

            // Reset this to false once the Mainloop has successfully exited. 
            this.clientForm.stopProcess = false;
        }
    }
}
