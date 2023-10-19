using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;


namespace Server_GUI
{



    public class Email_Sender
    {

        /// <summary>
        /// The code below was used to test email sending with a throw-away gmail account
        /// 
        /// In order to use gmail though .NET, you must enable "Less secure app access" through your gmail account.
        /// Otherwise you will get an error and gmail will prevent .NET from accessing the gmail account.
        /// Once Less Secure App Access is enabled, it must be used every now and then or google will disable it again.
        /// https://stackoverflow.com/questions/18503333/the-smtp-server-requires-a-secure-connection-or-the-client-was-not-authenticated
        /// </summary>

        //public static string smtpAddress = "smtp.gmail.com";
        //public static int portNumber = 587;
        //public static bool enableSSL = true;
        //public static string emailFromAddress = "<your email address to send from>"; //Sender Email Address  
        //public static string password = "<your password>"; //Sender Password  
        //public static string emailToAddress = "<email address to send to>"; //Receiver Email Address  
        //public static string subject = "test device-update";
        //public static string body = "testing sending email using C# .NET.";


        /// <summary>
        /// The code below "may" only work on the WD Network. 
        /// It's shown to be working from outside the network as of 4/22/22
        /// </summary>

        public static string pathToEmailRecipientList_deviceOwners = Application.StartupPath + "\\emailRecipientList_deviceOwners.txt";
        public static string pathToEmailRecipientList_managers = Application.StartupPath + "\\emailRecipientList_managers.txt";

        public static string smtpAddress = "smtp-mail.outlook.com";
        public static int portNumber = 587;
        public static bool enableSSL = true;

        // ********** Fill out sender and password info **********
        public static string emailFromAddress = "<your/sender's email>"; //Sender Email Address  
        public static string password = "<password>"; //Sender Password  
        public static string emailToAddress = "<recipients' email>"; //Receiver Email Address

        // Open the email list text file and set the send-to list. (addresses are delimited by commas.
        public static string emailToList_deviceOwners = ParseEmailRecipientSingleString(pathToEmailRecipientList_deviceOwners); //Receiver Email Address
        public static string emailToList_managers = ParseEmailRecipientSingleString(pathToEmailRecipientList_managers); //Receiver Email Address

        public static string subject = "Test Devices - update";
        public static string body = "testing sending email using C# .NET.";

