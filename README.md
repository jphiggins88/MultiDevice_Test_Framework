# MultiDevice_Test_Framework
This Windows (.NET Core) program is a versatile and customizable framework designed for initiating and monitoring multiple tests on various devices through socket communication. The application is comprised of two main components – the Server and the Client. This program is currently setup to simulate fake tests since the goal is to provide a generic and modifiable framework for testing. The user will need to incorporate their specific tests by modifying the source code. (Modification documentation coming soon). 

<img width="1654" alt="Server and Client" src="https://github.com/jphiggins88/MultiDevice_Test_Framework/assets/26196159/4e81bc66-b7d3-4863-9da9-0feb9929d295">

## Server
The Server acts as a central hub for monitoring all connected clients. Currently the server is designed to run on a System with dual network cards. The dual network cards are needed to connect to an isolated test network in which all client programs will be run. The 2nd network card allows the system to connect to the internet in order to send emails and store log files in cloud based storage if desired. The main selling point of this setup is the fact that all test PCs are isolated withing a LAN. Only the Server PC has access to the WAN. This network configuration can me modified rather easily in the source code.

## Client
Clients are responsible for executing tests on multiple remote systems. The current setup includes simulations of three hardware-based test types: Voltage Check, Power Test, and Boot Test. These simulation tests are intentionally configured to fail randomly, showcasing the error logging capabilities of the framework. However, these test simulations should be replaced with real tests relevant to the user's needs.

# Features
* Scalable Configuration: The Server can efficiently handle up to 120 tests in total, organized into 4 PC Groups. Each PC Group consists of 3 PCs, and each PC can accommodate 10 tests. This organization is analogous to a rack of servers/PCs, where individual PCs are uniquely named, and groups are designated by their "PC Group" name. See the Network Setup diagram for an example of the Server/CLient layout.

* Test Device Customization: Each Test Device is assigned a "Test Group ID" name, allowing users to group specific tests for comprehensive logging. The default name is "GenericTests," and users can select the desired "Slot Number," "PC Name," and "PC Group" from the Client GUI.

* Intuitive User Interface: The Client GUI provides an easy-to-use interface for selecting test parameters, including "Slot Number," "PC Name," and "PC Group." The "Slot Number" corresponds to the device's location in the Server GUI's "Big Picture" tab.

* Usage Guidelines
The Server is currently configured to manage slot numbers 1-10. If utilizing hardware devices, ensure that the COM ports of each test device are set to the corresponding slot number through Windows Device Manager.
Feel free to adapt and extend this framework to suit your specific testing needs. Explore the flexibility and scalability of this automated testing solution for efficient and reliable test execution.

# Network Setup
![NetworkLayout_2](https://github.com/jphiggins88/MultiDevice_Test_Framework/assets/26196159/ba55c828-3924-4e6b-ac21-2f5b62d1b91a)

# Instructions:
The LAN shared folder is to be accessible by both the Server PC and all clients running the Client program. The shared folder is used to accumulate all clients logs in one central place. It is also accessed by the Server to send error logs and daily update logs as email attachments to desired recipients.

# Example of a successful test cycle
In the diagram below, the Server and Client establish a socket connection and a single successful test cycle is completed by the client.
All Socket messages and file transfers can be seen in the diagram.
![SuccessfulTestExample (2)](https://github.com/jphiggins88/MultiDevice_Test_Framework/assets/26196159/ea10769d-088c-447d-bbcc-b4d1b682c007)




# Documentation in progress
* Daily triggers (Client and Server)
Each client will transfer its daily log to LAN Shared Folder at midnight, as well as send a transfer command to tell the Server to copy the log from the LAN Shared Folder to the WAN Shared Folder. This is a necessary process to keep the test environemnt isolated within its own LAN. Only the Server has access to the outside world. The Server will send out Daily updates at specified times to specified email addresses. 
* Email list
The user can specify the email addresses of all recipients who need to receive information about test data. The email addresses are separated into 2 lists. You can think of 1 list as containing the engineers and technicians on-site, running the tests. They need to be first to knwo when something goes wrong. They also need to receive all status updates. The 2nd list contains members who need less verbose information and who do not need to necessarily know about all tests that are running. Members on the email lists will receive daily updates of all tests or specific tests, real-time alerts when tests fail, and error logs of failing tests.
* Logging
* File transferring (errors, TestInfo, and daily updates)
* Network setup
* PC Naming Convention
PC names must follow the PC-01, PC-02,…PC-12 naming convention. You can manually enter/select PC names in the drop down box. If you are using the program for long term testing, it is recommended to rename the dedicated testing PC’s accordingly.
* Socket timers (ACK and HeartBeat timers)
Every message sent to/from the socket/client will listen for an acknowledgement message. If an ACK is not received before the ACK timer expires, then an error message will be displayed and the client or server will be deemed disconnected. If the client/server is sending an ACK, it will not listen for a ACK.
The heartbeat timer is used to test the connectivity of clients that may not be sending information at the moment. If a test is paused, or a test cycle takes a long time, the server may think the device is disconnected. Every device will send a HeartBeat at a given interval. The Server maintains a HeartBeat timer for each client. If any of the timers expire before receiving a HeartBeat from that client, then that client will be deemed disconnected.


