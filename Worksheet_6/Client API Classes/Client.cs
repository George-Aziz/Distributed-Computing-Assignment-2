// Filename: Client.cs
// Project:  DC Assignment 2 (Practical 6)
// Purpose:  Client struct
// Author:   George Aziz (19765453)
// Date:     16/05/2021

using Job_Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Client_API_Classes
{
    public class Client
    {
        public string Address;
        public string Port;
        public int FinishedJobsCount;
    }
}
