// Filename: DataGen.cs
// Project:  DC Assignment 2 (Practical 2)
// Purpose:  Database data generator
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
    internal class DataGen
    {
        private static readonly Random _rand = new Random(DateTime.Now.Second);

        /// <summary>
        /// Generates a new record in Database
        /// </summary>
        /// <param name="pin">User Pin</param>
        /// <param name="acctNo">User Account Number</param>
        /// <param name="firstName">User First Name</param>
        /// <param name="lastName">User Last Name</param>
        /// <param name="balance">User Balance</param>
        /// <param name="image">User Image</param>
        public void GetNextAccount(out uint pin, out uint acctNo, out string firstName, out string lastName, out int balance, out Bitmap image)
        {
            pin = GetPIN();
            acctNo = GetAcctNo();
            firstName = GetFirstname();
            lastName = GetLastname();
            balance = GetBalance();
            image = GetImage();
        }

        /// <summary>
        /// Generates a new First Name
        /// </summary>
        /// <returns>String of First Name</returns>
        private string GetFirstname()
        {
            string[] _firstnames = { "Matthew", "Mark", "John", "Luke", "James", "Peter", "Simon", "Michael", "Constantine", "Andre", "Miles", "Jackson", "George" };
            return _firstnames[_rand.Next(0, _firstnames.Length - 1)];
        }

        /// <summary>
        /// Generates a new Last Name
        /// </summary>
        /// <returns>String of Last Name</returns>
        private string GetLastname()
        {
            string[] _lastnames = { "Aziz", "Greene", "Demas", "Arthur", "Downey", "Rogers", "Evans", "Cremen", "Gerges", "Bekhit", "Alexander", "Michael", "Shalabi" };
            return _lastnames[_rand.Next(0, _lastnames.Length - 1)];
        }

        /// <summary>
        /// Generates a new PIN Number
        /// </summary>
        /// <returns>Integer that represents a pin number</returns>
        private uint GetPIN()
        {
            return (uint)_rand.Next(100);
        }

        /// <summary>
        /// Generates a new Account Number
        /// </summary>
        /// <returns>Integer that represents an Account Number</returns>
        private uint GetAcctNo()
        {
            return (uint)_rand.Next(10000);
        }

        /// <summary>
        /// Generates a new balance
        /// </summary>
        /// <returns>Integer that represents user balance</returns>
        private int GetBalance()
        {
            return _rand.Next(100000);
        }

        /// <summary>
        /// Generates a new image
        /// </summary>
        /// <returns>Bitmap that represents user image</returns>
        private Bitmap GetImage()
        {
            var _image = new Bitmap(3, 3);
            for (var x = 0; x < 3; x++)
            {
                for (var y = 0; y < 3; y++)
                {
                    _image.SetPixel(x, y, Color.FromArgb(_rand.Next(256), _rand.Next(256), _rand.Next(256)));
                }
            }
            return _image;
        }
    }
}
