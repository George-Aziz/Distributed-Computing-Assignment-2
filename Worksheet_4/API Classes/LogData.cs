// Filename: LogData.cs
// Project:  DC Assignment 2 (Practical 4)
// Purpose:  Log file
// Author:   George Aziz (19765453)
// Date:     06/05/2021

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Classes
{
    /// <summary>
    /// Logs all info into logfile
    /// </summary>
    public class LogData
    {
        //Log file path
        private string path = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName, "LogFiles/log.txt");
        private StreamWriter writer;
        private static int logNum = 1;
        public void LogMsg(string funcName, string message)
        {
            writer = new StreamWriter(path, append: true);
            //Creates a stream writer to append to file
            string date = DateTime.Now.ToString();
            writer.WriteLine(date + ": Task #" + logNum);
            writer.WriteLine("[" + funcName + "]" + " " + message);
            writer.Close();
            logNum++;
        }
    }
}
