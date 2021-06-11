// Filename: ClientCounter.cs
// Project:  DC Assignment 2 (Practical 8)
// Purpose:  Counter for port & ID that is static to be used among multiple clients
// Author:   George Aziz (19765453)
// Date:     17/05/2021

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_API_Classes
{
    public static class ClientCounter
    {
        public static uint CurrentPort = 8100;
        public static uint ClientID = 0;
    }
}
