// Filename: Program.cs
// Project:  DC Assignment 2 (Practical 2)
// Purpose:  Network setup for Business Service
// Author:   George Aziz (19765453)
// Date:     22/04/2021

using BizDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the Business server!");
            //This is the actual host service system
            ServiceHost host;
            NetTcpBinding tcp = new NetTcpBinding();
            //Bind server to the implementation of Business Server
            host = new ServiceHost(typeof(BusinessServer));
            //Service Endpoint to interact with
            host.AddServiceEndpoint(typeof(BusinessServerInterface), tcp, "net.tcp://0.0.0.0:8200/BusinessService");
            //Open Host
            host.Open();
            Console.WriteLine("Business System is now online");
            Console.ReadLine();
            //Ensure to close host after everything is complete
            host.Close();
        }
    }
}
