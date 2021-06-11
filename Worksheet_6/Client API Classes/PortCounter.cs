// Filename: PortCounter.cs
// Project:  DC Assignment 2 (Practical 6)
// Purpose:  Counter for port that is static to be used among multiple clients
// Author:   George Aziz (19765453)
// Date:     16/05/2021

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_API_Classes
{
    public static class PortCounter
    {
        public static uint CurrentPort = 8100;
    }
}
