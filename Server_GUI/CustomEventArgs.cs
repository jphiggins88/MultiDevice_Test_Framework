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


    public class CustomEventArgs3_withTargetClient : EventArgs
    {
        public CustomEventArgs3_withTargetClient(
            string id,
            string serialNum,
            string dateTime,
            string testGroupNum,
            string compNum,
            string slotNum,
            string programName,
            string testAppVersion,
            string status,
            string percent,
            string serialNumberLastFour,
            string testType,
            int targetClient,
            string cycleCount,
            string descriptionOfState,
            bool wasManuallyClosed)
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
            client_serialNumLastFour = serialNumberLastFour;
            client_testType = testType;
            gridView_targetClient = targetClient; // Holds the index of the row that will be overwritten with updated test data
            client_cycleCount = cycleCount;
            client_descriptionOfState = descriptionOfState;
            client_wasManuallyClosed = wasManuallyClosed;
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

        public string client_serialNumLastFour;
        public string client_testType;

        public int gridView_targetClient;

        public string client_cycleCount;
        public string client_descriptionOfState;
        public bool client_wasManuallyClosed;



    }
}




