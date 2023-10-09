using System;
using System.Collections.Generic;
using System.Text;

namespace Client_GUI
{

    public class CustomEventArgs4 : EventArgs
    {
        //public CustomEventArgs(string cli, string[,] valuesp, string Lastsend, string Lastreceived)
        public CustomEventArgs4(string programName, string serialNum, string pcGroupNumber, string compNum, string slotNum,
                                string testAppVersion, string testName, string status, string percent, string cycleCount, string testType)
        {
            client_programName = programName;
            client_serialNum = serialNum;

            client_pcGroupNumber = pcGroupNumber;
            client_compNum = compNum;
            client_slotNum = slotNum;

            client_testAppVersion = testAppVersion;
            client_testName = testName;

            client_status = status;
            client_percent = percent;
            client_cycleCount = cycleCount;

            client_testType = testType;
        }

        public string client_programName;
        public string client_serialNum;
        public string client_pcGroupNumber;
        public string client_compNum;
        public string client_slotNum;
        public string client_testAppVersion;
        public string client_testName;
        public string client_status;
        public string client_percent;
        public string client_cycleCount;
        public string client_testType;
    }

    public class CustomEventArgs5_StatusOnly : EventArgs
    {
        //public CustomEventArgs(string cli, string[,] valuesp, string Lastsend, string Lastreceived)
        public CustomEventArgs5_StatusOnly(string pcGroupNumber, string compNum, string slotNum,
                                string status, string percent, string cycleCount, string descriptionOfState, string pathToErrorLog)
        {

            client_pcGroupNumber = pcGroupNumber;
            client_compNum = compNum;
            client_slotNum = slotNum;

            client_status = status;
            client_percent = percent;
            client_cycleCount = cycleCount;

            client_descriptionOfState = descriptionOfState;
            client_pathToErrorLog = pathToErrorLog;

        }

        public string client_pcGroupNumber;
        public string client_compNum;
        public string client_slotNum;

        public string client_status;
        public string client_percent;
        public string client_cycleCount;
        public string client_descriptionOfState;
        public string client_pathToErrorLog;
    }


}

