using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;


namespace Server_GUI
{
    public class CustomEventArgs : EventArgs
    {
        //public CustomEventArgs(string cli, string[,] valuesp, string Lastsend, string Lastreceived)
        public CustomEventArgs(string clientArg, string valuesArg, string LastSend, string LastReceived)
        {
            client = clientArg;
            values = valuesArg;
            lastSend = LastSend;
            lastReceived = LastReceived;
        }
        public string lastSend;
        public string lastReceived;
        private string client;
        //private string[,] values;
        private string values;

        public string Client
        {
            get { return client; }
            set { client = value; }
        }

        //public string[,] Values
        public string Values
        {
            get { return values; }
            set { values = value; }
        }
    }

    public class CustomEventArgs2 : EventArgs
    {
        //public CustomEventArgs(string cli, string[,] valuesp, string Lastsend, string Lastreceived)
        public CustomEventArgs2(string id, string serialNum, string dateTime, 
                                string testGroupNum, string compNum, string slotNum, string programName, string testAppVersion,
                                string status, string percent)
        {
            client_id = id;
            client_serialNum = serialNum;
            client_dateTime = dateTime;
            client_testGroupNum = testGroupNum;
            client_compNum = compNum;
            client_slotNum = slotNum;
            client_programName = programName;
            client_testAppVersion = testAppVersion;
            client_status = status;
            client_percent = percent;
        }

        public string client_id;
        public string client_serialNum;
        public string client_dateTime;
        public string client_testGroupNum;
        public string client_compNum;
        public string client_slotNum;
        public string client_programName;
        public string client_testAppVersion;
        public string client_status;
        public string client_percent;
    }


    public class CustomEventArgs3 : EventArgs
    {
        public CustomEventArgs3(string clientID, string status, string percent)
        {
            client_id = clientID;
            client_status = status;
            client_percent = percent;
        }

        public string client_id;
        public string client_status;
        public string client_percent;
    }


    public class CustomEventArgs2_withTargetClient : EventArgs
    {
        public CustomEventArgs2_withTargetClient(string id, string serialNum, string dateTime,
                                string testGroupNum, string compNum, string slotNum, string programName, string testAppVersion,
                                string status, string percent, int targetClient)
        {
            client_id = id;
            client_serialNum = serialNum;
            client_dateTime = dateTime;
            client_testGroupNum = testGroupNum;
            client_compNum = compNum;
            client_slotNum = slotNum;
            client_programName = programName;
            client_testAppVersion = testAppVersion;
            client_status = status;
            client_percent = percent;

            // Holds the index of the row that will be overwritten with updated test data
            gridView_targetClient = targetClient;
        }

        public string client_id;
        public string client_serialNum;
        public string client_dateTime;
        public string client_testGroupNum;
        public string client_compNum;
        public string client_slotNum;
        public string client_programName;
        public string client_testAppVersion;
        public string client_status;
        public string client_percent;

        // Holds the index of the row that will be overwritten with updated test data
        public int gridView_targetClient;
    }

    public class CustomEventArgs3_withTargetClient : EventArgs
    {
        public CustomEventArgs3_withTargetClient(string id, string serialNum, string dateTime,
                                string testGroupNum, string compNum, string slotNum, string programName, string testAppVersion,
                                string status, string percent, string serialNumberLastFour, string testType)
        {
            client_id = id;
            client_serialNum = serialNum;
            client_dateTime = dateTime;
            client_testGroupNumber = testGroupNum;
            client_compNum = compNum;
            client_slotNum = slotNum;
            client_programName = programName;
            client_testAppVersion = testAppVersion;
            client_status = status;
            client_percent = percent;

            client_serialNumLastFour = serialNumberLastFour;
            client_testType = testType;

    }

        public string client_id;
        public string client_serialNum;
        public string client_dateTime;
        public string client_testGroupNumber;
        public string client_compNum;
        public string client_slotNum;
        public string client_programName;
        public string client_testAppVersion;
        public string client_status;
        public string client_percent;

        public string client_serialNumLastFour;
        public string client_testType;

    }

    public class CustomEventArgs4_statusUpdates : EventArgs
    {
        public CustomEventArgs4_statusUpdates(string id, string testGroupNum, string compNum, string slotNum,
                                string status, string percent, string cycleCount, string descriptionOfState, bool wasManuallyClosed)
        {
            client_id = id;
            client_testGroupNum = testGroupNum;
            client_compNum = compNum;
            client_slotNum = slotNum;
            client_status = status;
            client_percent = percent;
            client_cycleCount = cycleCount;
            client_descriptionOfState = descriptionOfState;
            client_wasManuallyClosed = wasManuallyClosed;


        }

        public string client_id;
        public string client_testGroupNum;
        public string client_compNum;
        public string client_slotNum;
        public string client_status;
        public string client_percent;
        public string client_cycleCount;
        public string client_descriptionOfState;
        public bool client_wasManuallyClosed;

    }


    public class CustomEventArg_txtBoxControl : EventArgs
    {
        public CustomEventArg_txtBoxControl(Control targetControl)
        {
            controlElement = targetControl;
        }

        public Control controlElement;
    }

    public class CustomEventArgs5 : EventArgs
    {
        public void CustomEventArg5(TableLayoutPanel tableLayoutPanelArg, int slotNumArg)
        {
            targetTableLayoutPanelArg = tableLayoutPanelArg;
            targetSlotNumArg = slotNumArg;
        }

        public TableLayoutPanel targetTableLayoutPanelArg;
        public int targetSlotNumArg;
    }


    public class CustomEventArgs_forAlertEmail : EventArgs
    {
        //public CustomEventArgs(string cli, string[,] valuesp, string Lastsend, string Lastreceived)
        public CustomEventArgs_forAlertEmail(string programName, string serialNum, string testName, string status,
                                            string cycleCount, string descriptionOfState, string testGroupNum, string compNum,
                                            string slotNum, string clientID)
        { 
            client_programName = programName;
            client_serialNum = serialNum;
            client_testName = testName;

            client_status = status;
            client_cycleCount = cycleCount;
            client_descriptionOfState = descriptionOfState;
            
            client_testGroupNum = testGroupNum;
            client_compNum = compNum;
            client_slotNum = slotNum;

            client_ID = clientID;
        }

        public string client_programName;
        public string client_serialNum;
        public string client_testName;

        public string client_status;
        public string client_cycleCount;
        public string client_descriptionOfState;

        public string client_testGroupNum;
        public string client_compNum;
        public string client_slotNum;

        public string client_ID;
    }



}




