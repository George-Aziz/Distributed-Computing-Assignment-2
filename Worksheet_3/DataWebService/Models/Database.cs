// Filename: Database.cs
// Project:  DC Assignment 2 (Practical 3)
// Purpose:  Data Tier Web Service Model for Database
// Author:   George Aziz (19765453)
// Date:     25/04/2021

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace DataWebService.Models
{
    public class Database
    {
        private readonly List<DatabaseStorage> _dataStorage;
        private static Database instance;

        /// <summary>
        /// Ensures that only one instance of database can be retrieved (Singleton)
        /// </summary>
        /// <returns>Current object instance</returns>
        public static Database Get()
        {
            if (instance == null)
            {
                instance = new Database();
            }
            return instance;
        }

        /// <summary>
        /// Constructor for Database Object
        /// </summary>
        private Database()
        {
            _dataStorage = new List<DatabaseStorage>();
            DataGen gen = new DataGen();
            for (int i = 0; i < 100000; i++)
            {
                DatabaseStorage data = new DatabaseStorage();
                gen.GetNextAccount(out data.Pin, out data.AcctNo, out data.FirstName, out data.LastName, out data.Balance, out data.Image);
                _dataStorage.Add(data);
            }
        }

        /// <summary>
        /// Gets Account Number Through Inputted Index
        /// </summary>
        /// <param name="index">Index to search database</param>
        /// <returns>Integer that represents Account Number</returns>
        public uint GetAcctNoByIndex(int index)
        {
            return _dataStorage[index].AcctNo;
        }

        /// <summary>
        /// Gets PIN NumberThrough Inputted Index
        /// </summary>
        /// <param name="index">Index to search database</param>
        /// <returns>Integer that represents PIN Number</returns>
        public uint GetPINByIndex(int index)
        {
            return _dataStorage[index].Pin;
        }

        /// <summary>
        /// Gets First Name Through Inputted Index
        /// </summary>
        /// <param name="index">Index to search database</param>
        /// <returns>String that contains first name of user</returns>
        public string GetFirstNameByIndex(int index)
        {
            return _dataStorage[index].FirstName;
        }

        /// <summary>
        /// Gets Last Name Through Inputted Index
        /// </summary>
        /// <param name="index">Index to search database</param>
        /// <returns>String that contains last name of user</returns>
        public string GetLastNameByIndex(int index)
        {
            return _dataStorage[index].LastName;
        }

        /// <summary>
        /// Gets Balance Through Inputted Index
        /// </summary>
        /// <param name="index">Index to search database</param>
        /// <returns>Integer that represents Balance</returns>
        public int GetBalanceByIndex(int index)
        {
            return _dataStorage[index].Balance;
        }

        /// <summary>
        /// Gets Image Through Inputted Index
        /// </summary>
        /// <param name="index">Index to search database</param>
        /// <returns>Bitmap that represents user image</returns>
        public Bitmap GetImageByIndex(int index)
        {
            return new Bitmap(_dataStorage[index].Image);
        }

        /// <summary>
        /// Gets Number of records in database
        /// </summary>
        /// <returns>Integer that represents total amount of records</returns>
        public int GetNumRecords()
        {
            return _dataStorage.Count;
        }
    }
}