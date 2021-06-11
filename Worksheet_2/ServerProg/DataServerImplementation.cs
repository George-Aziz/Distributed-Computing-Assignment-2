// Filename: DataServerImplementation.cs
// Project:  DC Assignment 2 (Practical 2)
// Purpose:  Database service implementation 
// Author:   George Aziz (19765453)
// Date:     22/04/2021


using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using DBDLL;
using DB;

namespace ServerProg
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    internal class DataServerImplementation : DataServerInterface
    {
        private readonly Database _database;

        /// <summary>
        /// Class Constructor
        /// </summary>
        public DataServerImplementation()
        {
            _database = Database.Get();
        }

        /// <summary>
        /// Get Number of Entries in Database 
        /// </summary>
        /// <returns>Number of Records in Database</returns>
        public int GetNumEntries()
        {
            return _database.GetNumRecords();
        }

        /// <summary>
        /// Searches Database with provided integer and retrieves values
        /// </summary>
        /// <param name="index">Index to search database</param>
        /// <param name="acctNo">Account Number of Record Found</param>
        /// <param name="pin">User Pin of Record Found</param>
        /// <param name="bal">User Balance of Record Found</param>
        /// <param name="fName">User First Name of Record Found</param>
        /// <param name="lName">User Last Name of Record Found</param>
        /// <param name="image">User Image of Record Found</param>
        public void GetValuesForEntry(int index, out uint acctNo, out uint pin, out int bal, out string fName, out string lName, out Bitmap image)
        {
            if (index < 0 || index >= _database.GetNumRecords())
            {
                Console.WriteLine("Index inputted is out of range!");
                throw new FaultException<IndexOutOfRangeFault>(new IndexOutOfRangeFault()
                { Issue = "Index was out of range" });
            }
            else
            {
                acctNo = _database.GetAcctNoByIndex(index);
                pin = _database.GetPINByIndex(index);
                bal = _database.GetBalanceByIndex(index);
                fName = _database.GetFirstNameByIndex(index);
                lName = _database.GetLastNameByIndex(index);
                image = _database.GetImageByIndex(index);
            }
        }
    }
}
