// Filename: LogData.cs
// Project:  DC Assignment 2 (Practical 4)
// Purpose:  Model for Logger to be used by Business Tier Controller Services
// Author:   George Aziz (19765453)
// Date:     25/04/2021
/*
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Hybrid_Biz_Presentation_Tier.Models
{
    public class LogData
    {
        //Log file path
        private string path = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName, "LogFiles/log.txt");
        private StreamWriter writer;
        
        public void logMsg(string funcName, string message)
        {
            writer = new StreamWriter(path, append: true);
            //Creates a stream writer to append to file
            writer.WriteLine(funcName + ": " + message);
            writer.Close();
        }
    }
}*/