        public void SendTest(string sampleString)
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(emailFromAddress);
                mail.To.Add(pathToEmailRecipientList_deviceOwners);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = false;
                //mail.Attachments.Add(new Attachment("D:\\TestFile.txt"));  // Uncomment this to send attachment  
                using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                {
                    smtp.Credentials = new NetworkCredential(emailFromAddress, password);
                    smtp.EnableSsl = enableSSL;
                    smtp.Send(mail);
                }
            }
        }

        // TODO remove if unused
        public void SendAlert(string clientID)
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(emailFromAddress);

                // Sending to emailToList will send a single email to all recipients in the address list file. All recipients should be comma delimited.
                mail.To.Add(emailToList_deviceOwners);

                mail.Subject = subject + ":: client " + clientID + " was lost";
                mail.Body = "Client " + clientID + " is unresponsive/disconnected/failed";
                mail.IsBodyHtml = false;
                //mail.Attachments.Add(new Attachment("D:\\TestFile.txt"));//--Uncomment this to send any attachment  
                using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                {
                    smtp.Credentials = new NetworkCredential(emailFromAddress, password);
                    smtp.EnableSsl = enableSSL;
                    smtp.Send(mail);
                }
            }
        }

        public void SendAlert_AllInfo(Client clientInfo, string pathToErrorLog)
        {
            string description = "...";

            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(emailFromAddress);

                // Sending to emailToList will send a single email to all recipients in the address list file. All recipients should be comma delimited on a single line.
                mail.To.Add(emailToList_deviceOwners);

                // If type2 tests, then add the type2 owners/team also. 
                if (clientInfo._testType == "VC" || clientInfo._testType == "BT" || clientInfo._testType == "PT")
                {
                    // Sending to emailToList will send a single email to all recipients in the address list file. All recipients should be comma delimited on a single line.
                    mail.To.Add(emailToList_managers);
                }

                // Do not use "\r" anywhere in the subject line. 
                mail.Subject = subject + "::     SN: " + clientInfo._serialNumber + "     Program: " + clientInfo._programName + "     Cause: " + clientInfo._status;


                if(clientInfo._status == "Unknown")
                {
                    description = "is unresponsive/disconnected";
                }
                else if (clientInfo._status == "Failed")
                {
                    description = "has Failed";
                }

                mail.Body = "Client " + clientInfo._serialNumber + " " + description + "\r\n" +
                            "Program: " + clientInfo._programName + "\t" +
                            "Test: " + clientInfo._testName + "\t" +
                            "Status: " + clientInfo._status + "\t" +
                            "Cycle count: " + clientInfo._cycleCount + "\r\n" +
                            " " + clientInfo._descriptionOfState + "\r\n" +
                            "Chamber: " + clientInfo._testGroupNumber + "\t" +
                            "PC: " + clientInfo._compNumber + "\t" +
                            "Slot: " + clientInfo._slotNumber;


                mail.IsBodyHtml = false;

                // If pathToErrorLog is blank, don't attach anything.
                // If the client sent a pathToErrorLog, then it means the client has already copied the error log to the LAN shared folder,
                // so we want to attach the file.
                if(pathToErrorLog != "")
                {
                    try
                    {
                        /*
                        Zip the file before attaching the log to an email since the email limit is 20Mb and logs can exceed 25Mb.

                        2 proposed methods:

                        Method 1: Have all the errors placed inside of they typical filepath but with an added subdirectory "\errors":
                        example:
                                 addressOftype2SharedFolder + "\\" + itrNumber + "\\" + serialNumber + "\\errors" + "Verbose_" + logFileName + ".log";

                        Method 2: Append "--fail" to the end of all failing logs and transfer them to the normal destination.
                        example:
                                 addressOftype2SharedFolder + "\\" + itrNumber + "\\" + serialNumber + "\\Verbose_" + logFileName + "--fail.log";


                        // This would be used for method 1 above. ####################################################
                        // We want to zip everything inside of errors folder, therefore the destination path for the zipped file needs to be outside of the errors folder.
                        int indexOfLastBackslash = pathToErrorLog.LastIndexOf("\\errors");
                        string zipPath = pathToErrorLog.Remove(indexOfLastBackslash);
                        zipPath = zipPath + "\\Error.zip";

                        // This should designate everything inside of the errors folder as the targets to be zipped.
                        int lastIndexOfBackslash = pathToErrorLog.LastIndexOf("\\");
                        pathToErrorLog = pathToErrorLog.Remove(lastIndexOfBackslash);

                        // If an error.zip file already exists, delete it.
                        if (File.Exists(zipPath))
                        {
                            File.Delete(zipPath);
                        }

                        ZipFile.CreateFromDirectory(pathToErrorLog, zipPath);
                        ###########################################################################################


                        This should be used for method 2 above. ####################################################
                        In order to zip a file, it needs to be in a folder.
                             1. We need to create a directory,
                             2. Copy the file into the directory,
                             3. Zip that folder,
                             4. Then potentially delete the directory we just created (TO DO later).


                        1. Create Directory
                             a. Remove the actual filename to get the folder the file is transferred to.
                             b. Add a new temp folder to that path. 
                        */


                        int indexOfLastBackslash = pathToErrorLog.LastIndexOf("\\");
                        // Extract filename from full path.
                        string filenameOfError = pathToErrorLog.Remove(0, (indexOfLastBackslash + 1));

                        // Remove everything after the last "\". This should remove the filename and leave you with the directory.
                        string errorPathRootDir = pathToErrorLog.Remove(indexOfLastBackslash);
                        string errorPathRootDir_TempDir = errorPathRootDir + "\\Error";
                        //Directory.CreateDirectory(Path.GetDirectoryName(errorPathRootDir_TempDir));
                        Directory.CreateDirectory(errorPathRootDir_TempDir);

                        // 2. Copy the file into the directory. Overwrite if necessary.
                        File.Copy(pathToErrorLog, (errorPathRootDir_TempDir + "\\" + filenameOfError), true);

                        // 3. Zip the folder containing the new copied error log
                        //      We use errorPathRootDir instead of errorPathRootDir_TempDir because we cannot create a zip file inside a directory we are zipping.
                        string zipPath = errorPathRootDir + "\\Error.zip";

                        // If an error.zip file already exists, delete it.
                        if (File.Exists(zipPath))
                        {
                            File.Delete(zipPath);
                        }

                        ZipFile.CreateFromDirectory(errorPathRootDir_TempDir, zipPath);
                        mail.Attachments.Add(new Attachment(zipPath));
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Unable to attach log to email\r\n\r\n" + e);
                    }
                }
                
                using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                {
                    smtp.Credentials = new NetworkCredential(emailFromAddress, password);
                    smtp.EnableSsl = enableSSL;
                    smtp.Send(mail);
                }
            }
        }

        /// <summary>
        /// Loops through all the clients and creates a subject line and email body based on the status of all type2 test devices running.
        /// Sends the email to everyone in the recipient email list located in the startup directory.
        /// </summary>
        /// <param name="bodyOfSummary"></param>
        /// <param name="quickDescription"></param>
        public void SendDailyUpdateEmailForAllDevicesOfType2(string bodyOfSummary, string quickDescription)
        {
            using (MailMessage mail = new MailMessage())
            {

                mail.From = new MailAddress(emailFromAddress);

                // Sending to emailToList will send a single email to all recipients in the address list file. All recipients should be comma delimited.
                mail.To.Add(emailToList_managers);
                try
                {
                    mail.Subject = "Test Device Daily Update - test device type 2:: " + quickDescription;
                    mail.Body = bodyOfSummary;

                }
                catch(Exception e)
                {
                    MessageBox.Show(e.ToString());

                }


                mail.IsBodyHtml = false;
                //mail.Attachments.Add(new Attachment("D:\\TestFile.txt"));//--Uncomment this to send any attachment  
                using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                {
                    smtp.Credentials = new NetworkCredential(emailFromAddress, password);
                    smtp.EnableSsl = enableSSL;
                    smtp.Send(mail);
                }
            }
        }

        /// <summary>
        /// Loops through all the clients and creates a subject line and email body based on the status of ALL test devices running.
        /// Sends the email only to the designated recipient (the manager of testDevice lab for example).
        /// </summary>
        /// <param name="bodyOfSummary"></param>
        /// <param name="quickDescription"></param>
        public void SendDailyUpdateEmailForAllDevices_AllTypes(string bodyOfSummary, string quickDescription)
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(emailFromAddress);

                // Sending to emailToList will send a single email to all recipients in the address list file. All recipients should be comma delimited.
                mail.To.Add(emailToList_deviceOwners);
                try
                {
                    mail.Subject = "Test Device Daily Update - All Devices:: " + quickDescription;
                    mail.Body = bodyOfSummary;

                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());

                }

                mail.IsBodyHtml = false;
                //mail.Attachments.Add(new Attachment("D:\\TestFile.txt"));//--Uncomment this to send any attachment  
                using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                {
                    smtp.Credentials = new NetworkCredential(emailFromAddress, password);
                    smtp.EnableSsl = enableSSL;
                    smtp.Send(mail);
                }
            }
        }

        /// <summary>
        /// Parse the text file in the application startup path that contains a list of email addresses. 
        /// The addresses are "delimited' by a comma. No new lines should be in the email list text file. 
        /// Place all recipients on a new line.
        /// </summary>
        /// <returns>A string array containing the list of all email recipients OR empty array if the recipient list file does not exist.</returns>
        public static string ParseEmailRecipientSingleString(string pathToList)
        {
            StreamReader reader = null;
            string addressList = null;

            try
            {
                reader = File.OpenText(pathToList);
                addressList = reader.ReadLine();
                reader.Close();
                return addressList;
            }
            catch (Exception e)
            {
                MessageBox.Show("Problem parsing email recipient list. Make sure <application startup path>\\mailRecipientList.txt exists\r\n\r\n" + e);
                return addressList;
            }
        }





    }
}
