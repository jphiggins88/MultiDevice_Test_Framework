using System;
using System.Collections.Generic;
using System.Text;

namespace Client_GUI
{
    /// <summary>
    /// This class acts as a poorly coded API to allow the client socket 
    /// code to interface with a real testApplication, doing real work, somewhere...
    /// </summary>
    class TestDevice_Interface
    {
        // Test App explicit fail function
        // Receive some sort of notification from the test app when the test app notifies us of an error.
        // this will not detect GUI crashes. This will only detect issues relayed from the test app GUI actively telling us something is wrong
        // call the client socket sending fuction to send an appropriate alert.
        // a string needs to be passed into this function, and then passed to the client socket sending functions
        // this string should tell what the error was, what time it occurred,
        // and what client sent it(this part can be handled in the client Socket code)


        //Check-in with Server Function
        //timer driven function to send periodic check-inst with the server
        //the time between pings should be less than the time the server expects the pings
        //ex: set the ping rate so the slient sends "status OK" once every 5min
        //      set the server somethingsWrong timer to expect a ping once every 10 min (double the client rate)
        //This "Status OK" is just to notify the server that everything is fine and the GUI is still alive.
        //We don't want to send a "status OK" after every good completed test cycle. 
        //instead will only send a message "status FAIL" when a problem is detected, or a "status OK" every 5 miin while the GUI is running
        //we may also want to include general test progress (progress percent, cycle count complete, what test is running...)
         

    }
}
