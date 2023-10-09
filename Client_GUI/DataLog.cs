using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Linq;

namespace Client_GUI
{
    public class DataLog
    {
        public void LogData(string path, string data)
        {
            FileStream fileStream;
            StreamWriter streamWriter;

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));

                if (System.IO.File.Exists(path))
                {
                    fileStream = new FileStream(path, FileMode.Append, FileAccess.Write);
                }
                else
                {
                    fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
                }

                streamWriter = new StreamWriter(fileStream);
                streamWriter.WriteLine(data);
                streamWriter.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot Open File " + path + ex);
                return;
            }
        }

        public void ClearLog(string path)
        {
            FileStream fileStream;
            StreamWriter streamWriter;

            try
            {

                fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
                streamWriter = new StreamWriter(fileStream);
                streamWriter.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot Clear File " + path + ex);
                return;
            }
        }

        public void LogAutomation(string path, string data)
        {
            FileStream fileStream;
            StreamWriter streamWriter;

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));

                fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);

                streamWriter = new StreamWriter(fileStream);
                streamWriter.WriteLine(data);
                streamWriter.Close();
            }
            catch
            {
                return;
            }
        }

        public void LogRestart(string path, string[] value)
        {
            FileStream fileStream;
            StreamWriter streamWriter;

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));

                fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);

                streamWriter = new StreamWriter(fileStream);

                foreach (string line in value)
                {
                    streamWriter.WriteLine(line);
                }

                streamWriter.Close();
            }
            catch
            {
                return;
            }

        }

        public string[] GetRestart(string path, int entries)
        {
            FileStream fileStream;
            StreamReader streamReader;
            string[] restartEntry = new string[entries];
            int i = 0;
            int emptyLineCounter = 0;
            int maxEmptyLines = 2;

            if (System.IO.File.Exists(path))
            {
                fileStream = new FileStream(path, FileMode.Open);
                streamReader = new StreamReader(fileStream);



                for (i = 0; i < entries; i++)
                {
                    // Added to allow for multiline test notes


                    if (emptyLineCounter <= maxEmptyLines)
                    {
                        restartEntry[i] = streamReader.ReadLine();

                        if (restartEntry[i] == null)
                        {
                            // Incriment the empty line counter. 2 empty lines in a row will dictate the end of the Test Notes.
                            emptyLineCounter++;
                        }

                    }

                    if (emptyLineCounter >= maxEmptyLines)
                    {
                        // Stop parsing the lines. You have reached the end.
                        i = entries;
                    }
                }

                streamReader.Close();
            }

            return restartEntry;
        }

        public void DirectoryCheck(string path)
        {
            if (Directory.Exists(path))
            {
                return;
            }
            else
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}

