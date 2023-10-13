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

namespace Server_GUI
{
    public class Client
    {
        public Socket _socket { get; set; }
        public int Id { get; set; }
        public int _unique_Id { get; set; }
        public string _thisIPaddress { get; set; }
        public string _dateAndTimeOfConnection { get; set; }

        public string _testGroupNumber { get; set; }
        public string _compNumber { get; set; }
        public string _slotNumber { get; set; }

        public string _testAppVersion { get; set; }

        public string _testType { get; set; }
        public string _testName { get; set; }

        public string _programName { get; set; }
        public string _serialNumber { get; set; }
        public string _serialNumberLastFour { get; set; }
        public string _status { get; set; }
        public string _percent { get; set; }
        public string _cycleCount { get; set; }
        public string _descriptionOfState { get; set; }
        public string _pathToErrorLog { get; set; }			// add this

        public bool _hasHeartBeat { get; set; }
        public bool _isNewClient_ignoreHeartBeatOneTime { get; set; }
        public bool _isDead { get; set; }
        public bool _manuallyClosed { get; set; }
        public bool _needToSendEmail { get; set; }

        public Client(Socket socket, int id, int unique_Id)
        {
            _socket = socket;
            Id = id;
            //this value gives a unique ID starting at 1000.
            //It doesn not decrement when Clients are removed.
            //The _unique_Id will never be used twice, even if the Client reconnects.
            //If client reconnects, It will receive a new _unique_Id
            _unique_Id = unique_Id;
            _thisIPaddress = socket.RemoteEndPoint.ToString();
            _dateAndTimeOfConnection = DateTime.Now.ToString();
            // The client will not have a heartbeat when created.
            // This flag will be set to true if the remote client GUI sends a heartbeat message to this server for the particular client in question
            _hasHeartBeat = false;
            //The server will loop through a list of all clients when the server-side heartbeat has elapsed. Any clients with the _isNewClient_ignoreHeartBeatOneTime set to true will be ignored.
            //Once this happens one time for the specific client, the _isNewClient_ignoreHeartBeatOneTime will be set to false. 
            _isNewClient_ignoreHeartBeatOneTime = true;
            // Used a quick flag to tell if the client should be removed
            _isDead = false;
            _manuallyClosed = false;
            _needToSendEmail = false;
        }

        public string ConcatenateClientInfo()
        {
            string connectedClientInformation = "ClientID:: " + _unique_Id.ToString() + " :: " +
                                                    _thisIPaddress.ToString() + " :: " +
                                                    _dateAndTimeOfConnection.ToString() + " :: " +
                                                    _programName + " :: " +
                                                    _testGroupNumber + " :: " +
                                                    _compNumber + " :: " +
                                                    _slotNumber + " :: " +
                                                    _testAppVersion + " :: " +
                                                    _serialNumber + " :: " +
                                                    _status + " :: " +
                                                    _percent;
            return connectedClientInformation;
        }

    }


    public class ClientController
    {
        // List of all the Client objects
        public List<Client> Clients = new List<Client>();

        //keeps a running count of connected clients
        //used to make the unique ID, starting at 1000
        //when clients are removed, this value does not decrement,
        //this allows all subsequent clients to have a different unique ID
        public int runningClientCount = 1000;
        public int lastClientConnectedID;


        public int AddClient(Socket socket)
        {
            //generate unique id and apply it to the the new clients's property
            int uniqueID = GenerateUniqueID();
            //once a client is added, give it a unique ID corresponding to the Client.count
            Clients.Add(new Client(socket, Clients.Count, uniqueID));
            return uniqueID;
        }

        public int GenerateUniqueID()
        {
            int uniqueId = runningClientCount;
            //specify this as the last client connected.
            lastClientConnectedID = uniqueId;
            runningClientCount++;
            return uniqueId;
        }

    }
}
