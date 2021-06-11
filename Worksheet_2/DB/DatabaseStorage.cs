// Filename: DatabaseStorage.cs
// Project:  DC Assignment 2 (Practical 2)
// Purpose:  Database Object to store content
// Author:   George Aziz (19765453)
// Date:     22/04/2021

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    internal class DatabaseStorage
    {
        public uint AcctNo;
        public uint Pin;
        public int Balance;
        public string FirstName;
        public string LastName;
        public Bitmap Image;

        public DatabaseStorage()
        {
            AcctNo = 0;
            Pin = 0;
            Balance = 0;
            FirstName = "";
            LastName = "";
            Image = null;
        }
    }
}
