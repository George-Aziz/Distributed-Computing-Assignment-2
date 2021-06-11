// Filename: DatabaseStorage.cs
// Project:  DC Assignment 2 (Practical 3)
// Purpose:  Data Tier Web Service Model for Database Storage Structure
// Author:   George Aziz (19765453)
// Date:     25/04/2021

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace DataWebService.Models
{
    public class DatabaseStorage
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