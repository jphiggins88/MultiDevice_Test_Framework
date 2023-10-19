# MultiDevice_Test_Framework

This Windows (.NET Core) program is a versatile and customizable framework designed for initiating and monitoring multiple tests on various devices through socket communication. The application comprises two main components – the Server and the Client.

##Server
The Server acts as a central hub for monitoring all connected clients. Clients are specialized to conduct tests on remote systems within the local network. The flexibility of this framework allows users to define and implement custom tests tailored to their specific requirements.

##Client
Clients are responsible for executing tests on multiple remote systems. The current setup includes simulations of three hardware-based test types: Voltage Check, Power Test, and Boot Test. These simulation tests are intentionally configured to fail randomly, showcasing the robust error logging capabilities of the framework. However, these simulation tests should be replaced with actual tests relevant to the user's needs.

#Features
*Scalable Configuration: The Server can efficiently handle up to 120 tests in total, organized into 4 PC Groups. Each PC Group consists of 3 PCs, and each PC can accommodate 10 tests. This organization is similar to a rack of servers/PCs, where individual PCs are uniquely named, and groups are designated by their "PC Group" name.

*Test Device Customization: Each Test Device is assigned a "Test Group ID" name, allowing users to group specific tests for comprehensive logging. The default name is "GenericTests," and users can select the desired "Slot Number," "PC Name," and "PC Group" from the Client GUI.

*Intuitive User Interface: The Client GUI provides an easy-to-use interface for selecting test parameters, including "Slot Number," "PC Name," and "PC Group." The "Slot Number" corresponds to the device's location in the Server GUI's "Big Picture" tab.

*Usage Guidelines
The Server is currently configured to manage slot numbers 1-10. If utilizing hardware devices, ensure that the COM ports of each test device are set to the corresponding slot number through Windows Device Manager.
Feel free to adapt and extend this framework to suit your specific testing needs. Explore the flexibility and scalability of this automated testing solution for efficient and reliable test execution.


#Instructions:


The LAN shared folder is to be accessible by both the Server PC and all clients running the Client program. The shared folder is used to accumulate all clients logs in one central place. It is also accessed by the Server to send error logs and daily update logs as email attachments to desired recipients.

Daily triggers (Client and Server)
Email list
Logging
File transferring (errors, TestInfo, and daily updates)
Network setup
PC names must follow the PC-01, PC-02,…PC-12 naming convention. You can manually enter/select PC names in the drop down box. If you are using the program for long term testing, it is recommended to rename the dedicated testing PC’s accordingly.
Sockets (ACK and HeartBeat timers)
Every message sent to/from the socket/client will listen for an acknowledgement message. If an ACK is not received before the ACK timer expires, then an error message will be displayed and the client or server will be deemed disconnected. If the client/server is sending an ACK, it will not listen for a ACK.
The heartbeat timer is used to test the connectivity of clients that may not be sending information at the moment. If a test is paused, or a test cycle takes a long time, the server may think the device is disconnected. Every device will send a HeartBeat at a given interval. The Server maintains a HeartBeat timer for each client. If any of the timers expire before receiving a HeartBeat from that client, then that client will be deemed disconnected.
