// Filename: Program.cs
// Project:  DC Assignment 2 (Practical 2)
// Purpose:  Network setup for Database Service
// Author:   George Aziz (19765453)
// Date:     22/04/2021

using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBDLL;

namespace ServerProg
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Database the server!");
            //This is the actual host service system
            ServiceHost host;
            NetTcpBinding tcp = new NetTcpBinding();
            //Bind server to the implementation of Data Server
            host = new ServiceHost(typeof(DataServerImplementation));
            //Service Endpoint to interact with
            host.AddServiceEndpoint(typeof(DataServerInterface), tcp, "net.tcp://0.0.0.0:8100/DataService");
            //Open Host
            host.Open();
            Console.WriteLine("Database System is now online");
            Console.ReadLine();
            //Ensure to close host after everything is complete
            host.Close();
        }
    }
}
