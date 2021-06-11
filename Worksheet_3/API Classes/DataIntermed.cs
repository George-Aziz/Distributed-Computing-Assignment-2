// Filename: DataIntermed.cs
// Project:  DC Assignment 2 (Practical 3)
// Purpose:  DataIntermed Object for communication between client and API
// Author:   George Aziz (19765453)
// Date:     25/04/2021

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Classes
{
    public class DataIntermed
    {
        public uint AcctNo;
        public uint Pin;
        public int Balance;
        public string FirstName;
        public string LastName;
        public byte[] Image;
    }
}
